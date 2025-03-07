﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Core.Graphics.Particles;

namespace TwilightEgress.Content.Particles
{
    public class AngelatinGasParticle : CasParticle
    {
        private readonly float BaseOpacity;

        private readonly Texture2D SmokeTexture;

        private readonly List<string> Smokes =
        [
            "TwilightEgress/Assets/Textures/Extra/GreyscaleObjects/SmokeCloud",
            "TwilightEgress/Assets/Textures/Extra/GreyscaleObjects/SmokeCloud2",
            "TwilightEgress/Assets/Textures/Extra/GreyscaleObjects/SmokeCloud3",
            "TwilightEgress/Assets/Textures/Extra/GreyscaleObjects/SmokeCloud4",
            "TwilightEgress/Assets/Textures/Extra/GreyscaleObjects/SmokeCloud5",
            "TwilightEgress/Assets/Textures/Extra/GreyscaleObjects/SmokeCloud6"
        ];

        public AngelatinGasParticle(Vector2 position, Vector2 velocity, Color color, float scale, float baseOpacity, int lifespan)
        {
            Position = position;
            DrawColor = color;
            Scale = new(scale);
            BaseOpacity = baseOpacity;
            Lifetime = lifespan;
            Velocity = velocity;

            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            SmokeTexture = ModContent.Request<Texture2D>(Smokes[Main.rand.Next(Smokes.Count)]).Value;
        }

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        public override BlendState BlendState => BlendState.Additive;

        public override void Update()
        {
            Rotation += Velocity.X * 0.004f;
            Velocity *= 0.97f;

            int fadeOutThreshold = Lifetime - 10;
            Opacity = MathHelper.Lerp(BaseOpacity, 0f, (Time - fadeOutThreshold) / 10f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 position = Position - Main.screenPosition;
            Vector2 origin = SmokeTexture.Size() / 2f;
            spriteBatch.Draw(SmokeTexture, position, SmokeTexture.Frame(), DrawColor * Opacity, Rotation, origin, Scale / 12f, 0, 0f);
        }
    }
}
