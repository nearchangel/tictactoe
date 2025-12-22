using Raylib_cs;
using static Raylib_cs.Raylib;

namespace UltimateTicTacToe
{
    public class Program
    {
        public static void Main()
        {
            const int screenWidth = 500;
            const int screenHeight = 500;

            InitWindow(screenWidth, screenHeight, "Ultimate Tic-Tac-Toe");
            SetTargetFPS(60);

            var mainMenu = new MainMenu(screenWidth, screenHeight);
            var game = new UltimateTicTacToeGame();
            GameOverScreen? gameOverScreen = null;

            while (!WindowShouldClose())
            {
                if (gameOverScreen != null)
                {
                    gameOverScreen.Update();
                    BeginDrawing();
                    gameOverScreen.Draw();
                    EndDrawing();

                    if (gameOverScreen.ReturnToMenu)
                    {
                        gameOverScreen = null;
                        mainMenu = new MainMenu(screenWidth, screenHeight);
                    }
                    continue;
                }

                if (!mainMenu.PlayClicked && !mainMenu.PlayVsAIClicked && !mainMenu.ExitClicked)
                {
                    mainMenu.Update();
                    BeginDrawing();
                    mainMenu.Draw();
                    EndDrawing();

                    if (mainMenu.ExitClicked) break;

                    if (mainMenu.PlayClicked)
                    {
                        game.Reset();
                        game.IsVsAI = false;
                    }
                    else if (mainMenu.PlayVsAIClicked)
                    {
                        game.Reset();
                        game.IsVsAI = true;
                    }
                }
                else
                {
                    game.Update();
                    game.Draw();

                    if (game.StatusMessage == "DRAW")
                    {
                        gameOverScreen = new GameOverScreen("DRAW", screenWidth, screenHeight);
                    }
                    else if (game.StatusMessage.StartsWith("WINNER: "))
                    {
                        string winner = game.StatusMessage.Replace("WINNER: ", "").Trim();
                        if (winner == "X" || winner == "O")
                        {
                            gameOverScreen = new GameOverScreen(winner, screenWidth, screenHeight);
                        }
                    }

                    if (game.BackToMenu)
                    {
                        mainMenu = new MainMenu(screenWidth, screenHeight);
                    }
                }
            }

            CloseWindow();
        }
    }
}
