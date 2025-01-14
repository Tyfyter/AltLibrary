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
				if (!TryFind(drunkEvilName, out drunkEvil)) {
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
		public override void Load() {
			drunkCobaltCycle = null;
			drunkMythrilCycle = null;
			drunkAdamantiteCycle = null;
		}

		public static Dictionary<int, int> biomeCountsWorking = [];
		public static Dictionary<int, int> biomeCountsFinished = [];
		public static Dictionary<int, byte> biomePercents = [];
		public override void OnWorldLoad() {
			biomePercents.Clear();
		}

		public override void OnWorldUnload() {
			biomePercents.Clear();
		}

		public override void Unload() {
			worldEvilBiome = null;
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
		}

		public override void SaveWorldData(TagCompound tag) {
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
		public override void LoadWorldData(TagCompound tag) {
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

		public override void NetSend(BinaryWriter writer) {
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

		public override void NetReceive(BinaryReader reader) {
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
