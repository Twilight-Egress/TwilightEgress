using TwilightEgress.Core.Globals;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Players;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress
{
    public static class GlobalExtensionMethods
    {
        #region GlobalNPCs
        public static TwilightEgressGlobalNPC TwilightEgress(this NPC npc) => npc.GetGlobalNPC<TwilightEgressGlobalNPC>();

        public static DebuffHandlerGlobalNPC TwilightEgress_Buffs(this NPC npc) => npc.GetGlobalNPC<DebuffHandlerGlobalNPC>();
        #endregion

        #region GlobalProjectiles
        public static TwilightEgressGlobalProjectile TwilightEgress(this Projectile projectile) => projectile.GetGlobalProjectile<TwilightEgressGlobalProjectile>();
        #endregion

        #region ModPlayers
        public static BuffHandler TwilightEgress_Buffs(this Player player) => player.GetModPlayer<BuffHandler>();

        public static ResplendentRoarPlayer TwilightEgress_ResplendentRoar(this Player player) => player.GetModPlayer<ResplendentRoarPlayer>();

        public static BeeFlightTimeBoostPlayer TwilightEgress_BeeFlightTimeBoost(this Player player) => player.GetModPlayer<BeeFlightTimeBoostPlayer>();

        public static OrbitalGravityPlayer TwilightEgress_OrbitalGravity(this Player player) => player.GetModPlayer<OrbitalGravityPlayer>();
        #endregion
    }
}
