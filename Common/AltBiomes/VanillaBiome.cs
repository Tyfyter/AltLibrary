using AltLibrary.Core;
using AltLibrary.Core.Baking;
using AltLibrary.Core.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes
{
	public abstract class VanillaBiome : AltBiome {
		public override string IconSmall => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow";
		public override string Name => name;
		public override Color NameColor => nameColor;

		private readonly string name;
		private readonly Color nameColor;
		public override int ConversionType => 0;
		AltMaterialContext materialContext = new();
		public override AltMaterialContext MaterialContext => materialContext;

		internal static readonly EvilBiomeGenerationPass corruptPass = new CorruptionEvilBiomeGenerationPass();
		internal static readonly EvilBiomeGenerationPass crimsonPass = new CrimsonEvilBiomeGenerationPass();

		protected internal VanillaBiome(string name, BiomeType biome, int type, Color nameColor, bool? fix = null) {
			ALReflection.ModType_Mod.SetValue(this, AltLibrary.Instance);
			this.name = name;
			if (name == "CorruptBiome") SpecialValueForWorldUIDoNotTouchElseYouCanBreakStuff = -1;
			if (name == "CrimsonBiome") SpecialValueForWorldUIDoNotTouchElseYouCanBreakStuff = -2;
			if (name == "HallowBiome") SpecialValueForWorldUIDoNotTouchElseYouCanBreakStuff = -3;
			if (name == "JungleBiome") SpecialValueForWorldUIDoNotTouchElseYouCanBreakStuff = -4;
			if (name == "UnderworldBiome") SpecialValueForWorldUIDoNotTouchElseYouCanBreakStuff = -5;
			BiomeType = biome;
			Type = type;
			this.nameColor = nameColor;
			IsForCrimsonOrCorruptWorldUIFix = fix;
			FleshDoorTile = TileID.ClosedDoor;
			FleshChairTile = TileID.Chairs;
			FleshTableTile = TileID.Tables;
			FleshChestTile = TileID.Containers;
		}
	}
	public class CorruptionAltBiome : VanillaBiome {
		public override EvilBiomeGenerationPass GetEvilBiomeGenerationPass() => corruptPass;
		public override int ConversionType => 1;
		public override LocalizedText DryadTextDescriptor => Language.GetText("Mods.AltLibrary.DryadSpecialText.WorldStatusCorrupt");
		public CorruptionAltBiome() : base("CorruptBiome", BiomeType.Evil, -333, Color.MediumPurple, false) { }
		public override void SetStaticDefaults() {
			BiomeOreItem = ItemID.DemoniteOre;
			SeedType = ItemID.CorruptSeeds;
			ArrowType = ItemID.UnholyArrow;

			BiomeChestItem = ItemID.ScourgeoftheCorruptor;
			BiomeChestTile = TileID.Containers;
			BiomeChestTileStyle = 24;

			AddTileConversion(TileID.Ebonstone, TileID.Stone);
			AddTileConversion(TileID.CorruptJungleGrass, TileID.JungleGrass);
			AddTileConversion(TileID.CorruptGrass, TileID.Grass);
			AddTileConversion(TileID.CorruptIce, TileID.IceBlock);
			AddTileConversion(TileID.Ebonsand, TileID.Sand);
			AddTileConversion(TileID.CorruptHardenedSand, TileID.HardenedSand);
			AddTileConversion(TileID.CorruptSandstone, TileID.Sandstone);
			AddTileConversion(TileID.CorruptThorns, TileID.JungleThorns);

			BiomeFlesh = TileID.LesionBlock;
			BiomeFleshWall = WallID.LesionBlock;

			FleshDoorTileStyle = 38;
			FleshChairTileStyle = 38;
			FleshTableTileStyle = 2;
			FleshChestTile = TileID.Containers2;
			FleshChestTileStyle = 3;

			MaterialContext.SetCombinationSword(ItemID.NightsEdge);
			MaterialContext.SetEvilBar(ItemID.DemoniteBar);
			MaterialContext.SetEvilBossDrop(ItemID.ShadowScale);
			MaterialContext.SetEvilHerb(ItemID.Deathweed);
			MaterialContext.SetEvilOre(ItemID.DemoniteOre);
			MaterialContext.SetEvilSword(ItemID.LightsBane);
			MaterialContext.SetVileComponent(ItemID.CursedFlame);
			MaterialContext.SetVileInnard(ItemID.RottenChunk);
			MaterialContext.SetTrueCombinationSword(ItemID.TrueNightsEdge);

			AddWallConversions(WallID.CorruptGrassUnsafe, WallID.Sets.Conversion.Grass);
			AddWallConversions(WallID.EbonstoneUnsafe, WallID.Sets.Conversion.Stone);
			AddWallConversions(WallID.CorruptHardenedSand, WallID.Sets.Conversion.HardenedSand);
			AddWallConversions(WallID.CorruptSandstone, WallID.Sets.Conversion.Sandstone);

			AddWallConversions(WallID.CorruptionUnsafe1, WallID.Sets.Conversion.NewWall1);
			AddWallConversions(WallID.CorruptionUnsafe2, WallID.Sets.Conversion.NewWall2);
			AddWallConversions(WallID.CorruptionUnsafe3, WallID.Sets.Conversion.NewWall3);
			AddWallConversions(WallID.CorruptionUnsafe4, WallID.Sets.Conversion.NewWall4);
		}
	}
	public class CrimsonAltBiome : VanillaBiome {
		public override EvilBiomeGenerationPass GetEvilBiomeGenerationPass() => crimsonPass;
		public override int ConversionType => 1;
		public override LocalizedText DryadTextDescriptor => Language.GetText("Mods.AltLibrary.DryadSpecialText.WorldStatusCrimson");
		public CrimsonAltBiome() : base("CrimsonBiome", BiomeType.Evil, -666, Color.IndianRed, true) { }
		public override void SetStaticDefaults() {
			BiomeOreItem = ItemID.CrimtaneOre;
			SeedType = ItemID.CrimsonSeeds;

			BiomeChestItem = ItemID.VampireKnives;
			BiomeChestTile = TileID.Containers;
			BiomeChestTileStyle = 25;

			AddTileConversion(TileID.Crimstone, TileID.Stone);
			AddTileConversion(TileID.CrimsonJungleGrass, TileID.JungleGrass);
			AddTileConversion(TileID.CrimsonGrass, TileID.Grass);
			AddTileConversion(TileID.FleshIce, TileID.IceBlock);
			AddTileConversion(TileID.Crimsand, TileID.Sand);
			AddTileConversion(TileID.CrimsonHardenedSand, TileID.HardenedSand);
			AddTileConversion(TileID.CrimsonSandstone, TileID.Sandstone);
			AddTileConversion(TileID.CrimsonThorns, TileID.JungleThorns);

			BiomeFlesh = TileID.FleshBlock;
			BiomeFleshWall = WallID.Flesh;

			FleshDoorTileStyle = 5;
			FleshChairTileStyle = 8;
			FleshTableTile = TileID.Tables2;
			FleshTableTileStyle = 5;
			FleshChestTileStyle = 43;

			MaterialContext.SetEvilBar(ItemID.CrimtaneBar);
			MaterialContext.SetEvilBossDrop(ItemID.TissueSample);
			MaterialContext.SetEvilOre(ItemID.CrimtaneOre);
			MaterialContext.SetEvilSword(ItemID.BloodButcherer);
			MaterialContext.SetVileComponent(ItemID.Ichor);
			MaterialContext.SetVileInnard(ItemID.Vertebrae);

			AddWallConversions(WallID.CrimsonGrassUnsafe, WallID.Sets.Conversion.Grass);
			AddWallConversions(WallID.CrimstoneUnsafe, WallID.Sets.Conversion.Stone);
			AddWallConversions(WallID.CrimsonHardenedSand, WallID.Sets.Conversion.HardenedSand);
			AddWallConversions(WallID.CrimsonSandstone, WallID.Sets.Conversion.Sandstone);

			AddWallConversions(WallID.CrimsonUnsafe1, WallID.Sets.Conversion.NewWall1);
			AddWallConversions(WallID.CrimsonUnsafe2, WallID.Sets.Conversion.NewWall2);
			AddWallConversions(WallID.CrimsonUnsafe3, WallID.Sets.Conversion.NewWall3);
			AddWallConversions(WallID.CrimsonUnsafe4, WallID.Sets.Conversion.NewWall4);
		}
	}
	public class HallowAltBiome : VanillaBiome {
		public override LocalizedText DryadTextDescriptor => Language.GetText("Mods.AltLibrary.DryadSpecialText.WorldStatusHallow");
		public HallowAltBiome() : base("HallowBiome", BiomeType.Hallow, -3, Color.HotPink) { }
		public override int ConversionType => 2;
		public override void SetStaticDefaults() {
			BiomeChestItem = ItemID.RainbowGun;
			BiomeChestTile = TileID.Containers;
			BiomeChestTileStyle = 26;

			AddTileConversion(TileID.Pearlstone, TileID.Stone);
			AddTileConversion(TileID.HallowedGrass, TileID.Grass);
			AddTileConversion(TileID.GolfGrassHallowed, TileID.GolfGrass);

			AddTileConversion(TileID.HallowedIce, TileID.IceBlock);
			AddTileConversion(TileID.Pearlsand, TileID.Sand);
			AddTileConversion(TileID.HallowHardenedSand, TileID.HardenedSand);
			AddTileConversion(TileID.HallowSandstone, TileID.Sandstone);

			AddTileConversion(-2, TileID.JungleThorns);

			AddWallConversions(WallID.HallowedGrassUnsafe, WallID.Sets.Conversion.Grass);
			AddWallConversions(WallID.PearlstoneBrickUnsafe, WallID.Sets.Conversion.Stone);
			AddWallConversions(WallID.HallowHardenedSand, WallID.Sets.Conversion.HardenedSand);
			AddWallConversions(WallID.HallowSandstone, WallID.Sets.Conversion.Sandstone);

			AddWallConversions(WallID.HallowUnsafe1, WallID.Sets.Conversion.NewWall1);
			AddWallConversions(WallID.HallowUnsafe2, WallID.Sets.Conversion.NewWall2);
			AddWallConversions(WallID.HallowUnsafe3, WallID.Sets.Conversion.NewWall3);
			AddWallConversions(WallID.HallowUnsafe4, WallID.Sets.Conversion.NewWall4);
		}
		public override int GetAltBlock(int BaseBlock, int k, int l, bool GERunner = false) {
			if (BaseBlock == TileID.Mud && (Main.tile[k - 1, l].TileType == TileID.HallowedGrass || Main.tile[k + 1, l].TileType == TileID.HallowedGrass || Main.tile[k, l - 1].TileType == TileID.HallowedGrass || Main.tile[k, l + 1].TileType == TileID.HallowedGrass)) {
				return TileID.Dirt;
			}
			return base.GetAltBlock(BaseBlock, k, l, GERunner);
		}
	}
	public class JungleAltBiome : VanillaBiome {
		public JungleAltBiome() : base("JungleBiome", BiomeType.Jungle, -4, Color.SpringGreen) { }
		public override void SetStaticDefaults() {
			BiomeChestItem = ItemID.PiranhaGun;
			BiomeChestTile = TileID.Containers;
			BiomeChestTileStyle = 23;
		}
	}
	public class UnderworldAltBiome : VanillaBiome {
		public UnderworldAltBiome() : base("UnderworldBiome", BiomeType.Hell, -5, Color.OrangeRed) { }
	}
	public class DeconvertAltBiome : VanillaBiome {
		public override int ConversionType => 0;
		public DeconvertAltBiome() : base("Deconvert", BiomeType.None, int.MinValue, Color.Green) {}
		public override int GetAltBlock(int BaseBlock, int posX, int posY, bool GERunner = false) {
			return ALConvertInheritanceData.tileParentageData.Deconversion.TryGetValue(BaseBlock, out int val) ? val : -1;
		}
		public override int GetAltWall(int BaseWall, int posX, int posY, bool GERunner = false) {
			return ALConvertInheritanceData.wallParentageData.Deconversion.TryGetValue(BaseWall, out int val) ? val : -1;
		}
	}
	public class MushroomAltBiome : VanillaBiome {
		public override int ConversionType => 3;
		public MushroomAltBiome() : base("Mushroom", BiomeType.None, -1, Color.Blue) { }
		public override void SetStaticDefaults() {
			AddTileConversion(TileID.MushroomGrass, TileID.JungleGrass);
			AddTileConversion(-2, TileID.JungleThorns);
			for (int i = 0; i < WallLoader.WallCount; i++) {
				if (WallID.Sets.CanBeConvertedToGlowingMushroom[i]) {
					AddWallConversions(WallID.MushroomUnsafe, i);
				}
			}
		}
	}
	#region 1.4.4 solutions
	public class DesertAltBiome : VanillaBiome {
		public DesertAltBiome() : base("DesertBiome", BiomeType.None, -2, Color.SandyBrown) { }
		public override int ConversionType => 0;
		public override void SetStaticDefaults() {
			NoDeconversion = true;

			AddTileConversion(TileID.Sand, TileID.Grass, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Sand, TileID.Sand, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Sand, TileID.SnowBlock, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Sand, TileID.Dirt, spread: false, oneWay: true, extraFunctions: false);

			AddTileConversion(TileID.HardenedSand, TileID.HardenedSand, spread: false, oneWay: true, extraFunctions: false);

			AddTileConversion(TileID.Sandstone, TileID.Stone, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Sandstone, TileID.IceBlock, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Sandstone, TileID.Sandstone, spread: false, oneWay: true, extraFunctions: false);

			AddTileConversion(-2, TileID.JungleThorns);
			
			for (int i = 0; i < WallLoader.WallCount; i++) {
				if ((WallID.Sets.Conversion.Stone[i] || WallID.Sets.Conversion.NewWall1[i] || WallID.Sets.Conversion.NewWall2[i] || WallID.Sets.Conversion.NewWall3[i] || WallID.Sets.Conversion.NewWall4[i] || WallID.Sets.Conversion.Ice[i] || WallID.Sets.Conversion.Sandstone[i])) {
					AddWallConversions(WallID.Sandstone, i);
				} else if ((WallID.Sets.Conversion.HardenedSand[i] || WallID.Sets.Conversion.Dirt[i] || WallID.Sets.Conversion.Snow[i])) {
					AddWallConversions(WallID.HardenedSand, i);
				}
			}
		}
		public override int GetAltBlock(int BaseBlock, int k, int l, bool GERunner = false) {
			int value = base.GetAltBlock(BaseBlock, k, l, GERunner);
			if (value == TileID.Sand && BaseBlock != TileID.Sand) {
				if (WorldGen.BlockBelowMakesSandConvertIntoHardenedSand(k, l)) {
					value = TileID.HardenedSand;
				}
			}
			return value;
		}
	}
	public class SnowAltBiome : VanillaBiome {
		public SnowAltBiome() : base("SnowBiome", BiomeType.None, -2, Color.White) { }
		public override int ConversionType => 0;
		public override void SetStaticDefaults() {
			NoDeconversion = true;

			AddTileConversion(TileID.SnowBlock, TileID.Grass, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.SnowBlock, TileID.Sand, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.SnowBlock, TileID.HardenedSand, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.SnowBlock, TileID.SnowBlock, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.SnowBlock, TileID.Dirt, spread: false, oneWay: true, extraFunctions: false);

			AddTileConversion(TileID.IceBlock, TileID.Stone, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.IceBlock, TileID.IceBlock, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.IceBlock, TileID.Sandstone, spread: false, oneWay: true, extraFunctions: false);

			AddTileConversion(-2, TileID.JungleThorns);

			for (int i = 0; i < WallLoader.WallCount; i++) {
				if ((WallID.Sets.Conversion.Stone[i] || WallID.Sets.Conversion.NewWall1[i] || WallID.Sets.Conversion.NewWall2[i] || WallID.Sets.Conversion.NewWall3[i] || WallID.Sets.Conversion.NewWall4[i] || WallID.Sets.Conversion.Ice[i] || WallID.Sets.Conversion.Sandstone[i])) {
					AddWallConversions(WallID.IceUnsafe, i);
				} else if ((WallID.Sets.Conversion.HardenedSand[i] || WallID.Sets.Conversion.Dirt[i] || WallID.Sets.Conversion.Snow[i])) {
					AddWallConversions(WallID.SnowWallUnsafe, i);
				}
			}
		}
	}
	public class ForestAltBiome : VanillaBiome {
		public ForestAltBiome() : base("ForestBiome", BiomeType.None, -2, Color.Brown) { }
		public override int ConversionType => 0;
		public override void SetStaticDefaults() {
			NoDeconversion = true;

			AddTileConversion(TileID.Grass, TileID.Grass, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.GolfGrass, TileID.GolfGrass);

			AddTileConversion(TileID.Dirt, TileID.Sand, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Dirt, TileID.SnowBlock, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Dirt, TileID.Dirt, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Dirt, TileID.HardenedSand, spread: false, oneWay: true, extraFunctions: false);

			AddTileConversion(TileID.Stone, TileID.Stone, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Stone, TileID.IceBlock, spread: false, oneWay: true, extraFunctions: false);
			AddTileConversion(TileID.Stone, TileID.Sandstone, spread: false, oneWay: true, extraFunctions: false);

			AddTileConversion(-2, TileID.JungleThorns, spread: false, oneWay: true, extraFunctions: false);

			for (int i = 0; i < WallLoader.WallCount; i++) {
				if (WallID.Sets.Conversion.Stone[i] || WallID.Sets.Conversion.Ice[i] || WallID.Sets.Conversion.Sandstone[i]) {
					AddWallConversions(WallID.Stone, i);
				} else if (WallID.Sets.Conversion.HardenedSand[i] || WallID.Sets.Conversion.Dirt[i] || WallID.Sets.Conversion.Snow[i]) {
					AddWallConversions(WallID.DirtUnsafe, i);
				} else if (WallID.Sets.Conversion.NewWall1[i]) {
					AddWallConversions(WallID.DirtUnsafe1, i);
				} else if (WallID.Sets.Conversion.NewWall2[i]) {
					AddWallConversions(WallID.DirtUnsafe2, i);
				} else if (WallID.Sets.Conversion.NewWall3[i]) {
					AddWallConversions(WallID.DirtUnsafe3, i);
				} else if (WallID.Sets.Conversion.NewWall4[i]) {
					AddWallConversions(WallID.DirtUnsafe4, i);
				}
			}
		}
		public override int GetAltBlock(int BaseBlock, int k, int l, bool GERunner = false) {
			int value = base.GetAltBlock(BaseBlock, k, l, GERunner);
			if (value == TileID.Dirt && BaseBlock != TileID.Dirt) {
				if (WorldGen.TileIsExposedToAir(k, l)) {
					value = TileID.Grass;
				}
			}
			return value;
		}
	}
	#endregion
	public class UnloadedAltBiome : AltBiome {
		public override string IconSmall => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow";
		public override string Name => name;
		public override Color NameColor => nameColor;

		private readonly string name;
		private readonly Color nameColor;
		public override int ConversionType => 0;
		AltMaterialContext materialContext = new();
		public override AltMaterialContext MaterialContext => materialContext;
		public UnloadedAltBiome(string name, BiomeType biome) {
			ALReflection.ModType_Mod.SetValue(this, AltLibrary.Instance);
			this.name = name;
			BiomeType = biome;
			Type = -1;
			nameColor = Color.Magenta;
		}
	}
}
