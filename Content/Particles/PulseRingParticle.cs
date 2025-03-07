﻿using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using TwilightEgress.Core;
using TwilightEgress.Core.Graphics.Particles;

namespace TwilightEgress.Content.Particles
{
    public class PulseRingParticle : CasParticle
    {
        public float InitialScale;

        public float MaxScale;

        public bool UseSoftTexture;

        public Vector2 Squish;

        private AtlasTexture RingTextureToDraw;

        public override string AtlasTextureName => "TwilightEgress.HollowCircleHardEdge.png";

        public override BlendState BlendState => BlendState.Additive;

        // Normal Pulse Ring.
        public PulseRingParticle(Vector2 position, Vector2 velocity, Color drawColor, float originalScale, float finalScale, int lifeTime, bool useSoftTexture = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            InitialScale = originalScale;
            MaxScale = finalScale;
            Lifetime = lifeTime;
            UseSoftTexture = useSoftTexture;

            Squish = Vector2.One;
            Rotation = Main.rand.NextFloat(MathF.Tau);
        }

        // Directional Pulse Ring.
        public PulseRingParticle(Vector2 position, Vector2 velocity, Color drawColor, float originalScale, float finalScale, Vector2 squish, float rotation, int lifeTime, bool useSoftTexture = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            InitialScale = originalScale;
            MaxScale = finalScale;
            Squish = squish;
            Rotation = rotation;
            Lifetime = lifeTime;
            UseSoftTexture = useSoftTexture;
        }

        public override void Update()
        {
            Scale = new(MathHelper.Lerp(InitialScale, MaxScale, EasingFunctions.QuartEaseOut(LifetimeRatio)));
            Opacity = MathF.Sin(MathHelper.PiOver2 + LifetimeRatio * MathHelper.PiOver2);

            Velocity *= 0.98f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            RingTextureToDraw = UseSoftTexture ? AtlasManager.GetTexture("TwilightEgress.HollowCircleSoftEdge.png") : Texture;
            spriteBatch.Draw(RingTextureToDraw, Position - Main.screenPosition, Frame, DrawColor * Opacity, Rotation, null, Scale * Squish);
        }
    }
}
