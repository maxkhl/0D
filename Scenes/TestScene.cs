using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank.Scenes
{
    class TestScene : Scene
    {
        hTexture Block;

        public TestScene() : base()
        {
            this.SamplerState = SamplerState.PointClamp; //Makes everything pixely
        }

        public override void LoadContent()
        {
            /*var playfield = new GameObjects.Field(this, new Vector2(500, 800), new Vector2(25));


            this.Block = new hTexture(
                Game.Content.Load<Texture2D>("Images/Block"), 
                new Vector2(50,50),
                25,
                25);

            this.Block.AnimationSequences.Add("Bling", new int[] { 5, 6, 7, 8, 9, 0 });
            this.Block.Play(true, "Bling", 20);*/


            BinaryReader reader = new BinaryReader(new FileStream(@"Content\Tree1.vox", FileMode.Open));

            var allBytes = reader.ReadBytes((int)reader.BaseStream.Length);
            var voxContent = GameObjects.VoxObject.LoadVoxContent(this.Game, "Tree", allBytes);

            Random rand = new Random();
            var visibleRect = Camera.Visible;
            for(int i = 0; i < 50; i++)
            {
                float x = rand.Next(visibleRect.Left, visibleRect.Right),
                      y = rand.Next(visibleRect.Top, visibleRect.Bottom);

                var voxObject = new GameObjects.VoxObject(this, voxContent) { Perspective = 4 };
                voxObject.DrawOrder = (int)y;
                voxObject.Size = new Vector2(4, 5);
                voxObject.Position += new Vector2(x, y);
            }


            reader = new BinaryReader(new FileStream(@"Content\Cloud1.vox", FileMode.Open));

            allBytes = reader.ReadBytes((int)reader.BaseStream.Length);
            var CloudVoxContent = GameObjects.VoxObject.LoadVoxContent(this.Game, "Cloud", allBytes);

            rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                float x = rand.Next(visibleRect.Left, visibleRect.Right),
                      y = rand.Next(visibleRect.Top, visibleRect.Bottom);

                var voxObject = new GameObjects.VoxObject(this, CloudVoxContent) { Perspective = 4, Height = 150 };
                voxObject.DrawOrder = (int)y;
                voxObject.Size = new Vector2(4, 5);
                voxObject.Position += new Vector2(x, y);
            }

            this.ClearColor = Color.Lime;

            /*var newObj = new GameObjects.SpriteObject(this);
            newObj.Texture = Block;
            newObj.Color = Color.Yellow;
            newObj.ColorAnimation.Animate(Color.Blue, 5000, GameObjects.Tools.Easing.EaseFunction.Linear);*/

            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(Game.InputManager.IsActionDown(InputManager.Action.ScrollIn))
                Camera.Zoom += 0.1f;
            if (Game.InputManager.IsActionDown(InputManager.Action.ScrollOut))
                Camera.Zoom -= 0.1f;

            GameObjects.VoxObject.GlobalRotation += 0.003f;
        }
    }
}
