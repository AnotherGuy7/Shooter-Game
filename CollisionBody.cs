using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ShooterGame
{
    public abstract class CollisionBody
    {
        public Rectangle hitbox;

        public virtual void Initialize()
        {}

        public virtual void Update()
        {}

        public virtual void Draw(SpriteBatch spriteBatch)
        {}

        public void DetectCollisions(List<CollisionBody> possibleIntersectors)
        {
            foreach (CollisionBody intersector in possibleIntersectors)
            {
                if (hitbox.Intersects(intersector.hitbox))
                {
                    HandleCollisions(intersector);
                }
            }
        }

        public virtual void HandleCollisions(CollisionBody collider)
        {}
    }
}
