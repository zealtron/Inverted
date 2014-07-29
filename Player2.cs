using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Platformer
{
	class Player2 : Sprite
    {
		private bool moving;
		private bool grounded;
		private int speed;
		private int x_accel;
		private double friction;
		public double x_vel;
		public double y_vel;
		public int movedX;
		private bool pushing;
		public double gravity = -.5;
		public int maxFallSpeed = -10;
		private int jumpPoint = 0;
        public Player p1;
        private bool held;
        public Rectangle rect;
        public Player2(int x, int y, int width, int height)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
			grounded = false;
			moving = false;
			pushing = false;
            held = false;

			// Movement
			speed = 5;
			friction = .15;
			x_accel = 0;
			x_vel = 0;
			y_vel = 0;
			movedX = 0;
        }

        public int getX(){
            return spriteX;
        }
        public int getY()
        {
            return spriteY;
        }
        public void setX(int x)
        {
            spriteX = x;
        }
        public void setY(int y)
        {
            spriteY = y;
        }
        public Player getP1()
        {
            return p1;
        }
        public void setP1(Player p1)
        {
            this.p1 = p1;
        }
        public bool getGrounded()
        {
            return grounded;
        }
        public bool getHold()
        {
            return held;
        }
        public void setHold(bool held)
        {
            this.held = held;
        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("Benny.png");
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

		public void Update(Controls controls, GameTime gameTime)
		{
			Move (controls);
			Jump (controls, gameTime);
		}

		public void Move(Controls controls)
		{

			// Sideways Acceleration
			if (controls.onPress(Keys.D, Buttons.DPadRight))
				x_accel += speed;
			else if (controls.onRelease(Keys.D, Buttons.DPadRight))
				x_accel -= speed;
			if (controls.onPress(Keys.A, Buttons.DPadLeft))
				x_accel -= speed;
			else if (controls.onRelease(Keys.A, Buttons.DPadLeft))
				x_accel += speed;

			double playerFriction = pushing ? (friction * 3) : friction;
			x_vel = x_vel * (1 - playerFriction) + x_accel * .10;
			movedX = Convert.ToInt32(x_vel);
			spriteX += movedX;

			// Gravity
			if (!grounded)
			{
				y_vel += gravity;
				if (y_vel < maxFallSpeed)
					y_vel = maxFallSpeed;
				spriteY += Convert.ToInt32(y_vel);
			}
			else
			{
				y_vel = -1;
			}

			grounded = false;

			// Check up/down collisions, then left/right
			checkYCollisions();
            rect = new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight);

		}

		private void checkYCollisions()
		{
			if (spriteY <= 100)
				grounded = true;
			else
				grounded = false;
		}

		private void Jump(Controls controls, GameTime gameTime)
		{
			// Jump on button press
			if (controls.onPress(Keys.S, Buttons.A) && !p1.getHold())
			{
				y_vel = 10;
				jumpPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
				grounded = false;
			}

			// Cut jump short on button release
			//else if (controls.onRelease(Keys.Space, Buttons.A) && y_vel < 0)
			//{
			//	y_vel /= 2;
			//}
		}
        public void Hold(Controls controls, Player p1)
        {
            p1 = this.p1;
            int sprite2Y = p1.getY() - 81;
            if (spriteY + 30 == sprite2Y && (spriteX - 15 <= p1.getX() && spriteX + 5 >= p1.getX()) && p1.getGrounded())
            {
                held = true;
            }
        }
        public void Drop(Controls controls, Player p1)
        {
            if (held)
            {
                setY(p1.getY() - 65);
                setX(p1.getX());
                if (controls.onPress(Keys.W, Buttons.LeftShoulder) && p1.getGrounded())
                {
                    held = false;
                }
            }
        }
    }
}
