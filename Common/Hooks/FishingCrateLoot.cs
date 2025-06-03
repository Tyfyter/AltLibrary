using AltLibrary.Common.AltOres;
using PegasusLib;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks
{
	internal class FishingCrateLoot : GlobalItem {
		//TODO: make alt ores so I can test this
		public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
		{
			if (!ItemID.Sets.IsFishingCrate[item.type])
				return;

			List<IItemDropRule> loot = itemLoot.Get(false);
			for (int i = 0; i < OreSlotLoader.OreSlotCount; i++) {
				AltOre[] ores = OreSlotLoader.GetOres(i).ToArray();
				bool isBar = false;
				bool Match(OneFromRulesRule rule) {
					isBar = false;
					if (rule.options.Any(x => x is CommonDropNotScalingWithLuck g && g.itemId == ores[0].oreItem)) return true;
					isBar = true;
					if (rule.options.Any(x => x is CommonDropNotScalingWithLuck g && g.itemId == ores[0].bar)) return true;
					isBar = false;
					return false;
				}
				foreach (OneFromRulesRule rule in loot.FindDropRules<OneFromRulesRule>(Match)) {
					CommonDropNotScalingWithLuck first = (CommonDropNotScalingWithLuck)rule.options[0];
					rule.options = ores.Select(ore => {
						return ItemDropRule.NotScalingWithLuck(isBar ? ore.bar : ore.oreItem, 1, first.amountDroppedMinimum, first.amountDroppedMaximum);
					}).ToArray();
				}
			}
		}
	}
}