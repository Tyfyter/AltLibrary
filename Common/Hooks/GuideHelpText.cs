using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AltLibrary.Common.Hooks
{
	internal class GuideHelpText
	{
		public static void Load()
		{
			Terraria.IL_Main.HelpText += Main_HelpText;
		}

		public static void Unload()
		{
		}

		//TODO: double check that this code makes sense to begin with
		private static void Main_HelpText(ILContext il)
		{
			ILCursor c = new(il);

			if (!c.TryGotoNext(i => i.MatchLdstr("GuideHelpTextSpecific.Help_1147"),
					i => i.MatchCall(out _),
					i => i.MatchStsfld<Main>(nameof(Main.npcChatText))))
			{
				AltLibrary.Instance.Logger.Info("8 $ 1");
				return;
			}

			c.Index += 3;
			c.EmitDelegate<Action>(() =>
			{
				int ore = WorldGen.SavedOreTiers.Adamantite;
				string key = "";
				if (ore == TileID.Titanium)
				{
					key = Language.GetTextValue("GuideHelpTextSpecific.Help_1148");
				}
				else if (ore == TileID.Adamantite)
				{
					key = Language.GetTextValue("GuideHelpTextSpecific.Help_1147");
				}
				else
				{
					key = AltLibrary.Ores.Find(x => x.OreType == OreType.Adamantite && x.ore == ore).GuideHelpText.Value ?? Language.GetTextValue("Mods.AltLibrary.OreHelpTextBase", AltLibrary.Ores.Find(x => x.OreType == OreType.Adamantite && x.ore == ore).DisplayName.Value.ToLowerInvariant());
				}
				Main.npcChatText = key;
			});

			if (!c.TryGotoPrev(i => i.MatchLdcI4(111)))
			{
				AltLibrary.Instance.Logger.Info("8 $ 2");
				return;
			}

			c.EmitDelegate<Func<int, int>>((int i) => WorldGen.altarCount > 2 ? 111 : 0);

			if (!c.TryGotoNext(i => i.MatchLdcI4(223)))
			{
				AltLibrary.Instance.Logger.Info("8 $ 3");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Ldc_I4_0);
		}
	}
}
