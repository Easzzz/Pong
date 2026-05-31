using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pong
{
    public partial class Pong : Form
    {
        private Timer gameTimer = new Timer();
        private float ballX = 390;
        private float ballY = 30;
        private float ballVX;
        private float ballVY;
        private float gameTime = 0;
        private float paddleX = 350;
        private const float paddleWidth = 100;
        private const float paddleHeight = 15;
        private float paddleSpeed = 8;
        private bool leftPressed, rightPressed;
        private bool gameOver;
        private float currentSpeed;
        private Random rng = new Random();
        public Pong()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            gameTimer.Interval = 8;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            double angle = rng.NextDouble() * Math.PI * 0.75 + Math.PI * 0.125;
            ballVX = (float)Math.Cos(angle);
            ballVY = (float)Math.Sin(angle);
        }
        private void GameLoop(object sender, EventArgs e)
        {
            if (gameOver) return;
            gameTime += gameTimer.Interval/1000f;
            // 计算当前速度大小
            float speed;
            if (gameTime < 1f)
                speed = gameTime * gameTime;     // v = t²
            else
                speed = gameTime;                // v = t

            ballX += ballVX * speed;
            ballY += ballVY * speed;

            //if (ballY > 0)
            //{
            //    float targetX = ballX - paddleWidth / 2;
            //    if (paddleX < targetX)
            //        paddleX += Math.Min(paddleSpeed, targetX - paddleX);
            //    else if (paddleX > targetX)
            //        paddleX -= Math.Min(paddleSpeed, paddleX - targetX);
            //}

            //if (ballVY > 0)
            //    paddleX = Math.Max(0, Math.Min(ballX - paddleWidth / 2, ClientSize.Width - paddleWidth));

            if (ballX < 0 || ballX > ClientSize.Width - 20)
                ballVX = -ballVX;

            
            if (ballY + 20 > ClientSize.Height - 30 && ballVY > 0)
            {
                if (ballX + 20 >= paddleX && ballX <= paddleX + paddleWidth)
                    ballVY = -ballVY;
                else
                {
                    gameTimer.Stop();
                    gameOver = true;
                }
            }
            

            if (ballY < 0)
                ballVY = -ballVY;
            if (leftPressed) paddleX -= paddleSpeed;
            if (rightPressed) paddleX += paddleSpeed;
            paddleX = Math.Max(0, Math.Min(paddleX, ClientSize.Width - paddleWidth));

            currentSpeed = speed;
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillEllipse(Brushes.WhiteSmoke, ballX, ballY, 20, 20);
            g.FillRectangle(Brushes.WhiteSmoke, paddleX, ClientSize.Height - 30, paddleWidth, paddleHeight);
            if (gameOver)
            {
                using (var pen = new Pen(Color.LightGoldenrodYellow, 2))
                {
                    g.DrawLine(pen, 0, ClientSize.Height - 30, ClientSize.Width, ClientSize.Height - 30);
                }
                using (var font = new Font("Courier New", 28))
                {
                    var text1 = "Game Over";
                    var text2 = "Press Enter to replay";
                    var text3 = $"Max Speed: {currentSpeed:F1}";
                    var size1 = g.MeasureString(text1, font);
                    var size2 = g.MeasureString(text2, font);
                    var size3 = g.MeasureString(text3, font);
                    g.DrawString(text1, font, Brushes.White,
                        (ClientSize.Width - size1.Width) / 2,
                        (ClientSize.Height - size1.Height)/2 - size3.Height);
                    g.DrawString(text2, font, Brushes.White,
                        (ClientSize.Width - size2.Width) / 2,
                        (ClientSize.Height - size2.Height)/2 + size3.Height);
                    g.DrawString(text3, font, Brushes.White,
                        (ClientSize.Width - size3.Width) / 2,
                        (ClientSize.Height - size3.Height) / 2);
                }
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) leftPressed = true;
            if (e.KeyCode == Keys.Right) rightPressed = true;
            if (e.KeyCode == Keys.Enter && gameOver)
                ResetGame();
        }

        private void ResetGame()
        {
            ballX = 390;
            ballY = 30;
            ballVX = (float)(Math.Cos(rng.NextDouble() * Math.PI * 0.75 + Math.PI * 0.125));
            ballVY = (float)(Math.Sin(rng.NextDouble() * Math.PI * 0.75 + Math.PI * 0.125));
            gameTime = 0;
            gameOver = false;
            gameTimer.Start();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) leftPressed = false;
            if (e.KeyCode == Keys.Right) rightPressed = false;
        }
    }
}
