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
		public static Condition GetWorldEvilCondition(AltBiome biome) => biome.ActiveShopCondition ??= new(
			Language.GetOrRegister("Mods.AltLibrary.Condition.Base").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldEvil(true, true) == biome
		);
		public static Condition GetWorldHallowCondition<T>() where T : AltBiome => GetWorldHallowCondition(GetInstance<T>());
		public static Condition GetWorldHallowCondition(AltBiome biome) => biome.ActiveShopCondition ??= new(
			Language.GetOrRegister("Mods.AltLibrary.Condition.Base").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldHallow(true) == biome
		);
		public static Condition GetWorldJungleCondition<T>() where T : AltBiome => GetWorldJungleCondition(GetInstance<T>());
		public static Condition GetWorldJungleCondition(AltBiome biome) => biome.ActiveShopCondition ??= new(
			Language.GetOrRegister("Mods.AltLibrary.Condition.Base").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldJungle(true) == biome
		);
		public static Condition GetWorldHellCondition<T>() where T : AltBiome => GetWorldHellCondition(GetInstance<T>());
		public static Condition GetWorldHellCondition(AltBiome biome) => biome.ActiveShopCondition ??= new(
			Language.GetOrRegister("Mods.AltLibrary.Condition.Base").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldHell(true) == biome
		);

		public static Condition NotWorldEvilCondition<T>() where T : AltBiome => NotWorldEvilCondition(GetInstance<T>());
		public static Condition NotWorldEvilCondition(AltBiome biome) => biome.InactiveShopCondition ??= new(
			Language.GetOrRegister("Mods.AltLibrary.Condition.NotBase").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldEvil(true, true) != biome
		);
		public static Condition NotWorldHallowCondition<T>() where T : AltBiome => NotWorldHallowCondition(GetInstance<T>());
		public static Condition NotWorldHallowCondition(AltBiome biome) => biome.InactiveShopCondition ??= new(
			Language.GetOrRegister("Mods.AltLibrary.Condition.NotBase").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldHallow(true) != biome
		);
		public static Condition NotWorldJungleCondition<T>() where T : AltBiome => NotWorldJungleCondition(GetInstance<T>());
		public static Condition NotWorldJungleCondition(AltBiome biome) => biome.InactiveShopCondition ??= new(
			Language.GetOrRegister("Mods.AltLibrary.Condition.NotBase").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldJungle(true) != biome
		);
		public static Condition NotWorldHellCondition<T>() where T : AltBiome => NotWorldHellCondition(GetInstance<T>());
		public static Condition NotWorldHellCondition(AltBiome biome) => biome.InactiveShopCondition ??= new(
			Language.GetOrRegister("Mods.AltLibrary.Condition.NotBase").WithFormatArgs(biome.DisplayName),
			() => WorldBiomeManager.GetWorldHell(true) != biome
		);
	}
}
