using AltLibrary.Core;
using AltLibrary.Core.Baking;
using AltLibrary.Core.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes
{
	public abstract class VanillaBiome : AltBiome
	{
		public override string IconSmall => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow";
		public override string Name => name;
		public override Color NameColor => nameColor;

		private readonly string name;
		private readonly Color nameColor;
		public override int ConversionType => 0;

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
		}
	}
	public class CorruptionAltBiome : VanillaBiome {
		public override EvilBiomeGenerationPass GetEvilBiomeGenerationPass() => corruptPass;
		public override int ConversionType => 1;
		public CorruptionAltBiome() : base("CorruptBiome", BiomeType.Evil, -333, Color.MediumPurple, false) { }
		public override void SetStaticDefaults() {
			AddConversion(TileID.Ebonstone, TileID.Stone);
			AddConversion(TileID.CorruptJungleGrass, TileID.JungleGrass);
			AddConversion(TileID.CorruptGrass, TileID.Grass);
			AddConversion(TileID.CorruptIce, TileID.IceBlock);
			AddConversion(TileID.Ebonsand, TileID.Sand);
			AddConversion(TileID.CorruptHardenedSand, TileID.HardenedSand);
			AddConversion(TileID.CorruptSandstone, TileID.Sandstone);
			AddConversion(TileID.CorruptThorns, TileID.JungleThorns);
		}
	}
	public class CrimsonAltBiome : VanillaBiome {
		public override EvilBiomeGenerationPass GetEvilBiomeGenerationPass() => crimsonPass;
		public override int ConversionType => 1;
		public CrimsonAltBiome() : base("CrimsonBiome", BiomeType.Evil, -666, Color.IndianRed, true) { }
		public override void SetStaticDefaults() {
			AddConversion(TileID.Crimstone, TileID.Stone);
			AddConversion(TileID.CrimsonJungleGrass, TileID.JungleGrass);
			AddConversion(TileID.CrimsonGrass, TileID.Grass);
			AddConversion(TileID.FleshIce, TileID.IceBlock);
			AddConversion(TileID.Crimsand, TileID.Sand);
			AddConversion(TileID.CrimsonHardenedSand, TileID.HardenedSand);
			AddConversion(TileID.CrimsonSandstone, TileID.Sandstone);
			AddConversion(TileID.CrimsonThorns, TileID.JungleThorns);
		}
	}
	public class HallowAltBiome : VanillaBiome {
		public HallowAltBiome() : base("HallowBiome", BiomeType.Hallow, -3, Color.HotPink) { }
		public override int ConversionType => 2;
		public override void SetStaticDefaults() {
			AddConversion(TileID.Pearlstone, TileID.Stone);
			AddConversion(TileID.HallowedGrass, TileID.Grass);
			AddConversion(TileID.GolfGrassHallowed, TileID.GolfGrass);

			AddConversion(TileID.HallowedIce, TileID.IceBlock);
			AddConversion(TileID.Pearlsand, TileID.Sand);
			AddConversion(TileID.HallowHardenedSand, TileID.HardenedSand);
			AddConversion(TileID.HallowSandstone, TileID.Sandstone);

			AddConversion(TileID.JungleThorns, -2);

			for (int i = 0; i < WallLoader.WallCount; i++) {
				if (WallID.Sets.CanBeConvertedToGlowingMushroom[i]) {
					WallContext.AddReplacement(i, WallID.MushroomUnsafe);
				}
			}
		}
		public override int GetAltBlock(int BaseBlock, int k, int l) {
			if (BaseBlock == TileID.Mud && (Main.tile[k - 1, l].TileType == TileID.HallowedGrass || Main.tile[k + 1, l].TileType == TileID.HallowedGrass || Main.tile[k, l - 1].TileType == TileID.HallowedGrass || Main.tile[k, l + 1].TileType == TileID.HallowedGrass)) {
				return TileID.Dirt;
			}
			return base.GetAltBlock(BaseBlock, k, l);
		}
	}
	public class JungleAltBiome : VanillaBiome {
		public JungleAltBiome() : base("JungleBiome", BiomeType.Jungle, -4, Color.SpringGreen) { }
	}
	public class UnderworldAltBiome : VanillaBiome {
		public UnderworldAltBiome() : base("UnderworldBiome", BiomeType.Hell, -5, Color.OrangeRed) { }
	}
	public class DeconvertAltBiome : VanillaBiome {
		public override int ConversionType => 0;
		public DeconvertAltBiome() : base("Deconvert", BiomeType.None, -1, Color.Green) {}
		public override int GetAltBlock(int BaseBlock, int posX, int posY) {
			return ALConvertInheritanceData.tileParentageData.Deconversion.TryGetValue(BaseBlock, out int val) ? val : -1;
		}
	}
	public class MushroomAltBiome : VanillaBiome {
		public override int ConversionType => 3;
		public MushroomAltBiome() : base("Mushroom", BiomeType.None, -1, Color.Blue) { }
		public override void SetStaticDefaults() {
			AddConversion(TileID.MushroomGrass, TileID.JungleGrass);
			AddConversion(-2, TileID.JungleThorns);
			for (int i = 0; i < WallLoader.WallCount; i++) {
				if (WallID.Sets.CanBeConvertedToGlowingMushroom[i]) {
					WallContext.AddReplacement(i, WallID.MushroomUnsafe);
				}
			}
		}
	}
	#region 1.4.4 solutions
	public class DesertAltBiome : VanillaBiome {
		public DesertAltBiome() : base("DesertBiome", BiomeType.None, -1, Color.SandyBrown) { }
		public override int ConversionType => 0;
		public override void SetStaticDefaults() {

			AddConversion(TileID.Sand, TileID.Grass, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Sand, TileID.Sand, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Sand, TileID.SnowBlock, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Sand, TileID.Dirt, spread: false, oneWay: true, extraFunctions: false);

			AddConversion(TileID.HardenedSand, TileID.HardenedSand, spread: false, oneWay: true, extraFunctions: false);

			AddConversion(TileID.Sandstone, TileID.Stone, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Sandstone, TileID.IceBlock, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Sandstone, TileID.Sandstone, spread: false, oneWay: true, extraFunctions: false);

			AddConversion(-2, TileID.JungleThorns);
			
			for (int i = 0; i < WallLoader.WallCount; i++) {
				if ((WallID.Sets.Conversion.Stone[i] || WallID.Sets.Conversion.NewWall1[i] || WallID.Sets.Conversion.NewWall2[i] || WallID.Sets.Conversion.NewWall3[i] || WallID.Sets.Conversion.NewWall4[i] || WallID.Sets.Conversion.Ice[i] || WallID.Sets.Conversion.Sandstone[i])) {
					WallContext.AddReplacement(i, WallID.Sandstone);
				} else if ((WallID.Sets.Conversion.HardenedSand[i] || WallID.Sets.Conversion.Dirt[i] || WallID.Sets.Conversion.Snow[i])) {
					WallContext.AddReplacement(i, WallID.HardenedSand);
				}
			}
		}
		public override int GetAltBlock(int BaseBlock, int k, int l) {
			int value = base.GetAltBlock(BaseBlock, k, l);
			if (value == TileID.Sand) {
				if (WorldGen.BlockBelowMakesSandConvertIntoHardenedSand(k, l)) {
					value = TileID.HardenedSand;
				}
			}
			return value;
		}
	}
	public class SnowAltBiome : VanillaBiome {
		public SnowAltBiome() : base("SnowBiome", BiomeType.None, -1, Color.White) { }
		public override int ConversionType => 0;
		public override void SetStaticDefaults() {
			AddConversion(TileID.SnowBlock, TileID.Grass, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.SnowBlock, TileID.Sand, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.SnowBlock, TileID.HardenedSand, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.SnowBlock, TileID.SnowBlock, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.SnowBlock, TileID.Dirt, spread: false, oneWay: true, extraFunctions: false);

			AddConversion(TileID.IceBlock, TileID.Stone, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.IceBlock, TileID.IceBlock, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.IceBlock, TileID.Sandstone, spread: false, oneWay: true, extraFunctions: false);

			AddConversion(-2, TileID.JungleThorns);

			for (int i = 0; i < WallLoader.WallCount; i++) {
				if ((WallID.Sets.Conversion.Stone[i] || WallID.Sets.Conversion.NewWall1[i] || WallID.Sets.Conversion.NewWall2[i] || WallID.Sets.Conversion.NewWall3[i] || WallID.Sets.Conversion.NewWall4[i] || WallID.Sets.Conversion.Ice[i] || WallID.Sets.Conversion.Sandstone[i])) {
					WallContext.AddReplacement(i, WallID.IceUnsafe);
				} else if ((WallID.Sets.Conversion.HardenedSand[i] || WallID.Sets.Conversion.Dirt[i] || WallID.Sets.Conversion.Snow[i])) {
					WallContext.AddReplacement(i, WallID.SnowWallUnsafe);
				}
			}
		}
	}
	public class ForestAltBiome : VanillaBiome {
		public ForestAltBiome() : base("ForestBiome", BiomeType.None, -1, Color.Brown) { }
		public override int ConversionType => 0;
		public override void SetStaticDefaults() {
			AddConversion(TileID.Grass, TileID.Grass, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.GolfGrass, TileID.GolfGrass);

			AddConversion(TileID.Dirt, TileID.Sand, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Dirt, TileID.SnowBlock, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Dirt, TileID.Dirt, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Dirt, TileID.HardenedSand, spread: false, oneWay: true, extraFunctions: false);

			AddConversion(TileID.Stone, TileID.Stone, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Stone, TileID.IceBlock, spread: false, oneWay: true, extraFunctions: false);
			AddConversion(TileID.Stone, TileID.Sandstone, spread: false, oneWay: true, extraFunctions: false);

			AddConversion(-2, TileID.JungleThorns, spread: false, oneWay: true, extraFunctions: false);

			for (int i = 0; i < WallLoader.WallCount; i++) {
				if ((WallID.Sets.Conversion.Stone[i] || WallID.Sets.Conversion.NewWall1[i] || WallID.Sets.Conversion.NewWall2[i] || WallID.Sets.Conversion.NewWall3[i] || WallID.Sets.Conversion.NewWall4[i] || WallID.Sets.Conversion.Ice[i] || WallID.Sets.Conversion.Sandstone[i])) {
					WallContext.AddReplacement(i, WallID.Sandstone);
				} else if ((WallID.Sets.Conversion.HardenedSand[i] || WallID.Sets.Conversion.Dirt[i] || WallID.Sets.Conversion.Snow[i])) {
					WallContext.AddReplacement(i, WallID.HardenedSand);
				}
			}
		}
		public override int GetAltBlock(int BaseBlock, int k, int l) {
			int value = base.GetAltBlock(BaseBlock, k, l);
			if (value == TileID.Dirt) {
				if (WorldGen.TileIsExposedToAir(k, l)) {
					value = TileID.Grass;
				}
			}
			return value;
		}
	}
	#endregion
}
