using Terraria.ModLoader;

namespace TwilightEgress.Core.Players
{
    public class MeteorHammerPlayer : ModPlayer
    {
        public int MeteorToHammer;

        public override void ResetEffects() => MeteorToHammer = -1;
    }
}
