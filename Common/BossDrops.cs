using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Conditions;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common
{
	internal class BossDrops : GlobalNPC {
		//TODO: double check that this code makes sense to begin with
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
			List<AltBiome> HallowList = new() {
				GetInstance<HallowAltBiome>()
			};
			List<AltBiome> HellList = new() {
				GetInstance<UnderworldAltBiome>()
			};
			List<AltBiome> JungleList = new() {
				GetInstance<JungleAltBiome>()
			};
			List<AltBiome> EvilList = new() {
				GetInstance<CorruptionAltBiome>(),
				GetInstance<CrimsonAltBiome>()
			};
			foreach (AltBiome biome in AltLibrary.Biomes)
			{
				switch (biome.BiomeType)
				{
					case BiomeType.Evil:
						EvilList.Add(biome);
						break;
					case BiomeType.Hallow:
						HallowList.Add(biome);
						break;
					case BiomeType.Hell:
						HellList.Add(biome);
						break;
					case BiomeType.Jungle:
						JungleList.Add(biome);
						break;
				}
			}

			var entries = npcLoot.Get(false);
			switch (npc.type) {
				case NPCID.EyeofCthulhu: {
					foreach (var entry in entries)
					{
						if (entry is ItemDropWithConditionRule conditionRule)
						{
							if (conditionRule.itemId == ItemID.DemoniteOre || conditionRule.itemId == ItemID.CrimtaneOre ||
								conditionRule.itemId == ItemID.CorruptSeeds || conditionRule.itemId == ItemID.CrimsonSeeds
								|| conditionRule.itemId == ItemID.UnholyArrow)
							{
								npcLoot.Remove(entry);
							}
						}
					}
					var expertCondition = new LeadingConditionRule(new Terraria.GameContent.ItemDropRules.Conditions.NotExpert());

					foreach (AltBiome biome in EvilList)
					{
						var biomeDropRule = new LeadingConditionRule(new EvilAltDropCondition(biome));
						if (biome.BiomeOreItem != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.BiomeOreItem, 1, 30, 90));
						if (biome.SeedType != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.SeedType, 1, 1, 3));
						if (biome.ArrowType != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.ArrowType, 1, 20, 50));
						expertCondition.OnSuccess(biomeDropRule);
					}
					npcLoot.Add(expertCondition);
					break;
				}
				case NPCID.WallofFlesh: {
					foreach (var entry in entries)
					{
						if (entry is ItemDropWithConditionRule rule && rule.itemId == ItemID.Pwnhammer)
						{
							npcLoot.Remove(rule);
						}
					}
					var expertCondition = new LeadingConditionRule(new Terraria.GameContent.ItemDropRules.Conditions.NotExpert());
					foreach (AltBiome biome in HallowList)
					{
						var biomeDropRule = new LeadingConditionRule(new HallowAltDropCondition(biome));
						biomeDropRule.OnSuccess(ItemDropRule.Common(biome.HammerType));
						expertCondition.OnSuccess(biomeDropRule);
					}
					npcLoot.Add(expertCondition);
					break;
				}
				case NPCID.TheDestroyer:
				case NPCID.SkeletronPrime:{
					foreach (var entry in entries)
					{
						if (entry is ItemDropWithConditionRule conditionRule && conditionRule.itemId == ItemID.HallowedBar)
						{
							npcLoot.Remove(entry);
							break;
						}
					}
					var expertCondition = new LeadingConditionRule(new Terraria.GameContent.ItemDropRules.Conditions.NotExpert());

					foreach (AltBiome biome in HallowList) {
						expertCondition.OnSuccess(ItemDropRule.ByCondition(new HallowAltDropCondition(biome), biome.MechDropItemType ?? ItemID.HallowedBar, 1, 15, 30));
					}
					npcLoot.Add(expertCondition);
					break;
				}
				case NPCID.Spazmatism:
				case NPCID.Retinazer: {
					IItemDropRule ChainSearch(IItemDropRule dropRule) {
						foreach (var entry in dropRule.ChainedRules) {
							if (entry.RuleToChain is CommonDrop commonRule && commonRule.itemId == ItemID.HallowedBar) {
								dropRule.ChainedRules.Remove(entry);
								return dropRule;
							}
							if (ChainSearch(entry.RuleToChain) is IItemDropRule value) {
								return value;
							}
						}
						return null;
					}
					IItemDropRule expertCondition = null;
					foreach (var entry in entries) {
						if (ChainSearch(entry) is IItemDropRule value) {
							expertCondition = value;
							break;
						}
					}
					if (expertCondition is not null) {
						foreach (AltBiome biome in HallowList) {
							expertCondition.OnSuccess(ItemDropRule.ByCondition(new HallowAltDropCondition(biome), biome.MechDropItemType ?? ItemID.HallowedBar, 1, 15, 30));
						}
					}
					break;
				}
			}
		}
	}
}
