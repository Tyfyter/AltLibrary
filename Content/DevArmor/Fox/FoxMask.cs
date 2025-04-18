﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Content.DevArmor.Fox {
	[AutoloadEquip(EquipType.Head)]
	internal class FoxMask : ModItem {
		public override void SetStaticDefaults() {
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}

		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 14;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Cyan;
			Item.vanity = true;
		}
	}
}
