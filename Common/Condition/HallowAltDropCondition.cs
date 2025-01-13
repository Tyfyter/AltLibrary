using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AltLibrary.Common.Conditions {
	public class HallowAltDropCondition(AltBiome biomeType) : IItemDropRuleCondition {
		public AltBiome BiomeType = biomeType;

		public bool CanDrop(DropAttemptInfo info) {
			if (!info.IsInSimulation) {
				return WorldBiomeManager.GetWorldHallow(true) == BiomeType;
			}
			return false;
		}

		public bool CanShowItemDropInUI() {
			return WorldBiomeManager.GetWorldHallow(true) == BiomeType;
		}

		public string GetConditionDescription() {
			return Language.GetTextValue("Mods.AltLibrary.DropRule.Base", BiomeType.DisplayName.Value);
		}
	}
}
