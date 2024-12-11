using Luminance.Common.Utilities;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;

namespace TwilightEgress.Core.Players
{
    public class ResplendentRoarPlayer : ModPlayer
    {
        public float ResplendentRazeCharge { get; set; }

        public int ResplendentRazeUpdateTimer { get; set; }

        public bool FinishedChargingResplendentRaze { get; set; }

        public override void UpdateDead()
        {
            ResplendentRazeCharge = 0f;
            ResplendentRazeUpdateTimer = 0;
            FinishedChargingResplendentRaze = false;
        }

        public override void PostUpdate()
        {
            // Decrease the stored charge after 12 seconds.
            ResplendentRazeUpdateTimer++;
            if (ResplendentRazeUpdateTimer >= 300)
            {
                ResplendentRazeCharge--;
                if (ResplendentRazeCharge <= 0f)
                {
                    ResplendentRazeCharge = 0f;
                }
                ResplendentRazeUpdateTimer = 300;
            }

            // Clamp to 100.
            if (ResplendentRazeCharge >= 100f)
            {
                ResplendentRazeCharge = 100f;
                if (!FinishedChargingResplendentRaze)
                {
                    SoundEngine.PlaySound(AssetRegistry.Sounds.YharonFireBreath);

                    Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                    Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                    Color fireColor = Color.Lerp(colorGroup, secondColorGroup, Main.rand.NextFloat(0.2f, 0.8f));

                    PulseRingParticle completionPulseRing = new(Player.Center, Vector2.Zero, fireColor, 0.01f, 5f, 60);
                    completionPulseRing.SpawnCasParticle();

                    FinishedChargingResplendentRaze = true;
                }
            }

            if (FinishedChargingResplendentRaze && ResplendentRazeCharge <= 0)
                FinishedChargingResplendentRaze = false;
        }
    }
}
