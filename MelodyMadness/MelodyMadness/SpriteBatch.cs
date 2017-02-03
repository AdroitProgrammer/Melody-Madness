 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MelodyMadness
{
    class SpriteBatch
    {
        private BufferedGraphics bfrGfx;
        private BufferedGraphicsContext bfxCntxt = BufferedGraphicsManager.Current;
        private Size clientSize;
        private Graphics Gfx;

        public SpriteBatch(Graphics gfx , Size Csize)
        {
            bfxCntxt.MaximumBuffer = new Size(Csize.Width + 1, Csize.Height + 1);
            bfrGfx = bfxCntxt.Allocate(gfx,new Rectangle(Point.Empty,Csize));
            Gfx = gfx;
            clientSize = Csize;
        }

        public void Begin()
        {
            bfrGfx.Graphics.Clear(Color.Black);
        }

        public void DrawImage(Bitmap b , Rectangle rec)
        {
            bfrGfx.Graphics.DrawImageUnscaled(b, rec);
        }

        public void DrawRectangle(Pen p, Rectangle rec)
        {
            bfrGfx.Graphics.DrawRectangle(p, rec);
        }

        public void DrawString(string msg , int size , Brush b , Point p)
        {
            bfrGfx.Graphics.DrawString(msg, new Font("Arial", size), b, new Point(p.X - size , p.Y - size ));
        }

        public void End()
        {
            bfrGfx.Render(Gfx);
        }
    }
}
