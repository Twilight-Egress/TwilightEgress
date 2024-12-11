using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Core.Globals.GlobalNPCs
{
    public partial class TwilightEgressGlobalNPC
    {
        public delegate void EditSpawnPoolDelegate(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo);
        public static event EditSpawnPoolDelegate EditSpawnPoolEvent;

        public delegate void EditSpawnRateDelegate(Player player, ref int spawnRate, ref int maxSpawns);
        public static event EditSpawnRateDelegate EditSpawnRateEvent;

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) => EditSpawnPoolEvent?.Invoke(pool, spawnInfo);

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) => EditSpawnRateEvent?.Invoke(player, ref spawnRate, ref maxSpawns);
    }
}
