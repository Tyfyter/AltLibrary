using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	internal class MimicSummon {
		public static Dictionary<int, List<(Func<bool> condition, int npcID)>> Mimics { get; private set; }

		public static void Init() {
			On_NPC.BigMimicSummonCheck += NPC_BigMimicSummonCheck;
		}

		public static void SetupContent() { }
		private static Func<bool> Condition(AltBiome biome) {
			if (biome.BiomeType == BiomeType.Evil) return () => WorldBiomeManager.GetWorldEvil(true, true) == biome;
			return () => WorldBiomeManager.GetWorldHallow(true) == biome;
		}

		public static void Unload() {
			Mimics = null;
		}

		private static bool NPC_BigMimicSummonCheck(On_NPC.orig_BigMimicSummonCheck orig, int x, int y, Player user) {
			if (Mimics is null) {
				Mimics = new() {
					[ItemID.NightKey] = [
						(() => WorldBiomeManager.IsCorruption, NPCID.BigMimicCorruption),
						(() => WorldBiomeManager.IsCrimson, NPCID.BigMimicCrimson)
					],
					[ItemID.LightKey] = [
						(() => WorldBiomeManager.WorldHallowName == "", NPCID.BigMimicHallow)
					]
				};
				foreach (AltBiome biome in AltLibrary.Biomes) {
					if (biome.BiomeType <= BiomeType.Hallow && biome.MimicType.HasValue) {
						bool isEvil = biome.BiomeType == BiomeType.Evil;
						int keyType = biome.MimicKeyType ?? (isEvil ? ItemID.NightKey : ItemID.LightKey);
						if (!Mimics.TryGetValue(keyType, out List<(Func<bool> condition, int npcID)> keyMimics)) Mimics.TryAdd(keyType, keyMimics ??= []);
						keyMimics.Add((Condition(biome), biome.MimicType.Value));
					}
				}
			}

			if (Main.netMode == NetmodeID.MultiplayerClient || !Main.hardMode) return false;

			int chestIndex = Chest.FindChest(x, y);
			if (chestIndex < 0) return false;

			ushort chestTile = Main.tile[Main.chest[chestIndex].x, Main.chest[chestIndex].y].TileType;
			int chestFrame = Main.tile[Main.chest[chestIndex].x, Main.chest[chestIndex].y].TileFrameX / 36;
			if (!TileID.Sets.BasicChest[chestTile] || (chestTile != 21 || chestFrame < 5 || chestFrame > 6)) ;
			int selectedKey = -1;
			for (int i = 0; i < 40; i++) {
				Item item = Main.chest[chestIndex].item[i];
				if (!(item?.IsAir ?? true)) {
					if (selectedKey != -1) return false;
					if (item.stack > 1) return false;
					if (Mimics.ContainsKey(item.type)) {
						selectedKey = item.type;
					}
				}
			}
			if (!Mimics.TryGetValue(selectedKey, out List<(Func<bool> condition, int npcID)> mimics)) return false;
			List<int> options = [];
			for (int i = 0; i < mimics.Count; i++) {
				if (mimics[i].condition()) options.Add(mimics[i].npcID);
			}
			if (options.Count > 0) {
				int selectedOption = Main.rand.Next(options);
				if (selectedOption < NPCID.Count) return orig(x, y, user);
				if (TileID.Sets.BasicChest[Main.tile[x, y].TileType]) {
					if (Main.tile[x, y].TileFrameX % 36 != 0) x--;
					if (Main.tile[x, y].TileFrameY % 36 != 0) y--;
					int number = Chest.FindChest(x, y);
					for (int j = 0; j < 40; j++) {
						Main.chest[chestIndex].item[j] = new Item();
					}
					Chest.DestroyChest(x, y);
					for (int k = x; k <= x + 1; k++) {
						for (int l = y; l <= y + 1; l++) {
							if (TileID.Sets.BasicChest[Main.tile[k, l].TileType]) {
								Main.tile[k, l].ClearTile();
							}
						}
					}
					int number2 = 1;
					if (Main.tile[x, y].TileType == TileID.Containers2) {
						number2 = 5;
					}
					if (Main.tile[x, y].TileType >= TileID.VioletMoss) {
						number2 = 101;
					}
					NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, number2, x, y, 0f, number, Main.tile[x, y].TileType, 0);
					NetMessage.SendTileSquare(-1, x, y, 3);
				}
				int mimicIndex = NPC.NewNPC(user.GetSource_TileInteraction(x, y), x * 16 + 16, y * 16 + 32, selectedOption, 0, 0f, 0f, 0f, 0f, 255);
				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, mimicIndex, 0f, 0f, 0f, 0, 0, 0);
				Main.npc[mimicIndex].BigMimicSpawnSmoke();
			}
			return false;
		}
	}
}
