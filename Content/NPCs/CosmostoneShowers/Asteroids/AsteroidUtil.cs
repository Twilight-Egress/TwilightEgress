using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids
{
    internal class AsteroidUtil
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
    }
}
