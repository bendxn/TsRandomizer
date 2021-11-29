﻿using System;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;
using TsRandomizer.Randomisation;

namespace TsRandomizer.LevelObjects
{
	abstract class ItemManipulator<T> : ItemManipulator where T : Mobile
	{
		public readonly T TypedObject;

		protected ItemManipulator(T typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
			TypedObject = typedObject;
		}
	}

	abstract class ItemManipulator : LevelObject
	{
		static ItemLocationMap itemLocationMap;

		public bool IsPickedUp => itemLocation.IsPickedUp;

		public readonly ItemInfo ItemInfo;
		readonly ItemLocation itemLocation;

		protected ItemManipulator(Mobile typedObject, ItemLocation itemLocation) : base(typedObject)
		{
			this.itemLocation = itemLocation;
			ItemInfo = itemLocation?.ItemInfo;
		}

		protected void AwardContainedItem()
		{
			Level.GameSave.AddItem(Level, ItemInfo.Identifier);
			if (ItemInfo.Identifier.LootType == LootType.ConstRelic)
				LevelReflected.UnlockRelic(ItemInfo.Identifier.Relic);

			OnItemPickup();
		}

		protected void OnItemPickup()
		{
			ItemInfo.OnPickup(Level);
			itemLocation.SetPickedUp();

			if (ItemInfo.IsProgression)
				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(itemLocationMap));
		}

		public static void Initialize(ItemLocationMap itemLocations)
		{
			itemLocationMap = itemLocations;
		}

		public static ItemManipulator GenerateShadowObject(Type levelObjectType, Mobile obj, ItemLocationMap itemLocations)
		{
			var itemKey = GetKey(obj);
			var itemLocation = itemLocations[itemKey];

			if (itemLocation == null)
			{
				Console.Out.WriteLine($"UnmappedItem: {itemKey}");
				return null;
			}

			return (ItemManipulator)Activator.CreateInstance(levelObjectType, obj, itemLocation);
		}

		protected void ShowItemAwardPopup() =>
			Level.ShowItemAwardPopup(ItemInfo.Identifier);

		

	}
}
