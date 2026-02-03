#pragma warning disable CS0649
using PegasusLib;
using PegasusLib.Reflection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader;
using static Terraria.ModLoader.TileLoader;

namespace AltLibrary.Core.Reflection {
	[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Members assigned via reflection")]
	internal class TileLoaderReflection : ReflectionLoader {
		[ReflectionParentType(typeof(TileLoader))]
		static FastStaticFieldInfo<List<ConvertTile>[][]> tileConversionDelegates;
		public static bool HasConversion(int tileType, int conversionType) => tileConversionDelegates.Value[tileType]?[conversionType]?.Count > 0;
	}
}
