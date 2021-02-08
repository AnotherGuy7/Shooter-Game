using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ShooterGame.Effects;
using System.Collections.Generic;
using System.Linq;

namespace ShooterGame.Enemies
{
    public class BossUFO : CollisionBody
    {
        public static Texture2D[] bossUFOAnimationArray;
        public static SoundEffect laserShootSound;
        public Vector2 position;

        private int frame = 0;
        private int frameCounter = 0;
        private int shootCounter = 0;
        private Vector2 shootVelocity = new Vector2(0f, 6f);
        private Vector2 shootOffset = new Vector2(50f, 130f);
        private int health = 8;
        private int flyDirection = 1;

        private Vector2 leftTurretPosition = new Vector2(12f, 59f);
        private Vector2 rightTurretPosition = new Vector2(118f, 63f);

        public static void NewBossUFO(Vector2 position)
        {
            BossUFO currentInstance = new BossUFO();
            /*for (int p = 0; p < Main.MaxProjectiles; p++)
            {
                if (Main.activeProjectiles[p] == null)
                {
                    Main.activeProjectiles[p] = currentInstance;
                }
            }*/
            currentInstance.position = position;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, bossUFOAnimationArray[0].Width, bossUFOAnimationArray[0].Height);
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

            if (position.X < 0)
            {
                flyDirection = 1;
            }
            else if (position.X > 700 - hitbox.Width)
            {
                flyDirection = -1;
            }
            velocity.X = 4 * flyDirection;

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            shootCounter++;
            if (shootCounter >= (2 * 60) / Main.gameDifficulty)
            {
                if (Main.random.Next(0, 2) == 0)
                {
                    Projectile.NewProjectile(position + leftTurretPosition, shootVelocity, false);
                }
                else
                {
                    Projectile.NewProjectile(position + rightTurretPosition, shootVelocity, false);
                }
                shootCounter = 0;
                laserShootSound.Play();
            }

            if (health <= 0)
            {
                DestroyInstance(this);
                Main.bossActive = false;
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
                if (frame >= bossUFOAnimationArray.Length)
                {
                    frame = 0;
                }
            }
        }

        public void DestroyInstance(BossUFO boss)
        {
            Main.gameScore += 1;
            Main.gameDifficulty += 1;
            Main.playerHealth += 1;
            Main.activeEntities.Remove(boss);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bossUFOAnimationArray[frame], hitbox, Color.White);
        }
    }
}
