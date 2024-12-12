using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace AltLibrary.ModSupport {
	public class FargoSeeds : ILoadable {
		public static Func<bool> BothEvils { get; internal set; } = () => false;
		public static Func<bool> AllOres { get; internal set; } = () => false;
		public void Load(Mod mod) {
			if (ModLoader.TryGetMod(nameof(global::FargoSeeds.FargoSeeds), out Mod fargoSeeds)) {
				if (fargoSeeds.Code.GetType("FargoSeeds.WorldConfig") is Type worldConfig && worldConfig.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static) is PropertyInfo configInstance) {
					if (worldConfig.GetField("BothEvils") is FieldInfo bothEvils) {
						DynamicMethod getterMethod = new("get_BothEvils", typeof(bool), [], true);
						ILGenerator gen = getterMethod.GetILGenerator();

						gen.Emit(OpCodes.Call, configInstance.GetGetMethod());
						gen.Emit(OpCodes.Ldfld, bothEvils);
						gen.Emit(OpCodes.Ret);

						BothEvils = getterMethod.CreateDelegate<Func<bool>>();
					}
					if (worldConfig.GetField("AllOres") is FieldInfo allOres) {
						DynamicMethod getterMethod = new("get_AllOres", typeof(bool), [], true);
						ILGenerator gen = getterMethod.GetILGenerator();

						gen.Emit(OpCodes.Call, configInstance.GetGetMethod());
						gen.Emit(OpCodes.Ldfld, allOres);
						gen.Emit(OpCodes.Ret);

						AllOres = getterMethod.CreateDelegate<Func<bool>>();
					}
				}
			}
		}
		public void Unload() {
			BothEvils = null;
			AllOres = null;
		}
	}
}
