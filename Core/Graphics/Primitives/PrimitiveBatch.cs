using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Geometry;

namespace TwilightEgress.Core.Graphics.Primitives
{
    public class PrimitiveBatch : ILoadable
    {
        internal static PrimitiveBatch Instance { get; private set; }

        VertexBuffer vertexBuffer;

        BasicEffect basicEffect;
        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);
        double angle = 0;

        public void Load(Mod mod) => Main.QueueMainThreadAction(() =>
        {
            if (Main.dedServ)
                return;

            basicEffect = new BasicEffect(Main.graphics.GraphicsDevice);

            VertexPositionColor[] vertices = new VertexPositionColor[18];

            List<Vector2> triangles = RandomPolygon.GeneratePolygon(Main.rand.Next(), Vector2.Zero, 0.5f, 0.8f, 0.3f, 6).Triangulate();

            for (int i = 0; i < triangles.Count; i++)
            {
                Vector2 triangle = triangles[i];

                vertices[i] = new VertexPositionColor(new Vector3(triangle.X, triangle.Y, 0f), Color.White);
            }

            /*VertexPositionColor[] vertices = new VertexPositionColor[60];
            vertices[0] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, 0.42532500f), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, 0.26286500f), Color.Orange);
            vertices[2] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, 0.42532500f), Color.Green);
            vertices[3] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, 0.42532500f), Color.Yellow);
            vertices[4] = new VertexPositionColor(new Vector3(-0.42532500f, -0.26286500f, 0.0000000f), Color.Red);
            vertices[5] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, 0.26286500f), Color.Orange);
            vertices[6] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, 0.42532500f), Color.Green);
            vertices[7] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, 0.26286500f), Color.Orange);
            vertices[8] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, 0.42532500f), Color.Yellow);
            vertices[9] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, 0.42532500f), Color.Yellow);
            vertices[10] = new VertexPositionColor(new Vector3(0.42532500f, 0.26286500f, 0.0000000f), Color.Red);
            vertices[11] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, 0.26286500f), Color.Orange);
            vertices[12] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, 0.42532500f), Color.Green);
            vertices[13] = new VertexPositionColor(new Vector3(0.42532500f, -0.26286500f, 0.0000000f), Color.Yellow);
            vertices[14] = new VertexPositionColor(new Vector3(0.42532500f, 0.26286500f, 0.0000000f), Color.Blue);
            vertices[15] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, -0.42532500f), Color.Red);
            vertices[16] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, -0.26286500f), Color.Yellow);
            vertices[17] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, -0.42532500f), Color.Blue);
            vertices[18] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, -0.42532500f), Color.Red);
            vertices[19] = new VertexPositionColor(new Vector3(-0.42532500f, 0.26286500f, 0.0000000f), Color.Orange);
            vertices[20] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, -0.26286500f), Color.Green);
            vertices[21] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, -0.42532500f), Color.Red);
            vertices[22] = new VertexPositionColor(new Vector3(-0.42532500f, -0.26286500f, 0.0000000f), Color.Yellow);
            vertices[23] = new VertexPositionColor(new Vector3(-0.42532500f, 0.26286500f, 0.0000000f), Color.Blue);
            vertices[24] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, -0.42532500f), Color.Orange);
            vertices[25] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, -0.26286500f), Color.Yellow);
            vertices[26] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, -0.42532500f), Color.Red);
            vertices[27] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, -0.42532500f), Color.Green);
            vertices[28] = new VertexPositionColor(new Vector3(0.42532500f, -0.26286500f, 0.0000000f), Color.Red);
            vertices[29] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, -0.26286500f), Color.Blue);
            vertices[30] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, 0.26286500f), Color.Red);
            vertices[31] = new VertexPositionColor(new Vector3(0.42532500f, 0.26286500f, 0.0000000f), Color.Green);
            vertices[32] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, -0.26286500f), Color.Yellow);
            vertices[33] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, 0.26286500f), Color.Orange);
            vertices[34] = new VertexPositionColor(new Vector3(-0.42532500f, 0.26286500f, 0.0000000f), Color.Blue);
            vertices[35] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, 0.42532500f), Color.Yellow);
            vertices[36] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, -0.26286500f), Color.Green);
            vertices[37] = new VertexPositionColor(new Vector3(0.42532500f, 0.26286500f, 0.0000000f), Color.Blue);
            vertices[38] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, -0.42532500f), Color.Yellow);
            vertices[39] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, -0.26286500f), Color.White);
            vertices[40] = new VertexPositionColor(new Vector3(-0.42532500f, 0.26286500f, 0.0000000f), Color.Yellow);
            vertices[41] = new VertexPositionColor(new Vector3(0.0000000f, 0.42532500f, 0.26286500f), Color.Red);
            vertices[42] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, 0.26286500f), Color.Green);
            vertices[43] = new VertexPositionColor(new Vector3(0.42532500f, -0.26286500f, 0.0000000f), Color.Red);
            vertices[44] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, 0.42532500f), Color.Orange);
            vertices[45] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, 0.26286500f), Color.White);
            vertices[46] = new VertexPositionColor(new Vector3(-0.42532500f, -0.26286500f, 0.0000000f), Color.Red);
            vertices[47] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, -0.26286500f), Color.Green);
            vertices[48] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, -0.26286500f), Color.Yellow);
            vertices[49] = new VertexPositionColor(new Vector3(0.42532500f, -0.26286500f, 0.0000000f), Color.Blue);
            vertices[50] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, 0.26286500f), Color.Red);
            vertices[51] = new VertexPositionColor(new Vector3(0.0000000f, -0.42532500f, -0.26286500f), Color.White);
            vertices[52] = new VertexPositionColor(new Vector3(-0.42532500f, -0.26286500f, 0.0000000f), Color.Red);
            vertices[53] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, -0.42532500f), Color.Yellow);
            vertices[54] = new VertexPositionColor(new Vector3(0.42532500f, 0.26286500f, 0.0000000f), Color.Green);
            vertices[55] = new VertexPositionColor(new Vector3(0.42532500f, -0.26286500f, 0.0000000f), Color.White);
            vertices[56] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, -0.42532500f), Color.Red);
            vertices[57] = new VertexPositionColor(new Vector3(-0.42532500f, 0.26286500f, 0.0000000f), Color.Blue);
            vertices[58] = new VertexPositionColor(new Vector3(-0.42532500f, -0.26286500f, 0.0000000f), Color.White);
            vertices[59] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, 0.42532500f), Color.Green);*/

            vertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColor), 60, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            Instance = this;
        });

        public void Unload() => Main.QueueMainThreadAction(() =>
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            vertexBuffer?.Dispose();
            vertexBuffer = null;
            basicEffect?.Dispose();
            basicEffect = null;
            Instance = null;
        });

        public void Update()
        {
            angle += 0.01f;
            view = Matrix.CreateLookAt(
                new Vector3(5 * (float)Math.Sin(angle), -2, 5 * (float)Math.Cos(angle)),
                new Vector3(0, 0, 0),
                Vector3.UnitY);
        }

        public void DrawPrimitives()
        {
            if (basicEffect == null)
                return;

            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullClockwiseFace;
            graphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 20);
            }

            graphicsDevice.SetVertexBuffer(null);
        }
    }
}
