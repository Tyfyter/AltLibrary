using AltLibrary.Common.AltBiomes;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AltLibrary.Common.Hooks
{
	internal class DryadText
	{
		public static void Init()
		{
			Terraria.On_Lang.GetDryadWorldStatusDialog += Lang_GetDryadWorldStatusDialog;
			Terraria.IL_WorldGen.AddUpAlignmentCounts += WorldGen_AddUpAlignmentCounts;
		}

		public static void Unload()
		{
		}

		private static string Lang_GetDryadWorldStatusDialog(Terraria.On_Lang.orig_GetDryadWorldStatusDialog orig, out bool worldIsEntirelyPure)
		{
			string text2;
			int tGood = WorldGen.tGood;
			int tEvil = WorldGen.tEvil + WorldGen.tBlood;
			worldIsEntirelyPure = tEvil == 0 && tGood == 0;
			if (tGood > 0 && tEvil > 0)
			{
				text2 = Language.GetTextValue("Mods.AltLibrary.DryadSpecialText.WorldStatusGoodEvil", Main.worldName, tGood, tEvil);
			}
			else if (tEvil > 0)
			{
				text2 = Language.GetTextValue("Mods.AltLibrary.DryadSpecialText.WorldStatusEvil", Main.worldName, tEvil);
			}
			else
			{
				if (tGood <= 0)
				{
					return Language.GetTextValue("DryadSpecialText.WorldStatusPure", Main.worldName);
				}
				text2 = Language.GetTextValue("Mods.AltLibrary.DryadSpecialText.WorldStatusGood", Main.worldName, tGood);
			}
			string arg = (tGood * 1.2 >= tEvil && tGood * 0.8 <= tEvil) ? Language.GetTextValue("DryadSpecialText.WorldDescriptionBalanced") : ((tGood >= tEvil) ? Language.GetTextValue("DryadSpecialText.WorldDescriptionFairyTale") : ((tEvil > tGood + 20) ? Language.GetTextValue("DryadSpecialText.WorldDescriptionGrim") : ((tEvil <= 5) ? Language.GetTextValue("DryadSpecialText.WorldDescriptionClose") : Language.GetTextValue("DryadSpecialText.WorldDescriptionWork"))));
			return text2 + " " + arg;
		}

		//TODO: double check that this code makes sense to begin with
		private static void WorldGen_AddUpAlignmentCounts(ILContext il)
		{
			ILCursor c = new(il);
			if (!c.TryGotoNext(MoveType.Before, 
				i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.tileCounts)),
				i => i.MatchLdcI4(0),
				i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.tileCounts)),
				i => i.MatchLdlen(),
				i => i.MatchConvI4(),
				i => i.MatchCall<Array>(nameof(Array.Clear))
				))
			{
				AltLibrary.Instance.Logger.Info($"Could not find {nameof(WorldGen.tileCounts)}.{nameof(Array.Clear)} in {nameof(WorldGen.AddUpAlignmentCounts)}");
				return;
			}
			
			c.EmitDelegate<Action>(() => {
				WorldGen.totalGood2 += WorldGen.tileCounts[TileID.HallowHardenedSand] + WorldGen.tileCounts[TileID.HallowSandstone];
				WorldGen.totalEvil2 += WorldGen.tileCounts[TileID.CorruptHardenedSand] + WorldGen.tileCounts[TileID.CorruptSandstone];
				WorldGen.totalBlood2 += WorldGen.tileCounts[TileID.CrimsonHardenedSand] + WorldGen.tileCounts[TileID.CrimsonSandstone];

				WorldGen.totalSolid2 += WorldGen.tileCounts[TileID.HardenedSand] + WorldGen.tileCounts[TileID.Sandstone];
				WorldGen.totalSolid2 += WorldGen.tileCounts[TileID.HallowHardenedSand] + WorldGen.tileCounts[TileID.HallowSandstone];
				WorldGen.totalSolid2 += WorldGen.tileCounts[TileID.CorruptHardenedSand] + WorldGen.tileCounts[TileID.CorruptSandstone];
				WorldGen.totalSolid2 += WorldGen.tileCounts[TileID.CrimsonHardenedSand] + WorldGen.tileCounts[TileID.CrimsonSandstone];

				int hallow = 0;
				int evil = 0;
				foreach (AltBiome biome in AltLibrary.Biomes)
				{
					if (biome.BiomeType == BiomeType.Hallow)
					{
						foreach (var tile in biome.TileConversions.Values) {
							hallow += WorldGen.tileCounts[tile];
						}
					}
					if (biome.BiomeType == BiomeType.Evil) {
						foreach (var tile in biome.TileConversions.Values) {
							evil += WorldGen.tileCounts[tile];
						}
					}
				}

				WorldGen.totalGood2 += hallow;
				WorldGen.totalEvil2 += evil;
				WorldGen.totalSolid2 += hallow + evil;
			});
		}
	}
}
