using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using VanillaConditions = Terraria.GameContent.ItemDropRules.Conditions;

namespace AltLibrary.Common {
	internal class BossDrops : GlobalNPC {
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
			IReadOnlyList<AltBiome> HallowList = AltLibrary.GetAltBiomes(BiomeType.Hallow);
			IReadOnlyList<AltBiome> HellList = AltLibrary.GetAltBiomes(BiomeType.Hell);
			IReadOnlyList<AltBiome> JungleList = AltLibrary.GetAltBiomes(BiomeType.Jungle);
			IReadOnlyList<AltBiome> EvilList = AltLibrary.GetAltBiomes(BiomeType.Evil);

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
					LeadingConditionRule expertCondition = new LeadingConditionRule(new VanillaConditions.NotExpert());

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
				case NPCID.TheDestroyer:
				case NPCID.SkeletronPrime:
				case NPCID.Spazmatism:
				case NPCID.Retinazer: {
					npcLoot.ReplaceDrops((CommonDrop commonRule) => {
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
		public virtual void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
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
	public abstract class DropByAltBiome<T> : IItemDropRule, INestedItemDropRule {
		public List<IItemDropRule> rules;
		public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; } = [];
		public bool CanDrop(DropAttemptInfo info) => true;
		protected abstract bool TrySelect(AltBiome biome, out T itemType);
		public DropByAltBiome(BiomeType biomeType, Func<T, IItemDropRule> ruleCreator, T defaultItem, bool includeDrunk = true) {
			IItemDropRuleCondition CreateBiomeCondition(AltBiome biome) => biomeType switch {
				BiomeType.Evil => new EvilAltDropCondition(biome, includeDrunk),
				BiomeType.Hallow => new HallowAltDropCondition(biome),
				BiomeType.Hell => new HellAltDropCondition(biome),
				BiomeType.Jungle => new JungleAltDropCondition(biome),
				_ => new VanillaConditions.NeverTrue()
			};
			IReadOnlyList<AltBiome> biomes = AltLibrary.GetAltBiomes(biomeType);
			rules = [];
			for (int i = 0; i < biomes.Count; i++) {
				if (TrySelect(biomes[i], out T itemType)) {
					rules.Add(new LeadingConditionRule(CreateBiomeCondition(biomes[i])).WithOnSuccess(ruleCreator(itemType)));
				}
			}
			rules.Add(ruleCreator(defaultItem));
		}
		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
			ItemDropAttemptResult result = default(ItemDropAttemptResult);
			result.State = ItemDropAttemptResultState.DidNotRunCode;
			return result;
		}
		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info, ItemDropRuleResolveAction resolveAction) {
			ItemDropAttemptResult result = default;
			for (int i = 0; i < rules.Count; i++) {
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
			List<IItemDropRuleCondition> conditions = [];
			for (int i = 0; i < rules.Count; i++) {
				DropRateInfoChainFeed currentRates = ratesInfo.With(ratesInfo.parentDroprateChance);
				if (rules[i] is LeadingConditionRule conditionRule) {
					conditions.Add(conditionRule.condition);
				} else {
					currentRates.conditions = ratesInfo.conditions?.ToList() ?? [];
					currentRates.conditions.Add(new NoConditionsMet(conditions));
				}
				rules[i].ReportDroprates(drops, currentRates);
			}

			Chains.ReportDroprates(ChainedRules, 1, drops, ratesInfo);
		}
		struct NoConditionsMet(List<IItemDropRuleCondition> conditions) : IItemDropRuleCondition {
			readonly bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) {
				for (int i = 0; i < conditions.Count; i++) {
					if (conditions[i].CanDrop(info)) return false;
				}
				return true;
			}

			readonly bool IItemDropRuleCondition.CanShowItemDropInUI() {
				for (int i = 0; i < conditions.Count; i++) {
					if (conditions[i].CanShowItemDropInUI()) return false;
				}
				return true;
			}
			readonly string IProvideItemConditionDescription.GetConditionDescription() => "";
		}
	}
	public class DropByAltBiome(BiomeType biomeType, Func<AltBiome, int?> itemFinder, Func<int, IItemDropRule> ruleCreator, int defaultItem) : DropByAltBiome<int>(biomeType, ruleCreator, defaultItem) {
		protected override bool TrySelect(AltBiome biome, out int itemType) {
			if (itemFinder(biome) is int type) {
				itemType = type;
				return true;
			}
			itemType = ItemID.None;
			return false;
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
			if (conditionSource is ItemDropWithConditionRule conditionRule) return new LeadingConditionRule(conditionRule.condition).WithOnSuccess(rule);
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
