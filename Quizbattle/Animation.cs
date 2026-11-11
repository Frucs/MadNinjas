using Spectre.Console;
using System.Text;

namespace Quizbattle
{
    public class Animation
    {
        public Animation()
        {

        }
        public void go()
        {
            Console.CursorVisible = false;
            Console.Clear();

            try
            {
                const int frames = 42;
                const int delay = 0;

                // Initial frame
                var initial = CreateLogo(-15);

                AnsiConsole.Live(initial)
                    .AutoClear(false)
                    .Start(ctx =>
                    {
                        for (int frame = 0; frame < frames; frame++)
                        {
                            // Move the diagonal light across the logo
                            int shine = -15 + frame * 2;

                            ctx.UpdateTarget(CreateLogo(shine));

                            Thread.Sleep(delay);
                        }

                        // Final static logo
                        ctx.UpdateTarget(CreateLogo(1000));
                    });
            }
            finally
            {
                Console.CursorVisible = true;
            }


            // ========================================
            // MAD NINJAS LOGO
            // ========================================

            static Panel CreateLogo(int shine)
            {
                var letters = new System.Collections.Generic.Dictionary<char, string[]>
                {
                    ['M'] = new[] {
                        "10001", "11011", "10101", "10101",
                        "10001", "10001", "10001"
                    },
                                ['A'] = new[] {
                        "01110", "10001", "10001", "11111",
                        "10001", "10001", "10001"
                    },
                                ['D'] = new[] {
                        "11110", "10001", "10001", "10001",
                        "10001", "10001", "11110"
                    },
                                ['N'] = new[] {
                        "10001", "11001", "11001", "10101",
                        "10011", "10011", "10001"
                    },
                                ['I'] = new[] {
                        "11111", "00100", "00100", "00100",
                        "00100", "00100", "11111"
                    },
                                ['J'] = new[] {
                        "00111", "00010", "00010", "00010",
                        "10010", "10010", "01100"
                    },
                                ['S'] = new[] {
                        "01111", "10000", "10000", "01110",
                        "00001", "00001", "11110"
                    }
                };

                string word = "MAD NINJAS";
                var sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine("[#A855F7]                   /\\_/\\\\[/]");
                sb.AppendLine("[#A855F7]                  (  o.o  )[/]");
                sb.AppendLine("[#A855F7]                   > ^ <[/]");
                sb.AppendLine();
                sb.AppendLine("[#6B21A8]          ◆━━━━━━━━━━━━━━━━━━━━◆[/]");
                sb.AppendLine();

                // Render pixel letters
                for (int row = 0; row < 7; row++)
                {
                    int x = 0;

                    foreach (char c in word)
                    {
                        if (c == ' ')
                        {
                            x += 3;
                            continue;
                        }

                        foreach (char pixel in letters[c][row])
                        {
                            if (pixel == '1')
                            {
                                // Diagonal reflection
                                int distance = Math.Abs(x - (shine - row * 2));

                                string color;

                                if (distance == 0)
                                    color = "#FFFFFF";
                                else if (distance == 1)
                                    color = "#FDF4FF";
                                else if (distance == 2)
                                    color = "#F0ABFC";
                                else if (distance == 3)
                                    color = "#D946EF";
                                else if (distance == 4)
                                    color = "#A855F7";
                                else if (distance == 5)
                                    color = "#7E22CE";
                                else
                                    color = "#6B21A8";

                                // Dark 3D shadow beneath the letters
                                sb.Append("[#2E1065]██[/]");
                                sb.Append($"[{color}]██[/]");
                            }
                            else
                            {
                                sb.Append("    ");
                            }

                            x++;
                        }

                        x++;
                    }

                    sb.AppendLine();

                    // Additional shadow depth
                    sb.AppendLine(
                        "[#2E1065]              ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄[/]"
                    );
                }

                sb.AppendLine();
                sb.AppendLine("[#D946EF]       ═══════════════════════════════════════════[/]");
                sb.AppendLine();
                sb.AppendLine("[bold white]                 SHADOWS NEVER REST[/]");
                sb.AppendLine();
                sb.AppendLine("[#A855F7]                       EST. 2026[/]");
                sb.AppendLine();

                return new Panel(new Markup(sb.ToString()))
                    .Border(BoxBorder.Double)
                    .BorderColor(Color.Purple)
                    .Expand();
            }
        }
    }
}
