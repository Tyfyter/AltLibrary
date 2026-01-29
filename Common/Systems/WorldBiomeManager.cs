using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
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
			if (!includeVanilla && worldHell is VanillaBiome) return null;
			return worldHell;
		}
		public static AltBiome GetWorldJungle(bool includeVanilla = true) {
			if (!includeVanilla && worldJungle is VanillaBiome) return null;
			return worldJungle;
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
			set {
				worldEvilBiome = value;
				worldEvilName = value.FullName;
			}
		}
		static string worldEvilName = "";
		[Obsolete("Should never have been designed this way to begin with, will be replaced with one of type AltBiome in 2.2", true)]
		public static string WorldEvil {
			get => worldEvilName;
			internal set => WorldEvilName = value;
		}
		static AltBiome worldHallowBiome;
		public static AltBiome WorldHallowBiome {
			get => worldHallowBiome;
			set {
				worldHallowBiome = value;
				worldHallowName = value.FullName;
			}
		}
		public static string WorldHallowName {
			get => worldHallowName;
			internal set {
				worldHallowName = value;
				if (value == "" || value.StartsWith(nameof(AltLibrary))) {
					worldHallowBiome = GetInstance<HallowAltBiome>();
					worldHallowName = worldHallowBiome.FullName;
				} else if (TryFind(worldHallowName, out AltBiome altHallow)) {
					worldHallowBiome = altHallow;
				} else {
					worldHallowBiome = new UnloadedAltBiome(value, BiomeType.Hallow);
				}
			}
		}
		static string worldHallowName = "";
		[Obsolete("Should never have been designed this way to begin with, will be replaced with one of type AltBiome in 2.2", true)]
		public static string WorldHallow {
			get => worldHallowName;
			internal set => WorldHallowName = value;
		}
		static AltBiome worldHell;
		public static AltBiome WorldHell {
			get => worldHell;
			set {
				worldHell = value;
				worldHellName = value.FullName;
			}
		}
		public static string WorldHellName {
			get => worldHellName;
			internal set {
				worldHellName = value;
				if (value == "" || value.StartsWith(nameof(AltLibrary))) {
					worldHell = GetInstance<UnderworldAltBiome>();
					worldHellName = worldHell.FullName;
				} else if (TryFind(worldHellName, out AltBiome altHell)) {
					worldHell = altHell;
				} else {
					worldHell = new UnloadedAltBiome(value, BiomeType.Hell);
				}
			}
		}
		static string worldHellName = "";
		static AltBiome worldJungle;
		public static AltBiome WorldJungle {
			get => worldJungle;
			set {
				worldJungle = value;
				worldJungleName = value.FullName;
			}
		}
		public static string WorldJungleName {
			get => worldJungleName;
			internal set {
				worldJungleName = value;
				if (value == "" || value.StartsWith(nameof(AltLibrary))) {
					worldJungle = GetInstance<JungleAltBiome>();
					worldJungleName = worldJungle.FullName;
				} else if (TryFind(worldJungleName, out AltBiome altJungle)) {
					worldJungle = altJungle;
				} else {
					worldJungle = new UnloadedAltBiome(value, BiomeType.Jungle);
				}
			}
		}
		static string worldJungleName = "";
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
		internal static AltOre[] ores;
		static List<(string slot, string ore)> unloadedOres;
		public static ref AltOre GetAltOre<TOreSlot>() where TOreSlot : OreSlot => ref GetAltOre(GetInstance<TOreSlot>());
		public static ref AltOre GetAltOre(OreSlot slot) => ref ores[slot.Type];
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
			Array.Clear(ores);
		}
		public override void ClearWorld() {
			ores ??= new AltOre[OreSlotLoader.OreSlotCount];
		}

		public override void Unload() {
			worldEvilBiome = null;
			worldEvilName = null;
			worldHallowBiome = null;
			worldHallowName = null;
			worldHell = null;
			worldHellName = null;
			worldJungle = null;
			worldJungleName = null;
			drunkEvilName = null;
			drunkEvil = null;
			hmOreIndex = 0;
			drunkCobaltCycle = null;
			drunkMythrilCycle = null;
			drunkAdamantiteCycle = null;
		}

		public override void SaveWorldData(TagCompound tag) {
			tag.Add("AltLibrary:WorldEvil", WorldEvilName);
			tag.Add("AltLibrary:WorldHallow", WorldHallowName);
			tag.Add("AltLibrary:WorldHell", WorldHell?.FullName ?? "AltLibrary/UnderworldAltBiome");
			tag.Add("AltLibrary:WorldJungle", WorldJungle?.FullName ?? "AltLibrary/JungleAltBiome");
			tag.Add("AltLibrary:DrunkEvil", drunkEvilName);
			tag.Add("AltLibrary:Ores", (ores ?? []).Select(ore => new TagCompound {
				["Slot"] = ore.OreSlot.FullName,
				["Ore"] = ore.FullName
			}).Concat((unloadedOres ?? []).Select(unloadedOre => new TagCompound {
				["Slot"] = unloadedOre.slot,
				["Ore"] = unloadedOre.ore
			})).ToList());
			tag.Add("AltLibrary:HardmodeOreIndex", hmOreIndex);
		}
		public override void SaveWorldHeader(TagCompound tag) {
			tag.Add("WorldEvil", WorldEvilName);
			tag.Add("WorldHallow", WorldHallowName);
			tag.Add("WorldHell", WorldHellName);
			tag.Add("WorldJungle", WorldJungleName);
			tag.Add("DrunkEvil", drunkEvilName);
		}
		public override void LoadWorldData(TagCompound tag) {
			WorldEvilName = tag.GetString("AltLibrary:WorldEvil");
			WorldHallowName = tag.GetString("AltLibrary:WorldHallow");
			WorldHellName = tag.GetString("AltLibrary:WorldHell");
			WorldJungleName = tag.GetString("AltLibrary:WorldJungle");
			drunkEvilName = tag.GetString("AltLibrary:DrunkEvil");
			RewriterSystem.ValidateBiomeSelections();
			ores = new AltOre[OreSlotLoader.OreSlotCount];
			unloadedOres = [];
			if (tag.TryGet("AltLibrary:Ores", out List<TagCompound> selectedOres)) {
				for (int i = 0; i < selectedOres.Count; i++) {
					if (!selectedOres[i].TryGet("Slot", out string slot) || !selectedOres[i].TryGet("Ore", out string ore)) continue;
					if (TryFind(slot, out OreSlot oreSlot)) {
						if (!TryFind(ore, out ores[oreSlot.Type])) {
							ores[oreSlot.Type] = new UnloadedOre(oreSlot, ore);
						}
					} else {
						unloadedOres.Add((slot, ore));
					}
				}
			}
			for (int i = 0; i < ores.Length; i++) {
				if (ores[i] is null) {
					OreSlot slot = OreSlotLoader.GetOreSlot(i);
					if (slot is CopperOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Copper) ?? GetInstance<CopperAltOre>();
					} else if (slot is IronOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Iron) ?? GetInstance<IronAltOre>();
					} else if (slot is SilverOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Silver) ?? GetInstance<SilverAltOre>();
					} else if (slot is GoldOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Gold) ?? GetInstance<GoldAltOre>();
					} else if (slot is CobaltOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Cobalt) ?? GetInstance<CobaltAltOre>();
					} else if (slot is MythrilOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Mythril) ?? GetInstance<MythrilAltOre>();
					} else if (slot is AdamantiteOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Adamantite) ?? GetInstance<AdamantiteAltOre>();
					}
					ores[i] ??= slot.FallbackOre;
				}
			}
			hmOreIndex = tag.GetInt("AltLibrary:HardmodeOreIndex");
			DrunkEvil = null;

			//reset every unload
			drunkCobaltCycle = null;
			drunkMythrilCycle = null;
			drunkAdamantiteCycle = null;
			UpdateSavedOreTiers();
		}
		internal static void UpdateSavedOreTiers() {
			for (int i = 0; i < ores.Length; i++) {
				if (ores[i] is null) {
					OreSlot slot = OreSlotLoader.GetOreSlot(i);
					if (slot is CopperOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Copper) ?? GetInstance<CopperAltOre>();
					} else if (slot is IronOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Iron) ?? GetInstance<IronAltOre>();
					} else if (slot is SilverOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Silver) ?? GetInstance<SilverAltOre>();
					} else if (slot is GoldOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Gold) ?? GetInstance<GoldAltOre>();
					} else if (slot is CobaltOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Cobalt) ?? GetInstance<CobaltAltOre>();
					} else if (slot is MythrilOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Mythril) ?? GetInstance<MythrilAltOre>();
					} else if (slot is AdamantiteOreSlot) {
						ores[i] = AltLibrary.Ores.FirstOrDefault(ore => ore.ore == WorldGen.SavedOreTiers.Adamantite) ?? GetInstance<AdamantiteAltOre>();
					}
					ores[i] ??= slot.FallbackOre;
				}
			}
			WorldGen.SavedOreTiers.Copper = GetAltOre<CopperOreSlot>().ore;
			WorldGen.SavedOreTiers.Iron = GetAltOre<IronOreSlot>().ore;
			WorldGen.SavedOreTiers.Silver = GetAltOre<SilverOreSlot>().ore;
			WorldGen.SavedOreTiers.Gold = GetAltOre<GoldOreSlot>().ore;
			WorldGen.SavedOreTiers.Cobalt = GetAltOre<CobaltOreSlot>().ore;
			WorldGen.SavedOreTiers.Mythril = GetAltOre<MythrilOreSlot>().ore;
			WorldGen.SavedOreTiers.Adamantite = GetAltOre<AdamantiteOreSlot>().ore;
		}
		public override void PostUpdateTime() => UpdateSavedOreTiers();

		public override void NetSend(BinaryWriter writer) {
			writer.Write(WorldEvilName);
			writer.Write(WorldHallowName);
			writer.Write(WorldHellName);
			writer.Write(WorldJungleName);
			writer.Write(drunkEvilName);
			for (int i = 0; i < ores.Length; i++) {
				writer.Write(ores[i].Type);
			}
			writer.Write(hmOreIndex);
		}

		public override void NetReceive(BinaryReader reader) {
			WorldEvilName = reader.ReadString();
			WorldHallowName = reader.ReadString();
			WorldHellName = reader.ReadString();
			WorldJungleName = reader.ReadString();
			string oldDrunkEvilName = drunkEvilName;
			drunkEvilName = reader.ReadString();
			if (drunkEvilName != oldDrunkEvilName) DrunkEvil = null;
			ores ??= new AltOre[OreSlotLoader.OreSlotCount];
			for (int i = 0; i < ores.Length; i++) {
				ores[i] = AltLibrary.Ores[reader.ReadInt32()];
			}
			hmOreIndex = reader.ReadInt32();
		}
	}
}
