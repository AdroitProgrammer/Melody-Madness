using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using NAudio;
using NAudio.CoreAudioApi;

namespace MelodyMadness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        private Stopwatch gameTime = new Stopwatch();
        private List<Keys> keysDown = new List<Keys>();
        private List<Keys> keysPressed = new List<Keys>();
        private ScreenManager screenM = new ScreenManager();
        private InputManager Im = new InputManager();
        private SpriteBatch spriteBatch;
        private Point MousePoint;
        private int Interval = 1000 / 60;
        private int Fps;
        private int fpsCounter;
        private bool Clicked;
        private bool AllowInput;
        private long lastFps;
        private float deltaTime;
        private long last_time;

        private void LoadContent()
        {
            spriteBatch = new SpriteBatch(this.CreateGraphics(), this.Size);
            Thread Game = new Thread(GameLoop);
            
            screenM.LoadContent(Im);
            Game.Start();
        }

        private void GameLoop()
        {
            gameTime.Start();
            while (this.Created)
            {
                deltaTime = gameTime.ElapsedMilliseconds - last_time;
                last_time = gameTime.ElapsedMilliseconds;
                Input();
                Logic();
                Render();
                while (gameTime.ElapsedMilliseconds - last_time < Interval) { }
            }
        }

        private void Input()
        {
            FpsCount();
            AllowInput = false;
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        Invokedstuff();
                    }));
                }
            }
            catch { System.Environment.Exit(0); }
            Im.Update(this.Size, MousePoint, Fps, deltaTime, keysPressed.ToArray(),keysDown.ToArray(),Clicked, gameTime);
            keysPressed.Clear();
            keysDown.Clear();
            Clicked = false;
            AllowInput = true;
        }

        private void Logic()
        {
            screenM.Update(Im);
        }

        private void Render()
        {
            spriteBatch.Begin();
            screenM.Draw(Im, spriteBatch);
            spriteBatch.End();
        }

        private void FpsCount()
        {
            if (gameTime.ElapsedMilliseconds - lastFps > 1000)
            {
                lastFps = gameTime.ElapsedMilliseconds;
                Fps = fpsCounter;
                fpsCounter = 0;
            }
            else
            {
                fpsCounter++;
            }
        }

        private void Invokedstuff()
        {
            MousePoint = this.PointToClient(Cursor.Position);
            this.Text = MousePoint.ToString() + " Fps: " + Fps.ToString();
           
            if (Im.MainDevice != null) 
            {
                Im.MasterPeakLevel = (int)(Im.MainDevice.AudioMeterInformation.MasterPeakValue * 100f) ;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(AllowInput)
            keysPressed.Add(e.KeyCode);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if(AllowInput)
            keysPressed.Add((Keys)e.KeyChar.ToString().ToUpper().ToCharArray()[0]);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (AllowInput)
                Clicked = true;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadContent();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            System.Environment.Exit(0);
        }

    }
}
