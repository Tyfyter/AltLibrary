using AltLibrary.Common.AltBiomes;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common.Systems {
	internal class RewriterSystem : ModSystem {
		public override void OnWorldLoad() {
			WorldBiomeManager.WorldEvil ??= WorldGen.crimson ? GetInstance<CrimsonAltBiome>() : GetInstance<CorruptionAltBiome>();
			WorldBiomeManager.WorldHallow ??= GetInstance<HallowAltBiome>();
			WorldBiomeManager.WorldHell ??= GetInstance<UnderworldAltBiome>();
			WorldBiomeManager.WorldJungle ??= GetInstance<JungleAltBiome>();
			WorldBiomeManager.drunkEvilName ??= "";
			WorldBiomeManager.DrunkEvil = null;
		}
		public static void ValidateBiomeSelections() {
			HashSet<AltBiome> biomes = [];
			biomes.Add(WorldBiomeManager.WorldEvil);
			biomes.Add(WorldBiomeManager.WorldHallow);
			biomes.Add(WorldBiomeManager.WorldHell);
			biomes.Add(WorldBiomeManager.WorldJungle);
			HashSet<BiomeType> hasBiomes = [];
			foreach (AltBiome biome in biomes) {
				if (biome is null) continue;
				hasBiomes.Add(biome.BiomeType);
				switch (biome.BiomeType) {
					case BiomeType.Evil:
					WorldBiomeManager.WorldEvil = biome;
					break;
					case BiomeType.Hallow:
					WorldBiomeManager.WorldHallow = biome;
					break;
					case BiomeType.Hell:
					WorldBiomeManager.WorldHell = biome;
					break;
					case BiomeType.Jungle:
					WorldBiomeManager.WorldJungle = biome;
					break;
				}
			}
			if (!hasBiomes.Contains(BiomeType.Evil)) WorldBiomeManager.WorldEvil = WorldGen.crimson ? GetInstance<CrimsonAltBiome>() : GetInstance<CorruptionAltBiome>();
			if (!hasBiomes.Contains(BiomeType.Hallow)) WorldBiomeManager.WorldHallow = GetInstance<HallowAltBiome>();
			if (!hasBiomes.Contains(BiomeType.Hell)) WorldBiomeManager.WorldHell = GetInstance<UnderworldAltBiome>();
			if (!hasBiomes.Contains(BiomeType.Jungle)) WorldBiomeManager.WorldJungle = GetInstance<JungleAltBiome>();

			if (WorldBiomeManager.WorldEvil is not VanillaBiome && WorldGen.crimson) {
				WorldGen.crimson = false;
			}
		}
	}
}
