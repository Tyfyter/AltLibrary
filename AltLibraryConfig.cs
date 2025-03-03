﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Terraria.ModLoader.Config;

namespace AltLibrary
{
#pragma warning disable CS0649
	public class AltLibraryConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public static AltLibraryConfig Config;

		public struct WorldDataValues
		{
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

		[DefaultValue(true)]
		public bool VanillaShowUpIfOnlyAltVarExist;

		[DefaultValue(true)]
		public bool SpecialSeedWorldPreview;

		[OptionStrings(new string[] { "None", "Hallow only", "Jungle only", "Both" })]
		[DefaultValue("Hallow only")]
		public string PreviewVisible;

		[DefaultValue(true)]
		public bool BiomeIconsVisible;

		[DefaultValue(true)]
		public bool OreIconsVisible;

		[DefaultValue(false)]
		public bool ZenithIconJank;

		[DefaultValue(false), ReloadRequired]
		public bool OldWorldCreationUI;

#pragma warning restore CS0649

		public override void OnLoaded() => Config = this;

		[DefaultListValue(false)]
		[JsonProperty]
		private Dictionary<string, WorldDataValues> worldData = new();

		public Dictionary<string, WorldDataValues> GetWorldData() => worldData;
		public void SetWorldData(Dictionary<string, WorldDataValues> newDict) => worldData = newDict;
		public static void Save(ModConfig config)
		{
			Directory.CreateDirectory(ConfigManager.ModConfigPath);
			string filename = config.Mod.Name + "_" + config.Name + ".json";
			string path = Path.Combine(ConfigManager.ModConfigPath, filename);
			string json = JsonConvert.SerializeObject(config, ConfigManager.serializerSettings);
			File.WriteAllText(path, json);
		}
	}
}
