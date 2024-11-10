namespace TwilightEgress.Core.Graphics.GraphicalObjects.SkyEntities
{
    public enum SkyEntityDrawContext
    {
        /// <summary>
        /// Draws the sky entity before any other custom skies are drawn. Note that all sky entities using this can and will be
        /// affected by factors such as what's drawn in other <see cref="CustomSky"/> instances.
        /// </summary>
        BeforeCustomSkies,
        /// <summary>
        /// Draws the sky entity after all custom skies have been drawn. Sky entities that use this setting will be unaffected
        /// by anything drawn within a CustomSky's Draw method. 
        /// </summary>
        AfterCustomSkies,
    }
}
