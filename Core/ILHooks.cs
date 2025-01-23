using AltLibrary.Common;
using AltLibrary.Common.Hooks;
using AltLibrary.Common.Systems;
#if CONTENT
using AltLibrary.Content.NPCs;
#endif
using AltLibrary.Core.Baking;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Core {
	internal class ILHooks {
		public static void OnInitialize() {
			On_Main.EraseWorld += Main_EraseWorld;
			WorldIcons.Init();
			OuterVisual.Init();
			EvenMoreWorldGen.Init();
			UnderworldVisual.Init();
			UIWorldCreationEdits.Init();
			HardmodeWorldGen.Init();
			DungeonChests.Init();
			SmashAltarMessage.Init();
			MowingGrassTile.Init();
			ShimmerDecraft.Load();
			MimicSummon.Init();
			SimpleReplacements.Load();
			DryadText.Init();
			JungleHuts.Init();
			TenthAnniversaryFix.Init();
			//BackgroundsAlternating.Inject();//TODO: redo
			EvilBiomeRangeTracker.Init();
		}

		public static void Unload() {
			On_Main.EraseWorld -= Main_EraseWorld;
			WorldIcons.Unload();
			OuterVisual.Unload();
			EvenMoreWorldGen.Unload();
			UnderworldVisual.Unload();
			UIWorldCreationEdits.Unload();
			HardmodeWorldGen.Unload();
			DungeonChests.Unload();
			SmashAltarMessage.Unload();
			MowingGrassTile.Unload();
			ShimmerDecraft.Unload();
			MimicSummon.Unload();
			SimpleReplacements.Unload();
			DryadText.Unload();
			JungleHuts.Unload();
			TenthAnniversaryFix.Unload();
			GenPasses.Unload();
			//BackgroundsAlternating.Uninit();
		}
		//TODO: clean config when a world has its data stored to its header
		private static void Main_EraseWorld(On_Main.orig_EraseWorld orig, int i) {
			Dictionary<string, AltLibraryConfig.WorldDataValues> tempDict = AltLibraryConfig.Config.GetWorldData();
			string path = Path.ChangeExtension(Main.WorldList[i].Path, ".twld");
			if (tempDict.ContainsKey(path)) {
				tempDict.Remove(path);
				AltLibraryConfig.Config.SetWorldData(tempDict);
				AltLibraryConfig.Save(AltLibraryConfig.Config);
			}
			orig(i);
		}
	}
}
