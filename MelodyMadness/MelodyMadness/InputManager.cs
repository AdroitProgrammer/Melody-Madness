using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using NAudio;
using NAudio.CoreAudioApi;


namespace MelodyMadness
{
    class InputManager
    {
        public Size ClientSize;
        public bool Clicked;
        public Point MousePoint;
        public int Fps;
        public bool HasLost;
        public Stopwatch Gametime;
        public float DeltaTime;
        public List<Sprite> InGameSprites = new List<Sprite>();
        public Keys[] KeysPressed;
        public Keys[] KeysDown;
        public MMDevice[] Devices;
        public MMDevice MainDevice;
        public int MasterPeakLevel;
        public Point Centered
        {
            get { return new Point(ClientSize.Width / 2, ClientSize.Height / 2); }
        }

        public InputManager()
        {
            
        }

        public void Update(Size cs , Point mp , int fps , float delta , Keys[] kp , Keys[] kd,bool clicked, Stopwatch gt)
        {
            ClientSize = new Size(cs.Width - 6, cs.Height - 24); ;
            MousePoint = mp;
            Fps = fps;
            DeltaTime = delta / 1000f;
            KeysPressed = kp;
            KeysDown = kd;
            Clicked = clicked;
            Gametime = gt;
            if (MainDevice == null)
            {
                MainDevice = Devices[0];
            }
        }
    }
}
