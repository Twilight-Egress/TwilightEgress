using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Content.EntityOverrides.Items.HoneyComb
{
    public class BeeFlightTimeBoostPlayer : ModPlayer
    {
        public int BeeFlightBoost { get; set; }

        public override void UpdateEquips()
        {
            switch (BeeFlightBoost)
            {
                case 1:
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.1);
                    break;
                case 2:
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.12);
                    break;
                case 3:
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.07);
                    break;
            }
        }

        public override void ResetEffects()
        {
            BeeFlightBoost = 0;
        }

        public override void UpdateDead()
        {
            BeeFlightBoost = 0;
        }

    }
}