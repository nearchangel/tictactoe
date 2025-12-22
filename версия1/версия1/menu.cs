using Raylib_cs;
using static Raylib_cs.Raylib;

namespace UltimateTicTacToe
{
    public class MainMenu
    {
        public bool PlayClicked { get; private set; }      
        public bool PlayVsAIClicked { get; private set; } 
        public bool ExitClicked { get; private set; }

        private Button pvpButton;
        private Button pveButton;
        private Button exitButton;

        public MainMenu(int screenWidth, int screenHeight)
        {
            pvpButton = new Button(screenWidth / 2 - 100, screenHeight / 2 - 80, 200, 50, "PvP");
            pveButton = new Button(screenWidth / 2 - 100, screenHeight / 2 - 20, 200, 50, "PvE (vs AI)");
            exitButton = new Button(screenWidth / 2 - 100, screenHeight / 2 + 40, 200, 50, "EXIT");
        }

        public void Update()
        {
            pvpButton.Update();
            pveButton.Update();
            exitButton.Update();

            PlayClicked = pvpButton.IsClicked();
            PlayVsAIClicked = pveButton.IsClicked();
            ExitClicked = exitButton.IsClicked();
        }

        public void Draw()
        {
            ClearBackground(new Color(255, 182, 193, 255)); // Светло-розовый

            string title = "Ultimate Tic-Tac-Toe";
            int fontSize = 40;
            int titleWidth = MeasureText(title, fontSize);
            int titleX = (500 - titleWidth) / 2;
            int titleY = 80;

            DrawText(title, titleX, titleY, fontSize, Color.DarkPurple);

            pvpButton.Draw();
            pveButton.Draw();
            exitButton.Draw();

            DrawText("Choose game mode", 150, 350, 20, Color.DarkGray);
        }
    }

    public class Button
    {
        public Rectangle Rect;
        public string Text;
        public Color Color;
        public Color HoverColor;
        public bool IsHovered;

        public Button(float x, float y, float width, float height, string text)
        {
            Rect = new Rectangle(x, y, width, height);
            Text = text;
            Color = new Color(255, 105, 180, 255);
            HoverColor = new Color(219, 112, 147, 255);
            IsHovered = false;
        }

        public void Update()
        {
            var mousePos = GetMousePosition();
            IsHovered = CheckCollisionPointRec(mousePos, Rect);
        }

        public void Draw()
        {
            var currentColor = IsHovered ? HoverColor : Color;
            DrawRectangleRounded(Rect, 0.3f, 10, currentColor);
            var textSize = MeasureText(Text, 30);
            var textX = Rect.X + (Rect.Width - textSize) / 2;
            var textY = Rect.Y + (Rect.Height - 30) / 2;
            DrawText(Text, (int)textX, (int)textY, 30, Color.White);
        }

        public bool IsClicked()
        {
            return IsHovered && IsMouseButtonPressed(MouseButton.Left);
        }
    }
}
