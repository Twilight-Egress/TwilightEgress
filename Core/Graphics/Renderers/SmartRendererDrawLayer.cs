namespace TwilightEgress.Core.Graphics.Renderers
{
    public enum SmartRendererDrawLayer 
    {
        /// <summary>
        /// Draws the Renderer after NPCs, Projectiles, Players, Tiles and many other important draw calls.
        /// </summary>
        AfterEverything,
        /// <summary>
        /// Draws the Renderer after NPCs have been drawn.
        /// </summary>
        AfterNPCs,
        /// <summary>
        /// Draws the Renderer after Projectiles have been drawn.
        /// </summary>
        AfterProjectiles,
        /// <summary>
        /// Draws the Renderer after Players have been draw after Projectiles.
        /// </summary>
        AfterPlayers,
        /// <summary>
        /// Draws the Renderer before Tiles have been drawn.
        /// </summary>
        BeforeTiles,
        /// <summary>
        /// Draws the contents of the screen onto the Renderer's <see cref="SmartRenderer.MainTarget"/> and then draws our Renderer.
        /// Should only be used for special screen-wide effects.
        /// </summary>
        BeforeFilters
    }
}
