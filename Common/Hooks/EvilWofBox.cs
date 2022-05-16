﻿using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using MonoMod.Cil;
using System;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common.Hooks
{
    internal class EvilWofBox
    {
        public static void Init()
        {
            IL.Terraria.NPC.CreateBrickBoxForWallOfFlesh += NPC_CreateBrickBoxForWallOfFlesh;
        }

        private static void NPC_CreateBrickBoxForWallOfFlesh(ILContext il)
        {
            ALUtils.ReplaceIDs(il,
                TileID.DemoniteBrick,
                (orig) => (ushort)(Find<AltBiome>(WorldBiomeManager.worldEvil).BiomeOreBrick ?? orig),
                (orig) => WorldBiomeManager.worldEvil != "" && Find<AltBiome>(WorldBiomeManager.worldEvil).BiomeOreBrick.HasValue);
        }
    }
}
