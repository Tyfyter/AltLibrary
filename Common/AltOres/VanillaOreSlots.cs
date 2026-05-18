using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres; 
public abstract class VanillaOreSlot : OreSlot {
	internal override string RecipeGroupName => Name[..^nameof(OreSlot).Length];
}
public class CopperOreSlot : VanillaOreSlot {
	public override AltOre FallbackOre => ModContent.GetInstance<CopperAltOre>();
}
public class IronOreSlot : VanillaOreSlot {
	public override AltOre FallbackOre => ModContent.GetInstance<IronAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<CopperOreSlot>()
	];
	internal override void SetupRecipeGroups() {
		Bars = RecipeGroup.recipeGroups[RecipeGroupID.IronBar];
		foreach (AltOre ore in OreSlotLoader.GetOres(this)) Bars.ValidItems.Add(ore.bar);
		Ores = new RecipeGroup(
			() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.Ores", RecipeGroupDisplayName.Value)}",
			OreSlotLoader.GetOres(this).Select(o => o.oreItem).ToArray()
		);
		RecipeGroup.RegisterGroup(RecipeGroupName + "Ores", Ores);
	}
}
public class SilverOreSlot : VanillaOreSlot {
	public override AltOre FallbackOre => ModContent.GetInstance<SilverAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<IronOreSlot>()
	];
}
public class GoldOreSlot : VanillaOreSlot {
	public override AltOre FallbackOre => ModContent.GetInstance<GoldAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<SilverOreSlot>()
	];
}

public class CobaltOreSlot : VanillaOreSlot {
	public override bool Hardmode => true;
	public override AltOre FallbackOre => ModContent.GetInstance<CobaltAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<GoldOreSlot>()
	];
}
public class MythrilOreSlot : VanillaOreSlot {
	public override bool Hardmode => true;
	public override AltOre FallbackOre => ModContent.GetInstance<MythrilAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<CobaltOreSlot>()
	];
}
public class AdamantiteOreSlot : VanillaOreSlot {
	public override bool Hardmode => true;
	public override AltOre FallbackOre => ModContent.GetInstance<AdamantiteAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<MythrilOreSlot>()
	];
}