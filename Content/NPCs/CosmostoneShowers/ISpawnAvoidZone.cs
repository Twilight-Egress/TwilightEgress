using Microsoft.Xna.Framework;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    interface ISpawnAvoidZone
    {
        float RadiusCovered { get; }
        Vector2 Position { get; }
        bool Active { get; }
    }
}
