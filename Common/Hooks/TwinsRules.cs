﻿using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Conditions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace AltLibrary.Common.Hooks
{
	internal class TwinsRules
	{
		public static void Init()
		{
			//Terraria.GameContent.ItemDropRules.IL_ItemDropDatabase.RegisterBoss_Twins += ItemDropDatabase_RegisterBoss_Twins;
		}

		public static void Unload()
		{
		}

		//TODO: double check that this code makes sense to begin with
		private static void ItemDropDatabase_RegisterBoss_Twins(ILContext il)
		{
			ILCursor c = new(il);
			if (!c.TryGotoNext(i => i.MatchLdcI4(1225)))
			{
				AltLibrary.Instance.Logger.Info("q $ 1");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchPop()))
			{
				AltLibrary.Instance.Logger.Info("q $ 2");
				return;
			}
			c.Index++;
			c.Emit(OpCodes.Ldloc, 1);
			c.EmitDelegate<Func<LeadingConditionRule, LeadingConditionRule>>(LeadingConditionRule);
			c.Emit(OpCodes.Stloc, 1);
		}

		private static LeadingConditionRule LeadingConditionRule(LeadingConditionRule leadCond)
		{
			leadCond.ChainedRules.RemoveAt(1);
			leadCond.OnSuccess(ItemDropRule.ByCondition(new HallowDropCondition(), ItemID.HallowedBar, 1, 15, 30));
			foreach (var biome in from AltBiome biome in AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Hallow)
								  where biome.MechDropItemType != null && biome.MechDropItemType.HasValue
								  select biome)
			{
				leadCond.OnSuccess(ItemDropRule.ByCondition(new HallowAltDropCondition(biome), biome.MechDropItemType.Value, 1, 15, 30));
			}
			return leadCond;
		}
	}
}
