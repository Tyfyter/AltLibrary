using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using MonoMod.Cil;
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
		try {
			IL_Lang.CreateDialogSubstitutionObject += IL_Lang_CreateDialogSubstitutionObject;
		} catch { }
	}

	private static void IL_Lang_CreateDialogSubstitutionObject(ILContext il) {
		ILCursor c = new(il);
		c.GotoNext(MoveType.AfterLabel, i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.crimson)));
		ILLabel label = c.DefineLabel();
		c.EmitBr(label);
		c.Next.Next.MatchBrtrue(out ILLabel crimLabel);
		c.GotoLabel(crimLabel, MoveType.AfterLabel);
		c.Prev.MatchBr(out ILLabel doneLabel);
		c.GotoLabel(doneLabel, MoveType.AfterLabel);
		c.MarkLabel(label);
		c.EmitDelegate(() => WorldBiomeManager.GetWorldEvil(includeDrunk: true)?.WorldEvilStone.Value);
	}

	public static void Unload() {
		predicate = null;
	}
	public static void ReplacePredicate(Condition condition, Func<bool> replacement) {
		predicate.SetValue(condition, replacement);
	}
}
