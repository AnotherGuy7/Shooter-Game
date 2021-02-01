using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterGame
{
    public class Projectile : CollisionBody
    {
        public static Texture2D playerBullet;
        public static Texture2D enemyLaser;

        public bool friendly = false;

        public Vector2 position;
        public Vector2 velocity;

        private Texture2D texture;
        private int lifeTimer = 0;

        public static void NewProjectile(Vector2 position, Vector2 velocity, bool friendly)
        {
            Projectile currentInstance = new Projectile();
            /*for (int p = 0; p < Main.MaxProjectiles; p++)
            {
                if (Main.activeProjectiles[p] == null)
                {
                    Main.activeProjectiles[p] = currentInstance;
                }
            }*/
            currentInstance.position = position;
            currentInstance.velocity = velocity;
            currentInstance.friendly = friendly;
            if (friendly)
            {
                currentInstance.texture = playerBullet;
            }
            else
            {
                currentInstance.texture = enemyLaser;
            }
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, currentInstance.texture.Width, currentInstance.texture.Height);
            Main.activeProjectiles.Add(currentInstance);            
        }

        public override void Update()
        {
            lifeTimer++;
            if (lifeTimer > 3 * 60)
            {
                DestroyInstance(this);
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, hitbox, Color.White);
        }

        public void DestroyInstance(Projectile projectile)
        {
            Main.activeProjectiles.Remove(projectile);
        }
    }
}
