using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
using AltLibrary.Core;
using AltLibrary.Core.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace AltLibrary.Common.Systems {
	public class WorldBiomeGeneration : ModSystem {
		public static class ChangeRange {
			static int minX, maxX, minY, maxY;
			public static void ResetRange() {
				minX = int.MaxValue;
				maxX = int.MinValue;

				minY = int.MaxValue;
				maxY = int.MinValue;
			}
			public static void AddChangeToRange(int i, int j) {
				if (i < minX) minX = i;
				if (i > maxX) maxX = i;

				if (j < minY) minY = j;
				if (j > maxY) maxY = j;
			}
			public static Rectangle GetRange() {
				return new(
					minX,
					minY,
					maxX - minX,
					maxY - minY
				);
			}
		}
		public static int WofKilledTimes { get; internal set; } = 0;
		static List<Rectangle> _evilBiomeGenRanges = [];
		public static ref List<Rectangle> EvilBiomeGenRanges => ref _evilBiomeGenRanges;

		public override void Unload() {
			EvilBiomeGenRanges = [];
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
			int resetIndex = tasks.FindIndex(genpass => genpass.Name == "Reset");
			if (resetIndex != -1) {
				tasks.Insert(resetIndex + 1, new PassLegacy("Alt Library Setup", new WorldGenLegacyMethod(WorldSetupTask)));
			}
			int corruptionIndex = tasks.FindIndex(i => i.Name.Equals("Corruption"));
			if ((WorldBiomeManager.IsAnyModdedEvil || WorldGen.drunkWorldGen || ModSupport.FargoSeeds.BothEvils()) && corruptionIndex != -1) {
				tasks[corruptionIndex] = new PassLegacy("Corruption", new WorldGenLegacyMethod(EvilTaskGen));
			}
			AltBiome[] biomesToProcess = [
				WorldBiomeManager.GetWorldEvil(false),
				WorldBiomeManager.GetWorldHallow(false),
				WorldBiomeManager.GetWorldJungle(false),
				WorldBiomeManager.GetWorldHell(false)
			];
			List<GenPass> passes = [];
			for (int i = 0; i < tasks.Count; i++) {
				passes.Clear();
				GenPass pass = tasks[i];
				passes.Add(pass);
				for (int j = 0; j < biomesToProcess.Length; j++) {
					biomesToProcess[j]?.ModifyGenPass(passes, pass);
				}
				bool passedPass = false;
				for (int j = 0; j < passes.Count; j++) {
					if (passes[j] == pass) {
						passedPass = true;
					} else {
						if (passedPass) tasks.Insert(++i, passes[j]);
						else tasks.Insert(i++, passes[j]);
					}
				}
			}
		}

		private void EvilTaskGen(GenerationProgress progress, GameConfiguration configuration) {
			EvilBiomeGenerationPassHandler.GenerateAllCorruption(GenVars.dungeonSide, GenVars.dungeonLocation, progress);
		}

		private void WorldSetupTask(GenerationProgress progress, GameConfiguration configuration) {
			AltOre ore = WorldBiomeManager.GetAltOre<CopperOreSlot>();
			GenVars.copper = WorldGen.SavedOreTiers.Copper = ore.ore;
			GenVars.copperBar = ore.bar;

			ore = WorldBiomeManager.GetAltOre<IronOreSlot>();
			GenVars.iron = WorldGen.SavedOreTiers.Iron = ore.ore;
			GenVars.ironBar = ore.bar;

			ore = WorldBiomeManager.GetAltOre<SilverOreSlot>();
			GenVars.silver = WorldGen.SavedOreTiers.Silver = ore.ore;
			GenVars.silverBar = ore.bar;

			ore = WorldBiomeManager.GetAltOre<GoldOreSlot>();
			GenVars.gold = WorldGen.SavedOreTiers.Gold = ore.ore;
			GenVars.goldBar = ore.bar;

			WorldGen.SavedOreTiers.Cobalt = WorldBiomeManager.GetAltOre<CobaltOreSlot>().ore;

			WorldGen.SavedOreTiers.Mythril = WorldBiomeManager.GetAltOre<MythrilOreSlot>().ore;

			WorldGen.SavedOreTiers.Adamantite = WorldBiomeManager.GetAltOre<AdamantiteOreSlot>().ore;

			if (WorldGen.drunkWorldGen) {
				List<AltBiome> biomes = AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Evil && x.Selectable).Append(ModContent.GetInstance<CorruptionAltBiome>()).Append(ModContent.GetInstance<CrimsonAltBiome>()).ToList();
				int index = WorldGen.genRand.Next(biomes.Count);
				AltBiome mainEvil = biomes[index];
				biomes.RemoveAt(index);
				WorldBiomeManager.DrunkEvil = biomes[WorldGen.genRand.Next(biomes.Count)];
			}
		}
		public override void ClearWorld() {
			EvilBiomeGenRanges = [];
		}
		public override void SaveWorldData(TagCompound tag) {
			tag.Add("AltLibrary:WofKilledTimes", WofKilledTimes);
			tag.Add("AltLibrary:EvilBiomeGenRanges", EvilBiomeGenRanges);
		}

		public override void LoadWorldData(TagCompound tag) {
			WofKilledTimes = tag.GetInt("AltLibrary:WofKilledTimes");
			if (!tag.TryGet("AltLibrary:EvilBiomeGenRanges", out EvilBiomeGenRanges)) EvilBiomeGenRanges = [];
		}
	}
}
