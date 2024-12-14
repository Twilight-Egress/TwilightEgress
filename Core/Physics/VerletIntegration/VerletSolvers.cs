using Microsoft.Xna.Framework;
using Terraria;

namespace TwilightEgress.Core.Physics.VerletIntegration
{
    public static class VerletSolvers
    {
        public static void UpdateVerlets(this VerletObject[] verletCollection, float deltaTime)
        {
            int substeps = 8;
            float substepDeltaTime = deltaTime / substeps;

            for (int i = 0; i < substeps; i++)
            {
                //verletCollection.ApplyConstraints();
                verletCollection.SolveCollisions();
                verletCollection.UpdatePositions(substepDeltaTime);
            }
        }

        public static void UpdatePositions(this VerletObject[] verletCollection, float deltaTime)
        {
            foreach (VerletObject verlet in verletCollection)
                verlet?.UpdatePosition(deltaTime);
        }

        public static void ApplyConstraints(this VerletObject[] verletCollection)
        {
            Vector2 position = Main.player[Main.myPlayer].Center;
            float radius = 400f;

            foreach (VerletObject verlet in verletCollection)
            {
                if (verlet is not null)
                {
                    Vector2 toObject = verlet.Position - position;
                    float distance = toObject.Length();

                    if (distance > radius - verlet.Radius)
                    {
                        Vector2 normal = toObject / distance;
                        verlet.Position = position + normal * (radius - verlet.Radius);
                    }
                }
            }
        }

        public static void SolveCollisions(this VerletObject[] verletCollection)
        {
            float gravity = Main.player[Main.myPlayer].gravity;

            for (int i = 0; i < verletCollection.Length; i++)
            {
                VerletObject verlet1 = verletCollection[i];

                if (verlet1 is not null)
                {
                    if (!verlet1.Active) continue;

                    for (int j = i + 1; j < verletCollection.Length; j++)
                    {
                        VerletObject verlet2 = verletCollection[j];

                        if (verlet2 is not null)
                        {
                            if (!verlet2.Active) continue;

                            Vector2 collisionAxis = verlet1.Position - verlet2.Position;
                            float distance = collisionAxis.Length();
                            float minDistance = verlet1.Radius + verlet2.Radius;

                            if (distance < minDistance)
                            {
                                Vector2 normal = collisionAxis / distance;
                                float delta = minDistance - distance;

                                verlet1.Position += delta * normal / 2f;
                                verlet2.Position -= delta * normal / 2f;

                                verlet1.OnCollide(verlet2);
                                verlet2.OnCollide(verlet1);
                            }
                        }
                    }
                }
            }
        }
    }
}
