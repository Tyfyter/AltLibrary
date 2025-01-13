using AltLibrary.Common.Systems;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common {
	internal class ALNPC : GlobalNPC {
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.WallofFlesh;
		public override void OnKill(NPC npc) {
			if (Main.drunkWorld) {
				WorldBiomeGeneration.WofKilledTimes++;
				if (WorldBiomeGeneration.WofKilledTimes > 1) {
					StartHardmode();
				}
			} else if (WorldBiomeGeneration.WofKilledTimes == 0) {
				WorldBiomeGeneration.WofKilledTimes = 1;
			}
		}

		public static void StartHardmode() {
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.smCallBack), 1);
			}
		}
	}
}
