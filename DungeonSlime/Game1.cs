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
        base.Update(gameTime);

        _slime.Update(gameTime);
        _bat.Update(gameTime);
        // TODO: Add your update logic here
        CheckKeyboardInput();
        CheckGamePadInput();

        if (_inputBuffer.Count > 0)
        {
            Vector2 direction = _inputBuffer.Dequeue();
            _slimePosition += direction * _speed;
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
        _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));

        SpriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
