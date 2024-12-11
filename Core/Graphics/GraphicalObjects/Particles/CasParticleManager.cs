using ReLogic.Threading;
using System.Collections.Generic;

namespace TwilightEgress.Core.Graphics.GraphicalObjects.Particles
{
    public class CasParticleManager : ModSystem
    {
        internal static List<CasParticle> ActiveCasParticles = [];

        public override void OnWorldUnload() => ActiveCasParticles.Clear();

        public override void PostUpdateDusts()
        {
            FastParallel.For(0, ActiveCasParticles.Count, (int x, int y, object context) =>
            {
                for (int i = x; i < y; i++)
                {
                    CasParticle casParticle = ActiveCasParticles[i];
                    casParticle.ParallaxStrength = Clamp(casParticle.ParallaxStrength, 1f, 100f);

                    // Old data value updating.
                    switch (casParticle.TrailingMode)
                    {
                        case 0:
                            for (int z = casParticle.TrailingLength - 1; z > 0 - 1; z--)
                            {
                                casParticle.OldPositions[z] = casParticle.OldPositions[z - 1];
                                casParticle.OldRotations[z] = casParticle.OldRotations[z - 1];
                                casParticle.OldDirections[z] = casParticle.OldDirections[z - 1];
                            }
                            casParticle.OldPositions[0] = casParticle.Position;
                            casParticle.OldRotations[0] = casParticle.Rotation;
                            casParticle.OldDirections[0] = casParticle.Direction;
                            break;

                        case 1:
                            for (int z = casParticle.TrailingLength - 1; z > 0; z--)
                            {
                                casParticle.OldPositions[z] = casParticle.OldPositions[z - 1];
                                casParticle.OldRotations[z] = casParticle.OldRotations[z - 1];
                                casParticle.OldDirections[z] = casParticle.OldDirections[z - 1];
                            }
                            casParticle.OldPositions[0] = casParticle.Position;
                            casParticle.OldRotations[0] = casParticle.Rotation;
                            casParticle.OldDirections[0] = casParticle.Direction;

                            float lerpAmount = 0.65f;
                            for (int a = casParticle.TrailingLength - 1; a > 0; a--)
                            {
                                if (casParticle.OldPositions[a] == Vector2.Zero)
                                    continue;

                                casParticle.OldPositions[a] = Vector2.Lerp(casParticle.OldPositions[a], casParticle.OldPositions[a - 1], lerpAmount);
                                casParticle.OldRotations[a] = (casParticle.OldPositions[a - 1] - casParticle.OldPositions[a]).SafeNormalize(Vector2.Zero).ToRotation();
                            }
                            break;
                    }
                }
            });

            ActiveCasParticles.RemoveAll(p => p.Time >= p.Lifetime);
        }

        public static int CountParticles<T>() where T : CasParticle
        {
            int count = 0;
            foreach (CasParticle casParticle in ActiveCasParticles)
            {
                if (casParticle.GetType() == typeof(T))
                    count++;
            }

            return count;
        }
    }
}
