using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	internal static class UnderworldVisual {
		public static void Init() {
			Terraria.IL_Main.DrawUnderworldBackgroudLayer += Main_DrawUnderworldBackgroudLayer;
		}

		public static void Unload() { }

		//TODO: replace, Depths probably has a better edit for it
		private static void Main_DrawUnderworldBackgroudLayer(ILContext il) {
			var c = new ILCursor(il);
			if (!c.TryGotoNext(MoveType.After, i => i.MatchStloc(2))) {
				AltLibrary.Instance.Logger.Error("r $ 1");
				return;
			}

			c.Emit(OpCodes.Ldloc, 0);
			c.Emit(OpCodes.Ldloc, 2);
			c.EmitDelegate<Func<int, Texture2D, Texture2D>>((index, orig) => {
				if (WorldBiomeManager.WorldHell != "") {
					return WorldBiomeManager.GetWorldHell().AltUnderworldBackgrounds[index].Value;
				}
				return orig;
			});
			c.Emit(OpCodes.Stloc, 2);

			if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcI4(11),
				i => i.MatchLdcI4(3),
				i => i.MatchLdcI4(7),
				i => i.MatchNewobj<Color>())) {
				AltLibrary.Instance.Logger.Error("r $ 2");
				return;
			}

			c.EmitDelegate<Func<Color, Color>>((orig) => {
				if (WorldBiomeManager.WorldHell != "") {
					return WorldBiomeManager.GetWorldHell().AltUnderworldColor;
				}
				return orig;
			});
		}
	}
}
