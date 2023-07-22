using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks
{
	public class AltOreInsideBodies
	{
		public static List<int> ores;

		internal static void Load()
		{
			On_NPC.AI_001_Slimes_GenerateItemInsideBody += NPC_AI_001_Slimes_GenerateItemInsideBody;
		}

		private static int NPC_AI_001_Slimes_GenerateItemInsideBody(On_NPC.orig_AI_001_Slimes_GenerateItemInsideBody orig, bool isBallooned)
		{
			int item = orig(isBallooned);
			if (item >= ItemID.IronOre && item <= ItemID.SilverOre || item >= ItemID.TinOre && item <= ItemID.PlatinumOre)
				item = Main.rand.Next(ores);

			return item;
		}

		internal static void Unload()
		{
		}
	}
}
