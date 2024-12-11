using CalamityMod;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;

namespace TwilightEgress.Content.Items.Dedicated.Fluffy
{
    public class HomingBastStatue : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];

        private Player ClosestTeamPlayer = null;

        private Player ClosestHostilePlayer = null;

        private ref float Timer => ref Projectile.ai[0];

        private ref float BounceLimit => ref Projectile.ai[1];

        private ref float RotationDirection => ref Projectile.TwilightEgress().ExtraAI[0];

        private ref float RotationSpeed => ref Projectile.TwilightEgress().ExtraAI[1];

        private bool CollidedWithTheOwner = false;

        private bool CollidedWithTeamPlayer = false;

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 0f;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
            RotationSpeed = Main.rand.NextFloat(45f, 150f);
        }

        public override void AI()
        {
            float maxDetectionRadius = 1500f;
            float maxTeamPlayerDetectionRadius = 500f;
            NPC closestTarget = FindClosestNPCToTarget(maxDetectionRadius);
            Player closestHostilePlayer = FindClosestEnemyPlayer(maxDetectionRadius);
            Player closestTeamPlayer = FindClosestTeamPlayer(maxTeamPlayerDetectionRadius);

            ClosestTeamPlayer = closestTeamPlayer;
            ClosestHostilePlayer = closestHostilePlayer;

            // Some small initialization.
            if (Timer == 0)
            {
                BounceLimit = 5;
                Projectile.netUpdate = true;
            }

            // The actual AI.
            if (Timer <= 60)
            {
                // Slow down for a second before homing in.
                Projectile.velocity *= 0.98f;
                Projectile.rotation += TwoPi / RotationSpeed * RotationDirection;
            }
            else
            {
                // If there are no NPCs, chase the player or any nearby team players.
                if (closestTarget == null && ClosestHostilePlayer == null)
                {
                    // Get the distance between the closest team player and the owner of the projectile.
                    // If the team player is within range, home onto them. Otherwise, home onto the owner.
                    if (ClosestTeamPlayer != null)
                    {
                        float distanceFromTeamPlayer = Vector2.DistanceSquared(ClosestTeamPlayer.Center, Owner.Center);
                        if (distanceFromTeamPlayer < maxTeamPlayerDetectionRadius)
                        {
                            if (Projectile.Hitbox.Intersects(ClosestTeamPlayer.Hitbox))
                            {
                                CollidedWithTheOwner = true;
                                Projectile.Kill();
                            }
                            Projectile.SimpleMove(ClosestTeamPlayer.Center, 40f, 45f);
                        }
                        else
                        {
                            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                            {
                                CollidedWithTeamPlayer = true;
                                Projectile.Kill();
                            }
                            Projectile.SimpleMove(Owner.Center, 40f, 45f);
                        }
                    }
                    else
                    {
                        if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                        {
                            CollidedWithTeamPlayer = true;
                            Projectile.Kill();
                        }
                        Projectile.SimpleMove(Owner.Center, 40f, 45f);
                    }

                }
                else
                {
                    if (ClosestHostilePlayer != null)
                    {
                        float distanceFronHostilePlayer = Vector2.DistanceSquared(ClosestHostilePlayer.Center, Owner.Center);
                        if (distanceFronHostilePlayer < maxDetectionRadius)
                            Projectile.SimpleMove(ClosestHostilePlayer.Center, 30f, 90f);
                        else
                            Projectile.SimpleMove(closestTarget.Center, 30f, 90f);
                    }
                    else
                        Projectile.SimpleMove(closestTarget.Center, 30f, 90f);
                }

                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() + 1.57f, 0.2f);
            }

            Timer++;
            Projectile.scale = Clamp(Projectile.scale + 0.05f, 0f, 1f);
            if (Main.rand.NextBool(3))
                TwilightEgressUtilities.CreateDustLoop(2, Main.rand.NextVector2Circular(Projectile.width, Projectile.height), Vector2.Zero, DustID.FireworkFountain_Yellow, shouldDefyGravity: true);
        }

        public override void OnKill(int timeLeft)
        {
            if (CollidedWithTheOwner || CollidedWithTeamPlayer)
            {
                // Friendly visuals and apply the buff.
                if (CollidedWithTheOwner)
                    Owner.AddBuff(BuffID.CatBast, 300);
                if (CollidedWithTeamPlayer)
                    ClosestTeamPlayer.AddBuff(BuffID.CatBast, 300);

                SoundEngine.PlaySound(SoundID.ResearchComplete, Projectile.Center);

                // Particle creation.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int sparkLifespan = Main.rand.Next(20, 36);
                    float sparkScale = Main.rand.NextFloat(0.75f, 1.25f);
                    Color sparkColor = Color.Lerp(Color.LightYellow, Color.Goldenrod, Main.rand.NextFloat());
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(3f, 8f);
                        SparkParticle deathSpark = new(Projectile.Center, sparkVelocity, sparkColor, sparkScale, sparkLifespan);
                        deathSpark.SpawnCasParticle();

                    }
                }
            }
            else
            {
                // KABOOM
                Projectile.BetterNewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Bastsplosion>(), Projectile.damage, Projectile.knockBack, owner: Projectile.owner);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.PvP)
            {
                // Land a critical hit if the projectile is on it's last bounce.
                if (BounceLimit == 1)
                    info.Damage *= 2;
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Land a critical hit if the projectile is on it's last bounce.
            if (BounceLimit == 1)
                hit.Crit = true;
            Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            BounceLimit--;
            if (BounceLimit <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundStyle meow = Utils.SelectRandom(Main.rand, SoundID.Item57, SoundID.Item58);
                SoundEngine.PlaySound(meow, Projectile.position);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Projectile.DrawBackglow(Projectile.GetAlpha(Color.Gold * 0.45f), 2f);
            Utilities.DrawAfterimagesCentered(Projectile, 0, Projectile.GetAlpha(Color.Gold));
            Main.spriteBatch.ResetToDefault();

            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(lightColor), Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }

        #region Helper Methods
        public NPC FindClosestNPCToTarget(float maxDetectionRadius)
        {
            NPC closestNPC = null;

            float squaredMaxDetectionRadius = maxDetectionRadius * maxDetectionRadius;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy())
                {
                    float squaredDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
                    if (squaredDistanceToTarget < squaredMaxDetectionRadius)
                    {
                        squaredMaxDetectionRadius = squaredDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        // Minor amounts of trolling
        public Player FindClosestEnemyPlayer(float maxDetectionRadius)
        {
            Player closestPlayer = null;

            float squaredMaxDetectionRadius = maxDetectionRadius * maxDetectionRadius;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player target = Main.player[i];
                if (target.InOpposingTeam(Owner) && target.hostile)
                {
                    float squaredDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
                    if (squaredDistanceToTarget < squaredMaxDetectionRadius)
                    {
                        squaredMaxDetectionRadius = squaredDistanceToTarget;
                        closestPlayer = target;
                    }
                }
            }

            return closestPlayer;
        }

        public Player FindClosestTeamPlayer(float maxDetectionRadius)
        {
            Player closestPlayer = null;

            float squaredMaxDetectionRadius = maxDetectionRadius * maxDetectionRadius;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player target = Main.player[i];
                if (!target.InOpposingTeam(Owner))
                {
                    float squaredDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
                    if (squaredDistanceToTarget < squaredMaxDetectionRadius)
                    {
                        squaredMaxDetectionRadius = squaredDistanceToTarget;
                        closestPlayer = target;
                    }
                }
            }

            return closestPlayer;
        }
        #endregion
    }
}
