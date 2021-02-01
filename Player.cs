using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShooterGame.Effects;
using System.Collections.Generic;
using System.Linq;

namespace ShooterGame
{
    public class Player : CollisionBody
    {
        public Texture2D[] playerAnimationArray;

        private const float moveSpeed = 5f;
        private const int dashDetectionTime = 3;
        private const int dashDurationTime = 10;
        private const int dashCooldownTime = 60;

        private Vector2 position;
        private float shootSpeed = 6f;
        private int shootTimer = 0;
        private int frame = 0;
        private int frameCounter = 0;
        private int explosionCounter = 0;
        private int dashTimer = 0;
        private int dashCooldown = 0;
        private int[] dashKeyPressTimers = new int[2];
        private bool[] dashKeyCanPressAgain = new bool[2];
        private int dashDirection = 1;
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
            hitbox = new Rectangle((int)position.X, (int)position.Y, playerAnimationArray[0].Width, playerAnimationArray[0].Height);
        }

        public override void Update()
        {
            if (shootTimer > 0)
                shootTimer--;
            if (dashCooldown > 0)
                dashCooldown--;

            AnimateShip();
            HandleAfterImages();
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());
            Vector2 velocity = Vector2.Zero;
            canMove = !dying && dashTimer <= 0;

            if (canMove)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    velocity.Y -= moveSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    velocity.X -= moveSpeed;
                    if (!dashKeyCanPressAgain[0])
                        dashKeyPressTimers[0] += dashDetectionTime;
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    velocity.Y += moveSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    velocity.X += moveSpeed;
                    if (!dashKeyCanPressAgain[1])
                        dashKeyPressTimers[1] += dashDurationTime;
                }
                if (keyboardState.IsKeyDown(Keys.Space) && shootTimer <= 0)
                {
                    shootTimer += 30;
                    Projectile.NewProjectile(position + centerTurretOffset, new Vector2(0f, -shootSpeed), true);
                }
                UpdatePlayerDash(keyboardState, velocity);
            }
            if (dashTimer > 0)
            {
                dashTimer--;
                velocity = new Vector2(moveSpeed * 3f * dashDirection, 0f);
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

        private void UpdatePlayerDash(KeyboardState keyboardState, Vector2 velocity)
        {
            for (int timerIndex = 0; timerIndex < dashKeyPressTimers.Length; timerIndex++)
            {
                if (dashKeyPressTimers[timerIndex] > 0)
                {
                    dashKeyPressTimers[timerIndex]--;
                }
                else
                {
                    dashKeyCanPressAgain[timerIndex] = false;
                }
            }

            if (dashKeyPressTimers[0] > 0 && keyboardState.IsKeyUp(Keys.A))
            {
                dashKeyCanPressAgain[0] = true;
            }
            if (dashKeyPressTimers[1] > 0 && keyboardState.IsKeyUp(Keys.D))
            {
                dashKeyCanPressAgain[1] = true;
            }
            if (dashTimer <= 0 && dashCooldown <= 0 && dashKeyCanPressAgain[0] && dashKeyPressTimers[0] > 0 && keyboardState.IsKeyDown(Keys.A))
            {
                dashTimer += dashDurationTime;
                velocity.X -= moveSpeed * 3f;
                dashDirection = -1;
                dashKeyPressTimers[0] = 0;
                dashKeyCanPressAgain[0] = false;
                dashCooldown = dashCooldownTime;
            }
            if (dashTimer <= 0 && dashCooldown <= 0 && dashKeyCanPressAgain[1] && dashKeyPressTimers[1] > 0 && keyboardState.IsKeyDown(Keys.D))
            {
                dashTimer += dashDurationTime;
                velocity.X += moveSpeed * 3f;
                dashDirection = 1;
                dashKeyPressTimers[1] = 0;
                dashKeyCanPressAgain[1] = false;
                dashCooldown = dashCooldownTime;
            }
        }

        private void HandleAfterImages()
        {
            if (dashTimer > 0 && dashTimer % 2 == 0)
            {
                afterImageAlpha.Add(255);
                afterImagePositions.Add(position);
                afterImageFrame.Add(frame);
            }

            if (afterImageAlpha.Count > 0)
            {
                int currentAfterImageCount = afterImageAlpha.Count;
                for (int a = 0; a < currentAfterImageCount; a++)
                {
                    if (a >= afterImageAlpha.Count)
                    {
                        break;
                    }
                    if (afterImageAlpha[a] > 0)
                    {
                        afterImageAlpha[a] -= 180 / dashDurationTime;
                        if (afterImageAlpha[a] <= 0)
                        {
                            afterImageAlpha.RemoveAt(a);
                            afterImagePositions.RemoveAt(a);
                            afterImageFrame.RemoveAt(a);
                            a--;
                        }
                    }
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
