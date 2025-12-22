using Raylib_cs;
using static Raylib_cs.Raylib;

namespace UltimateTicTacToe
{
    public class GameOverScreen
    {
        public bool ReturnToMenu { get; private set; }
        private string message;
        private Button menuButton;

        public GameOverScreen(string winnerOrDraw, int screenWidth, int screenHeight)
        {
            
            if (winnerOrDraw == "DRAW")
            {
                message = "Draw";
            }
            else
            {
                message = $"Win: {winnerOrDraw}";
            }

           
            menuButton = new Button(
                screenWidth / 2 - 100,
                screenHeight - 80,
                200,
                50,
                "MENU"
            );
        }

        public void Update()
        {
            menuButton.Update();
            if (menuButton.IsClicked())
            {
                ReturnToMenu = true;
            }
        }

        public void Draw()
        {
            
            ClearBackground(new Color(255, 182, 193, 255));

            
            int fontSize = 80;
            int textWidth = MeasureText(message, fontSize);
            int textX = (GetScreenWidth() - textWidth) / 2;
            int textY = GetScreenHeight() / 2 - fontSize / 2;

            DrawText(message, textX, textY, fontSize, Color.DarkPurple);

            
            menuButton.Draw();
        }
    }
}
