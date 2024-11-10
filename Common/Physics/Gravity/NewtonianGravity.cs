namespace TwilightEgress.Common.Physics.Gravity
{
    public static class NewtonianGravity
    {
        public const float G = 0.002f;

        public static Vector2 GravityAccelerationVector(Vector2 position) => position * (-G * G / position.Length());

        public static void ApplyGravity(this MassiveObject[] objects, float deltaTime)
        {
            foreach (MassiveObject? obj in objects)
            {
                if (obj is null || !obj.Active) continue;

                Vector2 TotalGravity = Vector2.Zero;
                foreach (MassiveObject? objN in objects)
                {
                    if (objN is null || objN == obj) continue;
                    TotalGravity += objN.Mass * obj.Mass * GravityAccelerationVector(obj.Position - objN.Position);
                }
                obj.Accelerate(deltaTime * TotalGravity);
            }
        }

        public static Vector2 GetGravityAtPosition(this MassiveObject[] objects, Vector2 position, float deltaTime)
        {
            Vector2 TotalGravity = Vector2.Zero;
            foreach (MassiveObject? obj in objects)
            {
                if (obj is null || !obj.Active) continue;
                TotalGravity += obj.Mass * GravityAccelerationVector(position - obj.Position);
            }
            return TotalGravity * deltaTime;
        }
    }
}
