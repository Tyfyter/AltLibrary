using AltLibrary.Common.AltBiomes;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common {
	[ReinitializeDuringResizeArrays]
	public static class WallSets {
		public static int[] OwnedByBiomeID { get; } = WallID.Sets.Factory.CreateNamedSet(nameof(OwnedByBiomeID))
			.RegisterIntSet(-1);
		public static AltBiome GetOwnerBiome(int wallType) => AltLibrary.GetAltBiome(OwnedByBiomeID[wallType]);
	}
}
