using Microsoft.Xna.Framework.Graphics;

namespace ShooterGame.Effects
{
    public abstract class VisualEffect
    {
        public int frame = 0;
        public int frameCounter = 0;

        public virtual void Update()
        {}

        public virtual void Draw(SpriteBatch spriteBatch)
        {}
    }
}
