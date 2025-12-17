using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace UltimateTicTacToe
{
    class Program
    {
        const int SMALL_CELL = 30;
        const int BIG_SQUARE = SMALL_CELL * 3; // 90
        const int BOARD_SIZE = BIG_SQUARE * 3; // 270
        const int OFFSET_X = 80;
        const int OFFSET_Y = 60;

        static char[,] smallBoard = new char[9, 9]; // 9x9 мелких клеток
        static char[,] bigBoard = new char[3, 3];   // 3x3 — кто выиграл в секторе

        static bool isXTurn = true;
        static string statusMessage = "go X";

        static void Main()
        {
            InitWindow(500, 400, "Tic-Tac-Toe 81");
            SetTargetFPS(60);
            ResetBoard();

            while (!WindowShouldClose())
            {
                HandleInput();
                DrawGame();
            }

            CloseWindow();
        }

        static void ResetBoard()
        {
            for (int y = 0; y < 9; y++)
                for (int x = 0; x < 9; x++)
                    smallBoard[x, y] = ' ';
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    bigBoard[x, y] = ' ';
            isXTurn = true;
            statusMessage = "go X";
        }

        static void HandleInput()
        {
            // Если уже есть победитель — не принимаем ходы
            if (statusMessage.StartsWith("WINNER"))
                return;

            // Перезапуск по нажатию SPACE
            if (IsKeyPressed(KeyboardKey.KeyboardMenu))
            {
                ResetBoard();
                return;
            }

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mouse = GetMousePosition();
                int gx = (int)(mouse.X - OFFSET_X);
                int gy = (int)(mouse.Y - OFFSET_Y);

                if (gx < 0 || gy < 0 || gx >= BOARD_SIZE || gy >= BOARD_SIZE) return;

                int smallX = gx / SMALL_CELL;
                int smallY = gy / SMALL_CELL;
                int bigX = smallX / 3;
                int bigY = smallY / 3;

                // Нельзя ходить, если сектор уже выигран или клетка занята
                if (bigBoard[bigX, bigY] != ' ') return;
                if (smallBoard[smallX, smallY] != ' ') return;

                // Делаем ход
                smallBoard[smallX, smallY] = isXTurn ? 'X' : 'O';

                // Проверяем, выиграл ли кто в этом секторе
                if (CheckWinInSector(bigX, bigY))
                {
                    bigBoard[bigX, bigY] = isXTurn ? 'X' : 'O';

                    // Проверяем, выиграл ли кто-то на всём большом поле
                    if (CheckBigBoardWin(out char gameWinner))
                    {
                        statusMessage = $"WINNER: {gameWinner}";
                        return;
                    }
                }

                isXTurn = !isXTurn;
                statusMessage = isXTurn ? "go X" : "go O";
            }
        }

        // Проверка победы в секторе [bigX, bigY]
        static bool CheckWinInSector(int bigX, int bigY)
        {
            // Извлекаем 3x3 сектор
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
            // Строки
            if (p00 != ' ' && p00 == p01 && p01 == p02) player = p00;
            else if (p10 != ' ' && p10 == p11 && p11 == p12) player = p10;
            else if (p20 != ' ' && p20 == p21 && p21 == p22) player = p20;
            // Столбцы
            else if (p00 != ' ' && p00 == p10 && p10 == p20) player = p00;
            else if (p01 != ' ' && p01 == p11 && p11 == p21) player = p01;
            else if (p02 != ' ' && p02 == p12 && p12 == p22) player = p02;
            // Диагонали
            else if (p00 != ' ' && p00 == p11 && p11 == p22) player = p00;
            else if (p02 != ' ' && p02 == p11 && p11 == p20) player = p02;

            return player != ' ';
        }

        // Проверка победы на большом поле (3x3 bigBoard)
        static bool CheckBigBoardWin(out char winner)
        {
            winner = ' ';

            // Строки
            for (int y = 0; y < 3; y++)
            {
                if (bigBoard[0, y] != ' ' && bigBoard[0, y] == bigBoard[1, y] && bigBoard[1, y] == bigBoard[2, y])
                {
                    winner = bigBoard[0, y];
                    return true;
                }
            }

            // Столбцы
            for (int x = 0; x < 3; x++)
            {
                if (bigBoard[x, 0] != ' ' && bigBoard[x, 0] == bigBoard[x, 1] && bigBoard[x, 1] == bigBoard[x, 2])
                {
                    winner = bigBoard[x, 0];
                    return true;
                }
            }

            // Диагонали
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

        static void DrawGame()
        {
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            // === 1. Тонкая сетка (все 81 клетка) ===
            for (int i = 0; i <= 9; i++)
            {
                DrawLine(OFFSET_X + i * SMALL_CELL, OFFSET_Y,
                         OFFSET_X + i * SMALL_CELL, OFFSET_Y + BOARD_SIZE,
                         Color.LightGray);
                DrawLine(OFFSET_X, OFFSET_Y + i * SMALL_CELL,
                         OFFSET_X + BOARD_SIZE, OFFSET_Y + i * SMALL_CELL,
                         Color.LightGray);
            }

            // === 2. Толстые главные линии (границы 3x3 секторов) ===
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

            // === 3. Отрисовка: СНАЧАЛА большие X/O для выигранных секторов ===
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
                            const float thick = 6.0f;
                            DrawLineEx(new Vector2(cx - len, cy - len), new Vector2(cx + len, cy + len), thick, Color.Red);
                            DrawLineEx(new Vector2(cx + len, cy - len), new Vector2(cx - len, cy + len), thick, Color.Red);
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

            // === 4. Потом — маленькие X/O, но ТОЛЬКО если сектор НЕ выигран ===
            for (int sx = 0; sx < 9; sx++)
            {
                for (int sy = 0; sy < 9; sy++)
                {
                    int bx = sx / 3;
                    int by = sy / 3;
                    if (bigBoard[bx, by] != ' ') continue; // пропускаем выигранные

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

            DrawText(statusMessage, 180, 20, 20, Color.DarkGray);
            DrawText("SPACE: new game", 160, 360, 16, Color.Gray);

            EndDrawing();
        }
    }
}