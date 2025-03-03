﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AltLibrary.Common.Hooks {
	internal class GuideHelpText {
		public static void Load() {
			IL_Main.HelpText += Main_HelpText;
		}

		public static void Unload() { }

		private static void Main_HelpText(ILContext il) {
			ILCursor c = new(il);

			if (!c.TryGotoNext(i => i.MatchLdstr("GuideHelpTextSpecific.Help_1147"),
					i => i.MatchCall(out _),
					i => i.MatchStsfld<Main>(nameof(Main.npcChatText)))
			) {
				AltLibrary.Instance.Logger.Error("Missing Hellforge upgrade tip in Main_HelpText");
				return;
			}

			c.Index += 3;
			c.EmitDelegate(() => {
				int ore = WorldGen.SavedOreTiers.Adamantite;
				string key = "";
				if (ore == TileID.Titanium) {
					key = Language.GetTextValue("GuideHelpTextSpecific.Help_1148");
				} else if (ore == TileID.Adamantite) {
					key = Language.GetTextValue("GuideHelpTextSpecific.Help_1147");
				} else {
					key = AltLibrary.Ores.Find(x => x.OreType == OreType.Adamantite && x.ore == ore).GuideHelpText.Value ?? Language.GetTextValue("Mods.AltLibrary.OreHelpTextBase", AltLibrary.Ores.Find(x => x.OreType == OreType.Adamantite && x.ore == ore).DisplayName.Value.ToLowerInvariant());
				}
				Main.npcChatText = key;
			});

			if (!c.TryGotoPrev(i => i.MatchLdcI4(TileID.Adamantite))) {
				AltLibrary.Instance.Logger.Error("Missing WorldGen.SavedOreTiers.Adamantite == TileID.Adamantite in Main_HelpText");
				return;
			}

			c.EmitDelegate((int i) => WorldGen.altarCount > 2 ? TileID.Adamantite : 0);

			if (!c.TryGotoNext(i => i.MatchLdcI4(TileID.Titanium))) {
				AltLibrary.Instance.Logger.Error("Missing WorldGen.SavedOreTiers.Adamantite == TileID.Titanium in Main_HelpText");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Ldc_I4_0);
		}
	}
}
