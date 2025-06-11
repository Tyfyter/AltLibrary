using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	[ReinitializeDuringResizeArrays]
	public class AltVines : GlobalTile {
		public static int[] VineTypeForAnchor { get; } = TileID.Sets.Factory.CreateIntSet(-1);
		public static void AddVine(int vine, params int[] anchors) {
			TileID.Sets.IsVine[vine] = true;
			VineTypeForAnchor[vine] = vine;
			for (int i = 0; i < anchors.Length; i++) VineTypeForAnchor[anchors[i]] = vine;
		}
		public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak) {
			if (TileID.Sets.IsVine[type]) {
				int typeForAnchor = VineTypeForAnchor[Main.tile[i, j - 1].TileType];
				if (typeForAnchor != -1) Main.tile[i, j].TileType = (ushort)typeForAnchor;
			}
			return true;
		}
	}
}
