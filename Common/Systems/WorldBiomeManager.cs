using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using System;

namespace AltLibrary.Common.Systems {
	//TODO: double check that this code makes sense to begin with
	public class WorldBiomeManager : ModSystem {
		public static AltBiome GetWorldEvil(bool includeVanilla = true, bool includeDrunk = false) {
			if (Main.drunkWorld && includeDrunk) {
				if (includeVanilla || !drunkEvilName.StartsWith("Terraria/")) {
					return DrunkEvil;
				}
			}
			if (includeVanilla || WorldEvilName != "") {
				return WorldEvilBiome;
			}
			return null;
		}
		public static AltBiome GetDrunkEvil(bool includeVanilla = true) {
			if (includeVanilla || !drunkEvilName.StartsWith("Terraria/")) {
				return DrunkEvil;
			}
			return null;
		}
		public static AltBiome GetWorldHallow(bool includeVanilla = true) {
			if (includeVanilla || WorldHallowName != "") {
				return WorldHallowBiome;
			}
			return null;
		}
		public static AltBiome GetWorldHell(bool includeVanilla = true) {
			if (TryFind(WorldHell, out AltBiome worldHell)) return worldHell;
			if (includeVanilla) {
				return GetInstance<UnderworldAltBiome>();
			}
			return null;
		}
		public static AltBiome GetWorldJungle(bool includeVanilla = true) {
			if (TryFind(WorldJungle, out AltBiome worldJungle)) return worldJungle;
			if (includeVanilla) {
				return GetInstance<JungleAltBiome>();
			}
			return null;
		}
		public static string WorldEvilName {
			get => worldEvilName;
			internal set {
				worldEvilName = value;
				if (value == "") {
					WorldEvilBiome = WorldGen.crimson ? GetInstance<CrimsonAltBiome>() : GetInstance<CorruptionAltBiome>();
				} else if (TryFind(worldEvilName, out AltBiome altEvil)) {
					WorldEvilBiome = altEvil;
				} else {
					WorldEvilBiome = new UnloadedAltBiome(value, BiomeType.Evil);
				}
			}
		}
		static AltBiome worldEvilBiome;
		public static AltBiome WorldEvilBiome {
			get => worldEvilBiome;
			internal set {
				worldEvilBiome = value;
				if (value.Type < 0) {
					worldEvilName = "";
				} else {
					worldEvilName = value.FullName;
				}
			}
		}
		static string worldEvilName = "";
		[Obsolete("Should never have been designed this way to begin with", false)]
		public static string WorldEvil {
			get => worldEvilName;
			internal set => WorldEvilName = value;
		}
		static AltBiome worldHallowBiome;
		public static AltBiome WorldHallowBiome {
			get => worldHallowBiome;
			internal set {
				worldHallowBiome = value;
				if (value.Type < 0) {
					worldHallowName = "";
				} else {
					worldHallowName = value.FullName;
				}
			}
		}
		public static string WorldHallowName {
			get => worldHallowName;
			internal set {
				worldHallowName = value;
				if (value == "") {
					worldHallowBiome = GetInstance<HallowAltBiome>();
				} else if (TryFind(worldHallowName, out AltBiome altHallow)) {
					worldHallowBiome = altHallow;
				} else {
					worldHallowBiome = new UnloadedAltBiome(value, BiomeType.Hallow);
				}
			}
		}
		static string worldHallowName = "";
		[Obsolete("Should never have been designed this way to begin with", false)]
		public static string WorldHallow {
			get => worldHallowName;
			internal set => WorldHallowName = value;
		}
		public static string WorldHell { get; internal set; } = "";
		public static string WorldJungle { get; internal set; } = "";
		internal static string drunkEvilName = "";
		private static AltBiome drunkEvil;
		public static AltBiome DrunkEvil {
			get {
				if (drunkEvil is not null) return drunkEvil;
				if(!TryFind(drunkEvilName, out drunkEvil)) {
					switch (drunkEvilName) {
						case "Terraria/Crimson":
						drunkEvil = GetInstance<CrimsonAltBiome>();
						break;

						default:
						drunkEvil = GetInstance<CorruptionAltBiome>();
						break;
					}
				}
				return drunkEvil;
			}
			set {
				if (value is null) {
					_ = DrunkEvil;
				} else {
					drunkEvil = value;
					if (value is VanillaBiome) {
						drunkEvilName = "Terraria/" + (value is CrimsonAltBiome ? "Crimson" : "Corruption");
					} else {
						drunkEvilName = value.FullName;
					}
				}
			}
		}
		public static int Copper { get; internal set; } = 0;
		public static int Iron { get; internal set; } = 0;
		public static int Silver { get; internal set; } = 0;
		public static int Gold { get; internal set; } = 0;
		public static int Cobalt { get; internal set; } = 0;
		public static int Mythril { get; internal set; } = 0;
		public static int Adamantite { get; internal set; } = 0;
		internal static int hmOreIndex = 0;

		public static bool IsCorruption => WorldEvilName == "" && !WorldGen.crimson;
		public static bool IsCrimson => WorldEvilName == "" && WorldGen.crimson;
		public static bool IsAnyModdedEvil => WorldEvilName != "" && !WorldGen.crimson;

		//do not need to sync, world seed should be constant between players
		internal static AltOre[] drunkCobaltCycle;
		internal static AltOre[] drunkMythrilCycle;
		internal static AltOre[] drunkAdamantiteCycle;

		internal static float[] AltBiomePercentages;
		public static float PurityBiomePercentage => AltBiomePercentages == null ? 0f : AltBiomePercentages[0];
		public static float CorruptionBiomePercentage => AltBiomePercentages == null ? 0f : AltBiomePercentages[1];
		public static float CrimsonBiomePercentage => AltBiomePercentages == null ? 0f : AltBiomePercentages[2];
		public static float HallowBiomePercentage => AltBiomePercentages == null ? 0f : AltBiomePercentages[3];
		public static float[] GetBiomePercentages
		{
			get
			{
				List<float> list = AltBiomePercentages?.ToList();
				list?.RemoveRange(0, 4);
				return list?.ToArray();
			}
		}

		public override void Load()
		{
			drunkCobaltCycle = null;
			drunkMythrilCycle = null;
			drunkAdamantiteCycle = null;
		}

		public static Dictionary<int, int> biomeCountsWorking = [];
		public static Dictionary<int, int> biomeCountsFinished = [];
		public static Dictionary<int, byte> biomePercents = [];
		public override void OnWorldLoad()
		{
			AltBiomePercentages = new float[AltLibrary.Biomes.Count + 5];
			AnalysisTiles(false);
		}

		public override void OnWorldUnload()
		{
			AltBiomePercentages = null;
		}

		internal static void AnalysisTiles(bool includeText = true)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;
			ThreadPool.QueueUserWorkItem(new WaitCallback(SmAtcb), 1);
			if (includeText)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(text), 1);
			}

			static void text(object threadContext) => Main.npcChatText = Language.GetTextValue("Mods.AltLibrary.AnalysisDone", Main.LocalPlayer.name, Main.worldName) + AnalysisDoneSpaces;
		}
		private static void SmAtcb(object threadContext)
		{
			int solid = 0;
			int purity = 0;
			int hallow = 0;
			int evil = 0;
			int crimson = 0;
			int[] mods = new int[AltLibrary.Biomes.Count];
			HashSet<int>[] modTiles = new HashSet<int>[AltLibrary.Biomes.Count];
			List<int> extraPureSolid = new();
			foreach (AltBiome biome in AltLibrary.Biomes)
			{
				modTiles[biome.Type - 1] = new HashSet<int>();
				if (biome.BiomeType == BiomeType.Evil || biome.BiomeType == BiomeType.Hallow) {
					foreach (KeyValuePair<int, int> pair in biome.TileConversions) {
						extraPureSolid.Add(pair.Key);
						modTiles[biome.Type - 1].Add(pair.Value);
					}
				}
			}
			for (int x = 0; x < Main.maxTilesX; x++)
			{
				for (int y = 0; y < Main.maxTilesY; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);
					if (tile.HasTile)
					{
						int type = tile.TileType;
						if (type == TileID.HallowedGrass || type == TileID.HallowedIce || type == TileID.Pearlsand || type == TileID.Pearlstone || type == TileID.HallowSandstone || type == TileID.HallowHardenedSand || type == TileID.GolfGrassHallowed)
						{
							hallow++;
							solid++;
						}
						if (type == TileID.CorruptGrass || type == TileID.CorruptIce || type == TileID.Ebonsand || type == TileID.Ebonstone || type == TileID.CorruptSandstone || type == TileID.CorruptHardenedSand)
						{
							evil++;
							solid++;
						}
						if (type == TileID.CrimsonGrass || type == TileID.FleshIce || type == TileID.Crimsand || type == TileID.Crimstone || type == TileID.CrimsonSandstone || type == TileID.CrimsonHardenedSand)
						{
							crimson++;
							solid++;
						}
						if (type == TileID.Grass || type == TileID.GolfGrass || type == TileID.Stone || type == TileID.JungleGrass || type == TileID.Sand || type == TileID.IceBlock || type == TileID.Sandstone || type == TileID.HardenedSand || extraPureSolid.Contains(type))
						{
							purity++;
							solid++;
						}
						for (int i = 0; i < mods.Length; i++)
						{
							if (modTiles[i].Contains(type))
							{
								mods[i]++;
								solid++;
							}
						}
					}
				}
			}

			AltBiomePercentages[0] = purity * 100f / (solid * 100f);
			AltBiomePercentages[1] = evil * 100f / (solid * 100f);
			AltBiomePercentages[2] = crimson * 100f / (solid * 100f);
			AltBiomePercentages[3] = hallow * 100f / (solid * 100f);
			for (int i = 0; i < mods.Length; i++)
			{
				AltBiomePercentages[i + 4] = mods[i] * 100f / (solid * 100f);
			}
		}
		internal const string AnalysisDoneSpaces = "\n\n\n\n\n\n\n\n\n\n";

		public override void Unload()
		{
			//TODO: uncomment
			//worldEvilBiome = null;
			worldEvilName = null;
			worldHallowBiome = null;
			worldHallowName = null;
			WorldHell = null;
			WorldJungle = null;
			drunkEvilName = null;
			drunkEvil = null;
			Copper = 0;
			Iron = 0;
			Silver = 0;
			Gold = 0;
			Cobalt = 0;
			Mythril = 0;
			Adamantite = 0;
			hmOreIndex = 0;
			drunkCobaltCycle = null;
			drunkMythrilCycle = null;
			drunkAdamantiteCycle = null;
			AltBiomePercentages = null;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag.Add("AltLibrary:WorldEvil", WorldEvilName);
			tag.Add("AltLibrary:WorldHallow", WorldHallowName);
			tag.Add("AltLibrary:WorldHell", WorldHell);
			tag.Add("AltLibrary:WorldJungle", WorldJungle);
			tag.Add("AltLibrary:DrunkEvil", drunkEvilName);
			tag.Add("AltLibrary:Copper", Copper);
			tag.Add("AltLibrary:Iron", Iron);
			tag.Add("AltLibrary:Silver", Silver);
			tag.Add("AltLibrary:Gold", Gold);
			tag.Add("AltLibrary:Cobalt", Cobalt);
			tag.Add("AltLibrary:Mythril", Mythril);
			tag.Add("AltLibrary:Adamantite", Adamantite);
			tag.Add("AltLibrary:HardmodeOreIndex", hmOreIndex);

			Dictionary<string, AltLibraryConfig.WorldDataValues> tempDict = AltLibraryConfig.Config.GetWorldData();
			AltLibraryConfig.WorldDataValues worldData;

			worldData.worldEvil = WorldEvilName;
			worldData.worldHallow = WorldHallowName;
			worldData.worldHell = WorldHell;
			worldData.worldJungle = WorldJungle;
			worldData.drunkEvil = drunkEvilName;

			string path = Path.ChangeExtension(Main.worldPathName, ".twld");
			tempDict[path] = worldData;
			AltLibraryConfig.Config.SetWorldData(tempDict);
			AltLibraryConfig.Save(AltLibraryConfig.Config);
		}
		public override void SaveWorldHeader(TagCompound tag) {
			tag.Add("WorldEvil", WorldEvilName);
			tag.Add("WorldHallow", WorldHallowName);
			tag.Add("WorldHell", WorldHell);
			tag.Add("WorldJungle", WorldJungle);
			tag.Add("DrunkEvil", drunkEvilName);
		}
		public override void LoadWorldData(TagCompound tag)
		{
			WorldEvilName = tag.GetString("AltLibrary:WorldEvil");
			WorldHallowName = tag.GetString("AltLibrary:WorldHallow");
			WorldHell = tag.GetString("AltLibrary:WorldHell");
			WorldJungle = tag.GetString("AltLibrary:WorldJungle");
			drunkEvilName = tag.GetString("AltLibrary:DrunkEvil");
			Copper = tag.GetInt("AltLibrary:Copper");
			Iron = tag.GetInt("AltLibrary:Iron");
			Silver = tag.GetInt("AltLibrary:Silver");
			Gold = tag.GetInt("AltLibrary:Gold");
			Cobalt = tag.GetInt("AltLibrary:Cobalt");
			Mythril = tag.GetInt("AltLibrary:Mythril");
			Adamantite = tag.GetInt("AltLibrary:Adamantite");
			hmOreIndex = tag.GetInt("AltLibrary:HardmodeOreIndex");
			DrunkEvil = null;

			//reset every unload
			drunkCobaltCycle = null;
			drunkMythrilCycle = null;
			drunkAdamantiteCycle = null;
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(WorldEvilName);
			writer.Write(WorldHallowName);
			writer.Write(WorldHell);
			writer.Write(WorldJungle);
			writer.Write(drunkEvilName);
			writer.Write(Copper);
			writer.Write(Iron);
			writer.Write(Silver);
			writer.Write(Gold);
			writer.Write(Cobalt);
			writer.Write(Mythril);
			writer.Write(Adamantite);
			writer.Write(hmOreIndex);
		}

		public override void NetReceive(BinaryReader reader)
		{
			WorldEvilName = reader.ReadString();
			WorldHallowName = reader.ReadString();
			WorldHell = reader.ReadString();
			WorldJungle = reader.ReadString();
			string oldDrunkEvilName = drunkEvilName;
			drunkEvilName = reader.ReadString();
			if (drunkEvilName != oldDrunkEvilName) DrunkEvil = null;
			Copper = reader.ReadInt32();
			Iron = reader.ReadInt32();
			Silver = reader.ReadInt32();
			Gold = reader.ReadInt32();
			Cobalt = reader.ReadInt32();
			Mythril = reader.ReadInt32();
			Adamantite = reader.ReadInt32();
			hmOreIndex = reader.ReadInt32();
		}
	}
}
