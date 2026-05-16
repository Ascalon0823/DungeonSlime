using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Tilemap
{
    private readonly Tileset _tileset;
    private readonly int[] _tiles;

    public int Rows { get; }
    public int Columns { get; }
    public int Count { get; }
    public Vector2 Scale { get; set; }

    public float TileWidth => _tileset.TileWidth * Scale.X;
    public float TileHeight => _tileset.TileHeight * Scale.Y;

    public static Tilemap FromFile(ContentManager contentManager, string fileName)
    {
        string filePath = Path.Combine(contentManager.RootDirectory, fileName);
        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;
                XElement tilesetElement = root.Element("Tileset");

                string regionAttribute = tilesetElement.Attribute("region").Value;
                string[] split = regionAttribute.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                int x = int.Parse(split[0]);
                int y = int.Parse(split[1]);
                int width = int.Parse(split[2]);
                int height = int.Parse(split[3]);

                int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                string contentPath = tilesetElement.Value;

                Texture2D texture = contentManager.Load<Texture2D>(contentPath);

                TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                XElement tilemapElement = root.Element("Tiles");
                string[] rows = tilemapElement.Value.Trim().Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
                int columnsCount = rows[0].Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Length;
                Tilemap tilemap = new Tilemap(tileset, columnsCount, rows.Length);

                for (int row = 0; row < rows.Length; row++)
                {
                    string[] columns = rows[row].Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    for (int column = 0; column < columnsCount; column++)
                    {
                        int tileIndex = int.Parse(columns[column]);
                        tilemap.SetTile(column, row, tileIndex);
                    }
                }

                return tilemap;
            }
        }
    }
    public Tilemap(Tileset tileset, int columns, int rows)
    {
        _tileset = tileset;
        Columns = columns;
        Rows = rows;
        Count = Columns * Rows;
        _tiles = new int[Count];
        Scale = Vector2.One;
    }

    public void SetTile(int index, int tileIndex)
    {
        _tiles[index] = tileIndex;
    }

    public void SetTile(int column, int row, int tileIndex)
    {
        _tiles[row * Columns + column] = tileIndex;
    }

    public TextureRegion GetTile(int index) => _tileset.GetTile(_tiles[index]);
    public TextureRegion GetTile(int column, int row) => _tileset.GetTile(_tiles[row * Columns + column]);

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Count; i++)
        {
            int column = i % Columns;
            int row = i / Columns;
            TextureRegion tile = GetTile(i);
            Vector2 position = new Vector2(column * TileWidth, row * TileHeight);
            tile.Draw(spriteBatch, position, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 1f);
        }
    }
}