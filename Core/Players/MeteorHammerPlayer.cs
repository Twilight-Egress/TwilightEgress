using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TwilightEgress.Core.Players
{
    public class MeteorHammerPlayer : ModPlayer
    {
        public int MeteorToHammer;

        public override void ResetEffects() => MeteorToHammer = -1;
    }
}
