using AltLibrary.Common.AltBiomes;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace AltLibrary.Common.Systems {
	//TODO: find out how this sort of error was supposed to happen to begin with
	internal class RewriterSystem : ModSystem {
		public override void OnWorldLoad() {
			WorldBiomeManager.WorldEvilName ??= "";
			WorldBiomeManager.WorldHallowName ??= "";
			WorldBiomeManager.WorldHellName ??= "";
			WorldBiomeManager.WorldJungleName ??= "";
			WorldBiomeManager.drunkEvilName ??= "";
			WorldBiomeManager.DrunkEvil = null;

			string evil = WorldBiomeManager.WorldEvilName;
			string hallow = WorldBiomeManager.WorldHallowName;
			string hell = WorldBiomeManager.WorldHellName;
			string jungle = WorldBiomeManager.WorldJungleName;
			int[] causedIssueBy = [-1, -1, -1, -1];
			if (evil != "" && ModContent.Find<AltBiome>(evil).BiomeType != BiomeType.Evil) {
				List<string> checks = [hallow, hell, jungle];
				foreach (string check in checks) {
					if (check != "" && ModContent.Find<AltBiome>(check).BiomeType == BiomeType.Evil) {
						causedIssueBy[0] = (int)AltLibrary.Biomes.First(x => x.FullName == check).BiomeType;
						break;
					}
				}
			}
			if (hallow != "" && ModContent.Find<AltBiome>(hallow).BiomeType != BiomeType.Hallow) {
				List<string> checks = [evil, hell, jungle];
				foreach (string check in checks) {
					if (check != "" && ModContent.Find<AltBiome>(check).BiomeType == BiomeType.Hallow) {
						causedIssueBy[1] = (int)AltLibrary.Biomes.First(x => x.FullName == check).BiomeType;
						break;
					}
				}
			}
			if (hell != "" && ModContent.Find<AltBiome>(hell).BiomeType != BiomeType.Hell) {
				List<string> checks = [evil, hallow, jungle];
				foreach (string check in checks) {
					if (check != "" && ModContent.Find<AltBiome>(check).BiomeType == BiomeType.Hell) {
						causedIssueBy[2] = (int)AltLibrary.Biomes.First(x => x.FullName == check).BiomeType;
						break;
					}
				}
			}
			if (jungle != "" && ModContent.Find<AltBiome>(jungle).BiomeType != BiomeType.Jungle) {
				List<string> checks = [evil, hallow, hell];
				foreach (string check in checks) {
					if (check != "" && ModContent.Find<AltBiome>(check).BiomeType == BiomeType.Jungle) {
						causedIssueBy[3] = (int)AltLibrary.Biomes.First(x => x.FullName == check).BiomeType;
						break;
					}
				}
			}
			for (int i = 0; i < 4; i++) {
				if (causedIssueBy[i] == -1) {
					AltLibrary.Instance.Logger.Info("Checking issues done! Enjoy game!");
				} else {
					string inside = causedIssueBy[i] == 0 ? "evil" : (causedIssueBy[i] == 1 ? "hallow" : (causedIssueBy[i] == 2 ? "hell" : "jungle"));

					AltLibrary.Instance.Logger.Info($"Found error in {inside}! Fixing...");

					switch (inside) {
						case "evil": {
							switch (causedIssueBy[i]) {
								case 1: {
									string evil2 = evil;
									string hallow2 = hallow;
									WorldBiomeManager.WorldHallowName = evil2;
									WorldBiomeManager.WorldEvilName = hallow2;
									break;
								}
								case 2: {
									string evil2 = evil;
									string hallow2 = hell;
									WorldBiomeManager.WorldHellName = evil2;
									WorldBiomeManager.WorldEvilName = hallow2;
									break;
								}
								case 3: {
									string evil2 = evil;
									string hallow2 = jungle;
									WorldBiomeManager.WorldJungleName = evil2;
									WorldBiomeManager.WorldEvilName = hallow2;
									break;
								}
							}
							break;
						}
						case "hallow": {
							switch (causedIssueBy[i]) {
								case 0: {
									string evil2 = evil;
									string hallow2 = hallow;
									WorldBiomeManager.WorldHallowName = evil2;
									WorldBiomeManager.WorldEvilName = hallow2;
									break;
								}
								case 2: {
									string evil2 = hell;
									string hallow2 = hallow;
									WorldBiomeManager.WorldHallowName = evil2;
									WorldBiomeManager.WorldHellName = hallow2;
									break;
								}
								case 3: {
									string evil2 = jungle;
									string hallow2 = hallow;
									WorldBiomeManager.WorldHallowName = evil2;
									WorldBiomeManager.WorldJungleName = hallow2;
									break;
								}
							}
							break;
						}
						case "hell": {
							switch (causedIssueBy[i]) {
								case 0: {
									string evil2 = evil;
									string hallow2 = hell;
									WorldBiomeManager.WorldHellName = evil2;
									WorldBiomeManager.WorldEvilName = hallow2;
									break;
								}
								case 1: {
									string evil2 = hell;
									string hallow2 = hallow;
									WorldBiomeManager.WorldHallowName = evil2;
									WorldBiomeManager.WorldHellName = hallow2;
									break;
								}
								case 3: {
									string evil2 = jungle;
									string hallow2 = hell;
									WorldBiomeManager.WorldHellName = evil2;
									WorldBiomeManager.WorldJungleName = hallow2;
									break;
								}
							}
							break;
						}
						case "jungle": {
							switch (causedIssueBy[i]) {
								case 0: {
									string evil2 = evil;
									string hallow2 = jungle;
									WorldBiomeManager.WorldJungleName = evil2;
									WorldBiomeManager.WorldEvilName = hallow2;
									break;
								}
								case 1: {
									string evil2 = jungle;
									string hallow2 = hallow;
									WorldBiomeManager.WorldHallowName = evil2;
									WorldBiomeManager.WorldJungleName = hallow2;
									break;
								}
								case 2: {
									string evil2 = jungle;
									string hallow2 = hell;
									WorldBiomeManager.WorldHellName = evil2;
									WorldBiomeManager.WorldJungleName = hallow2;
									break;
								}
							}
							break;
						}
					}

					AltLibrary.Instance.Logger.Info($"Error fixed! Report it if it's wrong.");
				}
			}

			if (WorldBiomeManager.WorldEvilName != "" && WorldGen.crimson) {
				WorldGen.crimson = false;
			}
		}
	}
}
