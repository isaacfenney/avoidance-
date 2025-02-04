namespace avoidance__
{
    abstract class Game
    {
        public int width, height, difficulty, hitPoints, score;
        public Vector2 playerPos;
        public List<Projectile> projectiles;

        public Game(int width, int height, int difficulty)
        {
            this.width = width;
            this.height = height;
            this.difficulty = difficulty;

            score = 0;
            hitPoints = 7 - (2 * difficulty);
            playerPos = new Vector2(width / 2, height / 2);
            projectiles = new List<Projectile>();
        }

        public abstract void PlayGame();

        public void WriteUI()
        {
            Console.SetCursorPosition(0, height + 1);
            Console.WriteLine("Score: " + score + "\nHP: " + hitPoints);
        }

        public void HandleInput()
        {
            DateTime time = DateTime.Now;
            while ((DateTime.Now - time).Milliseconds < 100)
            {
                if (Console.KeyAvailable)
                {
                    Console.SetCursorPosition(2 * playerPos.x, playerPos.y); Console.Write(' ');
                    ConsoleKey key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.W or ConsoleKey.UpArrow:
                            playerPos.y--; break;
                        case ConsoleKey.S or ConsoleKey.DownArrow:
                            playerPos.y++; break;
                        case ConsoleKey.A or ConsoleKey.LeftArrow:
                            playerPos.x--; break;
                        case ConsoleKey.D or ConsoleKey.RightArrow:
                            playerPos.x++; break;
                    }
                    Console.SetCursorPosition(2 * playerPos.x, playerPos.y); Console.Write('☺');
                }
            }
        }

        public void HandleProjectileMovement()
        {
            List<Projectile> toRemove = new List<Projectile>();

            foreach (Projectile p in projectiles)
            {
                Console.SetCursorPosition(2 * p.pos.x, p.pos.y); Console.Write(" ");
                p.Move();
                Console.SetCursorPosition(2 * p.pos.x, p.pos.y); Console.Write("X");

                if (Vector2.Equals(p.pos, playerPos))
                {
                    hitPoints--;
                    toRemove.Add(p);
                    Console.SetCursorPosition(2 * p.pos.x, p.pos.y); Console.Write(" ");
                }

                Vector2 pos = p.pos;
                if (pos.x <= 0 || pos.y <= 0 || pos.x >= width || pos.y >= height)
                {
                    toRemove.Add(p);
                    Console.SetCursorPosition(2 * p.pos.x, p.pos.y); Console.Write(" ");
                }
            }
            foreach (Projectile p in toRemove)
            {
                projectiles.Remove(p);
            }
        }

        public abstract void AddProjectiles();
    }

    class RandomGame : Game
    {
        public RandomGame(int width, int height, int difficulty) : base(width, height, difficulty) { }

        public override void PlayGame()
        {
            Console.SetWindowSize(width * 2, height + 1);
            while (hitPoints > 0)
            {
                WriteUI();
                HandleInput();
                HandleProjectileMovement();
                AddProjectiles();
                score++;
            }
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game over! Score: " + score);
            Thread.Sleep(1000); // such that the player does not immediately press a key and close the window
        }

        public override void AddProjectiles()
        {
            Random r = new();

            int edgeToSpawnAt = r.Next(0, 4);
            switch (edgeToSpawnAt)
            {
                case 0:
                    projectiles.Add(new Projectile(
                        new Vector2(0, r.Next(0, height - 1)),
                        new Vector2(1, 0)
                    ));
                    break;
                case 1:
                    projectiles.Add(new Projectile(
                        new Vector2(width, r.Next(0, height - 1)),
                        new Vector2(-1, 0)
                    ));
                    break;
                case 2:
                    projectiles.Add(new Projectile(
                        new Vector2(r.Next(0, width - 1), 0),
                        new Vector2(0, 1)
                    ));
                    break;
                case 3:
                    projectiles.Add(new Projectile(
                        new Vector2(r.Next(0, width - 1), height),
                        new Vector2(0, -1)
                    ));
                    break;
                default:
                    Console.WriteLine("AAAAAaaarrrrgggghhhhhh");
                    break;
            }
        }
    }

    class Projectile
    {
        public Vector2 pos;
        public Vector2 vel;

        public Projectile(Vector2 pos, Vector2 vel)
        {
            this.pos = pos;
            this.vel = vel;
        }

        public void Move()
        {
            pos += vel;
        }
    }

    class Vector2
    {
        public int x;
        public int y;

        public static Vector2 Up = new(0, -1);
        public static Vector2 Down = new(0, 1);
        public static Vector2 Left = new(-1, 0);
        public static Vector2 Right = new(1, 0);

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 v, Vector2 w)
        {
            return new Vector2(v.x + w.x, v.y + w.y);
        }

        public static bool Equals(Vector2 v, Vector2 w)
        {
            return v.x == w.x && v.y == w.y;
        }
    }

    internal class Program
    {
        static void Main()
        {
            RandomGame r = new RandomGame(20, 20, 1);
            r.PlayGame();
        }
    }
}