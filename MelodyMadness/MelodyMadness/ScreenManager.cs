using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Threading;
using NAudio;
using NAudio.CoreAudioApi;

namespace MelodyMadness
{
    class ScreenManager
    {
        public enum GameState { Splash , Menu , Game , Paused , Options }
        private bool ParalaxBackgrounds = true;
        private int ParalaxingSpeed = 2;
        private bool Transitioning;
        private bool CanSpawn = true;
        private bool TimerCompleted;
        private int SpawnedTillLevel = 20;
        private long GameStartTimer;
        private long LastSpawntime;
        private int TimerCounter = 4;
        private int Seperation = 8;
        private int sepcounter = 0;
        int Spawned = 0;
        private GameState PreviousState = GameState.Menu;
        private GameState CurrentState = GameState.Menu;
        private List<MenuItem> MenuItems = new List<MenuItem>();
        private List<MenuItem> OptionsMenuItems = new List<MenuItem>();
        private List<Sprite> BackgroundGameImages = new List<Sprite>();
        private Sprite LastSpawned;

        public void LoadContent(InputManager Im)
        {
            MenuItems.Add( new MenuItem(Properties.Resources.Start,500,100,200,100));
            MenuItems.Add(new MenuItem(Properties.Resources.Options, 500, 250, 200, 100));
            MenuItems.Add(new MenuItem(Properties.Resources.Instructions, 500, 350, 200, 100));
            Animation a = new Animation(Properties.Resources.Bat_flgiht_sheet, 15, 24,100,100);
            Im.InGameSprites.Add(new Sprite(a,500,200,100,100,Sprite.SpriteType.Player));
            BackgroundGameImages.Add(new Sprite(Properties.Resources.Cave2, 0, 0, 1227, 706, Sprite.SpriteType.ViewOnly));
            BackgroundGameImages.Add(new Sprite(Properties.Resources.Cave2, 1227, 0, 1227, 706, Sprite.SpriteType.ViewOnly));
            MMDeviceEnumerator Enumrator = new MMDeviceEnumerator();
            var devices = Enumrator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            string[] DStrings = new string[devices.Count];
            for (int i = 0; i < DStrings.Length; i++)
            {
                DStrings[i] = devices.ToArray()[i].ToString();
                Im.Devices = devices.ToArray();

            }
           
        }

        public void Update(InputManager Im)
        {
           

            switch (CurrentState)
            {
                case GameState.Splash:
                    break;
                case GameState.Menu:

                    foreach (MenuItem m in MenuItems)
                    {
                        m.Update(Im);
                    }

                    for(int i = 0; i < MenuItems.Count;i++)
                    {
                        if (MenuItems[i].Clicked)
                        {
                            switch (i)
                            {
                                case 0:
                                    MenuItems[i].Clicked = false;
                                    CurrentState = GameState.Game;
                                    break;
                                case 1:
                                    MenuItems[i].Clicked = false;
                                    CurrentState = GameState.Options;
                                    break;
                                case 2:
                                    break;
                            }
                        }
                    }

                    break;
                case GameState.Game:
                    if (Im.HasLost == true)
                    {
                        LastSpawned = null;
                        Im.InGameSprites.Clear();
                        Animation a = new Animation(Properties.Resources.Bat_flgiht_sheet, 15, 24, 100, 100);
                        Im.InGameSprites.Add(new Sprite(a, 500, 200, 100, 100, Sprite.SpriteType.Player));
                        LastSpawntime = 0;
                        GameStartTimer = 0;
                        Spawned = 0;
                        sepcounter = 0;
                        Seperation = 8;
                        CanSpawn = true;
                        TimerCompleted = false;
                        TimerCounter = 4;
                        Im.HasLost = false;
                        MenuItems.Clear();
                        MenuItems.Add(new MenuItem(Properties.Resources.Start, 500, 50, 200, 100));
                        MenuItems.Add(new MenuItem(Properties.Resources.Options, 500, 200, 200, 100));
                        MenuItems.Add(new MenuItem(Properties.Resources.Instructions, 500, 350, 200, 100));
                        CurrentState = GameState.Menu;
                    }
                    else
                    {
                        if (TimerCounter == 0)
                        {
                            TimerCompleted = true;
                        }

                        if (!TimerCompleted)
                            if (Im.Gametime.ElapsedMilliseconds - GameStartTimer > 1000)
                            {
                                GameStartTimer = Im.Gametime.ElapsedMilliseconds;
                                TimerCounter--;
                            }

                        if (TimerCompleted)
                        {
                            for (int i = 0; i < BackgroundGameImages.Count; i++)
                            {
                                if (BackgroundGameImages[i].X <= -Im.ClientSize.Width)
                                {
                                    BackgroundGameImages[i].X = Im.ClientSize.Width;
                                }
                                BackgroundGameImages[i].X -= ParalaxingSpeed;
                            }
                            if (LastSpawned != null)
                            {
                                if (LastSpawned.X + LastSpawned.Width * Seperation < Im.ClientSize.Width)
                                {
                                    CanSpawn = true;
                                }
                                /*if (Im.Gametime.ElapsedMilliseconds - LastSpawntime > 1000)
                                {
                                    CanSpawn = true;
                                    LastSpawntime = Im.Gametime.ElapsedMilliseconds;
                                }*/
                            }

                            if (Spawned > sepcounter)
                            {
                                if(Seperation > 2)
                                Seperation--;
                                sepcounter += SpawnedTillLevel  ;
                            }
                            
                            if (CanSpawn == true)
                            {

                                
                                int Height = Im.MasterPeakLevel * 5;
                                if (Height == 0)
                                    Height = 1;

                                if (Height > Im.ClientSize.Height - 200)
                                {
                                    Height = Im.ClientSize.Height - 201;
                                }
                                /*
                                Random r = new Random();
                               
                                if (LastSpawned != null)
                                {
                                    if (r.Next(1,4) < 3)
                                    {
                                        if (LastSpawned.Height >= Im.ClientSize.Height - 201)
                                        {
                                            LastSpawned.Height = Im.ClientSize.Height - 201;
                                            Height = r.Next(LastSpawned.Height - 150, LastSpawned.Height);
                                        }
                                        else if (LastSpawned.Height + 150 < Im.ClientSize.Height - 201)
                                            Height = r.Next(LastSpawned.Height, LastSpawned.Height + 150);
                                        else
                                        {
                                            Height = r.Next(LastSpawned.Height - 150, LastSpawned.Height);
                                        }
                                    }
                                    else
                                    {
                                        if (LastSpawned.Height - 150 <= 1)
                                        {
                                            LastSpawned.Height = 1;
                                            Height = r.Next(LastSpawned.Height, LastSpawned.Height + 150);
                                        }
                                        else
                                        Height = r.Next(LastSpawned.Height - 150, LastSpawned.Height);
                                    }
                                }
                                else
                                {
                                    Height = r.Next(1, Im.ClientSize.Height - 201);// first spawn
                                }*/
                                Sprite sm = new Sprite(Properties.Resources.Stalagmite, Im.ClientSize.Width, Im.ClientSize.Height - Height, 50, (int)Height, Sprite.SpriteType.CollideMovement);
                                Sprite st = new Sprite(Properties.Resources.Stalagtite, Im.ClientSize.Width, 0, 50, (int)sm.Y - 200, Sprite.SpriteType.CollideMovement);
                                st.Velocity.X = -400;
                                sm.Velocity.X = -400;
                                LastSpawned = sm;
                                Im.InGameSprites.Add(sm);
                                Im.InGameSprites.Add(st);
                                Spawned++;
                                CanSpawn = false;
                            }



                            foreach (Sprite s in Im.InGameSprites)
                            {
                                s.Update(Im);
                            }
                            foreach (Sprite ss in Im.InGameSprites)
                            {
                                if (ss.Dead)
                                {
                                    Im.InGameSprites.Remove(ss);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case GameState.Options:
                    foreach (MenuItem item in OptionsMenuItems)
                    {
                        item.Update(Im);
                    }
                    for (int k = 0; k < OptionsMenuItems.Count; k++)
                    {
                        if (OptionsMenuItems[k].Clicked)
                        {
                            OptionsMenuItems[k].Clicked = false;
                            Im.MainDevice = Im.Devices[k];
                            CurrentState = GameState.Menu;
                            //OptionsMenuItems.Clear();
                            break;
                        }
                    }
                    break;
                case GameState.Paused:
                    break;
            }
        }

        public void Transistion()
        {
           /* if (Transitioning == false)
                Transitioning = true;*/
            //transitioning = false;
        }

        public void Draw(InputManager Im, SpriteBatch sb)
        {
            switch (CurrentState)
            {
                case GameState.Splash:
                    break;
                case GameState.Menu:
                    foreach (MenuItem m in MenuItems)
                    {
                        m.Draw(sb);
                    }
                    break;
                case GameState.Game:
                    if (ParalaxBackgrounds == false)
                    {
                        foreach (Sprite b in BackgroundGameImages)
                        {
                            b.Draw(sb);
                        }
                    }
                    else
                    {
                        foreach (Sprite b in BackgroundGameImages)
                        {
                            b.Draw(sb);
                        }
                    }
                    foreach (Sprite s in Im.InGameSprites)
                    {
                        s.Draw(sb);
                    }
                    if (TimerCounter != 0)
                        sb.DrawString(TimerCounter.ToString(),50, Brushes.Yellow, Im.Centered);
                    
                    
                    break;
                case GameState.Options:
                    foreach (MenuItem item in OptionsMenuItems)
                    {
                        item.Draw(sb);
                    }
                    break;
                case GameState.Paused:
                    break;
            }
        }
    }

    class MenuItem
    {
        private Bitmap Texture;
        public String Text;
        public bool OnHovered;
        public bool Clicked;
        public int X, Y, Width, Height;

        public MenuItem(Bitmap b , int x , int y , int width , int height)
        {
            Texture = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(Texture);
            g.DrawImage(b, 0, 0, width, height);
            g.Dispose();
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public MenuItem(string s, int x, int y, int width, int height)
        {
            Text = s;
            Texture = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(Texture);
            g.DrawString(s, new Font("Arial", 20), Brushes.Red, Point.Empty);
            g.Dispose();
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void Update(InputManager Im)
        {
            OnHovered = false;
            if (this.ToRec.Contains(Im.MousePoint))
            {
                if (Im.Clicked == true)
                {
                    Clicked = true;
                }
                OnHovered = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if(Texture != null)
            sb.DrawImage(Texture, ToRec);
           
            if(OnHovered)
            sb.DrawRectangle(new Pen(Brushes.Red,10),ToRec);
        }

        public Rectangle ToRec
        {
            get { return new Rectangle(X, Y, Width, Height); }
        }

    }
}
