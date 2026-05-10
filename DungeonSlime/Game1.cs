using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace DungeonSlime;

public class Game1 : Core
{
    private Texture2D _logo;
    public Game1()
        : base("Dungeon Slime", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
        base.LoadContent();
        _logo = Content.Load<Texture2D>("images/logo");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Rectangle iconSourceRect = new Rectangle(0, 0, 128, 128);

        Rectangle wordmarkSourceRect = new Rectangle(150, 34, 458, 58);

        SpriteBatch.Begin(SpriteSortMode.FrontToBack);
        SpriteBatch.Draw(
            _logo,
            new Vector2(
                Window.ClientBounds.Width * 0.5f,
                Window.ClientBounds.Height * 0.5f
            ),
            iconSourceRect,
            Color.White,
            0f,
            new Vector2(iconSourceRect.Width, iconSourceRect.Height) * 0.5f,
            1.5f,
            SpriteEffects.None,
            1f
            );
        SpriteBatch.Draw(
            _logo,
            new Vector2(
                Window.ClientBounds.Width * 0.5f,
                Window.ClientBounds.Height * 0.5f
            ),
            wordmarkSourceRect,
            Color.White,
            0f,
            new Vector2(wordmarkSourceRect.Width, wordmarkSourceRect.Height) * 0.5f,
            1.5f,
            SpriteEffects.None,
            0f
        );
        SpriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
