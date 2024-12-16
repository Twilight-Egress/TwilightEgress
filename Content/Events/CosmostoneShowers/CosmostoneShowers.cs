﻿using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using Luminance.Assets;
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
using TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Planetoids;
using TwilightEgress.Content.Particles;
using TwilightEgress.Content.Projectiles;
using TwilightEgress.Content.Skies.SkyEntities;
using TwilightEgress.Content.Skies.SkyEntities.StationaryAsteroids;
using TwilightEgress.Content.Skies.SkyEntities.TravellingAsteroid;
using TwilightEgress.Core;
using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;
using TwilightEgress.Core.Graphics.GraphicalObjects.SkyEntities;
using TwilightEgress.Core.Systems.EventHandlerSystem;

namespace TwilightEgress.Content.Events.CosmostoneShowers
{
    public class CosmostoneShowers : Event
    {
        public VirtualCamera VirtualCameraInstance { get; private set; }

        private InterpolatedColorBuilder ShiningStarColorBuilder;

        private const int MaxShiningStars = 500;

        private const int MaxShiningStarsForeground = 30;

        private const int MaxTravellingAsteroids = 100;

        private const int MaxStationaryAsteroids = 25;

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

        private int TravellingAsteroidSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;
                return 100 * (int)Math.Round(MathHelper.Lerp(1f, 0.6f, Star.starfallBoost / 3f), 0);
            }
        }

        private int StationaryAsteroidSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;
                return 55;
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
                .AddColor(Color.Violet)
                .AddColor(Color.DeepSkyBlue)
                .AddColor(Color.CornflowerBlue)
                .AddColor(Color.White)
                .AddColor(Color.Yellow)
                .AddColor(Color.Orange)
                .AddColor(Color.Red);
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
            if (spawnInfo.Sky)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<Manaphage>(), 0.56f);
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
            int asteroidSpawnChance = 125;
            int planetoidSpawnChance = 500;

            List<NPC> activePlanetoids = TwilightEgress.BasePlanetoidInheriters.Where(p => p.active).ToList();
            List<NPC> activePlanetoidsOnScreen = new();

            // Get all active planetoids that are on-screen.
            foreach (NPC planetoid in activePlanetoids)
            {
                Rectangle planetoidBounds = new((int)planetoid.Center.X, (int)planetoid.Center.Y, (int)planetoid.localAI[0], (int)planetoid.localAI[1]);
                Rectangle screenBounds = new((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 100, Main.screenHeight + 100);
                if (planetoidBounds.Intersects(screenBounds))
                    activePlanetoidsOnScreen.Add(planetoid);
            }

            // Asteroids.
            if (closestPlayer.active && !closestPlayer.dead && closestPlayer.Center.Y <= Main.maxTilesY + 1000f && Main.rand.NextBool(asteroidSpawnChance))
            {
                // Default spawn position.
                Vector2 asteroidSpawnPosition = closestPlayer.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(1250f, 250f), Main.rand.NextFloat(600f, 200f));

                WeightedRandom<int> asteroids = new WeightedRandom<int>();
                asteroids.Add(ModContent.NPCType<CosmostoneAsteroidSmall>(), 4);
                asteroids.Add(ModContent.NPCType<CosmostoneAsteroidMedium>(), 2);
                asteroids.Add(ModContent.NPCType<CosmostoneAsteroidLarge>(), 1);
                asteroids.Add(ModContent.NPCType<CosmostoneGeode>(), 1.5);
                asteroids.Add(ModContent.NPCType<SilicateAsteroidSmall>(), 8);
                asteroids.Add(ModContent.NPCType<SilicateAsteroidMedium>(), 4);
                asteroids.Add(ModContent.NPCType<SilicateAsteroidLarge>(), 2);

                if (NPC.downedBoss2)
                    asteroids.Add(ModContent.NPCType<MeteoriteAsteroid>(), 0.5f);

                // Search for any active Planetoids currently viewable on-screen.
                // Change the spawn position of asteroids to a radius around the center of these Planetoids if there are any active at the time.
                // This allows most asteroids to not just spawn directly inside of Planetoids or their radius (may be buggy if there are 
                // multiple Planetoids close to each other).
                foreach (NPC planetoid in activePlanetoidsOnScreen)
                {
                    float radiusAroundPlanetoid = planetoid.localAI[0] + planetoid.localAI[1] + Main.rand.NextFloat(1000f, 200f);
                    Vector2 planetoidPositionWithRadius = planetoid.Center + Vector2.UnitX.RotatedByRandom(Math.Tau) * radiusAroundPlanetoid;
                    asteroidSpawnPosition = planetoidPositionWithRadius;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && !Collision.SolidCollision(asteroidSpawnPosition, 300, 300))
                {
                    int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), asteroidSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, asteroids.Get());
                    if (Main.projectile.IndexInRange(p))
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                }
            }

            // Planetoids.
            if (closestPlayer.active && !closestPlayer.dead && closestPlayer.Center.Y <= Main.maxTilesY + 300f && closestPlayer.Center.Y >= Main.maxTilesY * 0.5f && Main.rand.NextBool(planetoidSpawnChance))
            {
                Vector2 planetoidSpawnPosition = closestPlayer.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(2500f, 1500f), 600f);
                if (activePlanetoidsOnScreen.Count > 0)
                {
                    NPC otherPlanetoid = activePlanetoids.LastOrDefault();
                    if (otherPlanetoid.active)
                    {
                        float radiusAroundPlanetoid = otherPlanetoid.localAI[0] + otherPlanetoid.localAI[1] + Main.rand.NextFloat(2000f, 750f);
                        Vector2 planetoidPositionWithRadius = otherPlanetoid.Center + Vector2.UnitX.RotatedByRandom(Math.Tau) * radiusAroundPlanetoid;
                        planetoidSpawnPosition = planetoidPositionWithRadius;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && !Collision.SolidCollision(planetoidSpawnPosition, 1600, 1600) && activePlanetoids.Count < 10)
                {
                    int[] planetoidTypes =
                    {
                        ModContent.NPCType<GalileoPlanetoid>(),
                        ModContent.NPCType<ShatteredPlanetoid>()
                    };

                    int planetoid = planetoidTypes[Main.rand.Next(0, planetoidTypes.Length)];

                    int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), planetoidSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, planetoid);
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
            Texture2D skyTexture = AssetRegistry.Textures.CosmostoneShowersNebulaColors.Value;

            ShaderManager.TryGetShader("TwilightEgress.CosmostoneShowersSkyShader", out ManagedShader cosmoSkyShader);
            cosmoSkyShader.TrySetParameter("galaxyOpacity", globalOpacity);
            cosmoSkyShader.TrySetParameter("fadeOutMargin", fadeOutInterpolant);
            cosmoSkyShader.TrySetParameter("textureSize", new Vector2(skyTexture.Width, skyTexture.Height));
            cosmoSkyShader.SetTexture(AssetRegistry.Textures.RealisticClouds, 1, samplerState);
            cosmoSkyShader.SetTexture(AssetRegistry.Textures.RealisticClouds, 2, samplerState);
            cosmoSkyShader.SetTexture(AssetRegistry.Textures.PerlinNoise2, 3, samplerState);
            cosmoSkyShader.Apply();

            spriteBatch.Draw(skyTexture, new Rectangle(0, (int)(Main.worldSurface * gradientHeightInterpolant + 50f), (int)(Main.screenWidth * 1.5f), (int)(Main.screenHeight * 1.5f)), Color.White * globalOpacity);

            // Clouds below the nebula.
            Texture2D cloudTexture = AssetRegistry.Textures.NeuronNebulaGalaxyBlurred.Value;

            ShaderManager.TryGetShader("TwilightEgress.CosmostoneShowersCloudsShader", out ManagedShader cosmoCloudsShader);
            cosmoCloudsShader.TrySetParameter("cloudOpacity", globalOpacity * 0.6f);
            cosmoCloudsShader.TrySetParameter("fadeOutMarginTop", 0.92f);
            cosmoCloudsShader.TrySetParameter("fadeOutMarginBottom", 0.75f);
            cosmoCloudsShader.TrySetParameter("erosionStrength", 0.8f);
            cosmoCloudsShader.TrySetParameter("textureSize", cloudTexture.Size());
            cosmoCloudsShader.SetTexture(AssetRegistry.Textures.RealisticClouds, 1, samplerState);
            cosmoCloudsShader.SetTexture(AssetRegistry.Textures.PerlinNoise3, 2, samplerState);
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
            for (int i = 0; i < foregroundStarCount; i++)
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
            int totalAsteroidsLayers = 5;

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

            // Horizontally-travelling Asteroids.
            int travellingAsteroids = SkyEntityManager.CountActiveSkyEntities<TravellingCosmostoneAsteroidSmall>()
                + SkyEntityManager.CountActiveSkyEntities<TravellingCosmostoneAsteroidMedium>()
                + SkyEntityManager.CountActiveSkyEntities<TravellingCosmostoneAsteroidLarge>()
                + SkyEntityManager.CountActiveSkyEntities<TravellingCosmostoneGeode>()
                + SkyEntityManager.CountActiveSkyEntities<TravellingSilicateAsteroidSmall>()
                + SkyEntityManager.CountActiveSkyEntities<TravellingSilicateAsteroidMedium>()
                + SkyEntityManager.CountActiveSkyEntities<TravellingSilicateAsteroidLarge>()
                + SkyEntityManager.CountActiveSkyEntities<TravellingMeteoriteAsteroid>();

            if (travellingAsteroids < MaxTravellingAsteroids && Main.rand.NextBool(TravellingAsteroidSpawnChance))
            {
                for (int i = 0; i < totalAsteroidsLayers; i++)
                {
                    float x = VirtualCameraInstance.Position.X - 2500f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.1f, 0.25f);
                    Vector2 position = new(x, y);

                    float speed = Main.rand.NextFloat(3f, 15f);
                    Vector2 velocity = Vector2.UnitX * speed;

                    float maxScale = Main.rand.NextFloat(0.5f, 2f);
                    float depth = i + 3f;
                    int lifespan = Main.rand.Next(1200, 1800);

                    WeightedRandom<SkyEntity> asteroids = new WeightedRandom<SkyEntity>();
                    asteroids.Add(new TravellingCosmostoneAsteroidSmall(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan), 4);
                    asteroids.Add(new TravellingCosmostoneAsteroidMedium(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan), 2);
                    asteroids.Add(new TravellingCosmostoneAsteroidLarge(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan), 1);
                    asteroids.Add(new TravellingCosmostoneGeode(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan), 1.5f);
                    asteroids.Add(new TravellingSilicateAsteroidSmall(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan), 8);
                    asteroids.Add(new TravellingSilicateAsteroidMedium(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan), 4);
                    asteroids.Add(new TravellingSilicateAsteroidLarge(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan), 2);

                    if (NPC.downedBoss2)
                        asteroids.Add(new TravellingMeteoriteAsteroid(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan), 0.5);

                    asteroids.Get().Spawn();
                }
            }

            // Stationary, floating asteroids.
            int stationaryAsteroids = 0;

            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidSmall>()
                + SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidMedium>()
                + SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidLarge>()
                + SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneGeode>()
                + SkyEntityManager.CountActiveSkyEntities<StationarySilicateAsteroidSmall>()
                + SkyEntityManager.CountActiveSkyEntities<StationarySilicateAsteroidMedium>()
                + SkyEntityManager.CountActiveSkyEntities<StationarySilicateAsteroidLarge>()
                + SkyEntityManager.CountActiveSkyEntities<StationaryMeteoriteAsteroid>();

            if (stationaryAsteroids < MaxStationaryAsteroids && Main.rand.NextBool(StationaryAsteroidSpawnChance))
            {
                for (int i = 0; i < totalAsteroidsLayers; i++)
                {
                    float x = VirtualCameraInstance.Center.X + Main.rand.NextFloat(-VirtualCameraInstance.Size.X, VirtualCameraInstance.Size.X) * 2f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.1f, 0.225f);
                    Vector2 position = new(x, y);

                    float maxScale = Main.rand.NextFloat(0.5f, 2f);
                    int lifespan = Main.rand.Next(600, 1200);
                    float depth = i + 3f;

                    WeightedRandom<SkyEntity> asteroids = new WeightedRandom<SkyEntity>();
                    asteroids.Add(new StationaryCosmostoneAsteroidSmall(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan), 4);
                    asteroids.Add(new StationaryCosmostoneAsteroidMedium(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan), 2);
                    asteroids.Add(new StationaryCosmostoneAsteroidLarge(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan), 1);
                    asteroids.Add(new StationaryCosmostoneGeode(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan), 1.5);
                    asteroids.Add(new StationarySilicateAsteroidSmall(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan), 8);
                    asteroids.Add(new StationarySilicateAsteroidMedium(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan), 4);
                    asteroids.Add(new StationarySilicateAsteroidLarge(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan), 2);

                    if (NPC.downedBoss2)
                        asteroids.Add(new StationaryMeteoriteAsteroid(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan), 0.5);

                    asteroids.Get().Spawn();
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