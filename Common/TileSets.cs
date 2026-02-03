using AltLibrary.Common.AltBiomes;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common {
	[ReinitializeDuringResizeArrays]
	public static class TileSets {
		public static Color?[] BiomeSightColors { get; } = TileID.Sets.Factory.CreateCustomSet<Color?>(null);
		public static int[] OwnedByBiomeID { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(OwnedByBiomeID))
			.RegisterIntSet(-1);
		public static AltBiome GetOwnerBiome(int tileType) => AltLibrary.GetAltBiome(OwnedByBiomeID[tileType]);
	}
}
