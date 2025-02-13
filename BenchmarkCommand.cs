using AltLibrary.Common.AltBiomes;
using AltLibrary.Core.Baking;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;

namespace AltLibrary {
	public class BenchmarkCommand : ModCommand {
		public override string Command => "benchmark_parents";
		public override CommandType Type => CommandType.Chat;
		public override void Action(CommandCaller caller, string input, string[] args) {
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			(int parentTile, AltBiome parentBiome) data = default;
			int j = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 10000; i++) {
				if (ALConvertInheritanceData.tileParentageData.UnbakedParents.TryGetValue(tile.TileType, out data)) j++;
			}
			stopwatch.Stop();
			caller.Reply($"Dictionary: {stopwatch.ElapsedTicks},  {j}, {data.Item1}/{data.Item2}");

			j = 0;
			stopwatch.Restart();
			for (int i = 0; i < 10000; i++) {
				if (ALConvertInheritanceData.tileParentageData.TryGetParent(tile.TileType, out data)) j++;
			}
			stopwatch.Stop();
			caller.Reply($"TryGetParent: {stopwatch.ElapsedTicks},  {j}, {data.Item1}/{data.Item2}");
		}
	}
}
