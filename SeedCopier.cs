using AltLibrary.Common;
using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AltLibrary {
	class SeedCopier : ILoadable {
		public void Load(Mod mod) {
			On_WorldFileData.GetFullSeedText += On_WorldFileData_GetFullSeedText;
			try {
				IL_UIWorldCreation.ProcessSeed += IL_UIWorldCreation_ProcessSeed;
			} catch (Exception) {
#if DEBUG
				throw;
#endif
			}
			try {
				IL_UIWorldCreation.Click_SetSeed += IL_UIWorldCreation_Click_SetSeed;
			} catch (Exception) {
#if DEBUG
				throw;
#endif
			}
		}
		private string On_WorldFileData_GetFullSeedText(On_WorldFileData.orig_GetFullSeedText orig, WorldFileData self, bool allowCropping) {
			string original = orig(self, allowCropping);
			if (self.TryGetHeaderData<WorldBiomeManager>(out TagCompound tag)) {
				string[] parts = original.Split('.');
				List<string> selections = [];
				static string GetFullName(string name) => $"{name}Biome";
				if (tag.TryGet("WorldEvil", out string evil)) selections.Add(string.IsNullOrWhiteSpace(evil) ? GetFullName(self.HasCrimson ? "Crimson" : "Corrupt") : evil);
				if (AltLibrary.Biomes.Any(b => b.BiomeType == BiomeType.Hallow) && tag.TryGet("WorldHallow", out string hallow)) selections.Add(string.IsNullOrWhiteSpace(hallow) ? GetFullName("Hallow") : hallow);
				if (AltLibrary.Biomes.Any(b => b.BiomeType == BiomeType.Hell) && tag.TryGet("WorldHell", out string hell)) selections.Add(string.IsNullOrWhiteSpace(hell) ? GetFullName("Underworld") : hell);
				if (AltLibrary.Biomes.Any(b => b.BiomeType == BiomeType.Jungle) && tag.TryGet("WorldJungle", out string jungle)) selections.Add(string.IsNullOrWhiteSpace(jungle) ? GetFullName("Jungle") : jungle);
				parts[2] = string.Join(',', selections);
				original = string.Join('.', parts);
			}
			return original;
		}

		private void IL_UIWorldCreation_ProcessSeed(ILContext il) {
			ILCursor c = new(il);
			int loc = -1;
			c.GotoNext(MoveType.AfterLabel,
				i => i.MatchLdarg1(),//IL_00ce: ldarg.1
				i => i.MatchLdloc(out loc),//IL_00cf: ldloc.0
				i => i.MatchLdcI4(3),//IL_00d0: ldc.i4.3
				i => i.MatchLdelemRef(),//IL_00d1: ldelem.ref
				i => i.MatchStindRef()//IL_00d2: stind.ref
			);
			c.EmitLdloc(loc);
			c.EmitLdcI4(2);
			c.EmitLdelemRef();
			c.EmitDelegate((string text) => {
				if (text.Length == 1) {
					switch (text[0]) {
						case '1':
						NewWorldCreationMenu.selectedBiomes[(int)BiomeType.Evil] = ModContent.GetInstance<CorruptionAltBiome>();
						break;
						case '2':
						NewWorldCreationMenu.selectedBiomes[(int)BiomeType.Evil] = ModContent.GetInstance<CrimsonAltBiome>();
						break;
					}
					NewWorldCreationMenu.RefreshSelectionVisuals();
				} else {
					string[] selections = text.Split(',');
					AltBiome[] options = NewWorldCreationMenu.selectableBiomes.SelectMany(type => type.biomes).ToArray();
					for (int i = 0; i < selections.Length; i++) {
						if (!selections[i].Contains('/')) {
							selections[i] = "AltLibrary/" + selections[i];
						}
						for (int j = 0; j < options.Length; j++) {
							if (selections[i] == options[j].FullName) {
								NewWorldCreationMenu.selectedBiomes[(int)options[j].BiomeType] = options[j];
								break;
							}
						}
					}
					NewWorldCreationMenu.RefreshSelectionVisuals();
				}
			});
		}
		private void IL_UIWorldCreation_Click_SetSeed(ILContext il) {
			ILCursor c = new(il);
			c.GotoNext(MoveType.Before,
				i => i.MatchCallOrCallvirt<UIVirtualKeyboard>(nameof(UIVirtualKeyboard.SetMaxInputLength))
			);
			c.EmitLdcI4(400);
			c.EmitDelegate<Func<int, int, int>>(Math.Max);
		}
		public void Unload() { }
	}
}
