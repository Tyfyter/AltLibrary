using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common {
	[ReinitializeDuringResizeArrays]
	public static class TileSets {
		public static Color?[] BiomeSightColors { get; } = TileID.Sets.Factory.CreateCustomSet<Color?>(null);
	}
}
