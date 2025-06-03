using System.Collections.Generic;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres; 
public class CopperOreSlot : OreSlot {
	public override AltOre FallbackOre => ModContent.GetInstance<CopperAltOre>();
}
public class IronOreSlot : OreSlot {
	public override AltOre FallbackOre => ModContent.GetInstance<IronAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<CopperOreSlot>()
	];
}
public class SilverOreSlot : OreSlot {
	public override AltOre FallbackOre => ModContent.GetInstance<SilverAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<IronOreSlot>()
	];
}
public class GoldOreSlot : OreSlot {
	public override AltOre FallbackOre => ModContent.GetInstance<GoldAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<SilverOreSlot>()
	];
}

public class CobaltOreSlot : OreSlot {
	public override bool Hardmode => true;
	public override AltOre FallbackOre => ModContent.GetInstance<CobaltAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<GoldOreSlot>()
	];
}
public class MythrilOreSlot : OreSlot {
	public override bool Hardmode => true;
	public override AltOre FallbackOre => ModContent.GetInstance<MythrilAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<CobaltOreSlot>()
	];
}
public class AdamantiteOreSlot : OreSlot {
	public override bool Hardmode => true;
	public override AltOre FallbackOre => ModContent.GetInstance<AdamantiteAltOre>();
	public override IEnumerable<OreSlot> SortAfter() => [
		ModContent.GetInstance<MythrilOreSlot>()
	];
}