using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime;

public class Game1 : Core
{
    private AnimatedSprite _slime;
    private Vector2 _slimePosition;
    private const float MOVE_SPEED = 5f;
    private float _speed;
    private AnimatedSprite _bat;

    private Queue<Vector2> _inputBuffer;
    private const int MAX_INPUT_BUFFER_SIZE = 2;

    public Game1()
        : base("Dungeon Slime", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        _inputBuffer = new Queue<Vector2>();
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");
        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _slime.Scale = new Vector2(4f);
        _bat.Scale = new Vector2(4f);
        // TODO: use this.Content to load your game content here
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        _slime.Update(gameTime);
        _bat.Update(gameTime);
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        CheckKeyboardInput();
        CheckGamePadInput();

        if (_inputBuffer.Count > 0)
        {
            Vector2 direction = _inputBuffer.Dequeue();
            _slimePosition += direction * _speed;
        }
        base.Update(gameTime);
    }

    private void CheckKeyboardInput()
    {
        KeyboardState keyboardState = Keyboard.GetState();
        Vector2 newDirection = Vector2.Zero;
        if (keyboardState.IsKeyDown(Keys.Space))
        {
            _speed = 1.5f * MOVE_SPEED;
        }
        else
        {
            _speed = MOVE_SPEED;
        }
        if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
        {
            newDirection = -Vector2.UnitY;
        }
        else if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
        {
            newDirection = Vector2.UnitY;
        }
        else if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
        {
            newDirection = -Vector2.UnitX;
        }
        else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
        {
            newDirection = Vector2.UnitX;
        }
        if (newDirection != Vector2.Zero && _inputBuffer.Count < MAX_INPUT_BUFFER_SIZE)
        {
            _inputBuffer.Enqueue(newDirection);
        }
    }

    private void CheckGamePadInput()
    {
        GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
        float speed = MOVE_SPEED;
        if (gamePadState.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
        }
        else
        {
            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        }
        if (gamePadState.ThumbSticks.Left != Vector2.Zero)
        {
            _slimePosition.X += gamePadState.ThumbSticks.Left.X * speed;
            _slimePosition.Y -= gamePadState.ThumbSticks.Left.Y * speed;
        }
        else
        {
            if (gamePadState.IsButtonDown(Buttons.DPadUp))
            {
                _slimePosition.Y -= speed;
            }
            if (gamePadState.IsButtonDown(Buttons.DPadDown))
            {
                _slimePosition.Y += speed;
            }
            if (gamePadState.IsButtonDown(Buttons.DPadLeft))
            {
                _slimePosition.X -= speed;
            }
            if (gamePadState.IsButtonDown(Buttons.DPadRight))
            {
                _slimePosition.X += speed;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _slime.Draw(SpriteBatch, _slimePosition);
        _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));

        SpriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
