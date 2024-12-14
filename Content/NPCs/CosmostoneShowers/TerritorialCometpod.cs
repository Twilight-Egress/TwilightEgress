using CalamityMod;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    public class TerritorialCometpod : ChunkyCometpod
    {
        private List<Vector2> SegmentPositions;

        private List<float> SegmentRotations;

        private const int MaxSegmentsForPrims = 20;

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 25;
            NPC.defense = 12;
            NPC.knockBackResist = 0.3f;
            NPC.lifeMax = 120;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath25;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = 10;
        }

        public override void PostAI()
        {
            NPC.AdjustNPCHitboxToScale(32f, 32f);
            InitializePrimitiveSegments();
            UpdatePrimitiveSegments();
        }

        private void InitializePrimitiveSegments()
        {
            if (SegmentPositions is null)
            {
                SegmentPositions = [];
                SegmentRotations = [];

                for (int i = 0; i < MaxSegmentsForPrims; i++)
                {
                    SegmentPositions.Add(NPC.Center);
                    SegmentRotations.Add(NPC.rotation);
                }
            }
        }

        private void UpdatePrimitiveSegments()
        {
            float segmentOffset = 118f / MaxSegmentsForPrims * NPC.scale;

            // Update the first segment in the lists since it won't be updated in the loop below.
            Vector2 directionToNextSegment = NPC.Center - (NPC.Center * segmentOffset).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.WrapAngle(NPC.rotation) * 0.03f);
            SegmentRotations[0] = NPC.rotation;
            SegmentPositions[0] = directionToNextSegment;

            // Loop through all segments after the first and ensure the line up with the ones ahead of one another.
            for (int i = 1; i < SegmentPositions.Count; i++)
            {
                Vector2 positionAhead = SegmentPositions[i - 1];
                float rotationAhead = SegmentRotations[i - 1];

                directionToNextSegment = positionAhead - SegmentPositions[i];
                if (rotationAhead != SegmentRotations[i])
                    directionToNextSegment = directionToNextSegment.RotatedBy(MathHelper.WrapAngle(rotationAhead - SegmentRotations[i]) * 0.03f);
                directionToNextSegment = directionToNextSegment.SafeNormalize(Vector2.Zero);

                SegmentRotations[i] = directionToNextSegment.ToRotation() + MathHelper.PiOver2;
                SegmentPositions[i] = positionAhead - directionToNextSegment * segmentOffset;

                // Testing.
                //CascadeUtilities.CreateDustLoop(1, SegmentPositions[i], Vector2.Zero, DustID.Dirt);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D npcTexture = TextureAssets.Npc[Type].Value;

            // Prevents crashes with mods that call PreDraw before the NPC spawns in-game, such as DragonLens.
            if (SegmentPositions is null || SegmentRotations is null)
                return false;

            Vector2[] segmentPositions = [.. SegmentPositions];
            Vector2 segmentAreaTopLeft = Vector2.One * 999999f;
            Vector2 segmentAreaTopRight = Vector2.Zero;

            for (int i = 0; i < segmentPositions.Length; i++)
            {
                segmentPositions[i] += -NPC.rotation.ToRotationVector2() * Math.Sign(NPC.velocity.X);
                if (segmentAreaTopLeft.X > segmentPositions[i].X)
                    segmentAreaTopLeft.X = segmentPositions[i].X;
                if (segmentAreaTopLeft.Y > segmentPositions[i].Y)
                    segmentAreaTopLeft.Y = segmentPositions[i].Y;

                if (segmentAreaTopRight.X < segmentPositions[i].X)
                    segmentAreaTopRight.X = segmentPositions[i].X;
                if (segmentAreaTopRight.Y < segmentPositions[i].Y)
                    segmentAreaTopRight.Y = segmentPositions[i].Y;
            }

            float offsetAngle = (NPC.position - NPC.oldPos[1]).ToRotation();
            Vector2 primitiveArea = (segmentAreaTopRight - segmentAreaTopLeft).RotatedBy(-offsetAngle);
            while (MathF.Abs(primitiveArea.X) < 118f)
                primitiveArea.X *= 1.008f;

            // This draws with additive blending for some reason??? idfk man
            spriteBatch.EnterShaderRegion(BlendState.AlphaBlend);

            ShaderManager.TryGetShader("TwilightEgress.PrimitiveTextureMapTrail", out ManagedShader textureMapTrailShader);
            textureMapTrailShader.SetTexture(npcTexture, 1, SamplerState.LinearClamp);
            textureMapTrailShader.TrySetParameter("mapTextureSize", npcTexture.Size());
            textureMapTrailShader.TrySetParameter("textureScaleFactor", primitiveArea);
            textureMapTrailShader.TrySetParameter("flipVertically", NPC.velocity.X > 0);
            textureMapTrailShader.TrySetParameter("flipHorizontally", true);
            textureMapTrailShader.Apply();

            PrimitiveSettings settings = new(_ => NPC.width * NPC.scale, _ => NPC.GetAlpha(drawColor), null, true, Shader: textureMapTrailShader);
            if (segmentPositions.Length >= 2f)
                PrimitiveRenderer.RenderTrail(segmentPositions, settings, MaxSegmentsForPrims * 2);

            spriteBatch.ResetToDefault();

            return false;
        }
    }
}
