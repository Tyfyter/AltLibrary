using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AltLibrary {
#pragma warning disable CS0649
	internal class AltLibraryServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public static AltLibraryServerConfig Config;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool SecretFeatures;

		[Header("$Mods.AltLibrary.Config.Randomization")]

		[DefaultValue(false)]
		public bool HardmodeGenRandom;
#pragma warning restore CS0649

		public override void OnLoaded() => Config = this;
	}
}
