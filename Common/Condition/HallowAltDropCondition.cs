using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AltLibrary.Common.Conditions
{
	internal class HallowAltDropCondition : IItemDropRuleCondition
	{
		public AltBiome BiomeType;
		public HallowAltDropCondition(AltBiome biomeType)
		{
			BiomeType = biomeType;
		}

		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation && BiomeType.FullName != null && BiomeType.FullName != "")
			{
				if (WorldBiomeManager.WorldHallow == BiomeType.FullName) return WorldBiomeManager.WorldHallow == BiomeType.FullName;
			}
			return false;
		}

		public bool CanShowItemDropInUI()
		{
			return WorldBiomeManager.WorldHallow == BiomeType.FullName;
		}

		public string GetConditionDescription()
		{
			return Language.GetTextValue("Mods.AltLibrary.DropRule.Base", BiomeType.DisplayName.Value);
		}
	}
}
