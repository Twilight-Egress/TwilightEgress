﻿using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers;

namespace TwilightEgress
{
    public partial class TwilightEgress
    {
        #region NPC Lists
        internal static List<ISpawnAvoidZone> SpawnAvoidZoneInheriters { get; set; }

        public static List<NPC> BaseAsteroidInheriters { get; set; }
        #endregion

        #region Projectile Lists
        public static List<int> PickaxeProjectileIDs { get; set; }
        #endregion

        private static void LoadLists()
        {
            SpawnAvoidZoneInheriters = [];
            BaseAsteroidInheriters = [];

            PickaxeProjectileIDs = new()
            {
                ProjectileID.CobaltDrill,
                ProjectileID.PalladiumDrill,
                ProjectileID.OrichalcumDrill,
                ProjectileID.MythrilDrill,
                ProjectileID.AdamantiteDrill,
                ProjectileID.TitaniumDrill,
                ProjectileID.ChlorophyteDrill,
                ProjectileID.Hamdrax,
                ProjectileID.VortexDrill,
                ProjectileID.NebulaDrill,
                ProjectileID.SolarFlareDrill,
                ProjectileID.StardustDrill,
                ProjectileID.LaserDrill,
                ProjectileID.DrillMountCrosshair,
                ModContent.ProjectileType<MarniteObliteratorProj>(),
                ModContent.ProjectileType<WulfrumDrillProj>(),
                ModContent.ProjectileType<CrystylCrusherRay>()
            };
        }

        private static void UnloadLists()
        {
            SpawnAvoidZoneInheriters = null;
            BaseAsteroidInheriters = null;
            PickaxeProjectileIDs = null;
        }
    }
}
