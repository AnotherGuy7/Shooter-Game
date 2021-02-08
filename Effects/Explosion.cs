using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShooterGame.Effects
{
    public class Explosion : VisualEffect
    {
        public static Texture2D[] explosionAnimationArray;
        public static SoundEffect explosionSound;

        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private float rotation;
        private bool playedSound = false;

        public static void NewExplosion(Vector2 position, Vector2 velocity, float rotation = 0f)
        {
            Explosion newInstance = new Explosion();
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.rotation = rotation;
            Main.activeEffects.Add(newInstance);
        }

        public override void Update()
        {
            frameCounter++;
            if (frameCounter >= 8)
            {
                frame++;
                frameCounter = 0;
                if (frame >= explosionAnimationArray.Length)
                {
                    frame = 0;
                    DestroyInstance(this);
                }
            }
            if (!playedSound)
            {
                explosionSound.Play();
                playedSound = true;
            }

            if (velocity != Vector2.Zero)
            {
                position += velocity;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = new Rectangle((int)position.X, (int)position.Y, explosionAnimationArray[0].Width, explosionAnimationArray[0].Height);
            spriteBatch.Draw(explosionAnimationArray[frame], rect, Color.White);
        }

        public void DestroyInstance(Explosion explosion)
        {
            Main.activeEffects.Remove(explosion);
        }
    }
}
