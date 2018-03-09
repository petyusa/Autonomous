using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autonomous.Public;
using Autonomous.Impl.Viewports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Autonomous.Impl.GameObjects
{
    class FinishLine : GameObject
    {
        private static Texture2D _texture;
        private static BasicEffect _quadEffect;
        private static Quad _quad;

        private const float RoadWidth = GameConstants.RoadWidth;
        private const float QoadHeight = RoadWidth / 5f;


        public FinishLine(): base(false) { }

        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            _texture = content.Load<Texture2D>("finish_line2");

            _quadEffect = new BasicEffect(graphics.GraphicsDevice);
            _quadEffect.TextureEnabled = true;
            _quadEffect.Texture = _texture;

            _quad = new Quad(Vector3.Zero, Vector3.Up, Vector3.Backward, RoadWidth, QoadHeight);

        }

        public override void Initialize()
        {
            this.Type = GameObjectType.FinishLine;
        }

        public override void Draw(TimeSpan elapsed, ViewportWrapper viewport, GraphicsDevice device)
        {
            var world = Matrix.CreateTranslation(new Vector3(0, 0.01f, -Y));
            DrawQuad(world, viewport.View, viewport.Projection, device);
        }

        private void DrawQuad(Matrix cworld, Matrix view, Matrix projection, GraphicsDevice device)
        {
            _defaultLigthing.Apply(_quadEffect);
            _quadEffect.World = cworld;
            _quadEffect.View = view;
            _quadEffect.Projection = projection;
            _quadEffect.FogEnabled = true;
            _quadEffect.FogStart = 2;
            _quadEffect.FogEnd = 500;
            foreach (EffectPass pass in _quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives
                    <VertexPositionNormalTexture>(
                        PrimitiveType.TriangleList,
                        _quad.Vertices, 0, 4,
                        _quad.Indices, 0, 2);
            }
        }


    }
}
