using TwilightEgress.Content.Items.Materials;
using TwilightEgress.Core.BaseEntities.ModNPCs;
using Terraria.GameContent.ItemDropRules;
using TwilightEgress.Assets;
using CalamityMod;
using TwilightEgress.Content.Particles;
using Luminance.Core.Graphics;
using Luminance.Common.Utilities;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids
{
    public class CosmostoneAsteroidSmall : BaseAsteroid, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.BeforeNPCs;

        public new string LocalizationCategory => "NPCs.CosmostoneShowers";

        private float ShaderTimeMultiplier = 1f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 12;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 34;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 150;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.chaseable = false;
            NPC.knockBackResist = 0.4f;
            NPC.Opacity = 0f;

            NPC.HitSound = SoundID.Tink;
            NPC.DeathSound = SoundID.Item70;
        }

        public override void SafeOnSpawn(IEntitySource source)
        {
            // Initialize a bunch of fields.
            NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            NPC.scale = Main.rand.NextFloat(0.75f, 1.25f);
            NPC.spriteDirection = Main.rand.NextBool().ToDirectionInt();
            NPC.frame.Y = Main.rand.Next(0, 3) * 38;
            NPC.netUpdate = true;

            ShaderTimeMultiplier = Main.rand.NextFloat(0.1f, 1.5f) * Main.rand.NextBool().ToDirectionInt();
        }

        public override void OnMeteorCrashKill()
        {
            for (int i = 0; i < 45; i++)
            {
                Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f) * NPC.velocity.Y;
                Dust d = Dust.NewDustPerfect(NPC.Bottom, DustID.BlueTorch, speed * 0.85f);
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1f, 2f);
            }

            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 0.85f) * NPC.velocity.Y;
                Color initialColor = Color.Lerp(Color.DarkGray, Color.Cyan, Main.rand.NextFloat());
                Color fadeColor = Color.SaddleBrown;
                float scale = Main.rand.NextFloat(0.85f, 1.75f) * NPC.scale;
                float opacity = Main.rand.NextFloat(0.6f, 1f);
                MediumMistParticle deathSmoke = new(NPC.Bottom, velocity, initialColor, fadeColor, scale, opacity, Main.rand.Next(180, 240), 0.03f);
                deathSmoke.SpawnCasParticle();
            }
        }

        public override void SafeAI()
        {
            // Collision detection.
            List<NPC> activeAsteroids = Main.npc.Take(Main.maxNPCs).Where((npc) => npc.active && npc.whoAmI != NPC.whoAmI && AsteroidUtil.ViableCollisionTypes.Contains(npc.type)).ToList();
            int count = activeAsteroids.Count;

            if (count > 0)
            {
                // Bounce off of other nearby asteroids.
                foreach (NPC asteroid in activeAsteroids)
                {
                    if (NPC.Hitbox.Intersects(asteroid.Hitbox))
                    {
                        NPC.velocity = -NPC.DirectionTo(asteroid.Center) * (1f + NPC.velocity.Length() + asteroid.scale) * 0.15f;
                        asteroid.velocity = -asteroid.DirectionTo(NPC.Center) * (1f + NPC.velocity.Length() + NPC.scale) * 0.15f;
                    }
                }
            }
        }


        public void HandleOnHitDrops(Player player, Item item)
        {
            // Also, drop pieces of Cosmostone and Cometstone at a 1/10 chance.
            int chance = (int)(12 * MathHelper.Lerp(1f, 0.3f, NPC.scale / 2f) * MathHelper.Lerp(1f, 0.2f, item.pick / 250f));
            if (Main.rand.NextBool(chance))
            {
                int itemType = ModContent.ItemType<Cosmostone>();
                int itemStack = (int)Math.Round(1 * MathHelper.Lerp(1f, 3f, NPC.scale / 2f));
                int i = Item.NewItem(NPC.GetSource_OnHurt(player), NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height), itemType, itemStack);
                if (Main.item.IndexInRange(i))
                    Main.item[i].velocity = Main.rand.NextVector2Circular(4f, 4f);
            }
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            // If the player is using any pickaxe to hit the Asteroids...
            if (item.pick > 0)
                HandleOnHitDrops(player, item);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            // If the player is using some sort of drill or other mining tool which utilizes a held projectile...
            Player player = Main.player[projectile.owner];
            if (player.ActiveItem().pick > 0 && projectile.owner == player.whoAmI)
                HandleOnHitDrops(player, player.ActiveItem());
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            int minimumStack = (int)Math.Round(3 * MathHelper.Lerp(1f, 3f, NPC.scale / 2f));
            int maximumStack = (int)Math.Round(5 * MathHelper.Lerp(1f, 3f, NPC.scale / 2f));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cosmostone>(), default, minimumStack, maximumStack));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.BlueFairy, speed * 5f * hit.HitDirection);
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(1f, 2f);

                    Dust d2 = Dust.NewDustPerfect(NPC.Center, DustID.TintableDust, speed * 5f * hit.HitDirection);
                    d2.color = Color.Lerp(Color.SlateGray, Color.DarkGray, Main.rand.NextFloat());
                    d2.scale = Main.rand.NextFloat(1f, 2f);
                }

                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 7f) * hit.HitDirection;
                    Color initialColor = Color.Lerp(Color.DarkGray, Color.Cyan, Main.rand.NextFloat());
                    Color fadeColor = Color.SaddleBrown;
                    float scale = Main.rand.NextFloat(0.85f, 1.75f) * NPC.scale;
                    float opacity = Main.rand.NextFloat(0.6f, 1f);
                    MediumMistParticle deathSmoke = new(NPC.Center, velocity, initialColor, fadeColor, scale, opacity, Main.rand.Next(180, 240), Main.rand.NextFloat(0.1f, 0.4f));
                    deathSmoke.SpawnCasParticle();
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, speed * 5f * hit.HitDirection);
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(1f, 2f);


                    Dust d2 = Dust.NewDustPerfect(NPC.Center, DustID.TintableDust, speed * 5f * hit.HitDirection);
                    d2.color = Color.Lerp(Color.SlateGray, Color.DarkGray, Main.rand.NextFloat());
                    d2.scale = Main.rand.NextFloat(1f, 2f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawAsteroid(drawColor);
            return false;
        }

        public void DrawAsteroid(Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 origin = NPC.frame.Size() / 2f;

            /* Backglow effects.
            Main.spriteBatch.UseBlendState(BlendState.Additive);

            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                float spinAngle = Main.GlobalTimeWrappedHourly * 0.35f;
                Vector2 backglowDrawPosition = drawPosition + Vector2.UnitY.RotatedBy(spinAngle + TwoPi * i / 4) * 5f;
                DrawCosmostone(backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.Cyan), NPC.rotation, origin, NPC.scale, SpriteEffects.None);
            }
            Main.spriteBatch.ResetToDefault()*/

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, /*NPC.GetAlpha(Color.White)*/drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None);
            DrawCosmostone(drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, origin, NPC.scale, SpriteEffects.None);
        }

        public void DrawCosmostone(Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float worthless = 0f)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>("TwilightEgress/Content/NPCs/CosmostoneShowers/Asteroids/CosmostoneAsteroidSmall_Glowmask").Value; ;

            Main.spriteBatch.PrepareForShaders();

            ManagedShader shader = ShaderManager.GetShader("TwilightEgress.ManaPaletteShader");
            shader.TrySetParameter("flowCompactness", 3.0f);
            shader.TrySetParameter("gradientPrecision", 10f);
            shader.TrySetParameter("timeMultiplier", ShaderTimeMultiplier);
            shader.TrySetParameter("palette", TwilightEgressUtilities.CosmostonePalette);
            shader.TrySetParameter("opacity", NPC.Opacity);
            shader.Apply();

            Main.spriteBatch.Draw(glowmask, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
            Main.spriteBatch.ResetToDefault();
        }

        public float TrailWidthFunction(float trailLengthInterpolant) => 20f * Utils.GetLerpValue(0.75f, 0f, trailLengthInterpolant, true) * NPC.scale * NPC.Opacity;

        public Color TrailColorFunction(float trailLengthInterpolant) => Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, trailLengthInterpolant) * NPC.Opacity;

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ShaderManager.TryGetShader("TwilightEgress.SmoothTextureMapTrail", out ManagedShader smoothTrail);
            smoothTrail.SetTexture(AssetRegistry.Textures.MagicStreak, 1, SamplerState.LinearWrap);
            smoothTrail.TrySetParameter("time", Main.GlobalTimeWrappedHourly);

            PrimitiveSettings settings = new(TrailWidthFunction, TrailColorFunction, _ => NPC.Size * 0.5f, true, true, smoothTrail);
            PrimitiveRenderer.RenderTrail(NPC.oldPos, settings, 24);
        }
    }
}
