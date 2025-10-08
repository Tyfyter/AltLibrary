using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using System.Linq;
using PegasusLib;
using AltLibrary.Common.AltBiomes;

namespace AltLibrary {
#pragma warning disable CS0649
	public class AltLibraryConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public static AltLibraryConfig Config;

		public struct WorldDataValues {
			public string worldEvil;
			public string worldHallow;
			public string worldHell;
			public string worldJungle;
			public string drunkEvil;
			public void Validate() {
				worldEvil ??= "";
				worldHallow ??= "";
				worldHell ??= "";
				worldJungle ??= "";
				drunkEvil ??= "";
			}
		}

		[DefaultValue(false)]
		public bool ZenithIconJank;

		[DefaultValue(0), CustomModConfigItem(typeof(EvilBiomeCountRangeElement))]
		public int DrunkMaxBiomes;

#pragma warning restore CS0649

		public override void OnLoaded() => Config = this;

		[DefaultListValue(false)]
		[JsonProperty]
		private Dictionary<string, WorldDataValues> worldData = new();

		public Dictionary<string, WorldDataValues> GetWorldData() => worldData;
		public void SetWorldData(Dictionary<string, WorldDataValues> newDict) => worldData = newDict;
		public static void Save(ModConfig config) {
			Directory.CreateDirectory(ConfigManager.ModConfigPath);
			string filename = config.Mod.Name + "_" + config.Name + ".json";
			string path = Path.Combine(ConfigManager.ModConfigPath, filename);
			string json = JsonConvert.SerializeObject(config, ConfigManager.serializerSettings);
			File.WriteAllText(path, json);
		}
	}
	internal class EvilBiomeCountRangeElement : PrimitiveRangeElement<int> {
		public override int NumberTicks => (Max - Min) / Increment + 1;
		public override float TickIncrement => Increment / (float)(Max - Min);

		protected override float Proportion {
			get => (GetValue() - Min) / (float) (Max - Min);
			set => SetValue((int)Math.Round((value * (Max - Min) + Min) * (1f / Increment)) * Increment);
		}
		public EvilBiomeCountRangeElement() {
			Min = 0;
			Max = -1;
			foreach (AltBiome biome in AltLibrary.AllBiomes) {
				if (biome.BiomeType == BiomeType.Evil) Max++;
			}
			Increment = 1;
		}
		public override void OnBind() {
			base.OnBind();
			if (TList is null) {
				string name = Label ?? MemberInfo.Name;
				TextDisplayFunction = () => $"{name}: {(GetValue() == 0 ? "\u221e" : GetValue())}";
			}
		}
	}
}
