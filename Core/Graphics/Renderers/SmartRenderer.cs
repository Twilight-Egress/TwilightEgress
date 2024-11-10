namespace TwilightEgress.Core.Graphics.Renderers
{
    public abstract class SmartRenderer : ModType
    {
        /// <summary>
        /// The layer/order this Renderer's target should be drawn in.
        /// </summary>
        public abstract SmartRendererDrawLayer DrawLayer { get; }

        /// <summary>
        /// Whether or not this Renderer's target should be drawn.
        /// </summary>
        public abstract bool ShouldDrawRenderer { get; }

        /// <summary>
        /// The main target which you will be drawn.
        /// </summary>
        public ManagedRenderTarget MainTarget { get; private set; }

        protected sealed override void Register()
        {
            ModTypeLookup<SmartRenderer>.Register(this);

            // Avoids duplicates of the same renderer.
            if (SmartRendererManager.SmartRenderers.Contains(this))
                throw new Exception($"Snart Renderer '{Name}' has already been registered!");

            SmartRendererManager.SmartRenderers.Add(this);
        }

        public sealed override void SetupContent() => SetStaticDefaults();

        public sealed override void SetStaticDefaults() => MainTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);

        /// <summary>
        /// Override to draw contents onto the <see cref="MainTarget"/>.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void DrawToTarget(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Override to control how the target is drawn. By default, just draws the target.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void DrawTarget(SpriteBatch spriteBatch) => spriteBatch.Draw(MainTarget.Target, Vector2.Zero, Color.White);

        /// <summary>
        /// Override to run any extra code associated with your Renderer. Internally, this runs during <see cref="SmartRendererManager.PreUpdateEntities"/>
        /// </summary>
        public virtual void PreUpdate() { }

        /// <summary>
        /// Override to run any extra code associated with your Renderer. Internally, this runs during <see cref="SmartRendererManager.PostUpdateEverything"/>
        /// </summary>
        public virtual void PostUpdate() { }
    }
}
