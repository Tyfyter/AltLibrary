using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using PegasusLib;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace AltLibrary.Core;
internal class ALHooks {
	static FastFieldInfo<Condition, Func<bool>> predicate;
	public static void OnInitialize() {
		predicate = new("<Predicate>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
		CorruptionAltBiome corruption = ModContent.GetInstance<CorruptionAltBiome>();
		CrimsonAltBiome crimson = ModContent.GetInstance<CrimsonAltBiome>();
		ReplacePredicate(Condition.CorruptWorld, () => WorldBiomeManager.GetWorldEvil(true, true) == corruption);
		ReplacePredicate(Condition.CrimsonWorld, () => WorldBiomeManager.GetWorldEvil(true, true) == crimson);
		ReplacePredicate(Condition.DownedEaterOfWorlds, () => NPC.downedBoss2 && WorldBiomeManager.GetWorldEvil(true, true) == corruption);
		ReplacePredicate(Condition.DownedBrainOfCthulhu, () => NPC.downedBoss2 && WorldBiomeManager.GetWorldEvil(true, true) == crimson);
		ReplacePredicate(Condition.NotDownedEaterOfWorlds, () => !NPC.downedBoss2 && WorldBiomeManager.GetWorldEvil(true, true) == corruption);
		ReplacePredicate(Condition.NotDownedBrainOfCthulhu, () => !NPC.downedBoss2 && WorldBiomeManager.GetWorldEvil(true, true) == crimson);

		On_Conditions.IsCorruption.CanDrop += (orig, self, info) => WorldBiomeManager.GetWorldEvil(true, true) == corruption;
		On_Conditions.IsCorruption.CanShowItemDropInUI += (orig, self) => WorldBiomeManager.GetWorldEvil(true, true) == corruption;
		On_Conditions.IsCrimson.CanDrop += (orig, self, info) => WorldBiomeManager.GetWorldEvil(true, true) == crimson;
		On_Conditions.IsCrimson.CanShowItemDropInUI += (orig, self) => WorldBiomeManager.GetWorldEvil(true, true) == crimson;

		On_Conditions.IsCorruptionAndNotExpert.CanDrop += (orig, self, info) => !Main.expertMode && WorldBiomeManager.GetWorldEvil(true, true) == corruption;
		On_Conditions.IsCorruptionAndNotExpert.CanShowItemDropInUI += (orig, self) => !Main.expertMode && WorldBiomeManager.GetWorldEvil(true, true) == corruption;
		On_Conditions.IsCrimsonAndNotExpert.CanDrop += (orig, self, info) => !Main.expertMode && WorldBiomeManager.GetWorldEvil(true, true) == crimson;
		On_Conditions.IsCrimsonAndNotExpert.CanShowItemDropInUI += (orig, self) => !Main.expertMode && WorldBiomeManager.GetWorldEvil(true, true) == crimson;
	}

	public static void Unload() {
		predicate = null;
	}
	public static void ReplacePredicate(Condition condition, Func<bool> replacement) {
		predicate.SetValue(condition, replacement);
	}
}
