using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Projectiles.Magic
{
    public class StellascopeStar : ModProjectile
    {
        // Sound effect shtuff (kinda bad, placeholder)
        private static readonly SoundStyle StarSoundstyle1 = SoundID.Item4 with { Pitch = 0.5f, PitchVariance = 0.5f, MaxInstances = 4 };
        private static readonly SoundStyle StarSoundStyle2 = SoundID.Item9 with { Pitch = 0.5f, PitchVariance = 0.5f, MaxInstances = 4 };

        // Star drawcode shtuff
        public override string Texture => "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Large";
        private static readonly Color[] StarColorArray = [Color.RoyalBlue, Color.Cyan, Color.Blue, Color.BlueViolet, Color.Azure];
        private Color StarColor = StarColorArray[Main.rand.Next(StarColorArray.Length)];
        private static Asset<Texture2D> StarTexture;
        private int Time;

        // Star system shtuff
        private int PulseTimer = 60;
        private const float SquaredDistanceConst = 409600f;
        private bool SendingConstellation = false;
        private bool ReceivingConstellation = false;

        public override void Load() => StarTexture = ModContent.Request<Texture2D>(Texture);

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = StarTexture.Width();
            Projectile.height = StarTexture.Height();
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true; // LOCAL IFRAMES AAAAAA
            Projectile.localNPCHitCooldown = 7;
            Projectile.timeLeft = 4000;
        }

        public override void AI()
        {
            StellascopeStar targetStar = FindSyncStar();
            Time++;
            PulseTimer--;
            Projectile.position.Y -= MathF.Sin(Time * 0.06f) * 0.7f;

            bool createPulse = PulseTimer <= 0 && PulseTimer % 60 == 0 && Time * 0.01f >= 0.5f;
            if (targetStar != null && targetStar.Projectile.timeLeft > Projectile.timeLeft)
                PulseTimer = targetStar.PulseTimer;
            else if (targetStar != null && targetStar.Projectile.timeLeft < Projectile.timeLeft)
                targetStar.PulseTimer = PulseTimer;
            if (createPulse)
            {
                PulseRing starPulse = new PulseRing(Projectile.Center, Vector2.Zero, StarColor * 0.5f, 0.25f, 1.25f, 45);
                GeneralParticleHandler.SpawnParticle(starPulse);
                SoundEngine.PlaySound(StarSoundstyle1, Projectile.Center);
                SoundEngine.PlaySound(StarSoundStyle2, Projectile.Center);
                PulseTimer = 60;
            }
            // Would be cool to add a check to see if two star projectiles are colliding, and push them away from one another if they are
        }

        public StellascopeStar FindSyncStar() // maybe turn into an array
        {
            StellascopeStar sampleStar = null;
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.ModProjectile is StellascopeStar starProj && Vector2.DistanceSquared(Projectile.Center, projectile.Center) <= SquaredDistanceConst && projectile.whoAmI != Projectile.whoAmI && projectile.active)
                    sampleStar = projectile.ModProjectile as StellascopeStar;
            }
            return sampleStar;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(StarTexture.Value, Projectile.Center - Main.screenPosition, StarTexture.Frame(), StarColor * Math.Min((float)(Time * 0.01f), 1), 0, StarTexture.Size() * 0.5f, 0.5f, SpriteEffects.None, 0);
            return false;
        }
    }
}
