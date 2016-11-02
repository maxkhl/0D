using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ODBank.GameObjects
{
    class VoxObject : DrawableObject
    {
        public class VoxContent
        {
            public string Name;
            public Texture2D[] Textures;
        }

        private static List<VoxContent> VoxContentList = new List<VoxContent>();

        public static VoxContent LoadVoxContent(MainGame Game, string Name, byte[] FileData)
        {
            var defaultPalette = Game.Content.Load<Texture2D>("Images/DefaultVoxPalettte");
            var ColorData = new Color[defaultPalette.Width * defaultPalette.Height];
            defaultPalette.GetData<Color>(ColorData);

            var contentExists = (from cont in VoxContentList
                where cont.Name == Name
                select cont).SingleOrDefault();
            if (contentExists != null) return contentExists;


            var voxContent = new VoxContent();
            voxContent.Name = Name;

            var voxData = MVImporter.LoadVOXFromData(FileData);

            Texture2D[] Textures = new Texture2D[voxData.voxelChunk.sizeY];
            for (int y = 0; y < voxData.voxelChunk.sizeY; y++)
            {
                Texture2D texture = new Texture2D(Game.GraphicsDevice, voxData.voxelChunk.sizeX, voxData.voxelChunk.sizeZ);

                Color[] texData = new Color[voxData.voxelChunk.sizeX * voxData.voxelChunk.sizeZ];

                for (int x = 0; x < voxData.voxelChunk.sizeX; x++)
                    for (int z = 0; z < voxData.voxelChunk.sizeZ; z++)
                    {
                        float alpha = 1;
                        int ColorIndex = voxData.voxelChunk.voxels[x, y, z];

                        Color pixelColor;
                        if (ColorIndex > 0)
                           pixelColor = ColorData[voxData.voxelChunk.voxels[x, y, z]-1] * alpha;
                        else
                           pixelColor = Color.Transparent;

                        texData[x * voxData.voxelChunk.sizeZ + z] = pixelColor;
                    }

                texture.SetData<Color>(texData);

                Textures[y] = texture;

                //AssetDatabase.CreateAsset(texture, texFileName);
                //Debug.Log(string.Format("Creating file at {0}", texFileName));
            }

            voxContent.Textures = Textures;

            return voxContent;
        }

        public Vector2 Position { get; set; }

        public float Height { get; set; }

        public Texture2D Shadow { get; set; }

        public VoxContent voxContent;
        public VoxObject(Scene Scene, VoxContent voxContent)
            : base(Scene)
        {
            Position = new Vector2(0, 0);
            Size = new Vector2(1f, 1f);
            this.voxContent = voxContent;

            CalculateShadow();
        }

        public void CalculateShadow()
        {
            Shadow = new Texture2D(Game.GraphicsDevice, voxContent.Textures[0].Width, voxContent.Textures[0].Height);
            var ShadowData = new Color[Shadow.Width * Shadow.Height];
            foreach (var texture in voxContent.Textures)
            {
                var textureData = new Color[texture.Width * texture.Height];
                texture.GetData<Color>(textureData);
                for (int i = 0; i < textureData.Length; i++)
                {
                    if (textureData[i] != Color.Transparent)
                        ShadowData[i] = Color.Black;
                }
            }
        }

        public Vector2 Size = Vector2.One;
        public float Perspective = 1.4f;
        public static float GlobalRotation = 1;
        public float Rotation = 1;
        public override void Draw(GameTime gameTime)
        {
            var localPosition = Vector2.Transform(Position, Matrix.CreateRotationZ(GlobalRotation));
            if (this.DrawOrder != (int)localPosition.Y)
                this.DrawOrder = (int)localPosition.Y;
            //Perspective += 0.01f;

            int sY = (int)(localPosition.Y + (voxContent.Textures.Length / 2));
            GameScene.Game.SpriteBatch.Draw(
                Shadow,
                new Rectangle(
                    (int)localPosition.X,
                    sY,
                    (int)(Shadow.Width * Size.X),
                    (int)(Shadow.Height * Size.Y)),
                new Rectangle(
                    0,
                    0,
                    Shadow.Width,
                    Shadow.Height),
                Color.White,
                GlobalRotation + Rotation,
                //this.Position
                new Vector2(
                    Shadow.Width / 2,
                    Shadow.Height / 2),
                SpriteEffects.None,
                sY);

            for(int i = 0; i < voxContent.Textures.Length; i++)
            {
                int x = (int)localPosition.X;
                int y = (int)(localPosition.Y + (voxContent.Textures.Length / 2) - i * Perspective - Height);

                var texture = voxContent.Textures[i];
                if (texture == null) continue;
                GameScene.Game.SpriteBatch.Draw(
                    texture,
                    new Rectangle(
                        x,
                        y,
                        (int)(texture.Width * Size.X),
                        (int)(texture.Height * Size.Y)),
                    new Rectangle(
                        0,
                        0,
                        texture.Width,
                        texture.Height),
                    Color.White,
                    GlobalRotation + Rotation,
                    //this.Position
                    new Vector2(
                        texture.Width / 2,
                        texture.Height / 2),
                    SpriteEffects.None,
                    y);
            }


            base.Draw(gameTime);
        }
    }




    [System.Serializable]
    public struct MVFaceCollection
    {
        public byte[, ,] colorIndices;
    }

    [System.Serializable]
    public struct MVVoxel
    {
        public byte x, y, z, colorIndex;
    }

    [System.Serializable]
    public class MVVoxelChunk
    {
        // all voxels
        public byte[, ,] voxels;

        // 6 dir, x+. x-, y+, y-, z+, z-
        public MVFaceCollection[] faces;

        public int x = 0, y = 0, z = 0;

        public int sizeX { get { return voxels.GetLength(0); } }
        public int sizeY { get { return voxels.GetLength(1); } }
        public int sizeZ { get { return voxels.GetLength(2); } }
    }

    public enum MVFaceDir
    {
        XPos = 0,
        XNeg = 1,
        YPos = 2,
        YNeg = 3,
        ZPos = 4,
        ZNeg = 5
    }

    [System.Serializable]
    public class MVMainChunk
    {
        public MVVoxelChunk voxelChunk;
        public MVVoxelChunk alphaMaskChunk;

        public Color[] palatte;

        public int sizeX, sizeY, sizeZ;

        public byte[] version;

        #region default_palatte
        public static Color[] defaultPalatte = new Color[] {
		new Color(1.000000f, 1.000000f, 1.000000f),
		new Color(1.000000f, 1.000000f, 0.800000f),
		new Color(1.000000f, 1.000000f, 0.600000f),
		new Color(1.000000f, 1.000000f, 0.400000f),
		new Color(1.000000f, 1.000000f, 0.200000f),
		new Color(1.000000f, 1.000000f, 0.000000f),
		new Color(1.000000f, 0.800000f, 1.000000f),
		new Color(1.000000f, 0.800000f, 0.800000f),
		new Color(1.000000f, 0.800000f, 0.600000f),
		new Color(1.000000f, 0.800000f, 0.400000f),
		new Color(1.000000f, 0.800000f, 0.200000f),
		new Color(1.000000f, 0.800000f, 0.000000f),
		new Color(1.000000f, 0.600000f, 1.000000f),
		new Color(1.000000f, 0.600000f, 0.800000f),
		new Color(1.000000f, 0.600000f, 0.600000f),
		new Color(1.000000f, 0.600000f, 0.400000f),
		new Color(1.000000f, 0.600000f, 0.200000f),
		new Color(1.000000f, 0.600000f, 0.000000f),
		new Color(1.000000f, 0.400000f, 1.000000f),
		new Color(1.000000f, 0.400000f, 0.800000f),
		new Color(1.000000f, 0.400000f, 0.600000f),
		new Color(1.000000f, 0.400000f, 0.400000f),
		new Color(1.000000f, 0.400000f, 0.200000f),
		new Color(1.000000f, 0.400000f, 0.000000f),
		new Color(1.000000f, 0.200000f, 1.000000f),
		new Color(1.000000f, 0.200000f, 0.800000f),
		new Color(1.000000f, 0.200000f, 0.600000f),
		new Color(1.000000f, 0.200000f, 0.400000f),
		new Color(1.000000f, 0.200000f, 0.200000f),
		new Color(1.000000f, 0.200000f, 0.000000f),
		new Color(1.000000f, 0.000000f, 1.000000f),
		new Color(1.000000f, 0.000000f, 0.800000f),
		new Color(1.000000f, 0.000000f, 0.600000f),
		new Color(1.000000f, 0.000000f, 0.400000f),
		new Color(1.000000f, 0.000000f, 0.200000f),
		new Color(1.000000f, 0.000000f, 0.000000f),
		new Color(0.800000f, 1.000000f, 1.000000f),
		new Color(0.800000f, 1.000000f, 0.800000f),
		new Color(0.800000f, 1.000000f, 0.600000f),
		new Color(0.800000f, 1.000000f, 0.400000f),
		new Color(0.800000f, 1.000000f, 0.200000f),
		new Color(0.800000f, 1.000000f, 0.000000f),
		new Color(0.800000f, 0.800000f, 1.000000f),
		new Color(0.800000f, 0.800000f, 0.800000f),
		new Color(0.800000f, 0.800000f, 0.600000f),
		new Color(0.800000f, 0.800000f, 0.400000f),
		new Color(0.800000f, 0.800000f, 0.200000f),
		new Color(0.800000f, 0.800000f, 0.000000f),
		new Color(0.800000f, 0.600000f, 1.000000f),
		new Color(0.800000f, 0.600000f, 0.800000f),
		new Color(0.800000f, 0.600000f, 0.600000f),
		new Color(0.800000f, 0.600000f, 0.400000f),
		new Color(0.800000f, 0.600000f, 0.200000f),
		new Color(0.800000f, 0.600000f, 0.000000f),
		new Color(0.800000f, 0.400000f, 1.000000f),
		new Color(0.800000f, 0.400000f, 0.800000f),
		new Color(0.800000f, 0.400000f, 0.600000f),
		new Color(0.800000f, 0.400000f, 0.400000f),
		new Color(0.800000f, 0.400000f, 0.200000f),
		new Color(0.800000f, 0.400000f, 0.000000f),
		new Color(0.800000f, 0.200000f, 1.000000f),
		new Color(0.800000f, 0.200000f, 0.800000f),
		new Color(0.800000f, 0.200000f, 0.600000f),
		new Color(0.800000f, 0.200000f, 0.400000f),
		new Color(0.800000f, 0.200000f, 0.200000f),
		new Color(0.800000f, 0.200000f, 0.000000f),
		new Color(0.800000f, 0.000000f, 1.000000f),
		new Color(0.800000f, 0.000000f, 0.800000f),
		new Color(0.800000f, 0.000000f, 0.600000f),
		new Color(0.800000f, 0.000000f, 0.400000f),
		new Color(0.800000f, 0.000000f, 0.200000f),
		new Color(0.800000f, 0.000000f, 0.000000f),
		new Color(0.600000f, 1.000000f, 1.000000f),
		new Color(0.600000f, 1.000000f, 0.800000f),
		new Color(0.600000f, 1.000000f, 0.600000f),
		new Color(0.600000f, 1.000000f, 0.400000f),
		new Color(0.600000f, 1.000000f, 0.200000f),
		new Color(0.600000f, 1.000000f, 0.000000f),
		new Color(0.600000f, 0.800000f, 1.000000f),
		new Color(0.600000f, 0.800000f, 0.800000f),
		new Color(0.600000f, 0.800000f, 0.600000f),
		new Color(0.600000f, 0.800000f, 0.400000f),
		new Color(0.600000f, 0.800000f, 0.200000f),
		new Color(0.600000f, 0.800000f, 0.000000f),
		new Color(0.600000f, 0.600000f, 1.000000f),
		new Color(0.600000f, 0.600000f, 0.800000f),
		new Color(0.600000f, 0.600000f, 0.600000f),
		new Color(0.600000f, 0.600000f, 0.400000f),
		new Color(0.600000f, 0.600000f, 0.200000f),
		new Color(0.600000f, 0.600000f, 0.000000f),
		new Color(0.600000f, 0.400000f, 1.000000f),
		new Color(0.600000f, 0.400000f, 0.800000f),
		new Color(0.600000f, 0.400000f, 0.600000f),
		new Color(0.600000f, 0.400000f, 0.400000f),
		new Color(0.600000f, 0.400000f, 0.200000f),
		new Color(0.600000f, 0.400000f, 0.000000f),
		new Color(0.600000f, 0.200000f, 1.000000f),
		new Color(0.600000f, 0.200000f, 0.800000f),
		new Color(0.600000f, 0.200000f, 0.600000f),
		new Color(0.600000f, 0.200000f, 0.400000f),
		new Color(0.600000f, 0.200000f, 0.200000f),
		new Color(0.600000f, 0.200000f, 0.000000f),
		new Color(0.600000f, 0.000000f, 1.000000f),
		new Color(0.600000f, 0.000000f, 0.800000f),
		new Color(0.600000f, 0.000000f, 0.600000f),
		new Color(0.600000f, 0.000000f, 0.400000f),
		new Color(0.600000f, 0.000000f, 0.200000f),
		new Color(0.600000f, 0.000000f, 0.000000f),
		new Color(0.400000f, 1.000000f, 1.000000f),
		new Color(0.400000f, 1.000000f, 0.800000f),
		new Color(0.400000f, 1.000000f, 0.600000f),
		new Color(0.400000f, 1.000000f, 0.400000f),
		new Color(0.400000f, 1.000000f, 0.200000f),
		new Color(0.400000f, 1.000000f, 0.000000f),
		new Color(0.400000f, 0.800000f, 1.000000f),
		new Color(0.400000f, 0.800000f, 0.800000f),
		new Color(0.400000f, 0.800000f, 0.600000f),
		new Color(0.400000f, 0.800000f, 0.400000f),
		new Color(0.400000f, 0.800000f, 0.200000f),
		new Color(0.400000f, 0.800000f, 0.000000f),
		new Color(0.400000f, 0.600000f, 1.000000f),
		new Color(0.400000f, 0.600000f, 0.800000f),
		new Color(0.400000f, 0.600000f, 0.600000f),
		new Color(0.400000f, 0.600000f, 0.400000f),
		new Color(0.400000f, 0.600000f, 0.200000f),
		new Color(0.400000f, 0.600000f, 0.000000f),
		new Color(0.400000f, 0.400000f, 1.000000f),
		new Color(0.400000f, 0.400000f, 0.800000f),
		new Color(0.400000f, 0.400000f, 0.600000f),
		new Color(0.400000f, 0.400000f, 0.400000f),
		new Color(0.400000f, 0.400000f, 0.200000f),
		new Color(0.400000f, 0.400000f, 0.000000f),
		new Color(0.400000f, 0.200000f, 1.000000f),
		new Color(0.400000f, 0.200000f, 0.800000f),
		new Color(0.400000f, 0.200000f, 0.600000f),
		new Color(0.400000f, 0.200000f, 0.400000f),
		new Color(0.400000f, 0.200000f, 0.200000f),
		new Color(0.400000f, 0.200000f, 0.000000f),
		new Color(0.400000f, 0.000000f, 1.000000f),
		new Color(0.400000f, 0.000000f, 0.800000f),
		new Color(0.400000f, 0.000000f, 0.600000f),
		new Color(0.400000f, 0.000000f, 0.400000f),
		new Color(0.400000f, 0.000000f, 0.200000f),
		new Color(0.400000f, 0.000000f, 0.000000f),
		new Color(0.200000f, 1.000000f, 1.000000f),
		new Color(0.200000f, 1.000000f, 0.800000f),
		new Color(0.200000f, 1.000000f, 0.600000f),
		new Color(0.200000f, 1.000000f, 0.400000f),
		new Color(0.200000f, 1.000000f, 0.200000f),
		new Color(0.200000f, 1.000000f, 0.000000f),
		new Color(0.200000f, 0.800000f, 1.000000f),
		new Color(0.200000f, 0.800000f, 0.800000f),
		new Color(0.200000f, 0.800000f, 0.600000f),
		new Color(0.200000f, 0.800000f, 0.400000f),
		new Color(0.200000f, 0.800000f, 0.200000f),
		new Color(0.200000f, 0.800000f, 0.000000f),
		new Color(0.200000f, 0.600000f, 1.000000f),
		new Color(0.200000f, 0.600000f, 0.800000f),
		new Color(0.200000f, 0.600000f, 0.600000f),
		new Color(0.200000f, 0.600000f, 0.400000f),
		new Color(0.200000f, 0.600000f, 0.200000f),
		new Color(0.200000f, 0.600000f, 0.000000f),
		new Color(0.200000f, 0.400000f, 1.000000f),
		new Color(0.200000f, 0.400000f, 0.800000f),
		new Color(0.200000f, 0.400000f, 0.600000f),
		new Color(0.200000f, 0.400000f, 0.400000f),
		new Color(0.200000f, 0.400000f, 0.200000f),
		new Color(0.200000f, 0.400000f, 0.000000f),
		new Color(0.200000f, 0.200000f, 1.000000f),
		new Color(0.200000f, 0.200000f, 0.800000f),
		new Color(0.200000f, 0.200000f, 0.600000f),
		new Color(0.200000f, 0.200000f, 0.400000f),
		new Color(0.200000f, 0.200000f, 0.200000f),
		new Color(0.200000f, 0.200000f, 0.000000f),
		new Color(0.200000f, 0.000000f, 1.000000f),
		new Color(0.200000f, 0.000000f, 0.800000f),
		new Color(0.200000f, 0.000000f, 0.600000f),
		new Color(0.200000f, 0.000000f, 0.400000f),
		new Color(0.200000f, 0.000000f, 0.200000f),
		new Color(0.200000f, 0.000000f, 0.000000f),
		new Color(0.000000f, 1.000000f, 1.000000f),
		new Color(0.000000f, 1.000000f, 0.800000f),
		new Color(0.000000f, 1.000000f, 0.600000f),
		new Color(0.000000f, 1.000000f, 0.400000f),
		new Color(0.000000f, 1.000000f, 0.200000f),
		new Color(0.000000f, 1.000000f, 0.000000f),
		new Color(0.000000f, 0.800000f, 1.000000f),
		new Color(0.000000f, 0.800000f, 0.800000f),
		new Color(0.000000f, 0.800000f, 0.600000f),
		new Color(0.000000f, 0.800000f, 0.400000f),
		new Color(0.000000f, 0.800000f, 0.200000f),
		new Color(0.000000f, 0.800000f, 0.000000f),
		new Color(0.000000f, 0.600000f, 1.000000f),
		new Color(0.000000f, 0.600000f, 0.800000f),
		new Color(0.000000f, 0.600000f, 0.600000f),
		new Color(0.000000f, 0.600000f, 0.400000f),
		new Color(0.000000f, 0.600000f, 0.200000f),
		new Color(0.000000f, 0.600000f, 0.000000f),
		new Color(0.000000f, 0.400000f, 1.000000f),
		new Color(0.000000f, 0.400000f, 0.800000f),
		new Color(0.000000f, 0.400000f, 0.600000f),
		new Color(0.000000f, 0.400000f, 0.400000f),
		new Color(0.000000f, 0.400000f, 0.200000f),
		new Color(0.000000f, 0.400000f, 0.000000f),
		new Color(0.000000f, 0.200000f, 1.000000f),
		new Color(0.000000f, 0.200000f, 0.800000f),
		new Color(0.000000f, 0.200000f, 0.600000f),
		new Color(0.000000f, 0.200000f, 0.400000f),
		new Color(0.000000f, 0.200000f, 0.200000f),
		new Color(0.000000f, 0.200000f, 0.000000f),
		new Color(0.000000f, 0.000000f, 1.000000f),
		new Color(0.000000f, 0.000000f, 0.800000f),
		new Color(0.000000f, 0.000000f, 0.600000f),
		new Color(0.000000f, 0.000000f, 0.400000f),
		new Color(0.000000f, 0.000000f, 0.200000f),
		new Color(0.933333f, 0.000000f, 0.000000f),
		new Color(0.866667f, 0.000000f, 0.000000f),
		new Color(0.733333f, 0.000000f, 0.000000f),
		new Color(0.666667f, 0.000000f, 0.000000f),
		new Color(0.533333f, 0.000000f, 0.000000f),
		new Color(0.466667f, 0.000000f, 0.000000f),
		new Color(0.333333f, 0.000000f, 0.000000f),
		new Color(0.266667f, 0.000000f, 0.000000f),
		new Color(0.133333f, 0.000000f, 0.000000f),
		new Color(0.066667f, 0.000000f, 0.000000f),
		new Color(0.000000f, 0.933333f, 0.000000f),
		new Color(0.000000f, 0.866667f, 0.000000f),
		new Color(0.000000f, 0.733333f, 0.000000f),
		new Color(0.000000f, 0.666667f, 0.000000f),
		new Color(0.000000f, 0.533333f, 0.000000f),
		new Color(0.000000f, 0.466667f, 0.000000f),
		new Color(0.000000f, 0.333333f, 0.000000f),
		new Color(0.000000f, 0.266667f, 0.000000f),
		new Color(0.000000f, 0.133333f, 0.000000f),
		new Color(0.000000f, 0.066667f, 0.000000f),
		new Color(0.000000f, 0.000000f, 0.933333f),
		new Color(0.000000f, 0.000000f, 0.866667f),
		new Color(0.000000f, 0.000000f, 0.733333f),
		new Color(0.000000f, 0.000000f, 0.666667f),
		new Color(0.000000f, 0.000000f, 0.533333f),
		new Color(0.000000f, 0.000000f, 0.466667f),
		new Color(0.000000f, 0.000000f, 0.333333f),
		new Color(0.000000f, 0.000000f, 0.266667f),
		new Color(0.000000f, 0.000000f, 0.133333f),
		new Color(0.000000f, 0.000000f, 0.066667f),
		new Color(0.933333f, 0.933333f, 0.933333f),
		new Color(0.866667f, 0.866667f, 0.866667f),
		new Color(0.733333f, 0.733333f, 0.733333f),
		new Color(0.666667f, 0.666667f, 0.666667f),
		new Color(0.533333f, 0.533333f, 0.533333f),
		new Color(0.466667f, 0.466667f, 0.466667f),
		new Color(0.333333f, 0.333333f, 0.333333f),
		new Color(0.266667f, 0.266667f, 0.266667f),
		new Color(0.133333f, 0.133333f, 0.133333f),
		new Color(0.066667f, 0.066667f, 0.066667f),
		new Color(0.000000f, 0.000000f, 0.000000f)
	};
        #endregion
    }

    public static class MVImporter
    {

        public static MVMainChunk LoadVOXFromData(byte[] data, MVVoxelChunk alphaMask = null, bool generateFaces = true)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    MVMainChunk mainChunk = new MVMainChunk();

                    // "VOX "
                    br.ReadInt32();
                    // "VERSION "
                    mainChunk.version = br.ReadBytes(4);

                    byte[] chunkId = br.ReadBytes(4);
                    if (chunkId[0] != 'M' ||
                        chunkId[1] != 'A' ||
                        chunkId[2] != 'I' ||
                        chunkId[3] != 'N')
                    {
                        return null;
                    }

                    int chunkSize = br.ReadInt32();
                    int childrenSize = br.ReadInt32();

                    // main chunk should have nothing... skip
                    br.ReadBytes(chunkSize);

                    int readSize = 0;
                    while (readSize < childrenSize)
                    {
                        chunkId = br.ReadBytes(4);
                        if (chunkId[0] == 'S' &&
                            chunkId[1] == 'I' &&
                            chunkId[2] == 'Z' &&
                            chunkId[3] == 'E')
                        {

                            readSize += ReadSizeChunk(br, mainChunk);

                        }
                        else if (chunkId[0] == 'X' &&
                          chunkId[1] == 'Y' &&
                          chunkId[2] == 'Z' &&
                          chunkId[3] == 'I')
                        {

                            readSize += ReadVoxelChunk(br, mainChunk.voxelChunk);

                        }
                        else if (chunkId[0] == 'R' &&
                          chunkId[1] == 'G' &&
                          chunkId[2] == 'B' &&
                          chunkId[3] == 'A')
                        {

                            mainChunk.palatte = new Color[256];
                            readSize += ReadPalattee(br, mainChunk.palatte);

                        }
                        else
                        {
                            return null;
                        }
                    }

                    mainChunk.alphaMaskChunk = alphaMask;

                    if (generateFaces)
                        GenerateFaces(mainChunk.voxelChunk, alphaMask);

                    if (mainChunk.palatte == null)
                        mainChunk.palatte = MVMainChunk.defaultPalatte;

                    return mainChunk;
                }
            }
        }

        public static MVMainChunk LoadVOX(string path, MVVoxelChunk alphaMask = null, bool generateFaces = true)
        {
            byte[] bytes = File.ReadAllBytes(path);
            if (bytes[0] != 'V' ||
                bytes[1] != 'O' ||
                bytes[2] != 'X' ||
                bytes[3] != ' ')
            {
                return null;
            }

            return LoadVOXFromData(bytes, alphaMask, generateFaces);
        }

        public static void GenerateFaces(MVVoxelChunk voxelChunk, MVVoxelChunk alphaMask)
        {
            voxelChunk.faces = new MVFaceCollection[6];
            for (int i = 0; i < 6; ++i)
            {
                voxelChunk.faces[i].colorIndices = new byte[voxelChunk.sizeX, voxelChunk.sizeY, voxelChunk.sizeZ];
            }

            for (int x = 0; x < voxelChunk.sizeX; ++x)
            {
                for (int y = 0; y < voxelChunk.sizeY; ++y)
                {
                    for (int z = 0; z < voxelChunk.sizeZ; ++z)
                    {

                        int alpha = alphaMask == null ? (byte)0 : alphaMask.voxels[x, y, z];

                        // left right
                        if (x == 0 || DetermineEmptyOrOtherAlphaVoxel(voxelChunk, alphaMask, alpha, x - 1, y, z))
                            voxelChunk.faces[(int)MVFaceDir.XNeg].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

                        if (x == voxelChunk.sizeX - 1 || DetermineEmptyOrOtherAlphaVoxel(voxelChunk, alphaMask, alpha, x + 1, y, z))
                            voxelChunk.faces[(int)MVFaceDir.XPos].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

                        // up down
                        if (y == 0 || DetermineEmptyOrOtherAlphaVoxel(voxelChunk, alphaMask, alpha, x, y - 1, z))
                            voxelChunk.faces[(int)MVFaceDir.YNeg].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

                        if (y == voxelChunk.sizeY - 1 || DetermineEmptyOrOtherAlphaVoxel(voxelChunk, alphaMask, alpha, x, y + 1, z))
                            voxelChunk.faces[(int)MVFaceDir.YPos].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

                        // forward backward
                        if (z == 0 || DetermineEmptyOrOtherAlphaVoxel(voxelChunk, alphaMask, alpha, x, y, z - 1))
                            voxelChunk.faces[(int)MVFaceDir.ZNeg].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

                        if (z == voxelChunk.sizeZ - 1 || DetermineEmptyOrOtherAlphaVoxel(voxelChunk, alphaMask, alpha, x, y, z + 1))
                            voxelChunk.faces[(int)MVFaceDir.ZPos].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];
                    }
                }
            }
        }

        static int ReadSizeChunk(BinaryReader br, MVMainChunk mainChunk)
        {
            int chunkSize = br.ReadInt32();
            int childrenSize = br.ReadInt32();

            mainChunk.sizeX = br.ReadInt32();
            mainChunk.sizeZ = br.ReadInt32();
            mainChunk.sizeY = br.ReadInt32();

            mainChunk.voxelChunk = new MVVoxelChunk();
            mainChunk.voxelChunk.voxels = new byte[mainChunk.sizeX, mainChunk.sizeY, mainChunk.sizeZ];


            if (childrenSize > 0)
            {
                br.ReadBytes(childrenSize);
            }

            return chunkSize + childrenSize + 4 * 3;
        }

        static int ReadVoxelChunk(BinaryReader br, MVVoxelChunk chunk)
        {
            int chunkSize = br.ReadInt32();
            int childrenSize = br.ReadInt32();
            int numVoxels = br.ReadInt32();

            for (int i = 0; i < numVoxels; ++i)
            {
                int x = (int)br.ReadByte();
                int z = (int)br.ReadByte();
                int y = (int)br.ReadByte();

                chunk.voxels[x, y, z] = br.ReadByte();
            }

            if (childrenSize > 0)
            {
                br.ReadBytes(childrenSize);
            }

            return chunkSize + childrenSize + 4 * 3;
        }

        static int ReadPalattee(BinaryReader br, Color[] colors)
        {
            int chunkSize = br.ReadInt32();
            int childrenSize = br.ReadInt32();

            for (int i = 0; i < 256; ++i)
            {
                colors[i] = new Color((float)br.ReadByte() / 255.0f, (float)br.ReadByte() / 255.0f, (float)br.ReadByte() / 255.0f, (float)br.ReadByte() / 255.0f);
            }

            if (childrenSize > 0)
            {
                br.ReadBytes(childrenSize);
            }

            return chunkSize + childrenSize + 4 * 3;
        }


        private static bool CompareColor(int cidx, int alpha, MVVoxelChunk chunk, MVVoxelChunk alphaChunk, int f, int x, int y, int z)
        {
            if (alphaChunk == null)
                return chunk.faces[f].colorIndices[x, y, z] == cidx;
            else
                return chunk.faces[f].colorIndices[x, y, z] == cidx && alphaChunk.voxels[x, y, z] == alpha;
        }

        private static bool DetermineEmptyOrOtherAlphaVoxel(MVVoxelChunk voxelChunk, MVVoxelChunk alphaMask, int a, int x, int y, int z)
        {
            bool isEmpty = voxelChunk.voxels[x, y, z] == 0;

            if (alphaMask == null)
                return isEmpty;
            else
            {
                bool otherAlpha = alphaMask.voxels[x, y, z] != a;
                return isEmpty || otherAlpha;
            }
        }
    }
}
