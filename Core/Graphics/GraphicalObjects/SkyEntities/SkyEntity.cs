using Luminance.Core.Graphics;
using System.Linq;
using TwilightEgress.Core.Configs;

namespace TwilightEgress.Core.Graphics.GraphicalObjects.SkyEntities
{
    public abstract class SkyEntity
    {
        /// <summary>
        /// A variable that constantly increments by 1 once a sky entity has spawned. 
        /// </summary>
        public int Time;

        /// <summary>
        /// The total amount of time this sky entity should remain active for.
        /// </summary>
        public int Lifetime;

        /// <summary>
        /// A local variable which can be utilized by animated sky entities. For setting which frame of animation this sky entity is 
        /// currently at.
        /// </summary>
        public int Frame;

        /// <summary>
        /// A local variable which can be utilized by animated sky entities. For counting the amount of frames between when an animation frame
        /// updates.
        /// </summary>
        public int FrameCounter;

        /// <summary>
        /// The current position of the sky entity in the world. Updates with <see cref="Velocity"/>.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The current velocity which this sky entity is moving at.
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// The opacity of the sky entity.
        /// </summary>
        public float Opacity;

        /// <summary>
        /// The scale of the sky entity.
        /// </summary>
        public Vector2 Scale;

        /// <summary>
        /// The rotation of the sky entity.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The speed at which the sky entity rotates.
        /// </summary>
        public float RotationSpeed;

        /// <summary>
        /// The direction in which the sky entity rotates.
        /// </summary>
        public float RotationDirection;

        /// <summary>
        /// The depth of the sky entity; controls how slow an object moves with the screen to make it seem as if it's in the background. 
        /// Defaults to 1.
        /// </summary>
        public float Depth;

        /// <summary>
        /// The color of the sky entity. To be used in drawing.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The texture of the sky entity. Obtains its value from <see cref="AtlasTextureName"/>
        /// </summary>
        public AtlasTexture Texture { get; private set; }

        /// <summary>
        /// The texture name of this sky entity on the sky entity atlas. Should be prefixed with "TwilightEgress."
        /// </summary>
        public abstract string AtlasTextureName { get; } 

        /// <summary>
        /// The amount of vertical frames this sky entity has in its spritesheet.
        /// </summary>
        public virtual int MaxVerticalFrames => 1;

        /// <summary>
        /// The amount of horizontal frames this sky entity has in its spritesheet.
        /// </summary>
        public virtual int MaxHorizontalFrames => 1;

        /// <summary>
        /// Whether or not this sky entity should immediately be removed upon its <see cref="Time"/> variable reaching its <see cref="Lifetime"/>.
        /// </summary>
        public virtual bool DieWithLifespan => true;

        /// <summary>
        /// The blend state the sky entity shall use when being drawn. Defaults to <see cref="BlendState.AlphaBlend"/>.
        /// </summary>
        public virtual BlendState BlendState => BlendState.AlphaBlend;

        /// <summary>
        /// Affects when the sky entity is drawn, whether before vanilla's background fog or after it.
        /// </summary>
        public virtual SkyEntityDrawContext DrawContext => SkyEntityDrawContext.BeforeCustomSkies;

        /// <summary>
        /// Any code written here will be checked and ran every frame via <see cref="ModSystem.PostUpdateEverything"/>. 
        /// Use this method to update specific parts of your sky entity.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// How the sky entity shall be drawn. This method is called automatically via a detour of 
        /// <see cref="Terraria.GameContent.Skies.AmbientSky.Draw(SpriteBatch, float, float)"/>.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// The base draw position used in <see cref="GetDrawPositionBasedOnDepth"/>. Defaults to <see cref="Position"/>.
        /// </summary>
        /// <param name="positionOverride">An overrideable Vector2 parameter which can be used to change the base position if required.</param>
        public virtual Vector2 GetBaseDrawPosition(Vector2? positionOverride = null) => positionOverride ?? Position;

        /// <summary>
        /// Spawns the specified <see cref="SkyEntity"/>.
        /// </summary>
        public SkyEntity Spawn()
        {
            // Do not spawn any sky entities serverside.
            if (Main.netMode == NetmodeID.Server)
                return null;

            Time = new();

            if (SkyEntityManager.ActiveSkyEntities.Count > GraphicalConfig.Instance.SkyEntityLimit)
                SkyEntityManager.ActiveSkyEntities.First().Kill();

            SkyEntityManager.ActiveSkyEntities.Add(this);

            SetPositionByDepth(Position);

            Texture = AtlasManager.GetTexture(AtlasTextureName);
            return this;
        }

        /// <summary>
        /// Calculates the position at which this sky entity should properly draw in the world based on its depth.
        /// </summary>
        public Vector2 GetDrawPositionBasedOnDepth()
        {
            Vector2 drawPositionByDepth = (GetBaseDrawPosition() - Main.Camera.Center) * new Vector2(1f / Depth, 0.9f / Depth) + Main.Camera.Center;
            return drawPositionByDepth - Main.Camera.UnscaledPosition;
        }

        /// <summary>
        /// Removes the sky entity from the database entirely.
        /// </summary>
        public void Kill() => Time = Lifetime;

        public void SetPositionByDepth(Vector2 worldPosition)
        {
            Vector2 vector = worldPosition - Main.Camera.Center;
            Vector2 depthPosition = Main.Camera.Center + vector * (Depth / 3f);
            Position = depthPosition;
        }
    }
}
