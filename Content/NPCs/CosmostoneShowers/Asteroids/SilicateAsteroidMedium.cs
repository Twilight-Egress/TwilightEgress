using TwilightEgress.Content.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using CalamityMod;
using TwilightEgress.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids
{
    public class SilicateAsteroidMedium : Asteroid, ILocalizedModType
    {
        public new string LocalizationCategory => "NPCs.CosmostoneShowers";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 500;
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
            // Slower rotation
            RotationSpeedSpawnFactor = Main.rand.NextFloat(150f, 720f) * Utils.SelectRandom(Main.rand, -1, 1);

            // Initialize a bunch of fields.
            NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            NPC.scale = Main.rand.NextFloat(0.75f, 1.25f);
            NPC.spriteDirection = Main.rand.NextBool().ToDirectionInt();
            NPC.frame.Y = Main.rand.Next(0, 3) * 66;
            NPC.netUpdate = true;
        }

        public override void SafeAI()
        {
            // Collision detection.
            List<NPC> activeAsteroids = Main.npc.Take(Main.maxNPCs).Where((NPC npc) => npc.active && npc.whoAmI != NPC.whoAmI && AsteroidValues.ViableCollisionTypes.Contains(npc.type)).ToList();
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
                int itemType = ModContent.ItemType<SilicateCluster>();
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SilicateCluster>(), default, minimumStack, maximumStack));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);

                    Dust d2 = Dust.NewDustPerfect(NPC.Center, DustID.TintableDust, speed * 5f * hit.HitDirection);
                    d2.color = Color.Lerp(Color.SlateGray, Color.DarkGray, Main.rand.NextFloat());
                    d2.scale = Main.rand.NextFloat(1f, 2f);
                }

                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 7f) * hit.HitDirection;
                    Color initialColor = Color.DarkGray;
                    Color fadeColor = Color.SaddleBrown;
                    float scale = Main.rand.NextFloat(0.85f, 1.75f) * NPC.scale;
                    float opacity = Main.rand.NextFloat(0.6f, 1f);
                    MediumMistParticle deathSmoke = new(NPC.Center, velocity, initialColor, fadeColor, scale, opacity, Main.rand.Next(180, 240), Main.rand.NextFloat(0.1f, 0.4f));
                    deathSmoke.SpawnCasParticle();
                }
            }
            else
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);

                    Dust d2 = Dust.NewDustPerfect(NPC.Center, DustID.TintableDust, speed * 5f * hit.HitDirection);
                    d2.color = Color.Lerp(Color.SlateGray, Color.DarkGray, Main.rand.NextFloat());
                    d2.scale = Main.rand.NextFloat(1f, 2f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 origin = NPC.frame.Size() / 2f;

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None);
            return false;
        }
    }
}
