using Terraria.UI;

namespace Cascade.Content.UI.Dialogue.UIElements
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

        public List<Triangle> Triangles { get; private set; }

        public List<VertexPosition2DColorTexture> TriangleVertices { get; private set; }

        public static Vector2 InitialTriangleCenterPosition { get; private set; }


        public override void OnInitialize()
        {
            // Reinitialize the lists.
            TriangleVertices = [];
            Triangles = [];

            CalculatedStyle parentDimensions = Parent.GetInnerDimensions();

            // Get three random points around the textbox to act as the triangle points.
            Vector2 trianglePointA = parentDimensions.Position() + new Vector2(Parent.Width.Pixels, -Parent.Height.Pixels + 175f);
            Vector2 trianglePointB = parentDimensions.Position() + new Vector2(Parent.Width.Pixels / 2f, Parent.Width.Pixels - 350f);
            Vector2 trianglePointC = parentDimensions.Position() + new Vector2(-Parent.Width.Pixels + 500f, -Parent.Height.Pixels + 250f);
            Vector2 triangleCenter = (trianglePointA + trianglePointB + trianglePointC) / 3f;

            InitialTriangleCenterPosition = triangleCenter;

            // Create a new triangle and add it to the list.
            Triangle triangle = new(trianglePointA, trianglePointB, trianglePointC, triangleCenter, Color.Magenta);
            Triangles.Add(triangle);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle parentDimensions = Parent.GetInnerDimensions();

            spriteBatch.PrepareForShaders(null, true);

            foreach (Triangle triangle in Triangles)
            {
                Vector2 triangleVertexA = (triangle.PointA - parentDimensions.Center()) + triangle.DrawPosition;
                Vector2 triangleVertexB = (triangle.PointB - parentDimensions.Center()) + triangle.DrawPosition;
                Vector2 triangleVertexC = (triangle.PointC - parentDimensions.Center()) + triangle.DrawPosition;

                // Get the vertices for each triangle.
                TriangleVertices.Add(new(triangleVertexA, Color.Magenta, triangle.PointA, 1f));
                TriangleVertices.Add(new(triangleVertexB, Color.DarkViolet, triangle.PointB, 1f));
                TriangleVertices.Add(new(triangleVertexC, Color.Purple, triangle.PointC, 1f));

                if (TriangleVertices.Count > 0)
                {
                    Utilities.CalculatePrimitiveMatrices(Main.screenWidth, Main.screenHeight, out Matrix viewMatrix, out Matrix projectionMatrix, true);
                    var shader = ShaderManager.GetShader("Luminance.StandardPrimitiveShader");
                    shader.TrySetParameter("uWorldViewProjection", viewMatrix * projectionMatrix);
                    shader.Apply();

                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, TriangleVertices.ToArray(), 0, Triangles.Count);
                }
            }
           
            spriteBatch.ResetToDefaultUI();
        }
    }
}
