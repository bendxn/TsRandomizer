﻿using System.Collections.Generic;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.HUD;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutsceneCavesPast4")]
	class CutsceneCavesPast4 : LevelObject
	{
		public CutsceneCavesPast4(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}
		bool hasRun = false;

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			bool isRandomized = Level.GameSave.GetSettings().BossRando.Value != "Off";
			if (!isRandomized)
				return;

			if (!Level.GameSave.GetSaveBool("IsFightingBoss"))
			{
				foreach (Monster visibleEnemy in Level.AsDynamic().GetVisibleEnemies())
				{
					visibleEnemy.SilentKill();
				}
				Dynamic.SilentKill();

				//abort already triggered scripts
				((List<ScriptAction>)LevelReflected._activeScripts).Clear();
				((Queue<DialogueBox>)LevelReflected._dialogueQueue).Clear();
				((Queue<ScriptAction>)LevelReflected._waitingScripts).Clear();
			}
			else
			{
				// Set invulnerability during the Maw intro
				Level.AsDynamic().TogglePlayerIsInvulnerable(true);
			}
		}

		protected override void OnUpdate()
		{
			if (hasRun)
				return;
			// Undo invulnerability after the cutscene ends
			if (((List<ScriptAction>)LevelReflected._activeScripts).Count == 0 &&
				((Queue<DialogueBox>)LevelReflected._dialogueQueue).Count == 0 &&
				((Queue<ScriptAction>)LevelReflected._waitingScripts).Count == 0)
			{
				LevelReflected.TogglePlayerIsInvulnerable(false);
				hasRun = true;
			}
		}
	}
}

