using AltLibrary.Common.Hooks;
using AltLibrary.Core.Generation;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltLibrary {
	public partial class AltLibrary {
		public static void Call_AddInMimicList(ValueTuple<int, int> mimicType, Func<bool> condition) {
			if (!MimicSummon.Mimics.TryGetValue(mimicType.Item1, out List<(Func<bool> condition, int npcID)> keyMimics)) MimicSummon.Mimics.TryAdd(mimicType.Item1, keyMimics ??= []);
			keyMimics.Add((condition, mimicType.Item2));
		}
		public static void Call_AddDungeonChest(int chestTileType, int contain, int style, Func<bool> condition = null) {
			DungeonChests.extraDungeonChests.Add((chestTileType, contain, style, condition));
		}
		public static void Call_AddInvalidRangeHandler(string key, EvilBiomeGenerationPass.InvalidRangeHandler handler, int priority) {
			EvilBiomeGenerationPass.invalidRangeHandlers.Add(key, (handler, priority));
		}
		public enum Calls {
			AddInMimicList,
			AddDungeonChest,
			AddInvalidRangeHandler
		}
	}
}
