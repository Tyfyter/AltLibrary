using AltLibrary.Common.AltBiomes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.WorldGen;
using TileConversion = Terraria.ID.TileID.Sets.Conversion;
using WallConversion = Terraria.ID.WallID.Sets.Conversion;

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
			AltBiome parentBiome = WallSets.GetOwnerBiome(self.WallType) ?? TileSets.GetOwnerBiome(self.TileType);
			if (parentBiome is VanillaBiome or null) return;
			if (!WallConversion.Grass[self.WallType] && !WallConversion.Stone[self.WallType]) {
				if (!self.HasTile || !TileConversion.Stone[self.TileType]) return;
			}

			int x = i + genRand.Next(-2, 3);
			int y = j + genRand.Next(-2, 3);
			if (!InWorld(x, y, 10)) return;
			ushort wallType = Main.tile[x, y].WallType;
			if (wallType < WallID.GrassUnsafe || wallType > WallID.Flower) return;
			bool foundTile = false;
			for (int k = i - wallDist; k < i + wallDist && !foundTile; k++) {
				for (int l = j - wallDist; l < j + wallDist && !foundTile; l++) {
					if (Main.tile[k, l].HasTile && TileSets.GetOwnerBiome(Main.tile[k, l].TileType) == parentBiome) {
						foundTile = true;
					}
				}
			}
			if (!foundTile) return;

			Convert(x, y, parentBiome.ConversionType, 0, false, true);
		}
		static void On_WorldGen_SpreadDesertWalls(On_WorldGen.orig_SpreadDesertWalls orig, int wallDist, int i, int j) {
			Tile self = Main.tile[i, j];
			if (!InWorld(i, j, 10) || (!WallConversion.Sandstone[self.WallType] && (!self.HasTile || !TileConversion.Sandstone[self.TileType]) && !WallConversion.HardenedSand[self.WallType]))
				return;

			AltBiome parentBiome = WallSets.GetOwnerBiome(self.WallType) ?? TileSets.GetOwnerBiome(self.TileType);
			if (parentBiome is null) return;

			int x = i + genRand.Next(-2, 3);
			int y = j + genRand.Next(-2, 3);
			ushort wallType = Main.tile[x, y].WallType;
			if (!WallConversion.PureSand[wallType]) return;
			if (!WallConversion.Sandstone[wallType] && !WallConversion.HardenedSand[wallType]) return;
			bool foundTile = false;
			for (int k = i - wallDist; k < i + wallDist && !foundTile; k++) {
				for (int l = j - wallDist; l < j + wallDist && !foundTile; l++) {
					if (Main.tile[k, l].HasTile && TileSets.GetOwnerBiome(Main.tile[k, l].TileType) == parentBiome) {
						foundTile = true;
					}
				}
			}
			if (!foundTile) return;

			Convert(x, y, parentBiome.ConversionType, 0, false, true);
		}
	}
}
