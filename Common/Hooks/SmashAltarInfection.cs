using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using AltLibrary.Core.Baking;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks
{
	internal class SmashAltarInfection
	{
		public static void Init()
		{
			WorldGen.SmashAltar(0, 0);
			On_WorldGen.SmashAltar += On_WorldGen_SmashAltar;
			IL_WorldGen.SmashAltar += WorldGen_SmashAltar;
		}

		private static void On_WorldGen_SmashAltar(On_WorldGen.orig_SmashAltar orig, int i, int j) {
			orig(i, j);
			if (Main.netMode == NetmodeID.MultiplayerClient || !Main.hardMode || WorldGen.noTileActions || WorldGen.gen) return;
			const string baseKey = "Mods.AltLibrary.BlessBase";
			int substitutionTile = 0;
			switch (WorldGen.altarCount % 3) {
				case 1:
				substitutionTile = WorldGen.SavedOreTiers.Cobalt;
				break;
				case 2:
				substitutionTile = WorldGen.SavedOreTiers.Mythril;
				break;
				case 0:
				substitutionTile = WorldGen.SavedOreTiers.Adamantite;
				break;
			}
			if (Main.netMode == NetmodeID.SinglePlayer) {
				Main.NewText(Language.GetTextValue(baseKey, Lang._mapLegendCache[MapHelper.TileToLookup(substitutionTile, 0)].Value), 50, byte.MaxValue, 130);
			} else if (Main.netMode == NetmodeID.Server) {
				ModPacket packet = AltLibrary.Instance.GetPacket();
				packet.Write((byte)PacketType.SmashAltar);
				packet.Write((int)substitutionTile);
				packet.Send();
			}
		}

		public static void Unload()
		{
		}

		//TODO: double check that this code makes sense to begin with
		private static void WorldGen_SmashAltar(ILContext il)
		{
			ILCursor c = new(il);

			while (c.TryGotoNext(MoveType.Before, ins => ins.MatchCall<Main>(nameof(Main.NewText)))) {
				if (c.Next.Operand is MethodReference reference && reference.Parameters.Count == 4) {
					c.EmitDelegate<Action<string, byte, byte, byte>>((_, _, _, _) => {});
				} else {
					c.EmitDelegate<Action<object, Color>>((_, _) => { });
				}
				c.Remove();
			}

			c = new(il);

			while (c.TryGotoNext(MoveType.Before, ins => ins.MatchCall(typeof(ChatHelper), nameof(ChatHelper.BroadcastChatMessage)))) {
				c.EmitDelegate<Action<NetworkText, Color, int>>((_, _, _) => { });
				c.Remove();
			}
			return;
			if (!c.TryGotoNext(i => i.MatchLdsfld<Main>(nameof(Main.drunkWorld))))
			{
				AltLibrary.Instance.Logger.Info("n $ 0");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Ldloc_0);
			c.EmitDelegate<Func<bool, int, bool>>(DrunkenBaking.GetDrunkSmashingData);

			for (int j = 0; j < 3; j++)
			{
				if (j == 1)
				{
					if (!c.TryGotoNext(i => i.MatchLdsfld<Main>(nameof(Main.drunkWorld))))
					{
						AltLibrary.Instance.Logger.Info("n $ -1 " + j);
						return;
					}

					c.Index++;
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4_0);// was c.EmitDelegate(() => false);
				}
				else if (j == 2)
				{
					if (!c.TryGotoNext(i => i.MatchLdsfld<Main>(nameof(Main.drunkWorld))))
					{
						AltLibrary.Instance.Logger.Info("n $ -2 " + j);
						return;
					}

					c.Index++;
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4_0);// was c.EmitDelegate(() => false);
				}

				if (!c.TryGotoNext(i => i.MatchLdsfld<Lang>(nameof(Lang.misc)),
					i => i.MatchLdloc(7 + j),
					i => i.MatchLdelemRef(),
					i => i.MatchCallvirt<LocalizedText>("get_Value")))
				{
					AltLibrary.Instance.Logger.Info("n $ 1 " + j);
					return;
				}

				c.Index += 4;
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, j);
				c.EmitDelegate<Func<int, string>>(DrunkenBaking.GetSmashAltarText);

				if (!c.TryGotoNext(i => i.MatchLdsfld<Lang>(nameof(Lang.misc)),
					i => i.MatchLdloc(7 + j),
					i => i.MatchLdelemRef(),
					i => i.MatchLdfld<LocalizedText>(nameof(LocalizedText.Key)),
					i => i.MatchCall(out _),
					i => i.MatchCall<NetworkText>(nameof(NetworkText.FromKey))))
				{
					AltLibrary.Instance.Logger.Info("n $ 2 " + j);
					return;
				}

				c.Index += 6;
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, j);
				c.EmitDelegate<Func<int, NetworkText>>((j) => NetworkText.FromLiteral(DrunkenBaking.GetSmashAltarText(j)));
			}
		}
	}
}
