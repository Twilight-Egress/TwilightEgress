using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TwilightEgress.Core.Systems
{
    public class NoiseSystem : ModSystem
    {
        public static FastNoiseLite cellular;

        public override void Load()
        {
            cellular = new FastNoiseLite();
            cellular.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        }

        public override void Unload()
        {
            cellular = null;
        }
    }
}
