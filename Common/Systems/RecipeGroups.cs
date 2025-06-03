using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.Systems {
	//TODO: works fine, but it's ugly, make it better
	public class RecipeGroups : ModSystem {
		public static RecipeGroup EvilOres;
		public static RecipeGroup EvilBars;
		public static RecipeGroup HallowBars;
		public static RecipeGroup HellBars;
		public static RecipeGroup JungleBars;
		public static RecipeGroup MushroomBars;
		public static RecipeGroup EvilSwords;
		public static RecipeGroup HallowSwords;
		public static RecipeGroup HellSwords;
		public static RecipeGroup JungleSwords;
		public static RecipeGroup ComboSwords;
		public static RecipeGroup TrueComboSwords;
		public static RecipeGroup TrueHallowSwords;
		public static RecipeGroup RottenChunks;
		public static RecipeGroup PixieDusts;
		public static RecipeGroup UnicornHorns;
		public static RecipeGroup CrystalShards;
		public static RecipeGroup CursedFlames;
		public static RecipeGroup ShadowScales;
		public static RecipeGroup JungleSpores;
		public static RecipeGroup Deathweed;
		public static RecipeGroup Fireblossom;
		public static RecipeGroup Moonglow;
		public static RecipeGroup Hellforges;
		public static RecipeGroup CopperBars;
		public static RecipeGroup IronOres;
		public static RecipeGroup IronBars;
		public static RecipeGroup SilverBars;
		public static RecipeGroup GoldOres;
		public static RecipeGroup GoldBars;
		public static RecipeGroup CobaltBars;
		public static RecipeGroup MythrilBars;
		public static RecipeGroup AdamantiteBars;
		public static RecipeGroup GoldCandles;
		public static RecipeGroup CopperWatches;
		public static RecipeGroup SilverWatches;
		public static RecipeGroup GoldWatches;
		public static RecipeGroup SoulsOfEvil;

		public static Dictionary<int, Func<int>> AppropriateMaterials;
		public override void Unload() {
			EvilOres = null;
			EvilBars = null;
			HallowBars = null;
			HellBars = null;
			JungleBars = null;
			MushroomBars = null;
			EvilSwords = null;
			HallowSwords = null;
			HellSwords = null;
			JungleSwords = null;
			ComboSwords = null;
			TrueComboSwords = null;
			TrueHallowSwords = null;
			RottenChunks = null;
			PixieDusts = null;
			UnicornHorns = null;
			CrystalShards = null;
			CursedFlames = null;
			ShadowScales = null;
			JungleSpores = null;
			Deathweed = null;
			Fireblossom = null;
			Moonglow = null;
			Hellforges = null;
			CopperBars = null;
			IronBars = null;
			SilverBars = null;
			GoldBars = null;
			CobaltBars = null;
			MythrilBars = null;
			AdamantiteBars = null;
			GoldCandles = null;
			CopperWatches = null;
			SilverWatches = null;
			GoldWatches = null;
			GoldOres = null;
			IronOres = null;
			SoulsOfEvil = null;

			AppropriateMaterials = null;
		}

		public override void AddRecipeGroups() {
			static int DefaultTo(int? value, int @default) {
				return (value ?? -1) == -1 ? @default : value.Value;
			}
			AppropriateMaterials = [];
			List<AltBiome> Hell = AltLibrary.Biomes.FindAll(x => x.BiomeType == BiomeType.Hell);
			List<AltBiome> Light = AltLibrary.Biomes.FindAll(x => x.BiomeType == BiomeType.Hallow);
			List<AltBiome> Evil = AltLibrary.Biomes.FindAll(x => x.BiomeType == BiomeType.Evil);
			List<AltBiome> Tropic = AltLibrary.Biomes.FindAll(x => x.BiomeType == BiomeType.Jungle);

			List<int> array = [ItemID.DemoniteOre, ItemID.CrimtaneOre];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.EvilOre != -1) {
					array.Add(x.MaterialContext.EvilOre);
				}
			});
			EvilOres = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.EvilOres")}", array.ToArray());
			RecipeGroup.RegisterGroup("EvilOres", EvilOres);
			AppropriateMaterials.Add(EvilOres.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.EvilOre, ItemID.DemoniteOre);
			});

			array = [ItemID.DemoniteBar, ItemID.CrimtaneBar];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.EvilBar != -1) {
					array.Add(x.MaterialContext.EvilBar);
				}
			});
			EvilBars = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.EvilBars")}", array.ToArray());
			RecipeGroup.RegisterGroup("EvilBars", EvilBars);
			AppropriateMaterials.Add(EvilBars.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.EvilBar, ItemID.DemoniteBar);
			});

			array = [ItemID.HallowedBar];
			Light.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.LightBar != -1) {
					array.Add(x.MaterialContext.LightBar);
				}
			});
			HallowBars = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.HallowBars")}", array.ToArray());
			RecipeGroup.RegisterGroup("HallowBars", HallowBars);
			AppropriateMaterials.Add(HallowBars.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHallow(true).MaterialContext?.LightBar, ItemID.HallowedBar);
			});

			array = [ItemID.HellstoneBar];
			Hell.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.UnderworldBar != -1) {
					array.Add(x.MaterialContext.UnderworldBar);
				}
			});
			HellBars = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.HellBars")}", array.ToArray());
			RecipeGroup.RegisterGroup("HellBars", HellBars);
			AppropriateMaterials.Add(HellBars.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHell(true).MaterialContext?.UnderworldBar, ItemID.HellstoneBar);
			});

			array = [ItemID.ChlorophyteBar];
			Tropic.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.TropicalBar != -1) {
					array.Add(x.MaterialContext.TropicalBar);
				}
			});
			JungleBars = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.JungleBars")}", array.ToArray());
			RecipeGroup.RegisterGroup("JungleBars", JungleBars);
			AppropriateMaterials.Add(JungleBars.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldJungle(true).MaterialContext?.TropicalBar, ItemID.ChlorophyteBar);
			});

			array = [ItemID.ShroomiteBar];
			Tropic.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.MushroomBar != -1) {
					array.Add(x.MaterialContext.MushroomBar);
				}
			});
			MushroomBars = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.MushroomBars")}", array.ToArray());
			RecipeGroup.RegisterGroup("MushroomBars", MushroomBars);
			AppropriateMaterials.Add(MushroomBars.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldJungle(true).MaterialContext?.MushroomBar, ItemID.ShroomiteBar);
			});

			array = [ItemID.LightsBane, ItemID.BloodButcherer];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.EvilSword != -1) {
					array.Add(x.MaterialContext.EvilSword);
				}
			});
			EvilSwords = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.EvilSwords")}", array.ToArray());
			RecipeGroup.RegisterGroup("EvilSwords", EvilSwords);
			AppropriateMaterials.Add(EvilSwords.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.EvilSword, ItemID.LightsBane);
			});

			array = [ItemID.Excalibur];
			Light.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.LightSword != -1) {
					array.Add(x.MaterialContext.LightSword);
				}
			});
			HallowSwords = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.HallowSwords")}", array.ToArray());
			RecipeGroup.RegisterGroup("HallowSwords", HallowSwords);
			AppropriateMaterials.Add(HallowSwords.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHallow(true).MaterialContext?.LightSword, ItemID.Excalibur);
			});

			array = [ItemID.FieryGreatsword];
			Hell.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.UnderworldSword != -1) {
					array.Add(x.MaterialContext.UnderworldSword);
				}
			});
			HellSwords = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.HellSwords")}", array.ToArray());
			RecipeGroup.RegisterGroup("HellSwords", HellSwords);
			AppropriateMaterials.Add(HellSwords.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHell(true).MaterialContext?.UnderworldSword, ItemID.FieryGreatsword);
			});

			array = [ItemID.BladeofGrass];
			Tropic.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.TropicalSword != -1) {
					array.Add(x.MaterialContext.TropicalSword);
				}
			});
			JungleSwords = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.JungleSwords")}", array.ToArray());
			RecipeGroup.RegisterGroup("JungleSwords", JungleSwords);
			AppropriateMaterials.Add(JungleSwords.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldJungle(true).MaterialContext?.TropicalSword, ItemID.BladeofGrass);
			});

			array = [ItemID.NightsEdge];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.CombinationSword != -1) {
					array.Add(x.MaterialContext.CombinationSword);
				}
			});
			ComboSwords = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.ComboSwords")}", array.ToArray());
			RecipeGroup.RegisterGroup("ComboSwords", ComboSwords);
			AppropriateMaterials.Add(ComboSwords.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.CombinationSword, ItemID.NightsEdge);
			});

			array = [ItemID.TrueNightsEdge];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.TrueCombinationSword != -1) {
					array.Add(x.MaterialContext.TrueCombinationSword);
				}
			});
			TrueComboSwords = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.TrueComboSwords")}", array.ToArray());
			RecipeGroup.RegisterGroup("TrueComboSwords", TrueComboSwords);
			AppropriateMaterials.Add(TrueComboSwords.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.TrueCombinationSword, ItemID.TrueNightsEdge);
			});

			array = [ItemID.TrueExcalibur];
			Light.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.TrueLightSword != -1) {
					array.Add(x.MaterialContext.TrueLightSword);
				}
			});
			TrueHallowSwords = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.TrueHallowSwords")}", array.ToArray());
			RecipeGroup.RegisterGroup("TrueHallowSwords", TrueHallowSwords);
			AppropriateMaterials.Add(TrueHallowSwords.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHallow(true).MaterialContext?.TrueLightSword, ItemID.TrueExcalibur);
			});

			array = [ItemID.RottenChunk, ItemID.Vertebrae];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.VileInnard != -1) {
					array.Add(x.MaterialContext.VileInnard);
				}
			});
			RottenChunks = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.RottenChunks")}", array.ToArray());
			RecipeGroup.RegisterGroup("RottenChunks", RottenChunks);
			AppropriateMaterials.Add(RottenChunks.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.VileInnard, ItemID.RottenChunk);
			});

			array = [ItemID.PixieDust];
			Light.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.LightResidue != -1) {
					array.Add(x.MaterialContext.LightResidue);
				}
			});
			PixieDusts = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.PixieDusts")}", array.ToArray());
			RecipeGroup.RegisterGroup("PixieDusts", PixieDusts);
			AppropriateMaterials.Add(PixieDusts.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHallow(true).MaterialContext?.LightResidue, ItemID.PixieDust);
			});

			array = [ItemID.UnicornHorn];
			Light.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.LightInnard != -1) {
					array.Add(x.MaterialContext.LightInnard);
				}
			});
			UnicornHorns = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.UnicornHorns")}", array.ToArray());
			RecipeGroup.RegisterGroup("UnicornHorns", UnicornHorns);
			AppropriateMaterials.Add(UnicornHorns.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHallow(true).MaterialContext?.LightInnard, ItemID.UnicornHorn);
			});

			array = [ItemID.CrystalShard];
			Light.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.LightComponent != -1) {
					array.Add(x.MaterialContext.LightComponent);
				}
			});
			CrystalShards = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.CrystalShards")}", array.ToArray());
			RecipeGroup.RegisterGroup("CrystalShards", CrystalShards);
			AppropriateMaterials.Add(CrystalShards.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHallow(true).MaterialContext?.LightComponent, ItemID.CrystalShard);
			});

			array = [ItemID.CursedFlame, ItemID.Ichor];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.VileComponent != -1) {
					array.Add(x.MaterialContext.VileComponent);
				}
			});
			CursedFlames = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.CursedFlames")}", array.ToArray());
			RecipeGroup.RegisterGroup("CursedFlames", CursedFlames);
			AppropriateMaterials.Add(CursedFlames.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.VileComponent, ItemID.CursedFlame);
			});

			array = [ItemID.ShadowScale, ItemID.TissueSample];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.EvilBossDrop != -1) {
					array.Add(x.MaterialContext.EvilBossDrop);
				}
			});
			ShadowScales = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.ShadowScales")}", array.ToArray());
			RecipeGroup.RegisterGroup("ShadowScales", ShadowScales);
			AppropriateMaterials.Add(ShadowScales.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.EvilBossDrop, ItemID.ShadowScale);
			});

			array = [ItemID.JungleSpores];
			Tropic.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.TropicalComponent != -1) {
					array.Add(x.MaterialContext.TropicalComponent);
				}
			});
			JungleSpores = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.JungleSpores")}", array.ToArray());
			RecipeGroup.RegisterGroup("JungleSpores", JungleSpores);

			array = [ItemID.Deathweed];
			Evil.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.EvilHerb != -1) {
					array.Add(x.MaterialContext.EvilHerb);
				}
			});
			Deathweed = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.Deathweed")}", array.ToArray());
			RecipeGroup.RegisterGroup("Deathweed", Deathweed);
			AppropriateMaterials.Add(Deathweed.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldEvil(true, true).MaterialContext?.EvilHerb, ItemID.Deathweed);
			});

			array = [ItemID.Fireblossom];
			Hell.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.UnderworldHerb != -1) {
					array.Add(x.MaterialContext.UnderworldHerb);
				}
			});
			Fireblossom = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.Fireblossom")}", array.ToArray());
			RecipeGroup.RegisterGroup("Fireblossom", Fireblossom);
			AppropriateMaterials.Add(Fireblossom.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHell(true).MaterialContext?.UnderworldHerb, ItemID.Fireblossom);
			});

			array = [ItemID.Moonglow];
			Tropic.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.TropicalHerb != -1) {
					array.Add(x.MaterialContext.TropicalHerb);
				}
			});
			Moonglow = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.Moonglow")}", array.ToArray());
			RecipeGroup.RegisterGroup("Moonglow", Moonglow);
			AppropriateMaterials.Add(Moonglow.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldJungle(true).MaterialContext?.TropicalHerb, ItemID.Moonglow);
			});

			array = [ItemID.Hellforge];
			Hell.ForEach(x => {
				if (x.MaterialContext != null && x.MaterialContext.UnderworldForge != -1) {
					array.Add(x.MaterialContext.UnderworldForge);
				}
			});
			Hellforges = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.Hellforges")}", array.ToArray());
			RecipeGroup.RegisterGroup("Hellforges", Hellforges);
			AppropriateMaterials.Add(Hellforges.RegisteredId, () => {
				return DefaultTo(WorldBiomeManager.GetWorldHell(true).MaterialContext?.UnderworldForge, ItemID.Hellforge);
			});


			CopperBars = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.CopperBars")}",
				OreSlotLoader.GetOres<CopperOreSlot>().Select(o => o.bar).ToArray()
			);
			RecipeGroup.RegisterGroup("CopperBars", CopperBars);

			IronOres = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.IronOres")}",
				OreSlotLoader.GetOres<IronOreSlot>().Select(o => o.oreItem).ToArray()
			);
			RecipeGroup.RegisterGroup("IronOres", IronOres);

			IronBars = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.IronBars")}",
				OreSlotLoader.GetOres<IronOreSlot>().Select(o => o.bar).ToArray()
			);
			RecipeGroup.RegisterGroup("IronBars", IronBars);

			SilverBars = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.SilverBars")}",
				OreSlotLoader.GetOres<SilverOreSlot>().Select(o => o.bar).ToArray()
			);
			RecipeGroup.RegisterGroup("SilverBars", SilverBars);
			
			GoldOres = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.GoldOres")}",
				OreSlotLoader.GetOres<GoldOreSlot>().Select(o => o.oreItem).ToArray()
			);
			RecipeGroup.RegisterGroup("GoldOres", GoldOres);

			GoldBars = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.GoldBars")}",
				OreSlotLoader.GetOres<GoldOreSlot>().Select(o => o.bar).ToArray()
			);
			RecipeGroup.RegisterGroup("GoldBars", GoldBars);

			CobaltBars = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.CobaltBars")}",
				OreSlotLoader.GetOres<CobaltOreSlot>().Select(o => o.bar).ToArray()
			);
			RecipeGroup.RegisterGroup("CobaltBars", CobaltBars);

			MythrilBars = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.MythrilBars")}",
				OreSlotLoader.GetOres<MythrilOreSlot>().Select(o => o.bar).ToArray()
			);
			RecipeGroup.RegisterGroup("MythrilBars", MythrilBars);

			AdamantiteBars = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.AdamantiteBars")}",
				OreSlotLoader.GetOres<AdamantiteOreSlot>().Select(o => o.bar).ToArray()
			);
			RecipeGroup.RegisterGroup("AdamantiteBars", AdamantiteBars);

			GoldCandles = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.GoldCandles")}",
				OreSlotLoader.GetOres<GoldOreSlot>().Select(o => o.Candle ?? -1).Where(x => x != -1).ToArray()
			);
			RecipeGroup.RegisterGroup("GoldCandles", GoldCandles);

			CopperWatches = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.CopperWatches")}",
				OreSlotLoader.GetOres<CopperOreSlot>().Select(o => o.Watch ?? -1).Where(x => x != -1).ToArray()
			);
			RecipeGroup.RegisterGroup("CopperWatches", CopperWatches);

			SilverWatches = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.SilverWatches")}",
				OreSlotLoader.GetOres<SilverOreSlot>().Select(o => o.Watch ?? -1).Where(x => x != -1).ToArray()
			);
			RecipeGroup.RegisterGroup("SilverWatches", SilverWatches);

			GoldWatches = new RecipeGroup(
				() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue("Mods.AltLibrary.RecipeGroups.GoldWatches")}",
				OreSlotLoader.GetOres<GoldOreSlot>().Select(o => o.Watch ?? -1).Where(x => x != -1).ToArray()
			);
			RecipeGroup.RegisterGroup("GoldWatches", GoldWatches);
		}
	}
}
