using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Conditions;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common {
	internal class BossBags : GlobalItem {
		public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
			IReadOnlyList<AltBiome> HallowList = AltLibrary.GetAltBiomes(BiomeType.Hallow);
			IReadOnlyList<AltBiome> HellList = AltLibrary.GetAltBiomes(BiomeType.Hell);
			IReadOnlyList<AltBiome> JungleList = AltLibrary.GetAltBiomes(BiomeType.Jungle);
			IReadOnlyList<AltBiome> EvilList = AltLibrary.GetAltBiomes(BiomeType.Evil);

			List<IItemDropRule> entries = itemLoot.Get(false);
			switch (item.type) {
				case ItemID.EyeOfCthulhuBossBag: {
					foreach (IItemDropRule entry in entries) {
						if (entry is ItemDropWithConditionRule conditionRule) {
							if (conditionRule.itemId == ItemID.DemoniteOre || conditionRule.itemId == ItemID.CrimtaneOre ||
								conditionRule.itemId == ItemID.CorruptSeeds || conditionRule.itemId == ItemID.CrimsonSeeds
								|| conditionRule.itemId == ItemID.UnholyArrow) {
								itemLoot.Remove(entry);
							}
						}
					}
					foreach (AltBiome biome in EvilList) {
						LeadingConditionRule biomeDropRule = new LeadingConditionRule(new EvilAltDropCondition(biome));
						if (biome.BiomeOreItem != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.BiomeOreItem, 1, 30, 90));
						if (biome.SeedType != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.SeedType, 1, 1, 3));
						if (biome.ArrowType != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.ArrowType, 1, 20, 50));
						itemLoot.Add(biomeDropRule);
					}

					break;
				}
				case ItemID.WallOfFleshBossBag: {
					itemLoot.ReplaceDrops((CommonDrop commonRule) => {
						if (commonRule.itemId != ItemID.Pwnhammer) return commonRule;
						DropByAltBiome rules = new(BiomeType.Hallow,
							biome => biome.HammerType,
							type => ItemDropRule.Common(type, 1, commonRule.amountDroppedMinimum, commonRule.amountDroppedMaximum),
							ItemID.Pwnhammer
						);
						rules.ChainedRules.AddRange(commonRule.ChainedRules);
						return rules.CopyConditions(commonRule);
					});
					break;
				}
				case ItemID.DestroyerBossBag:
				case ItemID.SkeletronPrimeBossBag:
				case ItemID.TwinsBossBag: {
					itemLoot.ReplaceDrops((CommonDrop commonRule) => {
						if (commonRule.itemId != ItemID.HallowedBar) return commonRule;
						DropByAltBiome rules = new(BiomeType.Hallow,
							biome => biome.MechDropItemType,
							type => ItemDropRule.Common(type, 1, commonRule.amountDroppedMinimum, commonRule.amountDroppedMaximum),
							ItemID.HallowedBar
						);
						rules.ChainedRules.AddRange(commonRule.ChainedRules);
						return rules.CopyConditions(commonRule);
					});
					break;
				}
			}
		}
	}
}
