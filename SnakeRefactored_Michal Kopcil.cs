namespace Snake
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public abstract class GameBlock
    {
        public int snakePositionX { get; set; }
        public int snakePositionY { get; set; }
        public ConsoleColor Color { get; set; }
        public char Symbol { get; set; } = '■';

        public virtual void Draw()
        {
            Console.ForegroundColor = Color;
            Console.SetCursorPosition(snakePositionX, snakePositionY);
            Console.Write(Symbol);
        }
    }

    public class SnakePart : GameBlock
    {
        public SnakePart(int x, int y, ConsoleColor color)
        {
            snakePositionX = x;
            snakePositionY = y;
            Color = color;
        }
    }

    public class Berry : GameBlock
    {
        public Berry(int x, int y)
        {
            snakePositionX = x;
            snakePositionY = y;
            Color = ConsoleColor.Red;
        }
    }

    public class Snake : GameBlock
    {
        public List<SnakePart> BodyParts { get; private set; }
        public Direction CurrentDirection { get; set; }

        public Snake(int startX, int startY, ConsoleColor headColor)
        {
            snakePositionX = startX;
            snakePositionY = startY;
            Color = headColor;
            CurrentDirection = Direction.Right;
            BodyParts = new List<SnakePart>();
        }

        public void Move()
        {
            BodyParts.Add(new SnakePart(snakePositionX, snakePositionY, ConsoleColor.Green));
            switch (CurrentDirection)
            {
                case Direction.Up:
                    snakePositionY--;
                    break;
                case Direction.Down:
                    snakePositionY++;
                    break;
                case Direction.Left:
                    snakePositionX--;
                    break;
                case Direction.Right:
                    snakePositionX++;
                    break;
            }
        }

        public void TrimBody(int maxLength)
        {
            if (BodyParts.Count > maxLength)
            {
                BodyParts.RemoveAt(0);
            }
        }

        public new void Draw()
        {
            base.Draw();
            foreach (var segment in BodyParts)
            {
                segment.Draw();
            }
        }
    }

    public class Game
    {
        private readonly int screenWidth = 32;
        private readonly int screenHeight = 16;
        private int score;
        private bool isGameOver;
        private readonly Random random;
        private readonly Snake snake;
        private Berry berry;
        private readonly int moveDelayMilliseconds = 500;
        private DateTime lastMoveTime;

        public Game()
        {
            Console.WindowWidth = screenWidth;
            Console.WindowHeight = screenHeight;
            Console.BufferWidth = screenWidth;
            Console.BufferHeight = screenHeight;
            Console.CursorVisible = false;
            score = 5;
            isGameOver = false;
            random = new Random();
            int startX = screenWidth / 2;
            int startY = screenHeight / 2;
            snake = new Snake(startX, startY, ConsoleColor.DarkGreen);
            PlaceBerry();
            lastMoveTime = DateTime.Now;
        }

        private void PlaceBerry()
        {
            int x = random.Next(1, screenWidth - 1);
            int y = random.Next(1, screenHeight - 1);
            berry = new Berry(x, y);
        }

        public void Run()
        {
            while (!isGameOver)
            {
                while ((DateTime.Now - lastMoveTime).TotalMilliseconds < moveDelayMilliseconds)
                {
                    HandleInput();
                }
                Update();
                Draw();
                lastMoveTime = DateTime.Now;
            }
            GameOver();
        }

        private void Draw()
        {
            Console.Clear();
            DrawBorders();
            berry.Draw();
            snake.Draw();
        }

        private void DrawBorders()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int x = 0; x < screenWidth; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write('■');
                Console.SetCursorPosition(x, screenHeight - 1);
                Console.Write('■');
            }
            for (int y = 0; y < screenHeight; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write('■');
                Console.SetCursorPosition(screenWidth - 1, y);
                Console.Write('■');
            }
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (snake.CurrentDirection != Direction.Down)
                            snake.CurrentDirection = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        if (snake.CurrentDirection != Direction.Up)
                            snake.CurrentDirection = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (snake.CurrentDirection != Direction.Right)
                            snake.CurrentDirection = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (snake.CurrentDirection != Direction.Left)
                            snake.CurrentDirection = Direction.Right;
                        break;
                }
            }
        }

        private void Update()
        {
            snake.Move();
            if (snake.snakePositionX == 0 || snake.snakePositionX == screenWidth - 1 ||
                snake.snakePositionY == 0 || snake.snakePositionY == screenHeight - 1)
            {
                isGameOver = true;
                return;
            }
            foreach (var segment in snake.BodyParts)
            {
                if (segment.snakePositionX == snake.snakePositionX && segment.snakePositionY == snake.snakePositionY)
                {
                    isGameOver = true;
                    return;
                }
            }
            if (snake.snakePositionX == berry.snakePositionX && snake.snakePositionY == berry.snakePositionY)
            {
                score++;
                PlaceBerry();
            }
            else
            {
                snake.TrimBody(score);
            }
        }

        private void GameOver()
        {
            Console.Clear();
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2);
            Console.WriteLine($"Game over, Score: {score}");
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
            Console.ReadKey();
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var game = new Game();
            game.Run();
        }
    }
}
