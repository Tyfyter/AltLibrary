using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;

namespace AltLibrary {
	public partial class AltLibrary : Mod {
		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			PacketType type = (PacketType)reader.ReadByte();
			switch (type) {
				case PacketType.SmashAltar:
				if (Main.netMode == NetmodeID.MultiplayerClient) {
					const string baseKey = "Mods.AltLibrary.BlessBase";
					Main.NewText(Language.GetTextValue(baseKey, Lang._mapLegendCache[MapHelper.TileToLookup(reader.ReadInt32(), 0)].Value), 50, byte.MaxValue, 130);
				}
				break;
			}
		}
	}
    public enum PacketType : byte {
        SmashAltar
    }
}
