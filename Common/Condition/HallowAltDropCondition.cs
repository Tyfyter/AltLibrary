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
			if (!info.IsInSimulation)
			{
				return WorldBiomeManager.GetWorldHallow(true) == BiomeType;
			}
			return false;
		}

		public bool CanShowItemDropInUI()
		{
			return WorldBiomeManager.GetWorldHallow(true) == BiomeType;
		}

		public string GetConditionDescription()
		{
			return Language.GetTextValue("Mods.AltLibrary.DropRule.Base", BiomeType.DisplayName.Value);
		}
	}
}
