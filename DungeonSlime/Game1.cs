using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

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

    private Vector2 _batPosition;
    private Vector2 _batVelocity;

    public Game1()
        : base("Dungeon Slime", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        _inputBuffer = new Queue<Vector2>();
        // TODO: Add your initialization logic here
        _batPosition = new Vector2(_slime.Width + 10, 0);
        AssignRandomBatVelocity();
    }
    private void AssignRandomBatVelocity()
    {
        // Generate a random angle.
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Convert angle to a direction vector.
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        // Multiply the direction vector by the movement speed.
        _batVelocity = direction * MOVE_SPEED;
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
        base.Update(gameTime);

        _slime.Update(gameTime);
        _bat.Update(gameTime);
        // TODO: Add your update logic here
        CheckKeyboardInput();
        CheckGamePadInput();

        Rectangle screenBounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

        Circle slimeBounds = new Circle((int)(_slimePosition.X + _slime.Width * 0.5f), (int)(_slimePosition.Y + _slime.Height * 0.5f), (int)(_slime.Width * 0.5f));

        if (slimeBounds.Left < screenBounds.Left)
        {
            _slimePosition.X = screenBounds.Left;
        }
        else if (slimeBounds.Right > screenBounds.Right)
        {
            _slimePosition.X = screenBounds.Right - _slime.Width;
        }
        if (slimeBounds.Top < screenBounds.Top)
        {
            _slimePosition.Y = screenBounds.Top;
        }
        else if (slimeBounds.Bottom > screenBounds.Bottom)
        {
            _slimePosition.Y = screenBounds.Bottom - _slime.Height;
        }

        Vector2 newBatPosition = _batPosition + _batVelocity;

        Circle batBounds = new Circle((int)(newBatPosition.X + _bat.Width * 0.5f), (int)(newBatPosition.Y + _bat.Height * 0.5f), (int)(_bat.Width * 0.5f));

        Vector2 normal = Vector2.Zero;

        if (batBounds.Left < screenBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBatPosition.X = screenBounds.Left;
        }
        else if (batBounds.Right > screenBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBatPosition.X = screenBounds.Right - _bat.Width;
        }
        if (batBounds.Top < screenBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBatPosition.Y = screenBounds.Top;
        }
        else if (batBounds.Bottom > screenBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBatPosition.Y = screenBounds.Bottom - _bat.Height;
        }

        if (normal != Vector2.Zero)
        {
            normal.Normalize();
            _batVelocity = Vector2.Reflect(_batVelocity, normal);
        }

        _batPosition = newBatPosition;

        if (slimeBounds.Intersects(batBounds))
        {
            int totalColumns = GraphicsDevice.PresentationParameters.BackBufferWidth / (int)_bat.Width;
            int totalRows = GraphicsDevice.PresentationParameters.BackBufferHeight / (int)_bat.Height;

            int column = Random.Shared.Next(totalColumns);
            int row = Random.Shared.Next(totalRows);
            _batPosition = new Vector2(column * _bat.Width, row * _bat.Height);
            AssignRandomBatVelocity();
        }
    }

    private void CheckKeyboardInput()
    {
        Vector2 newDirection = Vector2.Zero;
        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            _speed = 1.5f * MOVE_SPEED;
        }
        else
        {
            _speed = MOVE_SPEED;
        }
        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            newDirection = -Vector2.UnitY;
        }
        else if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            newDirection = Vector2.UnitY;
        }
        else if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            newDirection = -Vector2.UnitX;
        }
        else if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            newDirection = Vector2.UnitX;
        }
        if (newDirection != Vector2.Zero && _inputBuffer.Count < MAX_INPUT_BUFFER_SIZE)
        {
            _inputBuffer.Enqueue(newDirection);
        }

        if (_inputBuffer.Count > 0)
        {
            Vector2 direction = _inputBuffer.Dequeue();
            _slimePosition += direction * _speed;
        }
    }

    private void CheckGamePadInput()
    {
        GamePadInfo gamePadOne = Input.GamePads[(int)PlayerIndex.One];
        float speed = MOVE_SPEED;
        if (gamePadOne.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            gamePadOne.SetVibration(0.5f, TimeSpan.FromSeconds(0.5));
        }
        else
        {
            gamePadOne.StopVibration();
        }
        if (gamePadOne.LeftThumbStick != Vector2.Zero)
        {
            _slimePosition.X += gamePadOne.LeftThumbStick.X * speed;
            _slimePosition.Y -= gamePadOne.LeftThumbStick.Y * speed;
        }
        else
        {
            if (gamePadOne.IsButtonDown(Buttons.DPadUp))
            {
                _slimePosition.Y -= speed;
            }
            if (gamePadOne.IsButtonDown(Buttons.DPadDown))
            {
                _slimePosition.Y += speed;
            }
            if (gamePadOne.IsButtonDown(Buttons.DPadLeft))
            {
                _slimePosition.X -= speed;
            }
            if (gamePadOne.IsButtonDown(Buttons.DPadRight))
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
        _bat.Draw(SpriteBatch, _batPosition);

        SpriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
