namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// Checks if it just became day time, which would be 4:30am in-game.
        /// </summary>
        public static bool JustTurnedToDay => Main.dayTime && Main.time == 0;

        /// <summary>
        /// Checks if it just became night time, which would be 7:30pm in-game.
        /// </summary>
        public static bool JustTurnedToNight => !Main.dayTime && Main.time == 0;
    }
}
