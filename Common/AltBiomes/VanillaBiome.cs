using AltLibrary.Core;
using AltLibrary.Core.Baking;
using AltLibrary.Core.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes
{
	internal abstract class VanillaBiome : AltBiome
	{
		public override string IconSmall => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow";
		public override string Name => name;
		public override Color NameColor => nameColor;

		private readonly string name;
		private readonly Color nameColor;
		public override int ConversionType => 0;
		public override Dictionary<int, int> TileConversions => new();
		internal virtual Dictionary<int, int> WallConversions => new();

		internal static readonly EvilBiomeGenerationPass corruptPass = new CorruptionEvilBiomeGenerationPass();
		internal static readonly EvilBiomeGenerationPass crimsonPass = new CrimsonEvilBiomeGenerationPass();

		public VanillaBiome(string name, BiomeType biome, int type, Color nameColor, bool? fix = null) {
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
	internal class CorruptionAltBiome : VanillaBiome {
		public override EvilBiomeGenerationPass GetEvilBiomeGenerationPass() => corruptPass;
		public CorruptionAltBiome() : base("CorruptBiome", BiomeType.Evil, -333, Color.MediumPurple, false) { }
		public override int ConversionType => 1;
		public override Dictionary<int, int> TileConversions => ALConvertInheritanceData.tileParentageData.CorruptionConversion;
		internal override Dictionary<int, int> WallConversions => ALConvertInheritanceData.wallParentageData.CorruptionConversion;
	}
	internal class CrimsonAltBiome : VanillaBiome {
		public override EvilBiomeGenerationPass GetEvilBiomeGenerationPass() => crimsonPass;
		public CrimsonAltBiome() : base("CrimsonBiome", BiomeType.Evil, -666, Color.IndianRed, true) { }
		public override int ConversionType => 1;
		public override Dictionary<int, int> TileConversions => ALConvertInheritanceData.tileParentageData.CrimsonConversion;
		internal override Dictionary<int, int> WallConversions => ALConvertInheritanceData.wallParentageData.CrimsonConversion;
	}
	internal class HallowAltBiome : VanillaBiome {
		public HallowAltBiome() : base("HallowBiome", BiomeType.Hallow, -3, Color.HotPink) { }
		public override int ConversionType => 2;
		public override Dictionary<int, int> TileConversions => ALConvertInheritanceData.tileParentageData.HallowConversion;
		internal override Dictionary<int, int> WallConversions => ALConvertInheritanceData.wallParentageData.HallowConversion;
	}
	internal class JungleAltBiome : VanillaBiome {
		public JungleAltBiome() : base("JungleBiome", BiomeType.Jungle, -4, Color.SpringGreen) { }
	}
	internal class UnderworldAltBiome : VanillaBiome {
		public UnderworldAltBiome() : base("UnderworldBiome", BiomeType.Hell, -5, Color.OrangeRed) { }
	}
	public class HHHHHAltBiome : AltBiome {
		public override string IconSmall => null;
		public override void SetStaticDefaults() {
			BiomeType = BiomeType.Hallow;
		}// && AltLibraryConfig.Config.BiomeIconsVisibleOutside
	}
}
