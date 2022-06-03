﻿using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Condition;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;
using Terraria.GameContent.ItemDropRules;

namespace AltLibrary.Common.Hooks
{
    internal class TwinsRules
    {
        public static void Init()
        {
            IL.Terraria.GameContent.ItemDropRules.ItemDropDatabase.RegisterBoss_Twins += ItemDropDatabase_RegisterBoss_Twins;
        }

        public static void Unload()
        {
            IL.Terraria.GameContent.ItemDropRules.ItemDropDatabase.RegisterBoss_Twins -= ItemDropDatabase_RegisterBoss_Twins;
        }

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
            c.EmitDelegate(LeadingConditionRule);
            c.Emit(OpCodes.Stloc, 1);
        }

        private static LeadingConditionRule LeadingConditionRule(LeadingConditionRule leadCond)
        {
            if (AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Hallow).Any())
            {
                foreach (var _ in from AltBiome biome in AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Hallow)
                                  where biome.MechDropItemType != null
                                  select new { })
                {
                    leadCond.ChainedRules.RemoveAt(1);
                    break;
                }
            }

            foreach (var biome in from AltBiome biome in AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Hallow)
                                  where biome.MechDropItemType != null && biome.MechDropItemType.HasValue
                                  select biome)
            {
                leadCond.OnSuccess(ItemDropRule.ByCondition(new HallowedBarAltDropCondition(biome), biome.MechDropItemType.Value, 1, 15, 30));
            }

            return leadCond;
        }
    }
}
