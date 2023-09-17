﻿namespace Cascade.Content.Projectiles.Summoner.GeminiGenies
{
    public class GeminiGeniePsychic : ModProjectile
    {
        private enum AIStates
        {
            Idle,
            Attacking
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AIState => ref Projectile.ai[1];

        private ref float AttackState => ref Projectile.ai[2];

        private bool HasSpawnedInWeaponsYet { get; set; }

        private static Projectile myself;

        public static Projectile Myself
        {
            get
            {
                if (myself is not null && !myself.active)
                    return null;
                return myself;
            }

            private set => myself = value;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 114;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasSpawnedInWeaponsYet);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasSpawnedInWeaponsYet = reader.ReadBoolean();
        }

        public override bool? CanDamage() => !Owner.CascadePlayer_Minions().GeminiGeniesVanity;

        public bool CheckActive()
        {
            if (Owner.CascadePlayer_Minions().GeminiGenies || Owner.CascadePlayer_Minions().GeminiGeniesVanity)
            {
                Projectile.timeLeft = 2;
                return true;
            }

            if (Owner.dead || !Owner.active)
            {
                Owner.CascadePlayer_Minions().GeminiGenies = false;
                Projectile.Kill();
                return false;
            }

            return false;
        }

        public override void AI()
        {
            if (!CheckActive())
                return;

            // Set the global Projectile instance.
            Myself = Projectile;

            // Search for nearby targets.
            Projectile.SearchForViableTargetsForMinion(Owner, 1750f, 100f, out bool foundTarget, out float _, out Vector2 targetCenter);
            // If the player is using the acceessory as vanity, make sure to always set this to false so no attacks are done.
            if (Owner.CascadePlayer_Minions().GeminiGeniesVanity)
                foundTarget = false;

            // Spawn in the weapons.
            if (!HasSpawnedInWeaponsYet)
            {
                HasSpawnedInWeaponsYet = true;
                for (int i = 0; i < 2; i++)
                {
                    int weaponType = i;
                    Projectile.SpawnProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TelekineticallyControlledWeapon>(), (int)(Projectile.damage * 0.65f), Projectile.knockBack, true, SoundID.DD2_DarkMageCastHeal, Projectile.owner, ai2: weaponType);
                }
                Projectile.netUpdate = true;
            }

            switch ((AIStates)AttackState)
            {
                case AIStates.Idle:
                    DoBehavior_Idle(foundTarget);
                    break;

                case AIStates.Attacking:
                    DoBehavior_Attacking(foundTarget, targetCenter);
                    break;
            }

            Timer++;
            Projectile.spriteDirection = -Projectile.direction;
            Projectile.rotation = Projectile.velocity.X * 0.03f;
            Projectile.AdjustProjectileHitboxByScale(54f, 114f);
        }

        public void DoBehavior_Idle(bool foundTarget)
        {
            Vector2 idlePosition = Owner.Center + Vector2.UnitX * 175f;
            idlePosition.Y += Lerp(-15f, 15f, SineInOutEasing(Timer / 240f, 0));

            float speed = 25f;
            Vector2 idealVelocity = idlePosition - Projectile.Center;
            float distance = idealVelocity.Length();

            if (distance > 2000f)
            {
                // Teleport when the player is too far away.
                Projectile.Center = Owner.Center;
                Utilities.CreateRandomizedDustExplosion(36, Projectile.Center, DustID.GoldCoin, 10f);
            }

            if (distance > 70f)
            {
                idealVelocity.Normalize();
                idealVelocity *= speed;
                Projectile.velocity = (Projectile.velocity * 40f + idealVelocity) / 41f;
            }

            if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X = -0.18f;
                Projectile.velocity.Y = -0.08f;
            }
        }

        public void DoBehavior_Attacking(bool foundTarget, Vector2 targetCenter)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D baseTexture = TextureAssets.Projectile[Type].Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "Glow").Value;

            int individualFrameHeight = glowTexture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, currentYFrame, glowTexture.Width, individualFrameHeight);

            // Draw the afterimagee trail.
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                SpriteEffects effects = Projectile.oldSpriteDirection[i] < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Lerp(Color.Magenta, Color.White, 0.7f) * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(glowTexture, drawPosition, projRec, Projectile.GetAlpha(trailColor), Projectile.oldRot[i], glowTexture.Size() / 2f, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            // Draw the main texture.
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, spriteEffects, animated: true);
            return false;
        }
    }
}
