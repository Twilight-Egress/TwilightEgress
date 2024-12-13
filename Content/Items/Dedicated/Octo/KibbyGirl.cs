using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.Dedicated.Octo
{
    public class KibbyGirl : ModProjectile, ILocalizedModType
    {
        public enum BehaviorState
        {
            Idle,
            Sleeping,
            Attacking
        }

        public enum AnimationState
        {
            Idle,
            Sleeping,
            Moving,
            Jumping
        }

        public enum FaceAnimationState
        {
            Idle,
            Blinking,
            Sleeping,
            Attacking,
        }

        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];

        public ref float LocalAIState => ref Projectile.ai[1];

        public ref float AIState => ref Projectile.ai[2];

        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 26;
            Main.projPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
    }
}
