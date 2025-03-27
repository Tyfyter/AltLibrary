using AltLibrary.Common.AltOres;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PegasusLib.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AltLibrary.Common.Hooks {
	public static class ExtractinatorOres {
		public static List<int> PrehardmodeOres { get; private set; }
		public static List<int> Ores { get; private set; }
		internal static void Setup() {
			PrehardmodeOres = [
				ItemID.CopperOre,
				ItemID.TinOre,
				ItemID.IronOre,
				ItemID.LeadOre,
				ItemID.SilverOre,
				ItemID.TungstenOre,
				ItemID.GoldOre,
				ItemID.PlatinumOre,
				ItemID.ToiletDynasty
			];
			Ores = [
				ItemID.CopperOre,
				ItemID.TinOre,
				ItemID.IronOre,
				ItemID.LeadOre,
				ItemID.SilverOre,
				ItemID.TungstenOre,
				ItemID.GoldOre,
				ItemID.PlatinumOre,
				ItemID.CobaltOre,
				ItemID.PalladiumOre,
				ItemID.MythrilOre,
				ItemID.OrichalcumOre,
				ItemID.AdamantiteOre,
				ItemID.TitaniumOre,
			];

			foreach (AltOre o in AltLibrary.Ores.Where(x => x.IncludeInExtractinator)) {
				int oreItem = TileLoader.GetItemDropFromTypeAndStyle(o.ore);
				if (o.OreType >= OreType.Copper && o.OreType <= OreType.Gold) PrehardmodeOres.Add(oreItem);
				Ores.Add(oreItem);
			}
		}
		internal static void Load() {
			IL_Player.ExtractinatorUse += Player_ExtractinatorUse;
		}

		internal static void Unload() {
			PrehardmodeOres = null;
			Ores = null;
		}
		static void Player_ExtractinatorUse(ILContext il) {
			MethodInfo next = typeof(Utils).GetMethods().FirstOrDefault(m => {
				if (m.Name != nameof(Utils.Next)) return false;
				if (!m.ContainsGenericParameters) return false;
				if (m.GetParameters().Length <= 1) return false;
				Type parameterType = m.GetParameters()[1].ParameterType;
				if (!parameterType.IsGenericType) return false;
				return parameterType.GetGenericTypeDefinition() == typeof(IList<>);
			}).MakeGenericMethod(typeof(int));
			void DoThing(int defaultType, string oresName) {
				ILCursor c = new(il);
				ILLabel[] cases = default;
				ILLabel defaultCase = default;
				c.GotoNext(MoveType.AfterLabel,
					i => i.MatchSwitch(out cases),
					i => i.MatchBr(out defaultCase) && defaultCase.Target.MatchLdcI4(defaultType)
				);
				MonoModMethods.SkipPrevArgument(c);
				defaultCase.Target.Next.MatchStloc(out int local);
				cases[0].Target.Next.Next.MatchBr(out ILLabel skipSwitch);
				c.EmitBr(skipSwitch);
				c.GotoLabel(skipSwitch);
				c.EmitCall(typeof(Main).GetProperty(nameof(Main.rand)).GetGetMethod());
				c.EmitCall(typeof(ExtractinatorOres).GetProperty(oresName).GetGetMethod());
				c.EmitCall(next);
				c.EmitStloc(local);
			}
			DoThing(ItemID.TitaniumOre, nameof(Ores));
			DoThing(ItemID.PlatinumOre, nameof(PrehardmodeOres));
		}
	}
}
