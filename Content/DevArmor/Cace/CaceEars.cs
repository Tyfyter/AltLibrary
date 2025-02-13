using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Content.DevArmor.Cace {
	//TODO: too tall, make layer to fix clipping 
	[AutoloadEquip(EquipType.Head)]
	public class CaceEars : ModItem {
		public override void SetStaticDefaults() {
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}
		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 26;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Cyan;
			Item.vanity = true;
		}
	}
	public class CaceEarsLayer : PlayerDrawLayer {
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.head == ModContent.GetInstance<CaceEars>().Item.headSlot;
		protected override void Draw(ref PlayerDrawSet drawInfo) {
			for (int i = drawInfo.DrawDataCache.Count - 1; i >= 0; i--) {
				DrawData drawData = drawInfo.DrawDataCache[i];
				if (drawData.texture == TextureAssets.ArmorHead[drawInfo.drawPlayer.head].Value) {
					if (drawData.sourceRect is Rectangle sourceRect && sourceRect.Y > 0) {
						sourceRect.Y -= 4;
						sourceRect.Height -= 4;
						drawData.sourceRect = sourceRect;
						drawData.position.Y -= 4 * drawData.scale.Y;
						drawInfo.DrawDataCache[i] = drawData;
					}
				}
			}
		}
	}
}
