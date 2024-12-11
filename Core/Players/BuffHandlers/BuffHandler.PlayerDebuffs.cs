using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Graphics.Renderers.ScreenRenderers;

namespace TwilightEgress.Core.Players.BuffHandlers
{
    public partial class BuffHandler
    {
        #region Misc Debuffs
        public bool CerebralMindtrick { get; set; }

        public bool CurseOfNecromancy { get; set; }

        public bool BellbirdStun { get; set; }
        #endregion

        #region Other Fields and Properties
        public int CurseOfNecromancyMinionSlotStack;

        private int BellbirdStunTime;

        private const int BellbirdStunMaxTime = 240;

        private float BellbirdStunTimeRatio => BellbirdStunTime / (float)BellbirdStunMaxTime;
        #endregion

        #region Methods
        private void HandlePlayerDebuffs()
        {
            // Everytime the right click ability is used with the debuff applied, the 
            // effects of the curse stack.
            if (CurseOfNecromancy)
            {
                Player.maxMinions -= CurseOfNecromancyMinionSlotStack;
                
                // Particle visuals.
                if (Main.rand.NextBool(8))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 spawnPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                        Color color = Color.Lerp(Color.MediumPurple, Color.Magenta, 0.4f);
                        float scale = Main.rand.NextFloat(0.65f, 0.85f);
                        float opacity = Main.rand.NextFloat(0.6f, 0.8f);
                        MediumMistParticle necromaticSmoke = new(spawnPosition, Vector2.Zero, color, Color.Violet, scale, opacity, Main.rand.Next(60, 120), 0.03f);
                        necromaticSmoke.SpawnCasParticle();
                    }
                }
            }

            if (!CurseOfNecromancy)
                CurseOfNecromancyMinionSlotStack = 0;

            if (BellbirdStun)
            {
                // Messed with a few player stats to mimic a more "realstic" disorentation effect.
                float statFuckeryInterpolant = Lerp(1f, 0.08f, BellbirdStunTimeRatio);

                // Reduce the player's acceleration and run speed.
                Player.runAcceleration = 0.08f * statFuckeryInterpolant;
                Player.moveSpeed = 1f * statFuckeryInterpolant;

                // Increase their fall speed from the mass disorentation.
                float fallSpeedInterpolant = Lerp(1f, 2.5f, BellbirdStunTimeRatio);
                Player.maxFallSpeed = 10f * fallSpeedInterpolant;

                // Visual effects.
                if (BellbirdStunTime <= BellbirdStunMaxTime)
                {
                    float abberationInterpolant = Lerp(0f, 25f, BellbirdStunTimeRatio);
                    ChromaticAbberationRenderer.ApplyChromaticAbberation(Main.LocalPlayer.Center, abberationInterpolant, 240);

                    float vignettePowerInterpolant = Lerp(20f, 2f, TwilightEgressUtilities.SineEaseInOut(BellbirdStunTimeRatio));
                    float vignetteBrightnessInterpolant = Lerp(0f, 3f, TwilightEgressUtilities.SineEaseInOut(BellbirdStunTimeRatio));
                    DarkVignetteRenderer.ApplyDarkVignette(Main.LocalPlayer.Center, vignettePowerInterpolant, vignetteBrightnessInterpolant, 180);
                }

                BellbirdStunTime++;
            }

            if (!BellbirdStun)
                BellbirdStunTime = 0;
        }
        #endregion
    }
}
