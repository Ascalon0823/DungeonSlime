using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite
{
    private int _currentFrame;
    private TimeSpan _elapsedTime;
    private Animation _animation;

    public AnimatedSprite() { }

    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }

    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }

    public void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;
        if (_elapsedTime >= _animation.Delay)
        {
            _elapsedTime -= _animation.Delay;
            _currentFrame++;
            if (_currentFrame >= _animation.Frames.Count)
            {
                _currentFrame = 0;
            }

            Region = _animation.Frames[_currentFrame];
        }
    }
}