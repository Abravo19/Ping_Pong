namespace Ping_Pong
{
    public partial class Form1 : Form
    {
        Rectangle zoneJeu, rj1, rj2, balle;
        int sj1, sj2, vBalle, vr, sensH, sensV;
        bool j1Haut, j1Bas, j2Haut, j2Bas;
        readonly Random rnd = new Random();
        System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;

            DoubleBuffered = true;
            BackColor = Color.Black;
            ClientSize = new Size(800, 600);

            Initialiser();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        int Hasard(int a, int b) => rnd.Next(a, b + 1);

        void Initialiser()
        {
            zoneJeu = new Rectangle(0, 0, 200, 200);
            rj1 = new Rectangle(10, 85, 5, 30);
            rj2 = new Rectangle(185, 85, 5, 30);
            balle = new Rectangle(98, 98, 4, 4);

            sj1 = 0;
            sj2 = 0;
            vBalle = 2;
            vr = 4;
            sensH = Hasard(1, 2);
            sensV = Hasard(1, 2);
        }

        void RemettreEnJeu()
        {
            balle.X = 98;
            balle.Y = 98;
            rj1.Y = 85;
            rj2.Y = 85;
            vBalle = 2;                 
            sensV = Hasard(1, 2);
        }


        void EffetRaquette(Rectangle r)
        {
            int centreBalle = balle.Y + balle.Height / 2;
            int tiers = r.Height / 3;

            if (centreBalle < r.Y + tiers) sensV = 1;
            else if (centreBalle > r.Bottom - tiers) sensV = 2;

            if (vBalle < 5) vBalle++;
        }

        void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z || e.KeyCode == Keys.W) j1Haut = true;
            if (e.KeyCode == Keys.S) j1Bas = true;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.O) j2Haut = true;
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.L) j2Bas = true;

            if (e.KeyCode == Keys.Space && !timer.Enabled)
            {
                Initialiser();
                timer.Start();
            }
            e.Handled = true;
        }

        void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z || e.KeyCode == Keys.W) j1Haut = false;
            if (e.KeyCode == Keys.S) j1Bas = false;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.O) j2Haut = false;
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.L) j2Bas = false;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (j1Haut && rj1.Y > 0) rj1.Y -= vr;
            if (j1Bas && rj1.Bottom < zoneJeu.Height) rj1.Y += vr;
            if (j2Haut && rj2.Y > 0) rj2.Y -= vr;
            if (j2Bas && rj2.Bottom < zoneJeu.Height) rj2.Y += vr;

            balle.X += (sensH == 1) ? -vBalle : vBalle;
            balle.Y += (sensV == 1) ? -vBalle : vBalle;

            if (balle.Y <= 0) sensV = 2;
            if (balle.Y >= 196) sensV = 1;

            if (sensH == 1 && balle.IntersectsWith(rj1))
            {
                sensH = 2;
                balle.X = rj1.Right;
                EffetRaquette(rj1);
            }

            if (sensH == 2 && balle.IntersectsWith(rj2))
            {
                sensH = 1;
                balle.X = rj2.X - balle.Width;
                EffetRaquette(rj2);
            }

            if (balle.X <= 0)
            {
                sj2++;
                RemettreEnJeu();
                sensH = 1;
            }

            if (balle.X >= 196)
            {
                sj1++;
                RemettreEnJeu();
                sensH = 2;
            }

            if (sj1 >= 5 || sj2 >= 5)
            {
                timer.Stop();
                Invalidate();
                Update();
                MessageBox.Show((sj1 >= 5 ? "Joueur 1 gagne !" : "Joueur 2 gagne !")
                    + "\nAppuyez sur Espace pour rejouer.");
                return;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float scale = Math.Min(
                (float)ClientSize.Width / zoneJeu.Width,
                (float)ClientSize.Height / zoneJeu.Height);

            float offsetX = (ClientSize.Width - zoneJeu.Width * scale) / 2f;
            float offsetY = (ClientSize.Height - zoneJeu.Height * scale) / 2f;

            e.Graphics.DrawRectangle(Pens.Gray,
                offsetX, offsetY, zoneJeu.Width * scale, zoneJeu.Height * scale);

            using (var pen = new Pen(Color.Gray) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                float milieu = offsetX + zoneJeu.Width * scale / 2f;
                e.Graphics.DrawLine(pen, milieu, offsetY, milieu, offsetY + zoneJeu.Height * scale);
            }

            e.Graphics.FillRectangle(Brushes.White,
                offsetX + rj1.X * scale, offsetY + rj1.Y * scale,
                rj1.Width * scale, rj1.Height * scale);

            e.Graphics.FillRectangle(Brushes.White,
                offsetX + rj2.X * scale, offsetY + rj2.Y * scale,
                rj2.Width * scale, rj2.Height * scale);

            e.Graphics.FillRectangle(Brushes.Yellow,
                offsetX + balle.X * scale, offsetY + balle.Y * scale,
                balle.Width * scale, balle.Height * scale);

            using (var font = new Font("Consolas", 28, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                e.Graphics.DrawString($"{sj1}  -  {sj2}", font, Brushes.White,
                    ClientSize.Width / 2f, offsetY + 10, sf);
            }
        }
    }
}