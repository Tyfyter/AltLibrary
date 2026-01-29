using AltLibrary.Common.AltBiomes;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common.Systems {
	internal class RewriterSystem : ModSystem {
		public override void OnWorldLoad() {
			WorldBiomeManager.WorldEvilName ??= "";
			WorldBiomeManager.WorldHallowName ??= "";
			WorldBiomeManager.WorldHellName ??= "";
			WorldBiomeManager.WorldJungleName ??= "";
			WorldBiomeManager.drunkEvilName ??= "";
			WorldBiomeManager.DrunkEvil = null;
		}
		public static void ValidateBiomeSelections() {
			HashSet<AltBiome> biomes = [];
			biomes.Add(WorldBiomeManager.WorldEvilBiome);
			biomes.Add(WorldBiomeManager.WorldHallowBiome);
			biomes.Add(WorldBiomeManager.WorldHell);
			biomes.Add(WorldBiomeManager.WorldJungle);
			HashSet<BiomeType> hasBiomes = [];
			foreach (AltBiome biome in biomes) {
				if (biome is null) continue;
				hasBiomes.Add(biome.BiomeType);
				switch (biome.BiomeType) {
					case BiomeType.Evil:
					WorldBiomeManager.WorldEvilBiome = biome;
					break;
					case BiomeType.Hallow:
					WorldBiomeManager.WorldHallowBiome = biome;
					break;
					case BiomeType.Hell:
					WorldBiomeManager.WorldHell = biome;
					break;
					case BiomeType.Jungle:
					WorldBiomeManager.WorldJungle = biome;
					break;
				}
			}
			if (hasBiomes.Add(BiomeType.Evil)) WorldBiomeManager.WorldEvilBiome = WorldGen.crimson ? GetInstance<CrimsonAltBiome>() : GetInstance<CorruptionAltBiome>();
			if (hasBiomes.Add(BiomeType.Hallow)) WorldBiomeManager.WorldHallowBiome = GetInstance<HallowAltBiome>();
			if (hasBiomes.Add(BiomeType.Hell)) WorldBiomeManager.WorldHell = GetInstance<UnderworldAltBiome>();
			if (hasBiomes.Add(BiomeType.Jungle)) WorldBiomeManager.WorldJungle = GetInstance<JungleAltBiome>();

			if (WorldBiomeManager.WorldEvilName != "" && WorldGen.crimson) {
				WorldGen.crimson = false;
			}
		}
	}
}
