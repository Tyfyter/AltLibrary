using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.UI;
using PegasusLib;

namespace AltLibrary.Core
{
	internal class ALReflection
	{
		internal static WorldGenScanTileColumnAndRemoveClumps WorldGen_ScanTileColumnAndRemoveClumps = null;
		internal static FastFieldInfo<UIList, UIElement> UIList__innerList = null;
		internal static FieldInfo UIWorldCreation__evilButtons = null;
		internal static PropertyInfo ModType_Mod = null;
		internal static FastFieldInfo<NPCShop.Entry, List<Condition>> ShopEntry_conditions = null;
		internal static FastFieldInfo<UIWorldListItem, UIElement> UIWorldListItem__worldIcon = null;
		internal static FastFieldInfo<Recipe, bool> Recipe_alchemy = null;


		internal delegate void WorldGenScanTileColumnAndRemoveClumps(int x);

		internal static void Init()
		{
			WorldGen_ScanTileColumnAndRemoveClumps = typeof(WorldGen).GetMethod("ScanTileColumnAndRemoveClumps", BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(int) }).CreateDelegate<WorldGenScanTileColumnAndRemoveClumps>();
			UIList__innerList = new("_innerList", BindingFlags.NonPublic);
			ModType_Mod = typeof(ModType).GetProperty("Mod");
			UIWorldCreation__evilButtons = typeof(UIWorldCreation).GetField("_evilButtons", BindingFlags.NonPublic | BindingFlags.Instance);
			ShopEntry_conditions = new("conditions", BindingFlags.NonPublic);
			UIWorldListItem__worldIcon = new("_worldIcon", BindingFlags.NonPublic);
			Recipe_alchemy = new("alchemy", BindingFlags.NonPublic);
		}

		internal static void Unload()
		{
			WorldGen_ScanTileColumnAndRemoveClumps = null;
			UIList__innerList = null;
			ModType_Mod = null;
			UIWorldCreation__evilButtons = null;
			ShopEntry_conditions = null;
			UIWorldListItem__worldIcon = null;
			Recipe_alchemy = null;
		}
	}
}
