using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Core.Players;

namespace TwilightEgress.Core.Globals.GlobalNPCs
{
    public class MeteorHammerNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int PlayerHammered;
        public Vector2 OriginalPosition;
        public bool IsNabbedForHammering;
        public int Timer;

        public override bool PreAI(NPC npc)
        {
            if (IsNabbedForHammering && Main.myPlayer == PlayerHammered)
            {
                float lerpAmount = EasingFunctions.SineEaseIn(Math.Clamp(Timer, 0, 60) / 60f);

                Player player = Main.player[PlayerHammered];
                player.GetModPlayer<MeteorHammerPlayer>().MeteorToHammer = npc.whoAmI;

                Vector2 targetPosition =  Main.MouseWorld - player.Center;
                targetPosition.Normalize();
                targetPosition *= 5 * 16f;

                npc.Center = Vector2.Lerp(player.Center + targetPosition, OriginalPosition, lerpAmount);
                npc.netUpdate = true;
                Timer--;
                return false;
            }

            Timer = 60;
            OriginalPosition = npc.Center;
            return base.PreAI(npc);
        }
    }
}
