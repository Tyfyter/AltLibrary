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
	internal class BossBags : GlobalItem {
		//TODO: double check that this code makes sense to begin with
		public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
		{
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

			var entries = itemLoot.Get(false);
			switch (item.type)
			{
				case ItemID.EyeOfCthulhuBossBag:
					{
						foreach (var entry in entries)
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
							var biomeDropRule = new LeadingConditionRule(new EvilAltDropCondition(biome));
							if (biome.BiomeOreItem != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.BiomeOreItem, 1, 30, 90));
							if (biome.SeedType != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.SeedType, 1, 1, 3));
							if (biome.ArrowType != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.ArrowType, 1, 20, 50));
							itemLoot.Add(biomeDropRule);
						}

						break;
					}
				case ItemID.WallOfFleshBossBag:
					{
						foreach (var entry in entries)
						{
							if (entry is ItemDropWithConditionRule rule && rule.itemId == ItemID.Pwnhammer)
							{
								itemLoot.Remove(rule);
							}
						}
						foreach (AltBiome biome in HallowList)
						{
							var biomeDropRule = new LeadingConditionRule(new HallowAltDropCondition(biome));
							biomeDropRule.OnSuccess(ItemDropRule.Common(biome.HammerType));
							itemLoot.Add(biomeDropRule);
						}
						break;
					}
				case ItemID.TwinsBossBag:
				case ItemID.DestroyerBossBag:
				case ItemID.SkeletronPrimeBossBag:
					{
						foreach (var entry in entries)
						{
							if (entry is ItemDropWithConditionRule conditionRule && conditionRule.itemId == ItemID.HallowedBar)
							{
								itemLoot.Remove(entry);
								break;
							}
						}
						foreach (AltBiome biome in HallowList)
						{
							var biomeDropRule = new LeadingConditionRule(new HallowAltDropCondition(biome));
							biomeDropRule.OnSuccess(ItemDropRule.Common(biome.MechDropItemType ?? ItemID.HallowedBar, 1, 15, 30));
							itemLoot.Add(biomeDropRule);
						}
						break;
					}
			}
		}
	}
}
