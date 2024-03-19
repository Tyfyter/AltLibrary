using AltLibrary.Common.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AltLibrary.Common.Conditions
{
	internal class HallowDropCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			return !info.IsInSimulation && WorldBiomeManager.WorldHallowName == "";
		}

		public bool CanShowItemDropInUI()
		{
			return WorldBiomeManager.WorldHallowName == "";
		}

		public string GetConditionDescription()
		{
			return $"{Language.GetTextValue("Mods.AltLibrary.DropRule.Base")} {Language.GetTextValue("Mods.AltLibrary.AltBiomeName.HallowBiome")}";
		}
	}
}
