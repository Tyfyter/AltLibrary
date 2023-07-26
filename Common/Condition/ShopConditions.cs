using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common.Conditions {
	public static class ShopConditions {
		public static Condition GetWorldEvilCondition<T>() where T : AltBiome => GetWorldEvilCondition(GetInstance<T>());
		public static Condition GetWorldEvilCondition(AltBiome biome) => new Condition(
			Language.GetOrRegister("Mods.AltLibrary.Condition.Base").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldEvil(true) == biome
		);
		public static Condition GetWorldHallowCondition<T>() where T : AltBiome => GetWorldHallowCondition(GetInstance<T>());
		public static Condition GetWorldHallowCondition(AltBiome biome) => new Condition(
			Language.GetOrRegister("Mods.AltLibrary.Condition.Base").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldHallow(true) == biome
		);
		public static Condition GetWorldJungleCondition<T>() where T : AltBiome => GetWorldJungleCondition(GetInstance<T>());
		public static Condition GetWorldJungleCondition(AltBiome biome) => new Condition(
			Language.GetOrRegister("Mods.AltLibrary.Condition.Base").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldJungle(true) == biome
		);
		public static Condition GetWorldHellCondition<T>() where T : AltBiome => GetWorldHellCondition(GetInstance<T>());
		public static Condition GetWorldHellCondition(AltBiome biome) => new Condition(
			Language.GetOrRegister("Mods.AltLibrary.Condition.Base").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldHell(true) == biome
		);
	}
}
