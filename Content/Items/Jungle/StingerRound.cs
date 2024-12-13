using CalamityMod.Projectiles;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core;

namespace TwilightEgress.Content.Items.Jungle
{
    public class StingerRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 14;
            Item.damage = 7;
            Item.ArmorPenetration = 10;
            Item.knockBack = 1f;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(copper: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<StingerRoundProjectile>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet;
        }
    }

    public class StingerRoundProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 600;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int stingerAmount = Main.rand.Next(2, 5);
            for (int i = 0; i < stingerAmount; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 7f;
                int damage = (int)Owner.GetTotalDamage(Projectile.DamageType).ApplyTo(3f);
                Projectile.BetterNewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<StingerRoundStinger>(), damage, Projectile.knockBack * 0.45f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }

    public class StingerRoundStinger : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public override string Texture => "Terraria/Images/Projectile_55";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 1;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);

            Dust dust = Dust.NewDustPerfect(spawnPosition, 18, Vector2.Zero);
            dust.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int chance = hit.Crit ? 1 : 12;
            if (Main.rand.NextBool(chance))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustRotation = Vector2.Normalize(Vector2.UnitY).RotatedBy(i - (15 / 2 - 1) * MathHelper.TwoPi / 15) + Projectile.Center;
                    Vector2 dustVelocity = dustRotation - Projectile.Center;
                    Dust dust = Dust.NewDustPerfect(dustRotation + dustVelocity, 18, Vector2.Normalize(dustVelocity) * 6f);
                    dust.noGravity = true;
                }
                target.AddBuff(BuffID.Poisoned, 360);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }
}
