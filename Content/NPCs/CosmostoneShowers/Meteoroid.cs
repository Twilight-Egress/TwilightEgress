using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Meteoroids;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    internal class MeteoroidValues
    {
        internal static List<int> ViableCollisionTypes = new List<int>()
        {
            ModContent.NPCType<CosmostoneMeteoroidSmall>(),
            ModContent.NPCType<CosmostoneMeteoroidMedium>(),
            ModContent.NPCType<CosmostoneMeteoroidLarge>(),
            ModContent.NPCType<CosmostoneGeode>(),
            ModContent.NPCType<SilicateMeteoroidSmall>(),
            ModContent.NPCType<SilicateMeteoroidMedium>(),
            ModContent.NPCType<SilicateMeteoroidLarge>(),
            ModContent.NPCType<MeteoriteMeteoroid>()
        };

        /// <summary>
        /// The palette used for mana flowing through Cosmostone Asteroids.
        /// </summary>
        public static readonly Vector4[] CosmostonePalette =
        {
            new Color(96, 188, 246).ToVector4(),
            new Color(81, 158, 245).ToVector4(),
            new Color(76, 131, 242).ToVector4(),
            new Color(3, 96, 243).ToVector4(),
            new Color(48, 65, 197).ToVector4(),
            new Color(104, 94, 228).ToVector4(),
            new Color(157, 113, 239).ToVector4(),
        };
    }

    public abstract class Meteoroid : ModNPC, ISpawnAvoidZone
    {
        public ref float Timer => ref NPC.ai[0];

        public ref float RotationSpeedSpawnFactor => ref NPC.ai[1];

        public ref float MaxTime => ref NPC.ai[2];

        public float RadiusCovered => 100f;

        public Vector2 Position => NPC.Center;

        public bool Active => NPC.active;

        public sealed override string LocalizationCategory => "NPCs.CosmostoneShowers.Meteoroids";

        public sealed override void OnSpawn(IEntitySource source)
        {
            RotationSpeedSpawnFactor = Main.rand.NextFloat(75f, 480f) * Utils.SelectRandom(Main.rand, -1, 1);
            MaxTime = Main.rand.NextFloat(1200, 7200);
            SafeOnSpawn(source);
        }

        public override bool PreAI()
        {
            // Add to the global list of classes that inherit this base class.
            TwilightEgress.BaseAsteroidInheriters.AddWithCondition(NPC, !TwilightEgress.BaseAsteroidInheriters.Contains(NPC));
            SafePreAI();
            return true;
        }

        public sealed override void AI()
        {
            NPC.TargetClosest();

            // Fade in.
            if (Timer < MaxTime)
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.02f, 0f, 1f);

            // Idly rotate.
            NPC.rotation += MathHelper.Pi / RotationSpeedSpawnFactor;
            NPC.rotation += NPC.velocity.X * 0.03f;

            // If an asteroid falls within a certain distance of Terraria's mesosphere, it
            // begins to be pulled in by the planet's gravity.

            // In simple terms, the Y-velocity is increased once the asteroid is pushed low enough.
            if (NPC.Bottom.Y >= Main.maxTilesY + 1000f)
            {
                // Increase damage as Y-velocity begins to increase.
                NPC.damage = 150 * (int)Utils.GetLerpValue(0f, 1f, NPC.velocity.Y / 12f, true);
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + 0.03f, 0f, 18f);

                // Die upon tile collision, explode.
                if (Collision.SolidCollision(NPC.Center, NPC.width, NPC.height))
                {
                    NPC.life = 0;
                    NPC.checkDead();
                    OnMeteorCrashKill();
                    NPC.netUpdate = true;
                    return;
                }
            }
            else
            {
                // Kill the asteroid's damage if it is simply floating around.
                NPC.damage = 0;
                // Always decrease velocity so that they don't drift off into No Man's Land.
                NPC.velocity *= 0.99f;
            }

            NPC.ShowNameOnHover = false;
            AdjustNPCHitboxToScale(NPC, 36f, 36f);

            // Despawn after some time as to not clog the NPC limit.
            Timer++;
            if (Timer >= MaxTime)
            {
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.02f, 0f, 1f);
                if (NPC.Opacity <= 0f)
                {
                    NPC.active = false;
                }
            }

            if (SafePreAI())
                SafeAI();
        }

        public void AdjustNPCHitboxToScale(NPC npc, float originalWidth, float originalHeight)
        {
            int oldWidth = npc.width;
            int idealWidth = (int)(npc.scale * originalWidth);
            int idealHeight = (int)(npc.scale * originalHeight);
            if (idealWidth != oldWidth)
            {
                npc.position.X += npc.width / 2;
                npc.position.Y += npc.height / 2;
                npc.width = idealWidth;
                npc.height = idealHeight;
                npc.position.X -= npc.width / 2;
                npc.position.Y -= npc.height / 2;
            }
        }

        public virtual void OnMeteorCrashKill() { }

        public virtual void SafeOnSpawn(IEntitySource source) { }

        public virtual bool SafePreAI() => true;

        public virtual void SafeAI() { }

        public virtual void SafeModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers) { }

        public virtual void SafeModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) { }
    }

    public class MeteoroidSystem : ModSystem
    {
        public override void Load()
        {
            On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += MineMeteoroid;
        }

        public override void Unload()
        {
            On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool -= MineMeteoroid;
        }

        private void MineMeteoroid(On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig, Player self, Item sItem, out bool canHitWalls, int x, int y)
        {
            orig(self, sItem, out canHitWalls, x, y);

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.ModNPC is Meteoroid && (self.Center - Main.MouseWorld).LengthSquared() <= Math.Pow((Player.tileRangeX + sItem.tileBoost) * 16, 2) && self.whoAmI == Main.myPlayer)
                {
                    if (npc.Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
                    {
                        int damageToAsteroid = sItem.pick;
                        npc.SimpleStrikeNPC(damageToAsteroid, 0, noPlayerInteraction: false);
                        self.ApplyItemTime(sItem, self.pickSpeed * 1.5f);
                    }
                }
            }
        }
    }
}
