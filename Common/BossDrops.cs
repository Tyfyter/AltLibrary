using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Conditions;
using PegasusLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common {
	internal class BossDrops : GlobalNPC {
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
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

			List<IItemDropRule> entries = npcLoot.Get(false);
			switch (npc.type) {
				case NPCID.EyeofCthulhu: {
					foreach (IItemDropRule entry in entries) {
						if (entry is ItemDropWithConditionRule conditionRule) {
							if (conditionRule.itemId == ItemID.DemoniteOre || conditionRule.itemId == ItemID.CrimtaneOre ||
								conditionRule.itemId == ItemID.CorruptSeeds || conditionRule.itemId == ItemID.CrimsonSeeds
								|| conditionRule.itemId == ItemID.UnholyArrow) {
								npcLoot.Remove(entry);
							}
						}
					}
					LeadingConditionRule expertCondition = new LeadingConditionRule(new Terraria.GameContent.ItemDropRules.Conditions.NotExpert());

					foreach (AltBiome biome in EvilList) {
						LeadingConditionRule biomeDropRule = new LeadingConditionRule(new EvilAltDropCondition(biome));
						if (biome.BiomeOreItem != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.BiomeOreItem, 1, 30, 90));
						if (biome.SeedType != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.SeedType, 1, 1, 3));
						if (biome.ArrowType != null) biomeDropRule.OnSuccess(ItemDropRule.Common((int)biome.ArrowType, 1, 20, 50));
						expertCondition.OnSuccess(biomeDropRule);
					}
					npcLoot.Add(expertCondition);
					break;
				}
				case NPCID.WallofFlesh: {
					npcLoot.ReplaceDrops((CommonDrop commonRule) => {
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
				case NPCID.TheDestroyer:
				case NPCID.SkeletronPrime:
				case NPCID.Spazmatism:
				case NPCID.Retinazer: {
					npcLoot.ReplaceDrops((CommonDrop commonRule) => {
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
	public class FirstMatchingRule(IEnumerable<IItemDropRule> rules) : IItemDropRule, INestedItemDropRule {
		public IItemDropRule[] rules = [..rules];
		public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; } = [];

		public bool CanDrop(DropAttemptInfo info) => true;
		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
			ItemDropAttemptResult result = default(ItemDropAttemptResult);
			result.State = ItemDropAttemptResultState.DidNotRunCode;
			return result;
		}
		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info, ItemDropRuleResolveAction resolveAction) {
			ItemDropAttemptResult result = default;
			for (int i = 0; i < rules.Length; i++) {
				IItemDropRule rule = rules[i];
				result = resolveAction(rule, info);
				if (result.State != ItemDropAttemptResultState.DoesntFillConditions) {
					return result;
				}
			}

			result.State = ItemDropAttemptResultState.DoesntFillConditions;
			return result;
		}
		public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
			List<DropRateInfo> buffer = [];
			for (int i = 0; i < rules.Length; i++) {
				buffer.Clear();
				rules[i].ReportDroprates(buffer, ratesInfo.With(ratesInfo.parentDroprateChance));
				buffer.RemoveAll(info => info.conditions?.Any(c => !c.CanShowItemDropInUI()) ?? false);
				drops.AddRange(buffer);
				if (buffer.Count > 0) break;
			}

			Chains.ReportDroprates(ChainedRules, 1, drops, ratesInfo);
		}
	}
	static class DropRuleExtensions {
		public static T WithOnFailedConditions<T>(this T rule, IItemDropRule ruleToChain, bool hideLootReport = false) where T : IItemDropRule {
			rule.OnFailedConditions(ruleToChain, hideLootReport);
			return rule;
		}
		public static T WithOnFailedRoll<T>(this T rule, IItemDropRule ruleToChain, bool hideLootReport = false) where T : IItemDropRule {
			rule.OnFailedRoll(ruleToChain, hideLootReport);
			return rule;
		}
		public static T WithOnSuccess<T>(this T rule, IItemDropRule ruleToChain, bool hideLootReport = false) where T : IItemDropRule {
			rule.OnSuccess(ruleToChain, hideLootReport);
			return rule;
		}
		public static IItemDropRule CopyConditions(this IItemDropRule rule, CommonDrop conditionSource) {
			if (conditionSource is ItemDropWithConditionRule conditionRule) new LeadingConditionRule(conditionRule.condition).WithOnSuccess(rule);
			return rule;
		}
		public static void ReplaceDrops<T>(this NPCLoot npcLoot, Func<T, IItemDropRule> replacer) where T : class, IItemDropRule {
			List<IItemDropRule> entries = npcLoot.Get(false);
			List<IItemDropRule> newEntries = entries.ToList();
			ReplaceDropRules(newEntries, replacer);
			for (int i = 0; i < newEntries.Count; i++) {
				if (!entries.Contains(newEntries[i])) {
					npcLoot.Add(newEntries[i]);
				}
			}
			for (int i = 0; i < entries.Count; i++) {
				if (!newEntries.Contains(entries[i])) {
					npcLoot.Remove(entries[i]);
				}
			}
		}
		public static void ReplaceDrops<T>(this ItemLoot itemLoot, Func<T, IItemDropRule> replacer) where T : class, IItemDropRule {
			List<IItemDropRule> entries = itemLoot.Get(false);
			List<IItemDropRule> newEntries = entries.ToList();
			ReplaceDropRules(newEntries, replacer);
			for (int i = 0; i < newEntries.Count; i++) {
				if (!entries.Contains(newEntries[i])) {
					itemLoot.Add(newEntries[i]);
				}
			}
			for (int i = 0; i < entries.Count; i++) {
				if (!newEntries.Contains(entries[i])) {
					itemLoot.Remove(entries[i]);
				}
			}
		}
		static void ReplaceDropRules<T>(this List<IItemDropRule> dropRules, Func<T, IItemDropRule> replacer) where T : class, IItemDropRule {
			for (int i = 0; i < dropRules.Count; i++) {
				IItemDropRule dropRule = dropRules[i];
				{
					if (dropRule is T oldRule && replacer(oldRule) is IItemDropRule newRule && newRule != oldRule) dropRules[i] = newRule;
				}
				if (dropRule.ChainedRules.Count != 0) {
					for (int j = 0; j < dropRule.ChainedRules.Count; j++) {
						if (dropRule.ChainedRules[j].RuleToChain is T oldRule && replacer(oldRule) is IItemDropRule newRule && newRule != oldRule) {
							if (dropRule.ChainedRules[j] is Chains.TryIfDoesntFillConditions tryIfDoesntFillConditions) {
								dropRule.ChainedRules[j] = new Chains.TryIfDoesntFillConditions(newRule, tryIfDoesntFillConditions.hideLootReport);
							} else if (dropRule.ChainedRules[j] is Chains.TryIfFailedRandomRoll tryIfFailedRandomRoll) {
								dropRule.ChainedRules[j] = new Chains.TryIfFailedRandomRoll(newRule, tryIfFailedRandomRoll.hideLootReport);
							} else if (dropRule.ChainedRules[j] is Chains.TryIfSucceeded tryIfSucceeded) {
								dropRule.ChainedRules[j] = new Chains.TryIfSucceeded(newRule, tryIfSucceeded.hideLootReport);
							}
						} else {
							ReplaceDropRules([dropRule.ChainedRules[j].RuleToChain], replacer);
						}
					}
				}
			}
		}
	}
}
