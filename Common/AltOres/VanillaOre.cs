using AltLibrary.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres; 
public abstract class VanillaOre<TOreSlot> : AltOre where TOreSlot : OreSlot {
	public override OreSlot OreSlot => ModContent.GetInstance<TOreSlot>();
	public VanillaOre(int ore, int oreItem, int bar, int? watch = null, int? candle = null) {
		this.ore = ore;
		this.oreItem = oreItem;
		this.bar = bar;
		this.Candle = candle;
		this.Watch = watch;
	}
}
public class CopperAltOre() : VanillaOre<CopperOreSlot>(TileID.Copper, ItemID.CopperOre, ItemID.CopperBar, ItemID.CopperWatch) { }
public class TinAltOre() : VanillaOre<CopperOreSlot>(TileID.Tin, ItemID.TinOre, ItemID.TinBar, ItemID.TinWatch) { }
public class IronAltOre() : VanillaOre<IronOreSlot>(TileID.Iron, ItemID.IronOre, ItemID.IronBar) { }
public class LeadAltOre() : VanillaOre<IronOreSlot>(TileID.Lead, ItemID.LeadOre, ItemID.LeadBar) { }
public class SilverAltOre() : VanillaOre<SilverOreSlot>(TileID.Silver, ItemID.SilverOre, ItemID.SilverBar, ItemID.SilverWatch) { }
public class TungstenAltOre() : VanillaOre<SilverOreSlot>(TileID.Tungsten, ItemID.TungstenOre, ItemID.TungstenBar, ItemID.TungstenWatch) { }
public class GoldAltOre() : VanillaOre<GoldOreSlot>(TileID.Gold, ItemID.GoldOre, ItemID.GoldBar, ItemID.GoldWatch, ItemID.Candle) { }
public class PlatinumAltOre() : VanillaOre<GoldOreSlot>(TileID.Platinum, ItemID.PlatinumOre, ItemID.PlatinumBar, ItemID.PlatinumWatch, ItemID.PlatinumCandle) { }
public class CobaltAltOre() : VanillaOre<CobaltOreSlot>(TileID.Cobalt, ItemID.CobaltOre, ItemID.CobaltBar) { }
public class PalladiumAltOre() : VanillaOre<CobaltOreSlot>(TileID.Palladium, ItemID.PalladiumOre, ItemID.PalladiumBar) { }
public class MythrilAltOre() : VanillaOre<MythrilOreSlot>(TileID.Mythril, ItemID.MythrilOre, ItemID.MythrilBar) { }
public class OrichalcumAltOre() : VanillaOre<MythrilOreSlot>(TileID.Orichalcum, ItemID.OrichalcumOre, ItemID.OrichalcumBar) { }
public class AdamantiteAltOre() : VanillaOre<AdamantiteOreSlot>(TileID.Adamantite, ItemID.AdamantiteOre, ItemID.AdamantiteBar) { }
public class TitaniumAltOre() : VanillaOre<AdamantiteOreSlot>(TileID.Titanium, ItemID.TitaniumOre, ItemID.TitaniumBar) { }