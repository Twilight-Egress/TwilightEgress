using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Geometry;
using TwilightEgress.Core.Physics;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    public class Asteroid : ModNPC
    {
        public override string LocalizationCategory => "NPCs.CosmostoneShowers.Asteroids";

        public override string Texture => "TwilightEgress/Assets/Textures/Extra/EmptyPixel";

        public Polygon shape;
        public List<Vector2> triangleMesh;

        public ref float Seed => ref NPC.ai[0];

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 500;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.chaseable = false;
            NPC.knockBackResist = 0.4f;
            NPC.Opacity = 0f;

            NPC.HitSound = SoundID.Tink;
            NPC.DeathSound = SoundID.Item70;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Seed = Main.rand.Next();
            shape = RandomPolygon.GeneratePolygon((int)Seed, NPC.Center, 7.5f * 16f, 0.8f, 0.2f, 8);
            triangleMesh = shape.Triangulate();

            NPC.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Microsoft.Xna.Framework.Color drawColor) => false;

        public override void OnKill()
        {
            shape = null;
            triangleMesh.Clear();
        }
    }

    public class AsteroidSystem : ModSystem
    {
        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = new Matrix(
            Main.GameViewMatrix.Zoom.X, 0, 0, 0,
            0, Main.GameViewMatrix.Zoom.X, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);

        public override void Load()
        {
            On_Main.DrawNPCs += DrawAsteroids;
            On_Collision.TileCollision += AsteroidCollision;
        }

        public override void Unload()
        {
            On_Main.DrawNPCs -= DrawAsteroids;
            On_Collision.TileCollision -= AsteroidCollision;
        }

        private Vector2 AsteroidCollision(On_Collision.orig_TileCollision orig, Vector2 position, Vector2 velocity, int width, int height, bool fallThrough, bool fall2, int gravDir)
        {
            // help me

            Vector2 asteroidCollision = Vector2.Zero;

            foreach (NPC npc in Main.npc)
            {
                if (npc.ModNPC is Asteroid asteroid && npc.active)
                {
                    Collider collider = new Collider();

                    Polygon hitbox = new Polygon([
                        position,
                        position + new Vector2(width, 0),
                        position + new Vector2(0, height),
                        position + new Vector2(width, height),
                    ]);

                    Vector2? collision = collider.SeparatingAxisTheorem(asteroid.shape, asteroid.NPC.Center, hitbox, position + (0.5f * new Vector2(width, height)));

                    if (collision != null)
                        asteroidCollision = collision.Value + velocity;
                }
            }

            return asteroidCollision + orig(position, velocity, width, height, fallThrough, fall2, gravDir);
        }

        private Vector2 WorldToWorldMatrixPos(Vector2 input)
        {
            return new Vector2(1, -1) * (input / (new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f) - Vector2.One);
        }

        private void DrawAsteroids(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);
            List<Vector2> triangleVertices = new List<Vector2>();

            foreach (NPC npc in Main.npc)
            {
                if (npc.ModNPC is Asteroid asteroid && npc.active)
                    triangleVertices.AddRange(asteroid.triangleMesh);
            }

            if (triangleVertices.Count is 0)
                return;

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            BasicEffect basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.VertexColorEnabled = true;

            VertexBuffer vertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColor), triangleVertices.Count, BufferUsage.WriteOnly);

            VertexPositionColor[] vertices = new VertexPositionColor[triangleVertices.Count];

            for (int i = 0; i < triangleVertices.Count; i++)
            {
                Vector3 vertexColor = Lighting.GetSubLight(triangleVertices[i]) * Microsoft.Xna.Framework.Color.White.ToVector3();
                Vector2 triangle = WorldToWorldMatrixPos(triangleVertices[i] - Main.screenPosition);
                vertices[i] = new VertexPositionColor(new Vector3(triangle.X, triangle.Y, 0f), new Microsoft.Xna.Framework.Color(vertexColor));
            }

            vertexBuffer?.SetData<VertexPositionColor>(vertices);

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;

            int numTriangles = triangleVertices.Count / 3;
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, numTriangles);
            }

            graphicsDevice.SetVertexBuffer(null);

            basicEffect?.Dispose();
            basicEffect = null;

            vertexBuffer?.Dispose();
            vertexBuffer = null;
        }
    }
}
