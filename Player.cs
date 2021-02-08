using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShooterGame.Effects;
using System.Collections.Generic;
using System.Linq;

namespace ShooterGame
{
    public class Player : CollisionBody
    {
        public static SoundEffect bulletShootSound;

        private const float moveSpeed = 5f;

        public Texture2D[] playerAnimationArray;
        public Vector2 position;

        private float shootSpeed = 6f;
        private int shootTimer = 0;
        private int frame = 0;
        private int frameCounter = 0;
        private int explosionCounter = 0;
        private bool canMove = false;
        private bool dying = false;

        private Vector2 leftTurretOffset = new Vector2(8f, -50f);
        private Vector2 centerTurretOffset = new Vector2(46f, -12f);
        private Vector2 rightTurretOffset = new Vector2(100f, -50f);

        private List<int> afterImageFrame = new List<int>();
        private List<Vector2> afterImagePositions = new List<Vector2>();
        private List<int> afterImageAlpha = new List<int>();

        public override void Initialize()
        {
            dying = false;
            explosionCounter = 0;
            hitbox = new Rectangle((int)position.X, (int)position.Y, playerAnimationArray[0].Width, playerAnimationArray[0].Height);
        }

        public override void Update()
        {
            if (shootTimer > 0)
                shootTimer--;

            AnimateShip();
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());
            Vector2 velocity = Vector2.Zero;
            canMove = !dying;

            if (canMove)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.W) && position.Y > 0)
                {
                    velocity.Y -= moveSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.A) && position.X > 0)
                {
                    velocity.X -= moveSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.S) && position.Y < 800 - hitbox.Height)
                {
                    velocity.Y += moveSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.D) && position.X < 700 - hitbox.Width)
                {
                    velocity.X += moveSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.Space) && shootTimer <= 0)
                {
                    shootTimer += 30;
                    Projectile.NewProjectile(position + centerTurretOffset, new Vector2(0f, -shootSpeed), true);
                    bulletShootSound.Play();
                }
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            if (dying)
            {
                explosionCounter++;
                if (explosionCounter % 10 == 0)
                {
                    if (explosionCounter >= 5 * 60)
                    {
                        Main.gameState = (int)Main.GameStates.GameState_GameOver;
                    }
                    float explosionOffsetX = Main.random.Next(0, playerAnimationArray[0].Width);
                    float explosionOffsetY = Main.random.Next(0, playerAnimationArray[0].Height);
                    Explosion.NewExplosion(position + new Vector2(explosionOffsetX, explosionOffsetY), Vector2.Zero);
                }
            }
        }

        private void AnimateShip()
        {
            frameCounter++;
            if (frameCounter >= 5)
            {
                frame++;
                frameCounter = 0;
                if (frame >= playerAnimationArray.Length)
                {
                    frame = 0;
                }
            }
        }

        public override void HandleCollisions(CollisionBody collider)
        {
            if (collider is Projectile)
            {
                Projectile collidingProjectile = collider as Projectile;
                if (!collidingProjectile.friendly)
                {
                    Main.playerHealth -= 1;
                    if (Main.playerHealth <= 0)
                    {
                        dying = true;
                    }
                    Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                    collidingProjectile.DestroyInstance(collidingProjectile);
                }
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int a = 0; a < afterImageAlpha.Count; a++)
            {
                if (afterImageAlpha[a] > 0)
                {
                    spriteBatch.Draw(playerAnimationArray[afterImageFrame[a]], afterImagePositions[a], Color.White * afterImageAlpha[a]);
                }
            }

            spriteBatch.Draw(playerAnimationArray[frame], hitbox, Color.White);
        }
    }
}
