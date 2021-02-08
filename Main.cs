using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        public const int MaxStars = 70;

        //public static CollisionBody[] activeEntities = new CollisionBody[MaxTotalEntities];
        //public static Projectile[] activeProjectiles = new Projectile[MaxProjectiles];
        public static List<CollisionBody> activeEntities = new List<CollisionBody>();
        public static List<Projectile> activeProjectiles = new List<Projectile>();
        public static List<VisualEffect> activeEffects = new List<VisualEffect>();
        public static Random random = new Random();

        public Player player;
        public static int playerHealth = 6;

        public static int gameDifficulty = 1;
        public static int gameScore = 1;
        public static bool bossActive = false;

        private Texture2D background;
        private Texture2D titleScreenBackground;
        private Texture2D gameOverBackground;
        private Texture2D playerHealthIcon;
        private Texture2D starTexture;
        private SpriteFont mainFont;
        private Song backgroundMusic;

        private Vector2[] starPositions = new Vector2[MaxStars];
        private float[] starFallSpeeds = new float[MaxStars];

        public enum GameStates
        {
            GameState_Title,
            GameState_Playing,
            GameState_GameOver
        }

        public static int gameState = (int)GameStates.GameState_Title;

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

            _graphics.PreferredBackBufferWidth = 700;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            for (int star = 0; star < MaxStars; star++)
            {
                float posX = random.Next(0, _graphics.PreferredBackBufferWidth);
                float posY = random.Next(0, _graphics.PreferredBackBufferHeight);
                starPositions[star] = new Vector2(posX, posY);
                starFallSpeeds[star] = random.Next(5, 9);
            }

            LoadContent();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("Images/Backgrounds/SpaceBackground2");
            titleScreenBackground = Content.Load<Texture2D>("Images/Backgrounds/TitleScreenBackground");
            gameOverBackground = Content.Load<Texture2D>("Images/Backgrounds/GameOverBackground");
            playerHealthIcon = Content.Load<Texture2D>("Images/PlayerHealthIcon");
            starTexture = Content.Load<Texture2D>("Images/Objects/Star");
            mainFont = Content.Load<SpriteFont>("Fonts/MainFont");
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

            Texture2D[] bossUFOFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                bossUFOFrames[i] = Content.Load<Texture2D>("Images/BattleUFO_" + (i + 1));
            }
            Enemies.BossUFO.bossUFOAnimationArray = bossUFOFrames;

            Player.bulletShootSound = Content.Load<SoundEffect>("Sounds/PlayerShoot");
            Enemies.BossUFO.laserShootSound = Enemies.UFO.laserShootSound = Content.Load<SoundEffect>("Sounds/Laser");
            backgroundMusic = Content.Load<Song>("Sounds/Music");
            Effects.Explosion.explosionSound = Content.Load<SoundEffect>("Sounds/Explosion");
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
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        gameState = (int)GameStates.GameState_Playing;
                        ReInitializeGame();
                    }
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
                    for (int star = 0; star < MaxStars; star++)
                    {
                        starPositions[star].Y += starFallSpeeds[star];
                        if (starPositions[star].Y > _graphics.PreferredBackBufferHeight)
                        {
                            float posX = random.Next(0, _graphics.PreferredBackBufferWidth);
                            float posY = random.Next(-30, -10);
                            starPositions[star] = new Vector2(posX, posY);
                            starFallSpeeds[star] = random.Next(5, 9);
                        }
                    }

                    if (!bossActive && gameScore % 25 != 0)
                    {
                        if (random.Next(1, 250) == 1)
                        {
                            float posX = random.Next(123, _graphics.PreferredBackBufferWidth - 123);
                            float posY = -200;
                            Enemies.UFO.NewUFO(new Vector2(posX, posY));
                        }
                    }
                    else
                    {
                        if (!bossActive)
                        {
                            float posX = _graphics.PreferredBackBufferWidth / 2;
                            float posY = -200;
                            Enemies.BossUFO.NewBossUFO(new Vector2(posX, posY));
                            bossActive = true;
                        }
                    }
                    break;
                case (int)GameStates.GameState_GameOver:

                    if (activeEntities.Count != 0)
                    {
                        activeEntities.Clear();
                        activeProjectiles.Clear();
                        activeEffects.Clear();
                        MediaPlayer.Stop();
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        gameState = (int)GameStates.GameState_Playing;
                        ReInitializeGame();
                    }
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (gameState)
            {
                case (int)GameStates.GameState_Title:
                    _spriteBatch.Draw(titleScreenBackground, GraphicsDevice.ScissorRectangle, Color.White);
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
                    for (int star = 0; star < MaxStars; star++)
                    {
                        _spriteBatch.Draw(starTexture, starPositions[star], Color.White);
                    }

                    DrawUI(_spriteBatch);

                    break;
                case (int)GameStates.GameState_GameOver:
                    _spriteBatch.Draw(gameOverBackground, GraphicsDevice.ScissorRectangle, Color.White);

                    _spriteBatch.DrawString(mainFont, "Score: " + gameScore, new Vector2(270, 680), Color.White);
                    break;
            }

            _spriteBatch.End();

        }

        private void DrawUI(SpriteBatch spriteBatch)
        {
            for (int healthIcon = 0; healthIcon < playerHealth; healthIcon++)
            {
                spriteBatch.Draw(playerHealthIcon, new Vector2(_graphics.PreferredBackBufferWidth - ((healthIcon + 1) * 42), 0), Color.White);
            }
        }

        private void ReInitializeGame()
        {
            player.Initialize();
            player.position.X = 270f;
            player.position.Y = 330f;
            activeEntities.Add(player);

            playerHealth = 5;
            gameDifficulty = 1;
            gameScore = 1;

            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
        }
    }
}
