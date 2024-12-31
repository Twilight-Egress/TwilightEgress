using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using TwilightEgress.Assets;
using TwilightEgress.Content.NPCs.CosmostoneShowers;
using TwilightEgress.Content.NPCs.CosmostoneShowers.DwarfMoons;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Meteoroids;
using TwilightEgress.Content.Particles;
using TwilightEgress.Content.Projectiles;
using TwilightEgress.Content.SkyEntities;
using TwilightEgress.Content.SkyEntities.CosmostoneShowers;
using TwilightEgress.Core;
using TwilightEgress.Core.Graphics.Particles;

namespace TwilightEgress.Content.Events.CosmostoneShowers
{
    public class CosmostoneShowers : Event
    {
        public VirtualCamera VirtualCameraInstance { get; private set; }

        private InterpolatedColorBuilder ShiningStarColorBuilder;

        private const int MaxShiningStars = 500;

        private const int MaxShiningStarsForeground = 30;

        private int ShiningStarSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;
                return 12 * (int)Math.Round(MathHelper.Lerp(1f, 0.4f, Star.starfallBoost / 3f), 0);
            }
        }

        private int ShiningStarForegroundSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;
                return 175 * (int)Math.Round(MathHelper.Lerp(1f, 0.4f, Star.starfallBoost / 3f), 0);
            }
        }

        private int SiriusSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;

                int spawnChance = Main.tenthAnniversaryWorld ? 10000 : LanternNight.LanternsUp ? 50000 : 100000;
                return spawnChance * (int)Math.Round(MathHelper.Lerp(1f, 0.4f, Star.starfallBoost / 3f));
            }
        }

        public override bool PersistAfterLeavingWorld => true;

        public override void SafeOnModLoad()
        {
            ShiningStarColorBuilder = new InterpolatedColorBuilder()
                .AddColor(new Color(71, 126, 255))
                .AddColor(new Color(228, 211, 200))
                .AddColor(new Color(249, 221, 77))
                .AddColor(new Color(232, 103, 39))
                .AddColor(new Color(255, 0, 68));
        }

        public override void SafeOnModUnload()
        {
            ShiningStarColorBuilder = null;
        }

        public override bool PreUpdateEvent()
        {
            bool shouldStopEvent = Main.bloodMoon || Main.pumpkinMoon || Main.snowMoon || BossRushEvent.BossRushActive;
            bool shouldIncreaseSpawnRate = LanternNight.NextNightIsLanternNight;

            // Start and stop the event.
            if (!Main.dayTime && Main.time == 0 && !shouldStopEvent && !EventIsActive && Main.rand.NextBool(shouldIncreaseSpawnRate ? 7 : 15))
            {
                Main.NewText("A mana-rich asteroid belt is travelling through the astrasphere...", Color.DeepSkyBlue);
                EventHandlerManager.StartEvent<CosmostoneShowers>();
            }

            if ((Main.dayTime && EventIsActive) || shouldStopEvent)
                EventHandlerManager.StopEvent<CosmostoneShowers>();

            return true;
        }

        public override void UpdateEvent()
        {
            if (!EventIsActive)
                return;

            VirtualCameraInstance = new(Main.LocalPlayer);

            float xWorldPosition = ((Main.maxTilesX - 50) + 100) * 16f;
            float yWorldPosition = Main.maxTilesY * 0.057f;
            Vector2 playerPositionInBounds = new(xWorldPosition, yWorldPosition);

            int closestPlayerIndex = Player.FindClosest(playerPositionInBounds, 1, 1);
            Player closestPlayer = Main.player[closestPlayerIndex];

            // Important entities.
            Entities_SpawnSpecialSpaceNPCs(closestPlayer);

            // Visual objects.
            if (closestPlayer.Center.Y <= Main.maxTilesY * 3.5f)
            {
                Visuals_SpawnAmbientSkyEntities();
                Visuals_SpawnForegroundParticles();
            }
        }

        public override void EditEventSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            bool zoneCosmostoneShowers = EventHandlerManager.SpecificEventIsActive<CosmostoneShowers>() && (spawnInfo.Player.ZoneOverworldHeight || spawnInfo.Player.ZoneSkyHeight);

            if (!zoneCosmostoneShowers || spawnInfo.Invasion)
                return;

            // Space additions.
            if (spawnInfo.Player.ZoneSkyHeight)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<Manaphage>(), 1f);
                pool.Add(ModContent.NPCType<ChunkyCometpod>(), 1f);
                pool.Add(ModContent.NPCType<TerritorialCometpod>(), 1f);
            }

            // Surface additions.
            if (spawnInfo.Player.ZoneOverworldHeight)
            {
                pool.Add(NPCID.Firefly, 0.85f);
                pool.Add(NPCID.EnchantedNightcrawler, 0.75f);
            }

            // FUCK YOU
            if (pool.ContainsKey(ModContent.NPCType<ShockstormShuttle>()))
                pool.Remove(ModContent.NPCType<ShockstormShuttle>());
        }

        #region Entity Management
        // TODO:
        // Overall this entire spawning method once the Planetoid and Asteroid reworks are finished.
        // -fryzahh
        private void Entities_SpawnSpecialSpaceNPCs(Player closestPlayer)
        {
            List<ISpawnAvoidZone> activeObjects = new List<ISpawnAvoidZone>();

            foreach (NPC npc in Main.npc.Where(a => a.active && a.ModNPC is ISpawnAvoidZone))
            {
                activeObjects.Add(npc.ModNPC as ISpawnAvoidZone);
            }

            // Walkable objects :3
            if (closestPlayer.active && !closestPlayer.dead && closestPlayer.Center.Y <= Main.maxTilesY + 1000f && closestPlayer.Center.Y >= Main.maxTilesY * 0.5f)
            {
                Vector2 spawnPosition = new Vector2(Main.rand.NextGaussian(Main.screenWidth + 100, closestPlayer.Center.X), Main.rand.NextGaussian(Main.screenHeight + 100, closestPlayer.Center.Y));
                Rectangle screenBounds = new((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 100, Main.screenHeight + 100);

                bool canSpawn = !Collision.SolidCollision(spawnPosition, 300, 300) && !screenBounds.Contains((int)spawnPosition.X, (int)spawnPosition.Y);

                foreach (ISpawnAvoidZone obj in activeObjects)
                {
                    if ((obj.Position - spawnPosition).LengthSquared() <= MathF.Pow(obj.RadiusCovered, 2))
                    {
                        canSpawn = false;
                        break;
                    }
                }

                if (canSpawn && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    WeightedRandom<int> thingsToSpawn = new WeightedRandom<int>();
                    thingsToSpawn.Add(ModContent.NPCType<Asteroid>(), 1f);
                    thingsToSpawn.Add(ModContent.NPCType<GalileoDwarfMoon>(), 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<ShatteredDwarfMoon>(), 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<CosmostoneMeteoroidSmall>(), 4 * 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<CosmostoneMeteoroidMedium>(), 2 * 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<CosmostoneMeteoroidLarge>(), 1 * 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<CosmostoneGeode>(), 1.5 * 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<SilicateMeteoroidSmall>(), 8 * 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<SilicateMeteoroidMedium>(), 4 * 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<SilicateMeteoroidLarge>(), 2 * 0.02f);
                    thingsToSpawn.Add(ModContent.NPCType<Manaphage>(), 0.2f);

                    if (NPC.downedBoss2)
                    {
                        thingsToSpawn.Add(ModContent.NPCType<MeteoriteMeteoroid>(), 0.5f * 0.008f);
                        //thingsToSpawn.Add(ModContent.NPCType<ChunkyCometpod>(), 0.2f);
                    }

                    int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, thingsToSpawn.Get());
                    if (Main.projectile.IndexInRange(p))
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                }
            }
        }
        #endregion

        #region Visuals
        #region Background Visuals
        public static void RenderBackground(SpriteBatch spriteBatch, float globalOpacity)
        {
            SamplerState samplerState = SamplerState.LinearWrap;

            // Makes the backgrounds move up or down/fade in and out on the screen depending on how high up near Space the player is.
            float gradientHeightInterpolant = MathHelper.Lerp(0.1f, -1.6f, Main.Camera.Center.Y / (float)(Main.worldSurface * 16f - Main.maxTilesY * 0.3f));
            float fadeOutInterpolant = MathHelper.Lerp(2f, 0.1f, Main.Camera.Center.Y / (float)(Main.worldSurface * 16f - Main.maxTilesY * 0.3f));

            // Bakcground nebula.
            Texture2D skyTexture = AssetRegistry.Textures.Gradients.CosmostoneShowersNebulaColors.Value;

            ShaderManager.TryGetShader("TwilightEgress.CosmostoneShowersSkyShader", out ManagedShader cosmoSkyShader);
            cosmoSkyShader.TrySetParameter("galaxyOpacity", globalOpacity);
            cosmoSkyShader.TrySetParameter("fadeOutMargin", fadeOutInterpolant);
            cosmoSkyShader.TrySetParameter("textureSize", new Vector2(skyTexture.Width, skyTexture.Height));
            cosmoSkyShader.SetTexture(AssetRegistry.Textures.Gradients.SwirlyNoise, 1, samplerState);
            cosmoSkyShader.SetTexture(AssetRegistry.Textures.Gradients.PerlinNoise2, 2, samplerState);
            cosmoSkyShader.Apply();

            spriteBatch.Draw(skyTexture, new Rectangle(0, (int)(Main.worldSurface * gradientHeightInterpolant + 50f), (int)(Main.screenWidth * 1.5f), (int)(Main.screenHeight * 1.5f)), Color.White * globalOpacity);

            // Clouds below the nebula.
            Texture2D cloudTexture = AssetRegistry.Textures.Gradients.NeuronNebulaGalaxyBlurred.Value;

            ShaderManager.TryGetShader("TwilightEgress.CosmostoneShowersCloudsShader", out ManagedShader cosmoCloudsShader);
            cosmoCloudsShader.TrySetParameter("cloudOpacity", globalOpacity * 0.6f);
            cosmoCloudsShader.TrySetParameter("fadeOutMarginTop", 0.92f);
            cosmoCloudsShader.TrySetParameter("fadeOutMarginBottom", 0.75f);
            cosmoCloudsShader.TrySetParameter("erosionStrength", 0.8f);
            cosmoCloudsShader.TrySetParameter("textureSize", cloudTexture.Size());
            cosmoCloudsShader.SetTexture(AssetRegistry.Textures.Gradients.RealisticClouds, 1, samplerState);
            cosmoCloudsShader.SetTexture(AssetRegistry.Textures.Gradients.PerlinNoise3, 2, samplerState);
            cosmoCloudsShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 3, samplerState);
            cosmoCloudsShader.Apply();

            spriteBatch.Draw(cloudTexture, new Rectangle(0, (int)(Main.worldSurface * gradientHeightInterpolant + 250f), Main.screenWidth, Main.screenHeight), Color.White * globalOpacity);
        }
        #endregion

        #region Graphical Object Handling
        private void Visuals_SpawnForegroundParticles()
        {
            // Ambient star particles; small sparkles that appear over the screen.
            int foregroundStarCount = Main.rand.Next(3) + 1;
            if (Main.rand.NextBool(3))
            {
                Vector2 starSpawnPos = Main.LocalPlayer.Center + Main.rand.NextVector2Circular(Main.screenWidth, Main.screenHeight);
                Vector2 starVelocity = Vector2.One.RotatedByRandom(Math.Tau) * Main.rand.NextFloat(-0.2f, 0.2f);
                float starScale = Main.rand.NextFloat(0.10f, 0.20f) * 2f;
                float parallaxStrength = Main.rand.NextFloat(1f, 5f);
                int starLifetime = Main.rand.Next(240, 360);
                Color starColor = ShiningStarColorBuilder.GetInterpolatedColor(Main.rand.NextFloat());

                new AmbientStarParticle(starSpawnPos, starVelocity, starScale, 0f, 1f, parallaxStrength, starLifetime, starColor).SpawnCasParticle();

                // Shining Stars; same ones that spawn in the background, only now rarely spawning in the foreground.
                if (CasParticleManager.CountParticles<ShiningStarParticle>() < MaxShiningStarsForeground && Main.rand.NextBool(ShiningStarForegroundSpawnChance))
                {
                    float shiningStarScale = Main.rand.NextFloat(0.75f, 2.25f);
                    float xStrectch = Main.rand.NextFloat(0.5f, 1.5f);
                    float yStretch = Main.rand.NextFloat(0.5f, 1.5f);
                    new ShiningStarParticle(starSpawnPos, starColor, shiningStarScale, parallaxStrength, new(xStrectch, yStretch), starLifetime).SpawnCasParticle();
                }
            }
        }

        private void Visuals_SpawnAmbientSkyEntities()
        {
            int totalStarLayers = 7;
            //int totalAsteroidsLayers = 5;

            // Shining Stars.
            if (SkyEntityManager.CountActiveSkyEntities<ShiningStar>() < MaxShiningStars && Main.rand.NextBool(ShiningStarSpawnChance))
            {
                for (int i = 0; i < totalStarLayers; i++)
                {
                    float x = VirtualCameraInstance.Center.X + Main.rand.NextFloat(-VirtualCameraInstance.Size.X, VirtualCameraInstance.Size.X) * 2f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(0.03f, 0.6f);
                    Vector2 position = new(x, y);

                    float maxScale = Main.rand.NextFloat(8f, 15f);
                    int lifespan = Main.rand.Next(120, 200);

                    float xStrectch = Main.rand.NextFloat(0.5f, 1.5f);
                    float yStretch = Main.rand.NextFloat(0.5f, 1.5f);

                    float depth = Main.rand.NextFloat(1f, 20f) * 10f;

                    Color starColor = ShiningStarColorBuilder.GetInterpolatedColor(Main.rand.NextFloat());

                    new ShiningStar(position, starColor, maxScale, depth, new Vector2(xStrectch, yStretch), lifespan).Spawn();
                }
            }

            // Have an extremely low chance for exactly one Sirius star to spawn.
            if (SkyEntityManager.CountActiveSkyEntities<Sirius>() < 1 && Main.rand.NextBool(SiriusSpawnChance))
            {
                int lifespan = Main.rand.Next(600, 1200);

                float x = VirtualCameraInstance.Center.X + Main.rand.NextFloat(-VirtualCameraInstance.Size.X, VirtualCameraInstance.Size.X) * 0.85f;
                float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, 0.08f);
                Vector2 position = new(x, y);

                new Sirius(position, Color.SkyBlue, 2f, lifespan).Spawn();
            }
        }
        #endregion

        #region World Lighting
        // Make things a little brighter during the event.
        public override void ModifyLightingBrightness(ref float scale)
        {
            Player player = Main.LocalPlayer;
            if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
                scale = 1.03f;
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            Player player = Main.LocalPlayer;
            if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
            {
                float brightnessMultiplierInterpolant = MathHelper.Lerp(2.15f, 1f, Main.Camera.Center.Y / (float)(Main.worldSurface * 16f - Main.maxTilesY * 0.3f));
                tileColor *= brightnessMultiplierInterpolant;
                backgroundColor *= brightnessMultiplierInterpolant;
            }
        }
        #endregion
        #endregion
    }
}
