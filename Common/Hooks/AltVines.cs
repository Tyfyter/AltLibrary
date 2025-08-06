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
		public override void SetStaticDefaults() {
			AddVine(TileID.JungleVines, TileID.JungleGrass);
			AddVine(TileID.HallowedVines, TileID.HallowedGrass);
			AddVine(TileID.CorruptVines, TileID.CorruptGrass, TileID.CorruptJungleGrass);
			AddVine(TileID.CrimsonVines, TileID.CrimsonGrass, TileID.CrimsonJungleGrass);
			AddVine(TileID.VineFlowers, TileID.VineFlowers);
			AddVine(TileID.Vines, TileID.Vines);
			AddVine(TileID.MushroomVines, TileID.MushroomGrass);
			AddVine(TileID.AshVines, TileID.AshGrass);
		}
		public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak) {
			if (TileID.Sets.IsVine[type]) {
				int typeForAnchor = VineTypeForAnchor[Main.tile[i, j - 1].TileType];
				switch (Main.tile[i, j - 1].TileType) {
					case TileID.Grass:
					typeForAnchor = type == TileID.VineFlowers ? TileID.VineFlowers : TileID.Vines;
					break;
				}
				if (typeForAnchor != -1 && type != typeForAnchor) {
					Main.tile[i, j].TileType = (ushort)typeForAnchor;
					WorldGen.SquareTileFrame(i, j);
				}
			}
			return true;
		}
	}
}
