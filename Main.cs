using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShooterGame.Effects;
using System;
using System.Collections.Generic;

namespace ShooterGame
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public const int MaxTotalEntities = 100;
        public const int MaxProjectiles = 20;

        //public static CollisionBody[] activeEntities = new CollisionBody[MaxTotalEntities];
        //public static Projectile[] activeProjectiles = new Projectile[MaxProjectiles];
        public static List<CollisionBody> activeEntities = new List<CollisionBody>();
        public static List<Projectile> activeProjectiles = new List<Projectile>();
        public static List<VisualEffect> activeEffects = new List<VisualEffect>();
        public static Random random = new Random();

        public Player player;
        public static int playerHealth = 6;

        private Texture2D background;
        private Texture2D playerHealthIcon;

        public enum GameStates
        {
            GameState_Title,
            GameState_Playing,
            GameState_GameOver
        }

        public static int gameState = (int)GameStates.GameState_Playing;

        public Main()
        {
            Window.Title = "Shooter Game";
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            /*foreach (CollisionBody entity in activeEntities)
            {
                entity.Initialize()
            }*/
            player = new Player();
            player.playerAnimationArray = new Texture2D[3] { Content.Load<Texture2D>("Images/Spaceship_1") , Content.Load<Texture2D>("Images/Spaceship_2"), Content.Load<Texture2D>("Images/Spaceship_3") };
            player.Initialize();
            activeEntities.Add(player);

            _graphics.PreferredBackBufferWidth = 700;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            LoadContent();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("Images/Backgrounds/SpaceBackground2");
            playerHealthIcon = Content.Load<Texture2D>("Images/PlayerHealthIcon");
            Projectile.playerBullet = Content.Load<Texture2D>("Images/Objects/Player_Bullet");
            Projectile.enemyLaser = Content.Load<Texture2D>("Images/Objects/EnemyLaser");
            Texture2D[] ufoFrames = new Texture2D[5];
            for (int i = 0; i < 5; i++)
            {
                ufoFrames[i] = Content.Load<Texture2D>("Images/UFO_" + (i + 1));
            }
            Enemies.UFO.ufoAnimationArray = ufoFrames;
            Texture2D[] explosionFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                explosionFrames[i] = Content.Load<Texture2D>("Images/Explosion_" + (i + 1));
            }
            Explosion.explosionAnimationArray = explosionFrames;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            switch (gameState)
            {
                case (int)GameStates.GameState_Title:
                    break;
                case (int)GameStates.GameState_Playing:
                    CollisionBody[] activeEntitiesClone = activeEntities.ToArray();     //This is so that when the active list changes, the loop isn't affected
                    Projectile[] activeProjectilesClone = activeProjectiles.ToArray();
                    VisualEffect[] activeEffectsClone = activeEffects.ToArray();
                    foreach (CollisionBody entity in activeEntitiesClone)
                    {
                        entity.Update();
                    }
                    foreach (Projectile projectile in activeProjectilesClone)
                    {
                        projectile.Update();
                    }
                    foreach (VisualEffect effect in activeEffectsClone)
                    {
                        effect.Update();
                    }

                    if (random.Next(1, 250) == 1)
                    {
                        float posX = random.Next(50, _graphics.PreferredBackBufferWidth - 50);
                        float posY = -200;
                        Enemies.UFO.NewUFO(new Vector2(posX, posY));
                    }
                    break;
                case (int)GameStates.GameState_GameOver:
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();


            switch (gameState)
            {
                case (int)GameStates.GameState_Title:
                    _spriteBatch.Draw(background, GraphicsDevice.ScissorRectangle, Color.White);
                    break;
                case (int)GameStates.GameState_Playing:

                    _spriteBatch.Draw(background, GraphicsDevice.ScissorRectangle, Color.White);

                    CollisionBody[] activeEntitiesClone = activeEntities.ToArray();
                    Projectile[] activeProjectilesClone = activeProjectiles.ToArray();
                    VisualEffect[] activeEffectsClone = activeEffects.ToArray();
                    foreach (CollisionBody entity in activeEntitiesClone)
                    {
                        entity.Draw(_spriteBatch);
                    }
                    foreach (Projectile projectile in activeProjectilesClone)
                    {
                        projectile.Draw(_spriteBatch);
                    }
                    foreach (VisualEffect effect in activeEffectsClone)
                    {
                        effect.Draw(_spriteBatch);
                    }

                    DrawUI(_spriteBatch);

                    break;
                case (int)GameStates.GameState_GameOver:
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawUI(SpriteBatch spriteBatch)
        {
            for (int healthIcon = 0; healthIcon < playerHealth; healthIcon++)
            {
                spriteBatch.Draw(playerHealthIcon, new Vector2(_graphics.PreferredBackBufferWidth - ((healthIcon + 1) * 42), 0), Color.White);
            }
        }
    }
}
