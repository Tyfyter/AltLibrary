using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.WorldGen;
using WallConversion = Terraria.ID.WallID.Sets.Conversion;
using TileConversion = Terraria.ID.TileID.Sets.Conversion;
using AltLibrary.Core.Baking;
using AltLibrary.Common.AltBiomes;

namespace AltLibrary.Common.Hooks {
	public class WallSpread : ILoadable {
		public void Unload() { }
		public void Load(Mod mod) {
			On_WorldGen.SpreadDesertWalls += On_WorldGen_SpreadDesertWalls;
			On_WorldGen.UpdateWorld_OvergroundTile += (orig, i, j, checkNPCSpawns, wallDist) => {
				orig(i, j, checkNPCSpawns, wallDist);
				UpdateTile(i, j, wallDist);
			};
			On_WorldGen.UpdateWorld_UndergroundTile += (orig, i, j, checkNPCSpawns, wallDist) => {
				orig(i, j, checkNPCSpawns, wallDist);
				UpdateTile(i, j, wallDist);
			};
		}
		static void UpdateTile(int i, int j, int wallDist) {
			Tile self = Main.tile[i, j];
			ALConvertInheritanceData.wallParentageData.TryGetParent(self.WallType, out int parentWall, out AltBiome parentBiome);
			if (parentWall is not WallID.Grass and not WallID.GrassUnsafe && !WallConversion.Stone[self.WallType]) {
				if (!self.HasTile) return;
				ALConvertInheritanceData.tileParentageData.TryGetParent(self.TileType, out int parentTile, out parentBiome);
				if (parentTile != TileID.Stone) return;
			}
			if (parentBiome is VanillaBiome or null) return;

			int x = i + genRand.Next(-2, 3);
			int y = j + genRand.Next(-2, 3);
			if (!InWorld(x, y, 10)) return;
			ref ushort wallType = ref Main.tile[x, y].WallType;
			if (wallType < WallID.GrassUnsafe || wallType > WallID.Flower) return;
			if (!parentBiome.WallConversions.TryGetValue(wallType, out int targetType)) return;
			bool foundTile = false;
			for (int k = i - wallDist; k < i + wallDist && !foundTile; k++) {
				for (int l = j - wallDist; l < j + wallDist && !foundTile; l++) {
					if (Main.tile[k, l].HasTile && ALConvertInheritanceData.tileParentageData.TryGetParent(Main.tile[k, l].TileType, out _, out AltBiome tileBiome) && tileBiome == parentBiome) {
						foundTile = true;
					}
				}
			}
			if (!foundTile) return;

			wallType = (ushort)targetType;
			if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, x, y);
		}
		static void On_WorldGen_SpreadDesertWalls(On_WorldGen.orig_SpreadDesertWalls orig, int wallDist, int i, int j) {
			Tile self = Main.tile[i, j];
			if (!InWorld(i, j, 10) || (!WallConversion.Sandstone[self.WallType] && (!self.HasTile || !TileConversion.Sandstone[self.TileType]) && !WallConversion.HardenedSand[self.WallType]))
				return;

			if (!ALConvertInheritanceData.wallParentageData.TryGetParent(self.WallType, out _, out AltBiome parentBiome)) {
				if (!ALConvertInheritanceData.tileParentageData.TryGetParent(self.TileType, out _, out parentBiome)) return;
			}
			if (parentBiome is null) return;

			int x = i + genRand.Next(-2, 3);
			int y = j + genRand.Next(-2, 3);
			ref ushort wallType = ref Main.tile[x, y].WallType;
			if (!WallConversion.PureSand[wallType]) return;
			if (!WallConversion.Sandstone[wallType] && !WallConversion.HardenedSand[wallType]) return;
			if (!parentBiome.WallConversions.TryGetValue(wallType, out int targetType)) return;
			bool foundTile = false;
			for (int k = i - wallDist; k < i + wallDist && !foundTile; k++) {
				for (int l = j - wallDist; l < j + wallDist && !foundTile; l++) {
					int type = Main.tile[k, l].TileType;
					if (Main.tile[k, l].HasTile && ALConvertInheritanceData.tileParentageData.TryGetParent(Main.tile[k, l].TileType, out _, out AltBiome tileBiome) && tileBiome == parentBiome) {
						foundTile = true;
					}
				}
			}
			if (!foundTile) return;

			wallType = (ushort)targetType;
			if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, x, y);
		}
	}
}
