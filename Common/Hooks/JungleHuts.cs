using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.Biomes.CaveHouse;

namespace AltLibrary.Common.Hooks {
	internal class JungleHuts {
		public static void Init() {
			IL_HouseUtils.CreateBuilder += HouseUtils_CreateBuilder;
		}

		public static void Unload() { }

		private static void HouseUtils_CreateBuilder(ILContext il) {
			ILCursor c = new(il);
			if (!c.TryGotoNext(MoveType.Before, i => i.MatchNewobj(typeof(JungleHouseBuilder).GetConstructor(BindingFlags.Public | BindingFlags.Instance, [typeof(IEnumerable<Rectangle>)])))) {
				AltLibrary.Instance.Logger.Error("Could not find new JungleHouseBuilder in HouseUtils_CreateBuilder");
				return;
			}

			c.Index--;
			c.MoveAfterLabels();
			ILLabel label = il.DefineLabel();

			c.EmitDelegate(() => WorldBiomeManager.WorldJungle is VanillaBiome);
			c.Emit(OpCodes.Brtrue_S, label);

			c.EmitDelegate(() => HouseBuilder.Invalid);
			c.Emit(OpCodes.Ret);

			c.MarkLabel(label);
		}
	}
}
