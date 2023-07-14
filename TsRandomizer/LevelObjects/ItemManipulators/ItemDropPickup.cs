﻿using System;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Items.ItemDropPickup")]
	// ReSharper disable once UnusedMember.Global
	class ItemDropPickup : ItemManipulator
	{
		bool hasDroppedLoot;

		public ItemDropPickup(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation) 
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			if (ItemInfo == null)
				return;

			if (IsPickedUp)
			{
				Dynamic.Kill();
			}
			else
			{
				Dynamic.IsFound = false;
				hasDroppedLoot = false;
			}
			
			Dynamic._itemData = ItemInfo.BestiaryItemDropSpecification;
			Dynamic._category = ItemInfo.Identifier.LootType.ToEInventoryCategoryType();

			switch (ItemInfo.Identifier.LootType)
			{
				case LootType.ConstRelic:
					Dynamic._relicType = ItemInfo.Identifier.Relic;
					break;

				case LootType.ConstUseItem:
					Dynamic._useItemType = ItemInfo.Identifier.UseItem;
					break;

				case LootType.ConstEquipment:
					Dynamic._equipmentType = ItemInfo.Identifier.Equipment;
					break;

				case LootType.ConstStat:
				case LootType.ConstOrb:
				case LootType.ConstFamiliar:
					Dynamic._category = EInventoryCategoryType.Equipment;
					Dynamic._equipmentType = EInventoryEquipmentType.None;
					break;

				default:
					throw new NotImplementedException($"LoottType {ItemInfo.Identifier.LootType} is not supported by ItemDropPickup");
			}

			Dynamic.ChangeAnimation(ItemInfo.AnimationIndex);
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasDroppedLoot || !Dynamic.IsFound)
				return;

			// ReSharper disable once PossibleNullReferenceException
			switch (ItemInfo.Identifier.LootType)
			{
				case LootType.ConstOrb:
				case LootType.ConstFamiliar:
				case LootType.ConstStat:
					AwardContainedItem();
					UndoBaseGameAwardedEquipment();
					ShowItemAwardPopup();
					break;

				case LootType.ConstRelic:
				case LootType.ConstUseItem:
				case LootType.ConstEquipment:
					OnItemPickup(); //awarding handled by base game
					break;

				default:
					throw new NotImplementedException($"LoottType {ItemInfo.Identifier.LootType} is not supported by ItemDropPickup");
			}

			hasDroppedLoot = true;
		}

		void UndoBaseGameAwardedEquipment()
		{
			Level.GameSave.Inventory.EquipmentInventory.RemoveItem(ItemInfo.BestiaryItemDropSpecification.Item, 1);
			GameplayScreen.HideItemPickupBar();
		}
	}
}
