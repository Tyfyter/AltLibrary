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
						conditions[i] = ShopConditions.GetWorldEvilCondition<CorruptionAltBiome>();
					} else if (conditions[i] == Condition.CrimsonWorld) {
						conditions[i] = ShopConditions.GetWorldEvilCondition<CrimsonAltBiome>();
					}
				}
			}
		}
	}
}
