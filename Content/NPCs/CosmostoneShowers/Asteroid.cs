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
    // 1. make the mesh and hitbox rotate with the asteroid
    // 2. find an algo that turns triangles into more triangles
    // 3. draw the rest of the owl (shader that draws the asteroid)
    // 4. probably gonna have to redo how the shapes generate based on the art

    public class Asteroid : ModNPC, ISpawnAvoidZone
    {
        public override string LocalizationCategory => "NPCs.CosmostoneShowers.Asteroids";

        public override string Texture => "TwilightEgress/Assets/Textures/Extra/EmptyPixel";

        public Polygon Shape;

        public List<Triangle> TriangleMesh;

        public float RadiusCovered => 360f;

        public Vector2 Position => NPC.Center;

        public bool Active => NPC.active;

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
            Shape = RandomPolygon.GeneratePolygon((int)Seed, Vector2.Zero, 7.5f * 16f, 0.8f, 0f, 8);
            TriangleMesh = Shape.Triangulate();

            NPC.netUpdate = true;
        }

        public override void AI()
        {
            bool shouldKill = true;

            foreach (Player player in Main.player)
            {
                if (player.Center.DistanceSQ(NPC.Center) <= 2560000)
                    shouldKill = false;
            }

            if (shouldKill)
                NPC.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => false;

        public override void OnKill() => TriangleMesh.Clear();
    }

    public class AsteroidSystem : ModSystem
    {
        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = new Matrix(
            Main.GameViewMatrix.Zoom.X, 0, 0, 0,
            0, Main.GameViewMatrix.Zoom.X, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);
        BasicEffect basicEffect;
        VertexBuffer vertexBuffer;

        private const short MaxVertices = 6144;

        public override void Load()
        {
            On_Main.DrawNPCs += DrawAsteroids;
            On_Collision.SlopeCollision += AsteroidSlopeCollision;

            Main.QueueMainThreadAction(() =>
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                basicEffect = new BasicEffect(Main.graphics.GraphicsDevice);
                basicEffect.World = world;
                basicEffect.View = view;
                basicEffect.VertexColorEnabled = true;

                vertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColor), (int)Math.Floor(short.MaxValue * 0.3), BufferUsage.WriteOnly);
            });
        }

        public override void Unload()
        {
            On_Main.DrawNPCs -= DrawAsteroids;
            On_Collision.SlopeCollision -= AsteroidSlopeCollision;

            Main.QueueMainThreadAction(() =>
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                basicEffect?.Dispose();
                basicEffect = null;
                vertexBuffer?.Dispose();
                vertexBuffer = null;
            });
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

        public static Vector2? AsteroidCollision(Vector2 position, int width, int height)
        {
            // help me
            Vector2 asteroidCollision = Vector2.Zero;
            Vector2 entityPosition = position;

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
                entityPosition = position - closestAsteroid.NPC.Center;

                Polygon hitbox = new Polygon([
                    entityPosition,
                    entityPosition + new Vector2(width, 0),
                    entityPosition + new Vector2(0, height),
                    entityPosition + new Vector2(width, height),
                ]);

                List<Vector2?> collisions = new List<Vector2?>();

                foreach (Triangle triangle in closestAsteroid.TriangleMesh)
                {
                    Vector2? collision = CollisionHelper.TestCollisions(triangle, closestAsteroid.NPC.Center, hitbox, entityPosition);

                    if (collision is not null && collision != Vector2.Zero)
                        collisions.Add(collision);
                }

                if (collisions.Count == 0)
                    return null;

                Vector2 collisionVector = Vector2.Zero;
                foreach (Vector2? collision in collisions)
                {
                    collisionVector += collision.Value;
                }

                return collisionVector / collisions.Count;

                //return collider.SeparatingAxisTheorem(closestAsteroid.shape, closestAsteroid.NPC.Center, hitbox, entityPosition);
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
                    {
                        triangleVertices.Add(triangle.Vertices[0] + npc.Center);
                        triangleVertices.Add(triangle.Vertices[1] + npc.Center);
                        triangleVertices.Add(triangle.Vertices[2] + npc.Center);
                    }
                }
            }

            if (triangleVertices.Count is 0)
                return;

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            VertexPositionColor[] vertices = new VertexPositionColor[triangleVertices.Count];

            for (int i = 0; i < triangleVertices.Count; i++)
            {
                Vector3 vertexColor = Lighting.GetSubLight(triangleVertices[i]) * Color.White.ToVector3();
                Vector2 triangle = WorldToWorldMatrixPos(triangleVertices[i] - Main.screenPosition);
                vertices[i] = new VertexPositionColor(new Vector3(triangle.X, triangle.Y, 0f), new Color(vertexColor));
            }

            vertexBuffer?.SetData<VertexPositionColor>(vertices, SetDataOptions.Discard);

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
        }
    }
}
