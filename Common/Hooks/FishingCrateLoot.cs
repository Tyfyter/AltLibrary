using AltLibrary.Common.AltOres;
using PegasusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	internal class FishingCrateLoot : GlobalItem {
		static readonly List<int> vanillaPreHardmodeOres = [];
		static readonly List<int> vanillaPreHardmodeBars = [];
		static readonly List<int> vanillaHardmodeOres = [];
		static readonly List<int> vanillaHardmodeBars = [];
		static readonly List<int> preHardmodeOres = [];
		static readonly List<int> preHardmodeBars = [];
		static readonly List<int> hardmodeOres = [];
		static readonly List<int> hardmodeBars = [];
		static readonly HashSet<int> workingSet = [];
		public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
			if (!ItemID.Sets.IsFishingCrate[item.type])
				return;
			if (vanillaPreHardmodeOres.Count <= 0) {
				foreach (AltOre ore in AltLibrary.GetAltOres().OrderBy(ore => ore.OreSlot)) {
					if (ore.OreSlot > ModContent.GetInstance<AdamantiteOreSlot>()) break;
					bool preHardmode = ore.OreSlot <= ModContent.GetInstance<GoldOreSlot>();
					switch ((preHardmode, ore.Mod == Mod)) {
						case (true, true):
						vanillaPreHardmodeOres.Add(ore.oreItem);
						vanillaPreHardmodeBars.Add(ore.bar);
						break;
						case (false, true):
						vanillaHardmodeOres.Add(ore.oreItem);
						vanillaHardmodeBars.Add(ore.bar);
						break;

						case (true, false):
						preHardmodeOres.Add(ore.oreItem);
						preHardmodeBars.Add(ore.bar);
						break;
						case (false, false):
						hardmodeOres.Add(ore.oreItem);
						hardmodeBars.Add(ore.bar);
						break;
					}
				}
			}
			List<IItemDropRule> loot = itemLoot.Get(false);
			List<int> toAdd = [];
			foreach (OneFromRulesRule rule in loot.FindDropRules<OneFromRulesRule>()) {
				toAdd.Clear();
				void Add(List<int> items) => toAdd.AddRange(items.Where(item => !workingSet.Contains(item)));
				if (ContainsAll(rule, vanillaPreHardmodeOres)) Add(preHardmodeOres);
				if (ContainsAll(rule, vanillaHardmodeOres)) Add(hardmodeOres);
				if (ContainsAll(rule, vanillaPreHardmodeBars)) Add(preHardmodeBars);
				if (ContainsAll(rule, vanillaHardmodeBars)) Add(hardmodeBars);
				if (toAdd.Count <= 0) continue;
				int startLength = rule.options.Length;
				CommonDropNotScalingWithLuck template = (CommonDropNotScalingWithLuck)rule.options[0];
				Array.Resize(ref rule.options, startLength + toAdd.Count);
				for (int i = 0; i < toAdd.Count; i++) {
					rule.options[startLength + i] = new CommonDropNotScalingWithLuck(toAdd[i], template.chanceDenominator, template.chanceNumerator, template.amountDroppedMinimum, template.amountDroppedMaximum);
				}
			}
			workingSet.Clear();
			bool ContainsAll(OneFromRulesRule oneFromOptions, List<int> items) {
				workingSet.Clear();
				CommonDropNotScalingWithLuck first = default;
				for (int i = 0; i < oneFromOptions.options.Length; i++) {
					if (oneFromOptions.options[i] is not CommonDropNotScalingWithLuck rule) return false;
					if (first is not null) {
						if (rule.chanceNumerator != first.chanceNumerator) return false;
						if (rule.chanceDenominator != first.chanceDenominator) return false;
						if (rule.amountDroppedMinimum != first.amountDroppedMinimum) return false;
						if (rule.amountDroppedMaximum != first.amountDroppedMaximum) return false;
					}
					first = rule;
					workingSet.Add(rule.itemId);
				}
				return items.All(workingSet.Contains);
			}
		}
	}
}