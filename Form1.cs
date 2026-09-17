using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pongspiel
{
    public partial class Form1 : Form
    {
        private Panel pnlStartMenu;
        private Panel pnlGame;
        private Panel paddleLeft, paddleRight, ball;
        private Label scorelbl;
        private Button pausebtn, resumebtn, exitbtn;
        private bool moveUpLeft, moveDownLeft, moveUpRight, moveDownRight;
        private int paddleSpeed = 10;
        private float ballSpeedX = 6, ballSpeedY = 6;
        private int scoreLeft = 0, scoreRight = 0;
        private bool isPaused = false;
        private const int maxScore = 10;
        private System.Windows.Forms.Timer gameTimer;
        private string player1Name;
        private string player2Name;
        private float ballPosX, ballPosY;
        private Panel pnlGameOver;

        public Form1()
        {
            this.Size = new Size(800, 500);
            this.Text = "Pong Game";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateStartMenu();
            CreateGamePanel();
            CreateGameOverPanel();



            this.Controls.Add(pnlStartMenu);
            this.Controls.Add(pnlGame);

            pnlGame.Visible = false;

            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += UpdateGame;
        }

        // Startmenü mit Buttons
        private void CreateStartMenu()
        {
            pnlStartMenu = new Panel()
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.Black
            };

            Label title = new Label()
            {
                Text = "PONG",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 100,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button startbtn = new Button()
            {
                Text = "Start Game",
                Size = new Size(200, 50),
                Location = new Point((ClientSize.Width - 200) / 2, 150),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            startbtn.FlatAppearance.BorderColor = Color.White;
            startbtn.FlatAppearance.BorderSize = 2;
            startbtn.Click += (s, e) =>
            {
                player1Name = Microsoft.VisualBasic.Interaction.InputBox("Enter name for Player 1:", "Player Name");
                player2Name = Microsoft.VisualBasic.Interaction.InputBox("Enter name for Player 2:", "Player Name");

                if (string.IsNullOrWhiteSpace(player1Name) || string.IsNullOrWhiteSpace(player2Name))
                {
                    MessageBox.Show("Please enter valid names for both players.", "Input Required");
                    return;
                }

                pnlStartMenu.Visible = false;
                pnlGame.Visible = true;
                ResetGame();
                gameTimer.Start();
                this.Focus();
            };

            Button instructionsbtn = new Button()
            {
                Text = "Instructions",
                Size = new Size(200, 50),
                Location = new Point((ClientSize.Width - 200) / 2, 220),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            instructionsbtn.FlatAppearance.BorderColor = Color.White;
            instructionsbtn.FlatAppearance.BorderSize = 2;
            instructionsbtn.Click += (s, e) =>
            {
                Form instructionsForm = new Form()
                {
                    Text = "Instructions",
                    Size = new Size(450, 450),
                    BackColor = Color.White,
                    StartPosition = FormStartPosition.CenterParent
                };

                Label Anleitunglbl = new Label()
                {
                    Text = "Anleitung:\n\n" +
                    "Klicke auf \"Start\". Zuerst gibt Spieler 1 seinen Namen ein, danach Spieler 2. " +
                    "Das Spiel beginnt sofort – der Ball wird in eine zufällige Richtung starten.\n\n" +
                    "Der rote Paddel (links) bewegt man mit Knöpfen,"+ 
                    "'W' - der Paddel geht nach oben und 'S' - der Paddel geht nach unten,"+
                    "der blaue Paddel (rechts) steuert man mit,"+
                    "Pfeile nach oben - der Paddel geht nach oben und Pfeile nach unten - der Paddel geht nach unten," +
                    "Beide Spieler steuern jeweils ein Paddel und müssen verhindern, " +
                    "dass der Ball die Wand hinter ihrem Paddel berührt. " +
                    "Gelingt dies nicht, erhält der Gegenspieler einen Punkt.\n\n" +
                    "Das Spiel endet, sobald ein Spieler 10 Punkte erreicht – dieser gilt dann als Sieger.\n\n" +
                    "Nach jedem erzielten Punkt wird die Ballgeschwindigkeit erhöht, wodurch das Spiel zunehmend herausfordernder wird.\n\n" +
                    "Viel Spass beim Spielen",
                    Location = new Point(20, 20),
                    Size = new Size(400, 400),
                    Font = new Font("Arial", 10),
                    AutoSize = false,
                    TextAlign = ContentAlignment.TopLeft
                };

                instructionsForm.Controls.Add(Anleitunglbl);
                instructionsForm.ShowDialog();
            };

            Button exitMenubtn = new Button()
            {
                Text = "Exit",
                Size = new Size(200, 50),
                Location = new Point((ClientSize.Width - 200) / 2, 290),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            exitMenubtn.FlatAppearance.BorderColor = Color.White;
            exitMenubtn.FlatAppearance.BorderSize = 2;
            exitMenubtn.Click += (s, e) => Application.Exit();

            pnlStartMenu.Controls.Add(title);
            pnlStartMenu.Controls.Add(startbtn);
            pnlStartMenu.Controls.Add(instructionsbtn);
            pnlStartMenu.Controls.Add(exitMenubtn);
        }

        // Mittellinie zeichnen
        private void DrawCenterLine(Graphics g)
        {
            Pen pen = new Pen(Color.Gray, 2);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            int centerX = pnlGame.Width / 2;

            g.DrawLine(pen, centerX, 0, centerX, pnlGame.Height);
        }
        
        // Spielfeld anzeigen
        private void CreateGamePanel()
        {
            pnlGame = new Panel()
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.Black
            };

            paddleLeft = new Panel() 
            { Size = new Size(10, 100), Location = new Point(20, 200), BackColor = Color.Red };
            paddleRight = new Panel() 
            { Size = new Size(10, 100), Location = new Point(760, 200), BackColor = Color.Blue };
            ball = new Panel() 
            { Size = new Size(10, 10), Location = new Point(395, 245), BackColor = Color.White };

            scorelbl = new Label()
            {
                Text = "0 : 0",
                ForeColor = Color.White,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Location = new Point(280, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            pausebtn = new Button()
            {
                Text = "Pause",
                Size = new Size(80, 30),
                Location = new Point(10, 10),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            pausebtn.FlatAppearance.BorderColor = Color.White;
            pausebtn.FlatAppearance.BorderSize = 1;
            pausebtn.Click += (s, e) =>
            {
                isPaused = true;
                gameTimer.Stop();
                resumebtn.Visible = true;
                exitbtn.Visible = true;
            };

            resumebtn = new Button()
            {
                Text = "Resume",
                Size = new Size(80, 30),
                Location = new Point(100, 10),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            resumebtn.FlatAppearance.BorderColor = Color.White;
            resumebtn.FlatAppearance.BorderSize = 1;
            resumebtn.Click += (s, e) =>
            {
                isPaused = false;
                this.ActiveControl = null;
                gameTimer.Start();
                resumebtn.Visible = false;
                exitbtn.Visible = false;                
            };

            pnlGame.Paint += (s, e) =>
            {
                DrawCenterLine(e.Graphics);
            };

            exitbtn = new Button()
            {
                Text = "Exit",
                Size = new Size(80, 30),
                Location = new Point(190, 10),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            exitbtn.FlatAppearance.BorderColor = Color.White;
            exitbtn.FlatAppearance.BorderSize = 1;
            exitbtn.Click += (s, e) => Application.Exit();

            pnlGame.Controls.Add(paddleLeft);
            pnlGame.Controls.Add(paddleRight);
            pnlGame.Controls.Add(ball);
            pnlGame.Controls.Add(scorelbl);
            pnlGame.Controls.Add(pausebtn);
            pnlGame.Controls.Add(resumebtn);
            pnlGame.Controls.Add(exitbtn);
        }
        // Spiel zurücksetzen
        private void ResetGame()
        {
            scoreLeft = 0;
            scoreRight = 0;
            UpdateScore();
            ResetBall();
        }
        // Ball und Paddles bewegen
        private void UpdateGame(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                MovePaddles();
                MoveBall();
            }
            pnlGame.Invalidate();
        }
        // Paddles steuern
        private void MovePaddles()
        {
            if (moveUpLeft && paddleLeft.Top > 0) paddleLeft.Top -= paddleSpeed;
            if (moveDownLeft && paddleLeft.Bottom < pnlGame.ClientSize.Height) paddleLeft.Top += paddleSpeed;
            if (moveUpRight && paddleRight.Top > 0) paddleRight.Top -= paddleSpeed;
            if (moveDownRight && paddleRight.Bottom < pnlGame.ClientSize.Height) paddleRight.Top += paddleSpeed;
        }
        // Ball bewegen, Kollision prüfen
        private void MoveBall()
        {
            ballPosX += ballSpeedX;
            ballPosY += ballSpeedY;
            ball.Location = new Point((int)ballPosX, (int)ballPosY);

            if (ball.Top <= 0 || ball.Bottom >= pnlGame.ClientSize.Height)
            {
                ballSpeedY = -ballSpeedY;
                IncreaseSpeed();
            }

            if (ball.Left <= paddleLeft.Right && ball.Bottom > paddleLeft.Top && ball.Top < paddleLeft.Bottom)
            {
                ballSpeedX = Math.Abs(ballSpeedX);
                IncreaseSpeed();
            }

            if (ball.Right >= paddleRight.Left && ball.Bottom > paddleRight.Top && ball.Top < paddleRight.Bottom)
            {
                ballSpeedX = -Math.Abs(ballSpeedX);
                IncreaseSpeed();
            }

            if (ball.Left <= 0)
            {
                scoreRight++;
                UpdateScore();
                CheckGameOver();
            }

            if (ball.Right >= pnlGame.ClientSize.Width)
            {
                scoreLeft++;
                UpdateScore();
                CheckGameOver();
            }
        }
        // Ball schneller machen
        private void IncreaseSpeed()
        {
            if (Math.Abs(ballSpeedX) < 20)
                ballSpeedX += ballSpeedX > 0 ? 0.5f : -0.5f;

            if (Math.Abs(ballSpeedY) < 20)
                ballSpeedY += ballSpeedY > 0 ? 0.5f : -0.5f;
        }

        // Sieg prüfen
        private void CheckGameOver()
        {
            if (scoreLeft >= maxScore)
                EndGame($"{player1Name} gewinnt");
            else if (scoreRight >= maxScore)
                EndGame($"{player2Name} gewinnt");
            else
                ResetBall();
        }
        // Spielende anzeigen
        private void EndGame(string message)
        {
            gameTimer.Stop();
            pnlGameOver.Visible = true;

            foreach (Control ctrl in pnlGameOver.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.Text = message;
                }
            }
        }
        // Ende-Menü anzeigen
        private void CreateGameOverPanel()
        {
            pnlGameOver = new Panel()
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(200, 0, 0, 0), 
                Visible = false
            };

            Label gameOverLabel = new Label()
            {
                Text = "Spiel beendet",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(300, 50),
                Location = new Point((ClientSize.Width - 300) / 2, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button restartButton = new Button()
            {
                Text = "Restart Game",
                Size = new Size(200, 50),
                Location = new Point((ClientSize.Width - 200) / 2, 180),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            restartButton.FlatAppearance.BorderColor = Color.White;
            restartButton.FlatAppearance.BorderSize = 2;
            restartButton.Click += (s, e) =>
            {
                pnlGameOver.Visible = false;
                ResetGame();
                gameTimer.Start();
                this.Focus();
            };

            Button backToMenuButton = new Button()
            {
                Text = "Main Menu",
                Size = new Size(200, 50),
                Location = new Point((ClientSize.Width - 200) / 2, 250),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            backToMenuButton.FlatAppearance.BorderColor = Color.White;
            backToMenuButton.FlatAppearance.BorderSize = 2;
            backToMenuButton.Click += (s, e) =>
            {
                pnlGameOver.Visible = false;
                pnlGame.Visible = false;
                pnlStartMenu.Visible = true;
            };

            Button exitButton = new Button()
            {
                Text = "Exit Game",
                Size = new Size(200, 50),
                Location = new Point((ClientSize.Width - 200) / 2, 320),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            exitButton.FlatAppearance.BorderColor = Color.White;
            exitButton.FlatAppearance.BorderSize = 2;
            exitButton.Click += (s, e) => Application.Exit();

            pnlGameOver.Controls.Add(gameOverLabel);
            pnlGameOver.Controls.Add(restartButton);
            pnlGameOver.Controls.Add(backToMenuButton); 
            pnlGameOver.Controls.Add(exitButton);       

            pnlGame.Controls.Add(pnlGameOver);
        }

        // Ball zurücksetzen
        private void ResetBall()
        {
            ballPosX = (pnlGame.Width - ball.Width) / 2;
            ballPosY = (pnlGame.Height - ball.Height) / 2;
            ball.Location = new Point((int)ballPosX, (int)ballPosY);

            Random rand = new Random();
            ballSpeedX = rand.Next(0, 2) == 0 ? 6 : -6;
            ballSpeedY = rand.Next(0, 2) == 0 ? 6 : -6;
        }

        // Punktestand aktualisieren
        private void UpdateScore()
        {
            scorelbl.Text = $"{player1Name} {scoreLeft} : {scoreRight} {player2Name}";
        }

        // Taste gedrückt
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) moveUpLeft = true;
            if (e.KeyCode == Keys.S) moveDownLeft = true;
            if (e.KeyCode == Keys.Up) moveUpRight = true;
            if (e.KeyCode == Keys.Down) moveDownRight = true;
        }
        // Taste losgelassen
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) moveUpLeft = false;
            if (e.KeyCode == Keys.S) moveDownLeft = false;
            if (e.KeyCode == Keys.Up) moveUpRight = false;
            if (e.KeyCode == Keys.Down) moveDownRight = false;
        }
    }
}

