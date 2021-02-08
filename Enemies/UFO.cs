using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ShooterGame.Effects;
using System.Collections.Generic;
using System.Linq;

namespace ShooterGame.Enemies
{
    public class UFO : CollisionBody
    {
        public static Texture2D[] ufoAnimationArray;
        public static SoundEffect laserShootSound;
        public Vector2 position;

        private int frame = 0;
        private int frameCounter = 0;
        private int shootCounter = 0;
        private Vector2 shootVelocity = new Vector2(0f, 6f);
        private Vector2 shootOffset = new Vector2(50f, 130f);
        private int health = 3;

        public static void NewUFO(Vector2 position)
        {
            UFO currentInstance = new UFO();
            /*for (int p = 0; p < Main.MaxProjectiles; p++)
            {
                if (Main.activeProjectiles[p] == null)
                {
                    Main.activeProjectiles[p] = currentInstance;
                }
            }*/
            currentInstance.position = position;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, ufoAnimationArray[0].Width, ufoAnimationArray[0].Height);
            Main.activeEntities.Add(currentInstance);
        }

        public override void Update()
        {
            AnimateShip();
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());

            Vector2 velocity = Vector2.Zero;

            if (position.Y < 50f)
            {
                velocity.Y = 1f;
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            shootCounter++;
            if (shootCounter >= (4 * 60) / Main.gameDifficulty)
            {
                laserShootSound.Play();
                Projectile.NewProjectile(position + shootOffset, shootVelocity, false);
                shootCounter = 0;
            }

            if (health <= 0)
            {
                DestroyInstance(this);
            }
        }

        public override void HandleCollisions(CollisionBody collider)
        {
            if (collider is Projectile)
            {
                Projectile collidingProjectile = collider as Projectile;
                if (collidingProjectile.friendly)
                {
                    health -= 1;
                    Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                    collidingProjectile.DestroyInstance(collidingProjectile);
                }
            }
        }

        private void AnimateShip()
        {
            frameCounter++;
            if (frameCounter >= 7)
            {
                frame++;
                frameCounter = 0;
                if (frame >= ufoAnimationArray.Length)
                {
                    frame = 0;
                }
            }
        }

        public void DestroyInstance(UFO ufo)
        {
            Main.gameScore += 1;
            Main.activeEntities.Remove(ufo);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ufoAnimationArray[frame], hitbox, Color.White);
        }
    }
}
