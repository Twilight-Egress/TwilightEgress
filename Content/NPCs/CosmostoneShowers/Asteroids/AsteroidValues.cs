using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids
{
    internal class AsteroidValues
    {
        internal static List<int> ViableCollisionTypes = new List<int>()
        {
            ModContent.NPCType<CosmostoneAsteroidSmall>(),
            ModContent.NPCType<CosmostoneAsteroidMedium>(),
            ModContent.NPCType<CosmostoneAsteroidLarge>(),
            ModContent.NPCType<CosmostoneGeode>(),
            ModContent.NPCType<SilicateAsteroidSmall>(),
            ModContent.NPCType<SilicateAsteroidMedium>(),
            ModContent.NPCType<SilicateAsteroidLarge>(),
            ModContent.NPCType<MeteoriteAsteroid>()
        };

        /// <summary>
        /// The palette used for mana flowing through Cosmostone Asteroids.
        /// </summary>
        public static readonly Vector4[] CosmostonePalette =
        {
            new Color(96, 188, 246).ToVector4(),
            new Color(81, 158, 245).ToVector4(),
            new Color(76, 131, 242).ToVector4(),
            new Color(3, 96, 243).ToVector4(),
            new Color(48, 65, 197).ToVector4(),
            new Color(104, 94, 228).ToVector4(),
            new Color(157, 113, 239).ToVector4(),
        };
    }
}
