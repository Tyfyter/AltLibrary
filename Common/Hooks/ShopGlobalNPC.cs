using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Conditions;
using AltLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	public class ShopGlobalNPC : GlobalNPC {
		public override void ModifyShop(NPCShop shop) {
			foreach (var entry in shop.Entries) {
				var conditions = ALReflection.ShopEntry_conditions.GetValue(entry);
				for (int i = 0; i < conditions.Count; i++) {
					if (conditions[i] == Condition.CorruptWorld) {
						if (entry.Item.type == ItemID.CrimsonSeeds) {
							conditions[i] = ShopConditions.NotWorldEvilCondition<CrimsonAltBiome>();
						} else {
							conditions[i] = ShopConditions.GetWorldEvilCondition<CorruptionAltBiome>();
						}
					} else if (conditions[i] == Condition.CrimsonWorld) {
						if (entry.Item.type == ItemID.CorruptSeeds) {
							conditions[i] = ShopConditions.NotWorldEvilCondition<CorruptionAltBiome>();
						} else {
							conditions[i] = ShopConditions.GetWorldEvilCondition<CrimsonAltBiome>();
						}
					}
				}
			}
		}
	}
}
