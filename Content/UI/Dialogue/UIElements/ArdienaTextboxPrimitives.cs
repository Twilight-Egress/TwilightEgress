using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.UI;

namespace TwilightEgress.Content.UI.Dialogue.UIElements
{
    public class ArdienaTextboxPrimitives : UIElement
    {
        public class Triangle(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 drawCenter, Color drawColor)
        {
            public Vector2 PointA = pointA;

            public Vector2 PointB = pointB;

            public Vector2 PointC = pointC;

            public Vector2 DrawPosition = drawCenter;

            public Color DrawColor = drawColor;
        }

        public class Rectangle(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD, Vector2 drawPosition, Color drawColor)
        {
            public Vector2 PointA = pointA;

            public Vector2 PointB = pointB;

            public Vector2 PointC = pointC;

            public Vector2 PointD = pointD;

            public Vector2 DrawPosition = drawPosition;

            public Color DrawColor = drawColor;
        }

        internal readonly short[] RectangleIndices = [0, 1, 2, 2, 3, 1];

        internal static DynamicVertexBuffer RectangleVertexBuffer;

        internal static DynamicIndexBuffer RectangleIndexBuffer;

        public List<Triangle> Triangles { get; private set; }

        public List<Rectangle> Rectangles { get; private set; }

        public VertexPosition2DColorTexture[] TriangleVertices { get; private set; }

        public VertexPosition2DColorTexture[] RectangleVertices { get; private set; }

        public override void OnInitialize()
        {
            Triangles = [];
            Rectangles = [];
            RectangleVertices = new VertexPosition2DColorTexture[60];
            TriangleVertices = new VertexPosition2DColorTexture[60];

            CalculatedStyle parentDimensions = Parent.GetInnerDimensions();

            InitializeRectangles(parentDimensions);
            InitializeTriangles(parentDimensions);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) => DrawPrimitiveShapes(spriteBatch);

        private void InitializeTriangles(CalculatedStyle parentDimensions)
        {
            // Get three random points around the textbox to act as the triangle points.

            // Leftmost point.
            Vector2 trianglePointA = parentDimensions.Position() + new Vector2(Parent.Width.Pixels, -Parent.Height.Pixels / 2f + Main.rand.Next(20, 175));
            // Bottom-middle point.
            Vector2 trianglePointB = parentDimensions.Position() + new Vector2(Parent.Width.Pixels - Main.rand.Next(275, 350), Parent.Width.Pixels - Main.rand.Next(325, 375));
            // Rightmost point.
            Vector2 trianglePointC = parentDimensions.Position() + new Vector2(-Parent.Width.Pixels + Main.rand.Next(500, 575), -Parent.Height.Pixels / 2f + Main.rand.Next(50, 150));

            Vector2 triangleCenter = (trianglePointA + trianglePointB + trianglePointC) / 3f;

            // Create a new triangle and add it to the list.
            Triangle triangle = new(trianglePointA, trianglePointB, trianglePointC, triangleCenter, new(144, 115, 225));
            Triangles.Add(triangle);
        }

        private void InitializeRectangles(CalculatedStyle parentDimensions)
        {
            int rectangleCount = Main.rand.Next(2, 5);
            for (int i = 0; i < rectangleCount; i++)
            {
                // Leftmost point.
                Vector2 rectanglePointA = parentDimensions.Position() + new Vector2(-Parent.Width.Pixels + Main.rand.Next(275, 350), -Parent.Height.Pixels + Main.rand.Next(50, 125));
                // Bottom-left point.
                Vector2 rectanglePointB = parentDimensions.Position() + new Vector2(-Parent.Width.Pixels + Main.rand.Next(275, 350), Parent.Height.Pixels - Main.rand.Next(50, 125));
                // Rightmost point.
                Vector2 rectanglePointC = parentDimensions.Position() + new Vector2(Parent.Width.Pixels - Main.rand.Next(275, 350), -Parent.Height.Pixels + Main.rand.Next(50, 125));
                // Bottom-right point.
                Vector2 rectanglePointD = parentDimensions.Position() + new Vector2(Parent.Width.Pixels - Main.rand.Next(275, 350), Parent.Height.Pixels - Main.rand.Next(50, 125));

                Vector2 rectangleCenter = (rectanglePointA + rectanglePointB + rectanglePointC + rectanglePointD) / 4f;

                Color rectangleColor = Color.Lerp(new(247, 135, 89), Color.Transparent, i * 2f / 10f);
                Rectangle rectangle = new(rectanglePointA, rectanglePointB, rectanglePointC, rectanglePointD, rectangleCenter, rectangleColor);
                Rectangles.Add(rectangle);
            }
        }

        private void DrawPrimitiveShapes(SpriteBatch spriteBatch)
        {
            CalculatedStyle parentDimensions = Parent.GetInnerDimensions();

            spriteBatch.PrepareForShaders(null, true);

            Utilities.CalculatePrimitiveMatrices(Main.screenWidth, Main.screenHeight, out Matrix viewMatrix, out Matrix projectionMatrix, true);
            var shader = ShaderManager.GetShader("Cascade.PencilArtOverlayShader");
            shader.TrySetParameter("uWorldViewProjection", viewMatrix * projectionMatrix);
            shader.Apply();

            // Rectangles.
            foreach (Rectangle rectangle in Rectangles)
            {
                Vector2 rectangleVertexA = rectangle.PointA - rectangle.DrawPosition + parentDimensions.Center();
                Vector2 rectangleVertexB = rectangle.PointB - rectangle.DrawPosition + parentDimensions.Center();
                Vector2 rectangleVertexC = rectangle.PointC - rectangle.DrawPosition + parentDimensions.Center();
                Vector2 rectangleVertexD = rectangle.PointD - rectangle.DrawPosition + parentDimensions.Center();

                RectangleVertices[0] = new(rectangleVertexA, rectangle.DrawColor, rectangle.PointA, 1f);
                RectangleVertices[1] = new(rectangleVertexB, rectangle.DrawColor, rectangle.PointB, 1f);
                RectangleVertices[2] = new(rectangleVertexC, rectangle.DrawColor, rectangle.PointC, 1f);
                RectangleVertices[3] = new(rectangleVertexD, rectangle.DrawColor, rectangle.PointD, 1f);

                int vertexIndex = 4;
                int primitiveCount = 2;
                RectangleVertexBuffer.SetData(RectangleVertices.ToArray(), 0, vertexIndex, SetDataOptions.Discard);
                RectangleIndexBuffer.SetData(RectangleIndices, 0, RectangleIndices.Length, SetDataOptions.Discard);

                Main.graphics.GraphicsDevice.SetVertexBuffer(RectangleVertexBuffer);
                Main.graphics.GraphicsDevice.Indices = RectangleIndexBuffer;
                Main.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexIndex, 0, primitiveCount);
            }

            // Triangles.
            foreach (Triangle triangle in Triangles)
            {
                Vector2 triangleVertexA = triangle.PointA - triangle.DrawPosition + parentDimensions.Center();
                Vector2 triangleVertexB = triangle.PointB - triangle.DrawPosition + parentDimensions.Center();
                Vector2 triangleVertexC = triangle.PointC - triangle.DrawPosition + parentDimensions.Center();

                // Get the vertices for each triangle.
                TriangleVertices[0] = new(triangleVertexA, triangle.DrawColor, triangle.PointA, 1f);
                TriangleVertices[1] = new(triangleVertexB, triangle.DrawColor, triangle.PointB, 1f);
                TriangleVertices[2] = new(triangleVertexC, triangle.DrawColor, triangle.PointC, 1f);

                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, TriangleVertices.ToArray(), 0, Triangles.Count);
            }

            spriteBatch.ResetToDefaultUI();
        }
    }
}
