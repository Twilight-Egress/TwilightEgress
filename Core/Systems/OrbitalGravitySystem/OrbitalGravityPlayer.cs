using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers.DwarfMoons;

namespace TwilightEgress.Core.Systems.OrbitalGravitySystem
{
    public class OrbitalGravityPlayer : ModPlayer
    {
        public DwarfMoon DwarfMoon = null;

        public float PlayerAngle = 0f;

        public float AngleSwitchTimer = 0f;

        public float GravitationalForce = 0f;

        public float AttractionCooldown = 0f;

        private const int MaxAttractionCooldown = 45;

        private const int MaxAngleSwitchTimer = 60;

        public override void PostUpdate()
        {
            // Properly resets rotation.
            if (AngleSwitchTimer < 1f)
                Player.fullRotation = 0f;

            if (DwarfMoon is not null && DwarfMoon.NPC.active)
            {
                // Disable mounts upon entering.
                Player.mount.Dismount(Player);

                // Calculations for the gravity of a planetoid. the GravitationalVariable variable is incremented in BasePlanetoid 
                // using BasePlanetoid.GravitationalIncrease, and clamps at BasePlanetoid.MaxGravitationalIncrease.
                // In this file, we then divide this by the player's center's distance from the Planetoid's center, and use this
                // new value to then increment the GravitationalForce variable, which is clamped to 1f. This ensures a smooth
                // gradual pull-in effect when a player is sucked into a Planetoid's atmosphere.
                float distanceBetweenBodies = Vector2.Distance(Player.Center, DwarfMoon.NPC.Center);
                GravitationalForce += DwarfMoon.GravitationalVariable * (1f / distanceBetweenBodies);
                GravitationalForce = MathHelper.Clamp(GravitationalForce, 0f, 1f);

                // Get the walkable position around the Planetoid and lerp the player's center to it using GravitationalForce.
                Vector2 walkablePlanetoidArea = DwarfMoon.GetWalkablePlanetoidPosition(Player) ?? DwarfMoon.NPC.Center + Vector2.UnitX.RotatedBy(PlayerAngle) * (DwarfMoon.WalkableRadius - (Player.height - Player.height / 4f));
                Player.MountedCenter = Vector2.Lerp(Player.MountedCenter, walkablePlanetoidArea, GravitationalForce / 1f);

                // Adjust the player's angle by the player's velocity, ensuring the player is constantly moving around the
                // Planetoid at the correct speed.
                PlayerAngle += Player.velocity.X / DwarfMoon.WalkableRadius;
                PlayerAngle %= MathF.Tau;

                // Eject the player from the Planetoid either once they jump or manage to leave a planetoid's attraction radius.
                float totalAttractionRadius = DwarfMoon.MaximumAttractionRadius + DwarfMoon.WalkableRadius;
                bool canEjectPlayer = Player.justJumped || Player.pulley || Vector2.Distance(DwarfMoon.NPC.Center, Player.Center) > totalAttractionRadius || Player.grapCount > 0;
                if (canEjectPlayer)
                {
                    Player.jump = 0;
                    Player.velocity = Vector2.UnitX.RotatedBy(PlayerAngle) * DwarfMoon.PlanetoidEjectionSpeed;
                    AttractionCooldown = MaxAttractionCooldown;
                    DwarfMoon = null;
                }

                AngleSwitchTimer = MathHelper.Clamp(AngleSwitchTimer + 4f, 0f, MaxAngleSwitchTimer);
            }
            else
            {
                AngleSwitchTimer = MathHelper.Clamp(AngleSwitchTimer - 4f, 0f, MaxAngleSwitchTimer);
                AttractionCooldown = MathHelper.Clamp(AttractionCooldown - 1f, 0f, MaxAttractionCooldown);
                GravitationalForce = 0f;
                DwarfMoon = null;
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (DwarfMoon is not null && DwarfMoon.NPC.active || AngleSwitchTimer > 1f)
            {
                // Ensure the player rotates towards the Planetoid properly.
                drawInfo.drawPlayer.fullRotationOrigin = drawInfo.drawPlayer.Size / 2f;
                drawInfo.drawPlayer.fullRotation = (PlayerAngle + MathHelper.PiOver2) * (AngleSwitchTimer / 59f);
            }

        }
    }
}
