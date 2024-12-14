using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.CosmostoneShowers
{
    public class SailorsSingularityVortex : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/NuclearFuryProjectile";
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.scale = 1f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }
        private int aiCounter
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private int explosionCounter
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private const float acceleration = 1f;
        private const int maxSpeed = 8;
        private NPC Target
        {
            get
            {
                Player owner = Main.player[Projectile.owner];
                if (owner.HasMinionAttackTargetNPC)
                    return CheckNPCTargetValidity(Main.npc[owner.MinionAttackTargetNPC]);
                else
                {
                    for (int npcIndex = 0; npcIndex < Main.npc.Length; npcIndex++)
                    {
                        NPC target = CheckNPCTargetValidity(Main.npc[npcIndex]);
                        if (target != null)
                            return target;
                    }
                }
                return null;
            }
        }
        public NPC CheckNPCTargetValidity(NPC potentialTarget)
        {
            if (potentialTarget.CanBeChasedBy(this, false))
            {
                float targetDist = Vector2.Distance(potentialTarget.Center, Projectile.Center);

                if (targetDist < 1000f && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, potentialTarget.position, potentialTarget.width, potentialTarget.height))
                    return potentialTarget;
            }

            return null;
        }

        public override void AI()
        {
            Projectile.scale = 2f;
            Player owner = Main.player[Projectile.owner];
            if (!Projectile.WithinRange(owner.Center, 1200f))
                Projectile.active = false;
            NPC target = Target;
            if (target != null)
            {
                Projectile.velocity += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                if (Projectile.velocity.LengthSquared() > maxSpeed * maxSpeed)
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
            }

            if (IsProjectileTouchingProjectile(Projectile, ModContent.ProjectileType<SailorsSingularityStar>()))
                explosionCounter++;
            if (IsProjectileTouchingProjectile(Projectile, ModContent.ProjectileType<SailorsSingularityBlast>()))
                explosionCounter = 6;

            if (explosionCounter >= 6 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CosmicDashExplosion>(), 200, 1f, Projectile.owner);
                if ((owner.Center - Projectile.Center).LengthSquared() < Projectile.width * 2 * (Projectile.width * 2))
                    owner.velocity += (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16;
                Projectile.active = false;
            }
            aiCounter++;
        }
        private static bool IsProjectileTouchingProjectile(Projectile myProjectile, int projType)
        {
            foreach (Projectile projectile in Main.projectile.Where(n => n.type == projType && n.active))
            {
                if (myProjectile.Hitbox.Intersects(projectile.Hitbox))
                {
                    projectile.active = false;
                    return true;
                }
            }
            return false;
        }
    }
}
