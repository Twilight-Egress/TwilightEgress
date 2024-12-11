namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// Creates a simple for loop that spawns dusts.
        /// </summary>
        /// <param name="maxDusts">The maximum amount of times the loop should spawn dusts.</param>
        /// <param name="dustPosition">The starting position of the dusts.</param>
        /// <param name="dustVelocity">The velocity of the dusts.</param>
        /// <param name="dustType">The ID of the type of the dusts being spawned.</param>
        /// <param name="dustAlpha">The initial alpha of the dusts.</param>
        /// <param name="dustColor">The initial color of the dusts.</param>
        /// <param name="dustScale">The initial scale of the dusts.</param>
        /// <param name="shouldDefyGravity">Controls whether or not the dusts are affected by gravity.</param>
        /// <param name="shouldntEmitLight">Controls whether or not the dusts emit light if able to.</param>
        public static void CreateDustLoop(int maxDusts, Vector2 dustPosition, Vector2 dustVelocity, int dustType, int dustAlpha = 0, Color dustColor = default, float dustScale = 1f, bool shouldDefyGravity = true, bool shouldntEmitLight = false)
        {
            for (int i = 0; i < maxDusts; i++)
            {
                Dust dust = Dust.NewDustPerfect(dustPosition, dustType, dustVelocity.SafeNormalize(Vector2.Zero), dustAlpha, dustColor, dustScale);
                dust.noGravity = shouldDefyGravity;
                dust.noLight = shouldntEmitLight;
            }
        }

        /// <summary>
        /// Creates an explosion of dusts with randomized velocities.
        /// </summary>
        /// <param name="maxDusts">The maximum amount of times the loop should spawn dusts.</param>
        /// <param name="dustPosition">The starting position of the dusts.</param>
        /// <param name="dustType">The ID of the type of the dusts being spawned.</param>
        /// <param name="dustSpeed">The initial speed of the dusts that are being spawned.</param>
        /// <param name="circleHalfWidth">The size of the width of the circle of dusts that is being created.</param>
        /// <param name="circlHalfHeight">The size of the height of the circle of dusts that is being created.</param>
        /// <param name="dustAlpha">The initial alpha of the dusts.</param>
        /// <param name="dustColor">The initial color of the dusts.</param>
        /// <param name="dustScale">The initial scale of the dusts.</param>
        /// <param name="shouldDefyGravity">Controls whether or not the dusts are affected by gravity.</param>
        /// <param name="shouldntEmitLight">Controls whether or not the dusts emit light if able to.</param>
        public static void CreateRandomizedDustExplosion(int maxDusts, Vector2 dustPosition, int dustType, float dustSpeed = 5f, float circleHalfWidth = 1f, float circlHalfHeight = 1f, int dustAlpha = 0, Color dustColor = default, float dustScale = 1f, bool shouldDefyGravity = true, bool shouldntEmitLight = false)
        {
            for (int i = 0; i < maxDusts; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(circleHalfWidth, circlHalfHeight);
                Dust dust = Dust.NewDustPerfect(dustPosition, dustType, dustVelocity * dustSpeed, dustAlpha, dustColor, dustScale);
                dust.noGravity = shouldDefyGravity;
                dust.noLight = shouldntEmitLight;
            }
        }

        /// <summary>
        /// Creates a perfect circle of dusts.
        /// </summary>
        /// <param name="maxDusts">The maximum amount of times the loop should spawn dusts.</param>
        /// <param name="dustPosition">The starting position of the dusts.</param>
        /// <param name="dustType">The ID of the type of the dusts being spawned.</param>
        /// <param name="dustSpeed">The initial speed of the dusts that are being spawned.</param>
        /// <param name="dustAlpha">The initial alpha of the dusts.</param>
        /// <param name="dustColor">The initial color of the dusts.</param>
        /// <param name="dustScale">The initial scale of the dusts.</param>
        /// <param name="shouldDefyGravity">Controls whether or not the dusts are affected by gravity.</param>
        /// <param name="shouldntEmitLight">Controls whether or not the dusts emit light if able to.</param>
        public static void CreateDustCircle(int maxDusts, Vector2 dustPosition, int dustType, float dustSpeed = 5f, int dustAlpha = 0, Color dustColor = default, float dustScale = 1f, bool shouldDefyGravity = true, bool shouldntEmitLight = false)
        {
            for (int i = 0; i < maxDusts; i++)
            {
                Vector2 dustRotation = Vector2.Normalize(Vector2.UnitY).RotatedBy((i - (maxDusts / 2 - 1) * MathHelper.TwoPi / maxDusts)) + dustPosition;
                Vector2 dustVelocity = dustRotation - dustPosition;
                Dust dust = Dust.NewDustPerfect(dustRotation + dustVelocity, dustType, Vector2.Normalize(dustVelocity) * dustSpeed, dustAlpha, dustColor, dustScale);
                dust.noGravity = shouldDefyGravity;
                dust.noLight = shouldntEmitLight;
            }
        }

        /// <summary>
        /// Creates a loop which constantly pulls dusts inwards to a specified center.
        /// </summary>
        /// <param name="maxDusts">The maximum amount of times the loop should spawn dusts.</param>
        /// <param name="startingPosition">The initial starting position of the dusts.</param>
        /// <param name="centerToPullTowards">The psoition the dusts should be pulled towards.</param>
        /// <param name="dustType">The ID of the type of the dusts being spawned.</param>
        /// <param name="dustSpeed">The initial speed of the dusts that are being spawned.</param>
        /// <param name="dustAlpha">The initial alpha of the dusts.</param>
        /// <param name="dustColor">The initial color of the dusts.</param>
        /// <param name="dustScale">The initial scale of the dusts.</param>
        /// <param name="shouldDefyGravity">Controls whether or not the dusts are affected by gravity.</param>
        /// <param name="shouldntEmitLight">Controls whether or not the dusts emit light if able to.</param>
        public static void PullDustsInTowardsCenter(int maxDusts, Vector2 startingPosition, Vector2 centerToPullTowards, int dustType, float pullSpeed = -8f, int dustAlpha = 0, Color dustColor = default, float dustScale = 1f, bool shouldDefyGravity = true, bool shouldntEmitLight = false)
        {
            for (int i = 0; i < maxDusts; i++)
            {
                Vector2 dustVelocity = (startingPosition - centerToPullTowards).SafeNormalize(Vector2.UnitY) * pullSpeed;
                Dust dust = Dust.NewDustPerfect(startingPosition, dustType, dustVelocity, dustAlpha, dustColor, dustScale);
                dust.noGravity = shouldDefyGravity;
                dust.noLight = shouldntEmitLight;
            }
        }
    }
}
