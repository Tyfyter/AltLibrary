using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Conditions;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common
{
	internal class BossBags : GlobalItem {
		//TODO: double check that this code makes sense to begin with
		public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
		{
			List<AltBiome> HallowList = [
				GetInstance<HallowAltBiome>()
			];
			List<AltBiome> HellList = [
				GetInstance<UnderworldAltBiome>()
			];
			List<AltBiome> JungleList = [
				GetInstance<JungleAltBiome>()
			];
			List<AltBiome> EvilList = [
				GetInstance<CorruptionAltBiome>(),
				GetInstance<CrimsonAltBiome>()
			];
			foreach (AltBiome biome in AltLibrary.Biomes) {
				switch (biome.BiomeType) {
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

			List<IItemDropRule> entries = itemLoot.Get(false);
			switch (item.type)
			{
				case ItemID.EyeOfCthulhuBossBag:
					{
						foreach (IItemDropRule entry in entries)
						{
							if (entry is ItemDropWithConditionRule conditionRule)
							{
								if (conditionRule.itemId == ItemID.DemoniteOre || conditionRule.itemId == ItemID.CrimtaneOre ||
									conditionRule.itemId == ItemID.CorruptSeeds || conditionRule.itemId == ItemID.CrimsonSeeds
									|| conditionRule.itemId == ItemID.UnholyArrow)
								{
									itemLoot.Remove(entry);
								}
							}
						}
						foreach (AltBiome biome in EvilList)
						{
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
						FirstMatchingRule rules = new([
							..HallowList.Select(biome => ItemDropRule.ByCondition(new HallowAltDropCondition(biome), biome.HammerType)),
							ItemDropRule.Common(ItemID.Pwnhammer)
						]);
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
						FirstMatchingRule rules = new([
							..HallowList.Where(biome => biome.MechDropItemType.HasValue)
								.Select(biome => ItemDropRule.ByCondition(new HallowAltDropCondition(biome), biome.MechDropItemType.Value, 1, commonRule.amountDroppedMinimum, commonRule.amountDroppedMaximum)),
							ItemDropRule.Common(ItemID.HallowedBar, 1, commonRule.amountDroppedMinimum, commonRule.amountDroppedMaximum)
						]);
						rules.ChainedRules.AddRange(commonRule.ChainedRules);
						return rules.CopyConditions(commonRule);
					});
					break;
				}
			}
		}
	}
}
