using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AltLibrary.Common.Conditions {
	public abstract class AltBiomeDropCondition(AltBiome biomeType) : IItemDropRuleCondition {
		public readonly AltBiome BiomeType = biomeType;
		public abstract AltBiome GetWorldBiome();
		public bool CanDrop(DropAttemptInfo info) => GetWorldBiome() == BiomeType;
		public bool CanShowItemDropInUI() => GetWorldBiome() == BiomeType;
		public string GetConditionDescription() => Language.GetTextValue("Mods.AltLibrary.DropRule.Base", BiomeType.DisplayName.Value);
	}
	public class EvilAltDropCondition(AltBiome biomeType, bool includeDrunk = true) : AltBiomeDropCondition(biomeType) {
		public override AltBiome GetWorldBiome() => WorldBiomeManager.GetWorldEvil(true, includeDrunk);
	}
	public class HallowAltDropCondition(AltBiome biomeType) : AltBiomeDropCondition(biomeType) {
		public override AltBiome GetWorldBiome() => WorldBiomeManager.WorldHallow;
	}
	public class HellAltDropCondition(AltBiome biomeType) : AltBiomeDropCondition(biomeType) {
		public override AltBiome GetWorldBiome() => WorldBiomeManager.WorldHell;
	}
	public class JungleAltDropCondition(AltBiome biomeType) : AltBiomeDropCondition(biomeType) {
		public override AltBiome GetWorldBiome() => WorldBiomeManager.WorldJungle;
	}
}
