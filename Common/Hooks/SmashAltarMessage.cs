using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
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

namespace AltLibrary.Common.Hooks {
	internal class SmashAltarMessage {
		public static void Init() {
			On_WorldGen.SmashAltar += On_WorldGen_SmashAltar;
			IL_WorldGen.SmashAltar += WorldGen_SmashAltar;
		}

		private static void On_WorldGen_SmashAltar(On_WorldGen.orig_SmashAltar orig, int i, int j) {
			orig(i, j);
			if (Main.netMode == NetmodeID.MultiplayerClient || !Main.hardMode || WorldGen.noTileActions || WorldGen.gen) return;
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
				DoBlessage(substitutionTile);
			} else if (Main.netMode == NetmodeID.Server) {
				ModPacket packet = AltLibrary.Instance.GetPacket();
				packet.Write((byte)PacketType.SmashAltar);
				packet.Write((int)substitutionTile);
				packet.Send();
			}
		}
		public static void DoBlessage(int tileType) {
			const string baseKey = "Mods.AltLibrary.BlessBase";
			string text = null;
			if (AltLibrary.Ores.FirstOrDefault(o => o.ore == tileType) is AltOre ore) {
				string lockey = ore.GetLocalizationKey("BlessMessage");
				if (Language.Exists(lockey)) text = Language.GetTextValue(lockey, Lang._mapLegendCache[MapHelper.TileToLookup(tileType, 0)].Value);
			}
			text ??= Language.GetTextValue(baseKey, Lang._mapLegendCache[MapHelper.TileToLookup(tileType, 0)].Value);
			Main.NewText(text, 50, byte.MaxValue, 130);
		}

		public static void Unload() { }

		private static void WorldGen_SmashAltar(ILContext il) {
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
		}
	}
}
