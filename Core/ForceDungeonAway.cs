using AltLibrary.Common.Systems;
using AltLibrary.Core.Generation;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AltLibrary.Core {
	public class ForceDungeonAway : ILoadable {
		public void Load(Mod mod) {
			//avoid EvilBiomeGenRanges by 200
			IL_WorldGen.DungeonStairs += IL_WorldGen_DungeonStairs;
		}
		void IL_WorldGen_DungeonStairs(ILContext il) {
			ILCursor c = new(il);
			//IL_0186: ldloca.s 0
			//IL_0188: ldc.r8 -0.5
			//IL_0191: stfld float64 [ReLogic]ReLogic.Utilities.Vector2D::X
			//IL_0196: ldsfld bool Terraria.WorldGen::drunkWorldGen
			//IL_019b: brfalse IL_05e8
			int direction = -1;
			if (!c.TryGotoNext(
				i => i.MatchLdloca(out direction),
				i => i.MatchLdcR8(-0.5f),
				i => i.MatchStfld<Vector2D>("X"),
				i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.drunkWorldGen)),
				i => i.MatchBrfalse(out _)
			)) {
				AltLibrary.Instance.Logger.Error($"Could not find if(drunkWorldGen) in {nameof(WorldGen.DungeonStairs)}");
				return;
			}
			//IL_05bf: ldloc.3
			//IL_05c0: ldloc.0
			//IL_05c1: call valuetype [ReLogic]ReLogic.Utilities.Vector2D [ReLogic]ReLogic.Utilities.Vector2D::op_Addition(valuetype [ReLogic]ReLogic.Utilities.Vector2D, valuetype [ReLogic]ReLogic.Utilities.Vector2D)
			//IL_05c6: stloc.3
			int position = -1;
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(out position),
				i => i.MatchLdloc(direction),
				i => i.MatchCall<Vector2D>("op_Addition"),
				i => i.MatchStloc(position)
			)) {
				AltLibrary.Instance.Logger.Error($"Could not find vector2D += zero; in {nameof(WorldGen.DungeonStairs)}");
				return;
			}
			c.EmitLdloca(position);
			c.EmitLdloca(direction);
			c.EmitDelegate(static (ref Vector2D position, ref Vector2D direction) => {
				int centerDir = (position.X < Main.maxTilesX / 2).ToDirectionInt();
				Rectangle paddingRect = new(0, (int)position.Y, 200, 1);
				Rectangle noPaddingRect = new(0, (int)position.Y, 1, 1);
				foreach (Rectangle range in WorldBiomeGeneration.EvilBiomeGenRanges) {
					int dir = Math.Sign(direction.X);
					Rectangle checktangle = dir == centerDir ? paddingRect : noPaddingRect;
					switch (dir) {
						case 1:
						checktangle.X = (int)position.X;
						if (range.X > position.X && checktangle.Intersects(range)) {
							direction.X *= -1;
						}
						break;
						case -1:
						checktangle.X = (int)position.X - checktangle.Width;
						if (range.X < position.X && checktangle.Intersects(range)) {
							direction.X *= -1;
						}
						break;
					}
				}
			});
		}
		public void Unload() { }
	}
}
