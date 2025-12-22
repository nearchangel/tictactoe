using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace UltimateTicTacToe
{
    public class UltimateTicTacToeGame
    {
        const int SMALL_CELL = 30;
        const int BIG_SQUARE = SMALL_CELL * 3; 
        const int BOARD_SIZE = BIG_SQUARE * 3; 
        const int OFFSET_X = 80;
        const int OFFSET_Y = 60;

        char[,] smallBoard = new char[9, 9];
        char[,] bigBoard = new char[3, 3];

        bool isXTurn = true;
        public string StatusMessage { get; private set; } = "go X";
        public bool BackToMenu { get; private set; } = false;
        int activeSectorX = -1;
        int activeSectorY = -1;

        public bool IsVsAI { get; set; } = false;

        private Button newGameButton;
        private Button menuButton;

        public UltimateTicTacToeGame()
        {
            const int screenWidth = 500;
            const int buttonWidth = 130;
            const int buttonHeight = 40;
            const int buttonY = 440; 

          
            int leftButtonX = screenWidth / 2 - buttonWidth - 10;  
            int rightButtonX = screenWidth / 2 + 10;               

            newGameButton = new Button(leftButtonX, buttonY, buttonWidth, buttonHeight, "New Game");
            menuButton = new Button(rightButtonX, buttonY, buttonWidth, buttonHeight, "Menu");
        }

        public void Reset()
        {
            for (int y = 0; y < 9; y++)
                for (int x = 0; x < 9; x++)
                    smallBoard[x, y] = ' ';
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    bigBoard[x, y] = ' ';
            isXTurn = true;
            StatusMessage = "go X";
            BackToMenu = false;
            activeSectorX = -1;
            activeSectorY = -1;
        }

        public void Update()
        {
            newGameButton.Update();
            menuButton.Update();

            if (newGameButton.IsClicked())
            {
                Reset();
                return;
            }

            if (menuButton.IsClicked())
            {
                BackToMenu = true;
                return;
            }

            if (IsKeyPressed(KeyboardKey.Escape))
            {
                BackToMenu = true;
                return;
            }

            if (StatusMessage.StartsWith("WINNER") || StatusMessage == "DRAW")
            {
                return;
            }

            if (IsKeyPressed(KeyboardKey.Space))
            {
                Reset();
                return;
            }

            if (isXTurn)
            {
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    MakePlayerMove();
                }
            }
            else if (IsVsAI && !isXTurn)
            {
                MakeAIMove();
            }
            else if (!IsVsAI && IsMouseButtonPressed(MouseButton.Left))
            {
                MakePlayerMove();
            }
        }

        private void MakePlayerMove()
        {
            Vector2 mouse = GetMousePosition();
            int gx = (int)(mouse.X - OFFSET_X);
            int gy = (int)(mouse.Y - OFFSET_Y);

            if (gx < 0 || gy < 0 || gx >= BOARD_SIZE || gy >= BOARD_SIZE) return;

            int smallX = gx / SMALL_CELL;
            int smallY = gy / SMALL_CELL;
            int bigX = smallX / 3;
            int bigY = smallY / 3;

            if (activeSectorX != -1 && (bigX != activeSectorX || bigY != activeSectorY))
                return;

            if (bigBoard[bigX, bigY] != ' ' || smallBoard[smallX, smallY] != ' ')
                return;

            PlaceMove(smallX, smallY);
        }

        private void MakeAIMove()
        {
            List<(int x, int y)> availableMoves = new List<(int, int)>();

            if (activeSectorX != -1 && activeSectorY != -1)
            {
                for (int dx = 0; dx < 3; dx++)
                {
                    for (int dy = 0; dy < 3; dy++)
                    {
                        int sx = activeSectorX * 3 + dx;
                        int sy = activeSectorY * 3 + dy;
                        if (smallBoard[sx, sy] == ' ' && bigBoard[activeSectorX, activeSectorY] == ' ')
                        {
                            availableMoves.Add((sx, sy));
                        }
                    }
                }
            }
            else
            {
                for (int bx = 0; bx < 3; bx++)
                {
                    for (int by = 0; by < 3; by++)
                    {
                        if (bigBoard[bx, by] == ' ')
                        {
                            for (int dx = 0; dx < 3; dx++)
                            {
                                for (int dy = 0; dy < 3; dy++)
                                {
                                    int sx = bx * 3 + dx;
                                    int sy = by * 3 + dy;
                                    if (smallBoard[sx, sy] == ' ')
                                    {
                                        availableMoves.Add((sx, sy));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (availableMoves.Count > 0)
            {
                var random = new Random();
                var move = availableMoves[random.Next(availableMoves.Count)];
                PlaceMove(move.x, move.y);
            }
        }

        private void PlaceMove(int smallX, int smallY)
        {
            smallBoard[smallX, smallY] = isXTurn ? 'X' : 'O';

            int bigX = smallX / 3;
            int bigY = smallY / 3;

            if (CheckWinInSector(bigX, bigY))
            {
                bigBoard[bigX, bigY] = isXTurn ? 'X' : 'O';
            }

            int nextSectorX = smallX % 3;
            int nextSectorY = smallY % 3;

            if (bigBoard[nextSectorX, nextSectorY] != ' ' || IsSectorFull(nextSectorX, nextSectorY))
            {
                activeSectorX = -1;
                activeSectorY = -1;
            }
            else
            {
                activeSectorX = nextSectorX;
                activeSectorY = nextSectorY;
            }

            if (CheckBigBoardWin(out char winner))
            {
                StatusMessage = $"WINNER: {winner}";
                activeSectorX = -1;
                activeSectorY = -1;
            }
            else if (IsEntireBoardFull())
            {
                StatusMessage = "DRAW";
                activeSectorX = -1;
                activeSectorY = -1;
            }
            else
            {
                isXTurn = !isXTurn;
                StatusMessage = isXTurn ? "go X" : "go O";
            }
        }

        public void Draw()
        {
            BeginDrawing();
            ClearBackground(new Color(255, 182, 193, 255));

            for (int i = 0; i <= 9; i++)
            {
                DrawLine(OFFSET_X + i * SMALL_CELL, OFFSET_Y,
                         OFFSET_X + i * SMALL_CELL, OFFSET_Y + BOARD_SIZE,
                         Color.Black);
                DrawLine(OFFSET_X, OFFSET_Y + i * SMALL_CELL,
                         OFFSET_X + BOARD_SIZE, OFFSET_Y + i * SMALL_CELL,
                         Color.Black);
            }

            const float MAIN_THICK = 4.0f;
            for (int i = 1; i < 3; i++)
            {
                DrawLineEx(new Vector2(OFFSET_X + i * BIG_SQUARE, OFFSET_Y),
                           new Vector2(OFFSET_X + i * BIG_SQUARE, OFFSET_Y + BOARD_SIZE),
                           MAIN_THICK, Color.Black);
                DrawLineEx(new Vector2(OFFSET_X, OFFSET_Y + i * BIG_SQUARE),
                           new Vector2(OFFSET_X + BOARD_SIZE, OFFSET_Y + i * BIG_SQUARE),
                           MAIN_THICK, Color.Black);
            }

            for (int bx = 0; bx < 3; bx++)
            {
                for (int by = 0; by < 3; by++)
                {
                    if (bigBoard[bx, by] != ' ')
                    {
                        int cx = OFFSET_X + bx * BIG_SQUARE + BIG_SQUARE / 2;
                        int cy = OFFSET_Y + by * BIG_SQUARE + BIG_SQUARE / 2;

                        if (bigBoard[bx, by] == 'X')
                        {
                            int len = BIG_SQUARE / 3;
                            DrawLineEx(new Vector2(cx - len, cy - len), new Vector2(cx + len, cy + len), 6.0f, Color.Red);
                            DrawLineEx(new Vector2(cx + len, cy - len), new Vector2(cx - len, cy + len), 6.0f, Color.Red);
                        }
                        else if (bigBoard[bx, by] == 'O')
                        {
                            DrawRing(new Vector2(cx, cy),
                                     BIG_SQUARE / 3 - 3,
                                     BIG_SQUARE / 3 + 3,
                                     0, 360, 40, Color.Blue);
                        }
                    }
                }
            }

            for (int sx = 0; sx < 9; sx++)
            {
                for (int sy = 0; sy < 9; sy++)
                {
                    int bx = sx / 3;
                    int by = sy / 3;
                    if (bigBoard[bx, by] != ' ') continue;

                    if (smallBoard[sx, sy] == 'X')
                    {
                        int cx = OFFSET_X + sx * SMALL_CELL + SMALL_CELL / 2;
                        int cy = OFFSET_Y + sy * SMALL_CELL + SMALL_CELL / 2;
                        int len = SMALL_CELL / 3;
                        DrawLineEx(new Vector2(cx - len, cy - len), new Vector2(cx + len, cy + len), 2.5f, Color.Red);
                        DrawLineEx(new Vector2(cx + len, cy - len), new Vector2(cx - len, cy + len), 2.5f, Color.Red);
                    }
                    else if (smallBoard[sx, sy] == 'O')
                    {
                        int cx = OFFSET_X + sx * SMALL_CELL + SMALL_CELL / 2;
                        int cy = OFFSET_Y + sy * SMALL_CELL + SMALL_CELL / 2;
                        DrawRing(new Vector2(cx, cy),
                                 SMALL_CELL / 3 - 1,
                                 SMALL_CELL / 3 + 1,
                                 0, 360, 20, Color.Blue);
                    }
                }
            }

            DrawText(StatusMessage, 180, 20, 40, Color.DarkPurple);

            newGameButton.Draw();
            menuButton.Draw();

            if (activeSectorX != -1 && activeSectorY != -1)
            {
                Rectangle activeRect = new Rectangle(
                    OFFSET_X + activeSectorX * BIG_SQUARE,
                    OFFSET_Y + activeSectorY * BIG_SQUARE,
                    BIG_SQUARE,
                    BIG_SQUARE
                );
                DrawRectangle(
                    (int)activeRect.X, (int)activeRect.Y,
                    (int)activeRect.Width, (int)activeRect.Height,
                    new Color(128, 0, 128, 80)
                );
                DrawRectangleLinesEx(activeRect, 3, Color.DarkPurple);
            }

            EndDrawing();
        }

        bool CheckWinInSector(int bigX, int bigY)
        {
            char p00 = smallBoard[bigX * 3 + 0, bigY * 3 + 0];
            char p01 = smallBoard[bigX * 3 + 0, bigY * 3 + 1];
            char p02 = smallBoard[bigX * 3 + 0, bigY * 3 + 2];
            char p10 = smallBoard[bigX * 3 + 1, bigY * 3 + 0];
            char p11 = smallBoard[bigX * 3 + 1, bigY * 3 + 1];
            char p12 = smallBoard[bigX * 3 + 1, bigY * 3 + 2];
            char p20 = smallBoard[bigX * 3 + 2, bigY * 3 + 0];
            char p21 = smallBoard[bigX * 3 + 2, bigY * 3 + 1];
            char p22 = smallBoard[bigX * 3 + 2, bigY * 3 + 2];

            char player = ' ';
            if (p00 != ' ' && p00 == p01 && p01 == p02) player = p00;
            else if (p10 != ' ' && p10 == p11 && p11 == p12) player = p10;
            else if (p20 != ' ' && p20 == p21 && p21 == p22) player = p20;
            else if (p00 != ' ' && p00 == p10 && p10 == p20) player = p00;
            else if (p01 != ' ' && p01 == p11 && p11 == p21) player = p01;
            else if (p02 != ' ' && p02 == p12 && p12 == p22) player = p02;
            else if (p00 != ' ' && p00 == p11 && p11 == p22) player = p00;
            else if (p02 != ' ' && p02 == p11 && p11 == p20) player = p02;

            return player != ' ';
        }

        bool CheckBigBoardWin(out char winner)
        {
            winner = ' ';
            for (int y = 0; y < 3; y++)
            {
                if (bigBoard[0, y] != ' ' && bigBoard[0, y] == bigBoard[1, y] && bigBoard[1, y] == bigBoard[2, y])
                {
                    winner = bigBoard[0, y];
                    return true;
                }
            }
            for (int x = 0; x < 3; x++)
            {
                if (bigBoard[x, 0] != ' ' && bigBoard[x, 0] == bigBoard[x, 1] && bigBoard[x, 1] == bigBoard[x, 2])
                {
                    winner = bigBoard[x, 0];
                    return true;
                }
            }
            if (bigBoard[0, 0] != ' ' && bigBoard[0, 0] == bigBoard[1, 1] && bigBoard[1, 1] == bigBoard[2, 2])
            {
                winner = bigBoard[0, 0];
                return true;
            }
            if (bigBoard[2, 0] != ' ' && bigBoard[2, 0] == bigBoard[1, 1] && bigBoard[1, 1] == bigBoard[0, 2])
            {
                winner = bigBoard[2, 0];
                return true;
            }
            return false;
        }

        bool IsSectorFull(int bigX, int bigY)
        {
            for (int dy = 0; dy < 3; dy++)
            {
                for (int dx = 0; dx < 3; dx++)
                {
                    int sx = bigX * 3 + dx;
                    int sy = bigY * 3 + dy;
                    if (smallBoard[sx, sy] == ' ')
                        return false;
                }
            }
            return true;
        }

        bool IsEntireBoardFull()
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (bigBoard[x, y] == ' ' && !IsSectorFull(x, y))
                        return false;
                }
            }
            return true;
        }
    }
}
