using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MelodyMadness
{
    class Sprite
    {
        public Bitmap Texture;
        public Animation MainAnimation;
        public float X, Y;
        public int Width, Height;
        private const float Gravity = 5.8f;
        public float Friction = .65f;
        public  float Bounciness = .90f;// 1 = 100 % bounciness (never stops bouncing).0 = No bounce
        public PointF Velocity;
        public bool CanCollideSoft;
        public bool CanMove;
        public bool CanAnimate;
        public bool Dead;
        private bool HasGravity;
        public SpriteType myType; 

        public enum SpriteType { Player  ,  CollideMovementGravity , CollideMovement ,CollideOnly , ViewOnly , Enemy , Circle }

        public Sprite(Bitmap b, float x , float y , int width , int height , SpriteType Type)
        {

            Texture = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(Texture);
            g.DrawImage(b,0,0,width,height);
            g.Dispose();

            X = x; Y = y;
            Width = width;
            Height = height;
            myType = Type;
             
            switch(Type)
            {
                case SpriteType.Player:
                    CanCollideSoft = true;
                    CanMove = true;
                    HasGravity = true;
                    Bounciness = 0.0f;
                    break;
                case SpriteType.CollideMovement:
                    CanCollideSoft = true;
                    CanMove = true;
                    Bounciness = 0.0f;
                    break;
                case SpriteType.CollideMovementGravity:
                    CanCollideSoft = true;
                    CanMove = true;
                    HasGravity = true;
                    Bounciness = .0f;
                    break;
                case SpriteType.CollideOnly:
                    CanCollideSoft = false;
                    break;
                case SpriteType.ViewOnly:
                    break;
                case SpriteType.Circle:
                    CanMove = true;
                    HasGravity = true;
                    CanCollideSoft = true;
                    Bounciness = .80f;
                    break;
            }
        }

        public Sprite(Animation a, float x, float y, int width, int height, SpriteType Type)
        {
            MainAnimation = a;
            Texture = new Bitmap(width, height);
            Texture = a.CurrentFrame;

            X = x; Y = y;
            Width = width;
            Height = height;
            myType = Type;

            switch (Type)
            {
                case SpriteType.Player:
                    Texture = a.Frames[3];
                    CanCollideSoft = true;
                    CanMove = true;
                    HasGravity = true;
                    Bounciness = 0.0f;
                    break;
                case SpriteType.CollideMovement:
                    CanCollideSoft = true;
                    CanMove = true;
                    Bounciness = 0.0f;
                    break;
                case SpriteType.CollideMovementGravity:
                    CanCollideSoft = true;
                    CanMove = true;
                    HasGravity = true;
                    Bounciness = .0f;
                    break;
                case SpriteType.CollideOnly:
                    CanCollideSoft = false;
                    break;
                case SpriteType.ViewOnly:
                    break;
                case SpriteType.Circle:
                    CanMove = true;
                    HasGravity = true;
                    CanCollideSoft = true;
                    Bounciness = .80f;
                    break;
            }
        }

        public void Update(InputManager Im)
        {
            if (HasGravity)
                Velocity.Y += Gravity;

            if (CanMove)
            {
                this.X += Velocity.X * Im.DeltaTime;
                this.Y += Velocity.Y * Im.DeltaTime;
            }

            if (this.X + this.Width > Im.ClientSize.Width)
                this.X = Im.ClientSize.Width - this.Width;
            if (this.X < 0 - this.Width)
                this.Dead = true;

            if (this.Y + this.Height > Im.ClientSize.Height)
            {
                this.Y = Im.ClientSize.Height - this.Height;
                this.Velocity.Y *= -Bounciness;
            }
            if (this.Y < 0)
            {
                this.Y = 0;
                this.Velocity.Y = 0;
            }

            if (Math.Abs(Velocity.Y) < 2)
                Velocity.Y = 0;

            switch (myType)
            {
                case SpriteType.Player:
                    if (Im.Clicked)
                    {
                        Velocity.Y -= 200;
                        CanAnimate = true;
                    }
                    if (MainAnimation.Ended == true)
                    {
                        CanAnimate = false;
                        MainAnimation.Reset();
                        MainAnimation.CurrentFrameNumber = 3;
                        MainAnimation.CurrentFrame = MainAnimation.Frames[3];
                        Texture = MainAnimation.CurrentFrame;
                    }
                    else
                    {
                        if(CanAnimate)
                        MainAnimation.Update(Im);
                        Texture = MainAnimation.CurrentFrame;
                    }
                    
                    break;
            }

            /*if (Math.Abs(Velocity.X) < 2)
                Velocity.X = 0;*/

            CollisionCheck(Im);


        }

        public void Draw(SpriteBatch sb)
        {
            sb.DrawImage(Texture, this.ToRec);
            sb.DrawRectangle(new Pen(Brushes.Red, 2), this.ToRec);
        }

       

        private void CollisionCheck(InputManager Im)
        {
            foreach (Sprite s in Im.InGameSprites)
            {
                if (this.ToRec.IntersectsWith(s.ToRec))
                {
                    switch (myType)
                    {
                        case SpriteType.Player:
                            switch (s.myType)
                            {
                                case SpriteType.Enemy:
                                    if(this.Top.IntersectsWith(s.Bottom))
                                    {
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                    }
                                    else if(this.Right.IntersectsWith(s.Left))
                                    {
                                    }

                                    break;
                                case SpriteType.CollideOnly:
                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                        this.Y = s.Y + s.Height;
                                        Velocity.Y /= 2;
                                        Velocity.Y = -Velocity.Y;
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {
                                        this.Y = s.Y - this.Height;
                                        Velocity.Y *= -Bounciness;
                                        Velocity.X *= Friction;
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;
                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }
                                    break;
                                case SpriteType.CollideMovementGravity:
                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                        this.Y = s.Y + s.Height;
                                        Velocity.Y /= 2;
                                        Velocity.Y = -Velocity.Y;
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {
                                        this.Y = s.Y - this.Height;
                                        Velocity.Y *= -Bounciness;
                                        Velocity.X *= Friction;
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;

                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }
                                    break;
                                case SpriteType.CollideMovement:
                                    
                                    if (IntersectPixels(this.ToRec,GetColorData(this.Texture),s.ToRec,GetColorData(s.Texture)) == true)
                                    {
                                        Im.HasLost = true;
                                    }
                                     
                                    break;
                              
                            }
                            break;
                      
                        case SpriteType.CollideMovementGravity:
                            switch (s.myType)
                            {
                                case SpriteType.Enemy:
                                    break;
                                case SpriteType.Player:
                                    this.Velocity.X *= s.Velocity.X;
                                   
                                    break;
                                case SpriteType.CollideOnly:
                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                        this.Y = s.Y + s.Height;
                                        Velocity.Y /= 2;
                                        Velocity.Y = -Velocity.Y;
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {
                                        this.Y = s.Y - this.Height;
                                        Velocity.Y *= -Bounciness;
                                        Velocity.X *= Friction;
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;

                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }
                                    break;
                                case SpriteType.CollideMovementGravity:
                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                        this.Y = s.Y + s.Height;
                                        Velocity.Y /= 2;
                                        Velocity.Y = -Velocity.Y;
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {
                                        this.Y = s.Y - this.Height;
                                        Velocity.Y *= -Bounciness;
                                        Velocity.X *= Friction;
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;

                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }
                                    break;
                                case SpriteType.CollideMovement:
                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                        this.Y = s.Y + s.Height;
                                        Velocity.Y /= 2;
                                        Velocity.Y = -Velocity.Y;
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {
                                        this.Y = s.Y - this.Height;
                                        Velocity.Y *= -Bounciness;
                                        Velocity.X *= Friction;
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;

                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }
                                    break;
                            }
                            break;
                        case SpriteType.Enemy:
                            switch (s.myType)
                            {
                                case SpriteType.Enemy:
                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    { 
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;
                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }
                                    break;
                                case SpriteType.CollideMovement:

                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                        this.Y = s.Y + s.Height;
                                        this.Velocity.Y *= -1;
                                        this.Velocity.Y /= 2;
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {
                                        this.Y = s.Y - this.Height;
                                        Velocity.X *= Friction;
                                        Velocity.Y *= -Bounciness;
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;
                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }

                                    break;
                                case SpriteType.CollideMovementGravity:
                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                        this.Y = s.Y + s.Height;
                                        this.Velocity.Y *= -1;
                                        this.Velocity.Y /= 2;
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {
                                        this.Y = s.Y - this.Height;
                                        Velocity.X *= Friction;
                                        Velocity.Y *= -Bounciness;
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;
                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }
                                    break;
                                case SpriteType.CollideOnly:
                                    if (this.Top.IntersectsWith(s.Bottom))
                                    {
                                        this.Y = s.Y + s.Height;
                                        this.Velocity.Y *= -1;
                                        this.Velocity.Y /= 2;
                                    }
                                    else if (this.Bottom.IntersectsWith(s.Top))
                                    {  
                                        this.Y = s.Y - this.Height;
                                        Velocity.X *= Friction;
                                        Velocity.Y *= -Bounciness;
                                    }
                                    else if (this.Left.IntersectsWith(s.Right))
                                    {
                                        this.X = s.X + s.Width;
                                        this.Velocity.X = 0;
                                    }
                                    else if (this.Right.IntersectsWith(s.Left))
                                    {
                                        this.X = s.X - this.Height;
                                        this.Velocity.X = 0;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }


        private Color[] GetColorData(Bitmap image)
        {
            List<Color>PixelColors = new List<Color>();
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color a = Color.FromArgb(image.GetPixel(x, y).R, image.GetPixel(x, y).G, image.GetPixel(x, y).B);
                    PixelColors.Add(a);
                }
            }
            return PixelColors.ToArray();
        }

        private bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorA = dataA[(x - rectangleA.Left) +
                                (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                (y - rectangleB.Top) * rectangleB.Width];

                    if (colorA.R != 0 | colorA.G != 0 | colorA.B != 0)
                    {
                        if (colorB.R != 0 | colorB.G != 0 | colorB.B != 0)
                        {
                            return true;
                        }
                        
                    }
                }
            }
            return false;
        } 

        public Rectangle ToRec
        {
            get { return new Rectangle((int)X, (int)Y, Width, Height); }
        }

        public Rectangle Top
        {
            get { return new Rectangle((int)X,(int)Y,Width,Height / 4); }
        }

        public Rectangle Bottom
        {
            get { return new Rectangle((int)X,(int)Y + Height / 2 + Height / 4, Width,Height / 4); }
        }

        public Rectangle Left
        {
            get { return new Rectangle((int)X,(int)Y,Width / 4, Height); }
        }

        public Rectangle Right
        {
            get { return new Rectangle((int)X + Width / 2 + Width / 4,(int)Y,Width / 4,Height); }
        }



    }
}
