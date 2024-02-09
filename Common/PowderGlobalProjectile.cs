using AltLibrary.Common.AltBiomes;
using AltLibrary.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common {
	public class PowderGlobalProjectile : GlobalProjectile {
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
			return entity.type is ProjectileID.PurificationPowder or ProjectileID.VilePowder or ProjectileID.ViciousPowder;
		}
		public override bool PreAI(Projectile projectile) {
			projectile.velocity *= 0.95f;
			projectile.ai[0] += 1f;
			if (projectile.ai[0] == 180f) {
				projectile.Kill();
			}
			if (projectile.ai[1] == 0f) {
				projectile.ai[1] = 1f;
				int dustType = 0;
				switch (projectile.type) {
					case ProjectileID.PurificationPowder:
					dustType = DustID.PurificationPowder;
					break;
					case ProjectileID.VilePowder:
					dustType = DustID.VilePowder;
					break;
					case ProjectileID.ViciousPowder:
					dustType = DustID.ViciousPowder;
					break;
				}
				for (int i = 0; i < 30; i++) {
					Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X, projectile.velocity.Y, 50);
				}
			}
			if (Main.myPlayer == projectile.owner) {
				int minX = (int)(projectile.position.X / 16f) - 1;
				int minY = (int)(projectile.position.Y / 16f) - 1;
				int maxX = (int)((projectile.position.X + projectile.width) / 16f) + 2;
				int MaxY = (int)((projectile.position.Y + projectile.height) / 16f) + 2;
				if (minX < 0) {
					minX = 0;
				}
				if (minY < 0) {
					minY = 0;
				}
				if (maxX > Main.maxTilesX) {
					maxX = Main.maxTilesX;
				}
				if (MaxY > Main.maxTilesY) {
					MaxY = Main.maxTilesY;
				}
				AltBiome biome = default;
				switch (projectile.type) {
					case ProjectileID.PurificationPowder:
					biome = ModContent.GetInstance<DeconvertAltBiome>();
					break;

					case ProjectileID.VilePowder:
					biome = ModContent.GetInstance<CorruptionAltBiome>();
					break;

					case ProjectileID.ViciousPowder:
					biome = ModContent.GetInstance<CrimsonAltBiome>();
					break;
				}
				float tileWorldX;
				float tileWorldY;
				for (int i = minX; i < maxX; i++) {
					for (int j = minY; j < MaxY; j++) {
						tileWorldX = i * 16;
						tileWorldY = j * 16;
						if (!(projectile.position.X + projectile.width > tileWorldX)
							|| !(projectile.position.X < tileWorldX + 16f)
							|| !(projectile.position.Y + projectile.height > tileWorldY)
							|| !(projectile.position.Y < tileWorldY + 16f) 
							|| !Main.tile[i, j].HasTile) {
							continue;
						}
						ALConvert.ConvertTile(i, j, biome);
						ALConvert.ConvertWall(i, j, biome);
					}
				}
			}
			return false;
		}
	}
}
