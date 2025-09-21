using AltLibrary.Common.AltBiomes;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks
{
	public class ShopHelper_EvilBiomes : ILoadable {
		private static IShoppingBiome[] _dangerousBiomes;
		public static List<IShoppingBiome> DangerousBiomes { get; private set; } = new() {
			new CorruptionBiome(),
			new CrimsonBiome(),
			new DungeonBiome()
		};
		public void Load(Mod mod) {
			IL_ShopHelper.IsPlayerInEvilBiomes += IL_ShopHelper_IsPlayerInEvilBiomes;
		}

		private static void IL_ShopHelper_IsPlayerInEvilBiomes(ILContext il) {
			ILCursor c = new(il);
			Func<IShoppingBiome[], IShoppingBiome[]> getDangerBiomes = (_) => {
				if (_dangerousBiomes is null || _dangerousBiomes.Length != DangerousBiomes.Count) {
					_dangerousBiomes = DangerousBiomes.ToArray();
				}
				return _dangerousBiomes;
			};
			while (c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<ShopHelper>("_dangerousBiomes")
			)) {
				c.EmitDelegate(getDangerBiomes);
			}
		}
		public void Unload() {
			_dangerousBiomes = null;
			DangerousBiomes = null;
		}
	}
}
