using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Geometry;
using TwilightEgress.Core.Physics;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    // things to do:
    // 1. find an algo that turns triangles into more triangles
    // 2. make collision iterate through the triangle mesh instead of the shape
    // 3. make jumping work while moving
    // 4. make grappling hooks work

    public class Asteroid : ModNPC
    {
        public override string LocalizationCategory => "NPCs.CosmostoneShowers.Asteroids";

        public override string Texture => "TwilightEgress/Assets/Textures/Extra/EmptyPixel";

        public Polygon shape;
        public List<Triangle> TriangleMesh;

        public ref float Seed => ref NPC.ai[0];

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ProjectileNPC[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TeleportationImmune[Type] = true;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 128;
            NPC.height = 128;
            NPC.lifeMax = 2;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Seed = Main.rand.Next();
            shape = RandomPolygon.GeneratePolygon((int)Seed, NPC.Center, 7.5f * 16f, 0.8f, 0.2f, 8);
            TriangleMesh = shape.Triangulate();

            NPC.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => false;

        public override void OnKill()
        {
            shape = null;
            TriangleMesh.Clear();
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
            On_Collision.SlopeCollision += AsteroidSlopeCollision;
        }

        public override void Unload()
        {
            On_Main.DrawNPCs -= DrawAsteroids;
            On_Collision.SlopeCollision -= AsteroidSlopeCollision;
        }

        private Vector4 AsteroidSlopeCollision(On_Collision.orig_SlopeCollision orig, Vector2 position, Vector2 velocity, int width, int height, float gravity, bool fall)
        {
            Vector2? collision = AsteroidCollision(position, width, height);
            if (collision != null)
            {
                Vector2 normal = collision.Value;
                normal.Normalize();

                Collision.down = true;
                Collision.stair = true;
                //velocity.X *= 0.95f;
                Vector2 newVelocity = velocity;

                newVelocity.Y = Math.Abs(velocity.Y) * normal.Y;

                if (collision.Value.Y > 0)
                {
                    newVelocity.X = Math.Abs(velocity.X) * normal.X;
                    position.X += collision.Value.X;
                }
                else if (velocity.X != 0)
                {
                    newVelocity.Y = velocity.X * normal.X;
                    position.X += collision.Value.X;
                    newVelocity.X *= 0.95f;
                }
                else
                {
                    newVelocity.Y = 0;
                    newVelocity.X *= 0.95f;
                }

                if (collision.Value.Y <= 0)
                {
                    position.Y += newVelocity.Y;
                    newVelocity.Y = 0;
                }
                return new Vector4(position.X, position.Y + collision.Value.Y + newVelocity.Y, newVelocity.X, 0);
            }

            return orig(position, velocity, width, height, gravity, fall);
        }

        private Vector2? AsteroidCollision(Vector2 position, int width, int height)
        {
            // help me
            Vector2 asteroidCollision = Vector2.Zero;
            Vector2 entityPosition = position + (0.5f * new Vector2(width, height));

            Asteroid closestAsteroid = null;
            float distanceToAsteroid = float.MaxValue;

            foreach (NPC activeNPC in Main.ActiveNPCs)
            {
                if (activeNPC.ModNPC is not Asteroid)
                    continue;

                bool canHit = Collision.CanHit(entityPosition, 1, 1, activeNPC.Center, 1, 1);
                if (Vector2.DistanceSquared(entityPosition, activeNPC.Center) < distanceToAsteroid && canHit)
                {
                    distanceToAsteroid = Vector2.DistanceSquared(entityPosition, activeNPC.Center);
                    closestAsteroid = activeNPC.ModNPC as Asteroid;
                }
            }

            if (closestAsteroid is not null)
            {
                // This could maybe only run calculations for the closest asteroid to the middle of the entity
                Collider collider = new Collider();

                Polygon hitbox = new Polygon([
                    position,
                    position + new Vector2(width, 0),
                    position + new Vector2(0, height),
                    position + new Vector2(width, height),
                ]);

                /*foreach (Triangle triangle in closestAsteroid.TriangleMesh)
                {
                    Vector2? collision = collider.SeparatingAxisTheorem(triangle, closestAsteroid.NPC.Center, hitbox, entityPosition);

                    if (collision is not null && collision != Vector2.Zero)
                        return collision;
                }*/

                return collider.SeparatingAxisTheorem(closestAsteroid.shape, closestAsteroid.NPC.Center, hitbox, entityPosition);
            }

            return null;
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
                {
                    foreach (Triangle triangle in asteroid.TriangleMesh)
                        triangleVertices.AddRange(triangle.Vertices);
                }
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
