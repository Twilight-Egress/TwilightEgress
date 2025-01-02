using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers;
using TwilightEgress.Content.NPCs.CosmostoneShowers.DwarfMoons;

namespace TwilightEgress.Core.Systems
{
    public class GrapplingHookHookingSystem : ModSystem
    {
        public override void Load()
        {
            On_Projectile.AI_007_GrapplingHooks += GrappleMiscObjects;
        }

        public override void Unload()
        {
            On_Projectile.AI_007_GrapplingHooks -= GrappleMiscObjects;
        }

        private void GrappleMiscObjects(On_Projectile.orig_AI_007_GrapplingHooks orig, Projectile self)
        {
            orig(self);

            if (self.ai[0] == 2)
                return;

            foreach (NPC activeNPC in Main.ActiveNPCs)
            {
                if (activeNPC.ModNPC is not DwarfMoon dwarfMoon)
                    continue;

                if ((self.Center - activeNPC.Center).LengthSquared() <= Math.Pow(dwarfMoon.WalkableRadius - (Main.player[self.owner].height * 0.75f), 2))
                {
                    SetGrapple(self.position, self);
                    return;
                }
            }

            Vector2? collision = AsteroidSystem.AsteroidCollision(self.position, self.width, self.height);
            if (collision != null)
            {
                SetGrapple(self.position, self);
                return;
            }
        }

        /// <summary>
        /// Makes a grappling hook think it's grappled onto an object.
        /// This function was written by @Impaxim on discord. Thank you Impaxim!
        /// </summary>
        /// <param name="position">The position you want the grappling hook to grapple to.</param>
        /// <param name="grapple">The grappling hook projectile.</param>
        private void SetGrapple(Vector2 position, Projectile grapple)
        {
            //grapple.tileCollide = true;
            grapple.ai[0] = 2;
            Main.player[grapple.owner].grappling[Main.player[grapple.owner].grapCount] = grapple.whoAmI;
            Main.player[grapple.owner].grapCount++;
            grapple.velocity = Vector2.Zero;
            grapple.netUpdate = true;
            //Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, grapple.Center);
        }
    }
}
