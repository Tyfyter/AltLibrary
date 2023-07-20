using AltLibrary.Common.AltOres;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AltLibrary.Common.Hooks {
	public static class ExtractinatorOres {
		private const string OldNote = "Yes, this is a detour, and it WILL have incompatibilities with ILs, but it was made for the sake of other detours.";
		private const string NoteToFox = "Could you explain what the above note meant? My experience with competing detours has consisted of detours that don't call orig breaking those that don't, and being fixed most effectively by using IL";

		public static List<int> Gems;
		public static List<int> PrehardmodeOres;
		public static List<int> Ores;

		internal static void Load()
		{
			Gems = new()
			{
				ItemID.Amethyst,
				ItemID.Topaz,
				ItemID.Sapphire,
				ItemID.Emerald,
				ItemID.Ruby,
				ItemID.Diamond,
			};

			PrehardmodeOres = new()
			{
				ItemID.CopperOre,
				ItemID.TinOre,
				ItemID.IronOre,
				ItemID.LeadOre,
				ItemID.SilverOre,
				ItemID.TungstenOre,
				ItemID.GoldOre,
				ItemID.PlatinumOre
			};
			Ores = new()
			{
				ItemID.CopperOre,
				ItemID.TinOre,
				ItemID.IronOre,
				ItemID.LeadOre,
				ItemID.SilverOre,
				ItemID.TungstenOre,
				ItemID.GoldOre,
				ItemID.PlatinumOre
			};

			foreach (AltOre o in AltLibrary.Ores.Where(x => x.IncludeInExtractinator)) {
				int oreItem = TileLoader.GetItemDropFromTypeAndStyle(o.ore);
				if (o.OreType >= OreType.Copper && o.OreType <= OreType.Gold) PrehardmodeOres.Add(oreItem);
				Ores.Add(oreItem);
			}

			IL_Player.ExtractinatorUse += Player_ExtractinatorUse;
		}

		internal static void Unload()
		{

			Gems = null;
			Ores = null;
		}
		static void Player_ExtractinatorUse(ILContext il) {
			ILCursor c = new(il);
			ILLabel elseBlock = default;
			try {
				c.GotoNext(MoveType.After,
					ins => ins.MatchLdarg(2),
					ins => ins.MatchLdcI4(TileID.ChlorophyteExtractinator),
					ins => ins.MatchBneUn(out elseBlock)
				);
				for (int i = 0; i < 2; i++) {
					int randParam = -1;
					Func<Instruction, bool>[] predicates = {
						ins => ins.MatchCall<Main>("get_rand"),
						ins => ins.MatchLdcI4(out _),
						ins => ins.MatchCall<UnifiedRandom>("Next"),
						ins => ins.MatchStloc(out randParam),
						ins => ins.MatchLdloc(randParam),
						ins => ins.MatchSwitch(out _),
						ins => ins.MatchBr(out _)
					};
					c.GotoNext(MoveType.Before, predicates);
					c.RemoveRange(predicates.Length);
					int itemType = -1;
					loop:
					if (c.RemoveMatching(
						ins => ins.MatchLdcI4(out _),
						ins => ins.MatchStloc(out itemType),
						ins => ins.MatchBr(out _)
					)) goto loop;
					c.RemoveMatchingThrow(AltLibrary.Instance, il,
						ins => ins.MatchLdcI4(out _),
						ins => ins.MatchStloc(itemType)
					);
					c.Emit(OpCodes.Ldloca, itemType);
					switch (i) {
						case 0:
						c.EmitDelegate<_SetExtractinatorDrop>(SetGreenExtractinatorDrop);
						break;
						case 1:
						c.EmitDelegate<_SetExtractinatorDrop>(SetExtractinatorDrop);
						break;
					}
				}
			} catch (Exception e) {
				if (e is not ILPatchFailureException) MonoModHooks.DumpIL(AltLibrary.Instance, il);
				throw;
			}
		}
		delegate void _SetExtractinatorDrop(ref int itemType);
		static void SetExtractinatorDrop(ref int itemType) {
			itemType = Main.rand.Next(PrehardmodeOres);
		}
		static void SetGreenExtractinatorDrop(ref int itemType) {
			itemType = Main.rand.Next(Ores);
		}
	}
}
