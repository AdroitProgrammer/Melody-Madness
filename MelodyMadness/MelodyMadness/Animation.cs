using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MelodyMadness
{
    class Animation
    {
        private long lastCall;
        public Bitmap CurrentFrame;
        public int CurrentFrameNumber;
        public Bitmap[] Frames;
        public bool Ended;
        private int FPS;

        public Animation(Bitmap sheet ,int numframes , int fps, int width , int height)
        {
            Frames = new Bitmap[numframes];
            int framewidth = sheet.Width / numframes;
            for (int i = 0; i < numframes; i++)
            {
                Bitmap b = new Bitmap(framewidth,sheet.Height);
                Graphics g = Graphics.FromImage(b);
                g.DrawImage(sheet.Clone(new Rectangle(i * framewidth,0,framewidth,sheet.Height),sheet.PixelFormat),0,0,width,height);
                g.Dispose();
                Frames[i] = b;
            }
            FPS = fps;
            CurrentFrame = Frames[0];
        }


        public void Reset()
        {
            CurrentFrameNumber = 0;
            CurrentFrame = Frames[0];
            Ended = false;
        }

        public void Update(InputManager Im)
        {
            if (Im.Gametime.ElapsedMilliseconds - lastCall > 1000 / FPS )
            {
                lastCall = Im.Gametime.ElapsedMilliseconds;
                if (CurrentFrameNumber >= Frames.Length)
                {
                    Ended = true;
                    CurrentFrameNumber = 0;
                }
                CurrentFrame = Frames[CurrentFrameNumber];
                CurrentFrameNumber++;
            }
        }

    }
}
