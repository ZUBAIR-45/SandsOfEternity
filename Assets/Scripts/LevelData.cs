using UnityEngine;

// Saare levels ka data yahan hai. Naya level add karna ho to Levels array mai ek aur Make(...) likh do.
//   # wall | E entrance | G sun gem | D golden door | H heart | S spike trap
//   h = boulder (left-right) | v = boulder (up-down) | . floor
public class LevelInfo
{
    public string name;
    public string[] map;
    public float escapeTime;     // heart uthane ke baad kitne second milte hain
    public float boulderSpeed;
    public float spikeUp;        // spikes kitni der upar (khatarnak)
    public float spikeDown;      // spikes kitni der neeche (safe)

    public int Width { get { return map[0].Length; } }
    public int Height { get { return map.Length; } }

    public int GemCount
    {
        get
        {
            int n = 0;
            foreach (string row in map)
                foreach (char ch in row)
                    if (ch == 'G') n++;
            return n;
        }
    }
}

public static class LevelData
{
    static LevelInfo Make(string name, float escape, float boulderSpeed, float spikeUp, float spikeDown, string[] map)
    {
        return new LevelInfo { name = name, escapeTime = escape, boulderSpeed = boulderSpeed,
                               spikeUp = spikeUp, spikeDown = spikeDown, map = map };
    }

    public static readonly LevelInfo[] Levels = new LevelInfo[]
    {
        Make("The Entrance", 30f, 2.5f, 1.5f, 2.0f,
            new string[] {
                "###################",
                "#E#.....#...S...#G#",
                "#.#.#.#.#.###.#.#.#",
                "#.#.#.#.#...#.#...#",
                "#.#.#.........###S#",
                "#.#G#....h....#...#",
                "#S###.........#.###",
                "#...#.#.#.#...#.#H#",
                "#.v.#S#.###.###.#D#",
                "#.....#.....#G....#",
                "###################"
            }),

        Make("The Sunken Hall", 28f, 2.7f, 1.5f, 2.0f,
            new string[] {
                "###################",
                "#E#G......#S....#H#",
                "#.###.#.#.#.###.#D#",
                "#....S#.#.S.....#.#",
                "#######.###.#.###.#",
                "#G#........S#.#...#",
                "#.#.#######.#.#.#.#",
                "#.#...#...#.#...#.#",
                "#.###.#.#.#.#.h.#.#",
                "#.......#G#.......#",
                "###################"
            }),

        Make("Whispering Halls", 26f, 2.9f, 1.6f, 1.9f,
            new string[] {
                "###################",
                "#E#....G#.S.......#",
                "#.#.#.###.#####.v.#",
                "#.#.#S#....G#S....#",
                "#.#.###.#####.#####",
                "#.#.....#.........#",
                "#.#.#.h.#.#.#.#.#.#",
                "#...#.....#.#S....#",
                "###.#S#####.###.#D#",
                "#G....#G......S.#H#",
                "###################"
            }),

        Make("The Scarab Vault", 39f, 3.0f, 1.7f, 1.8f,
            new string[] {
                "#####################",
                "#E#..S..#..S#.....#H#",
                "#.#.###S#.#.#.###.#D#",
                "#.#.#...#.#.........#",
                "#.#.#.###.#########.#",
                "#.#.#.#.....S...#G..#",
                "#.#.#.#.h.#.###.###.#",
                "#.#.#.....#.#G#S....#",
                "#.#.###.#.#.#.###S#.#",
                "#...#G#...#.....#...#",
                "#####.#.#.#S#####.v.#",
                "#G..................#",
                "#####################"
            }),

        Make("Hall of Mirrors", 35f, 3.2f, 1.8f, 1.7f,
            new string[] {
                "#####################",
                "#E#G..........#...#H#",
                "#.###.###.h.#.#.#S#D#",
                "#...#..S....#...#...#",
                "###.#S#.#.#.#######.#",
                "#...#...#.#......G#.#",
                "#.#####.#.#.###.###.#",
                "#.#G.S....#.#.......#",
                "#.#######.#.#.#.v.###",
                "#.S.#..S#.S.#.....#G#",
                "###.#.#.#.#.#.#####.#",
                "#G....#...#S......S.#",
                "#####################"
            }),

        Make("The Burning Sands", 28f, 3.3f, 1.9f, 1.6f,
            new string[] {
                "#######################",
                "#E#........S#...#.....#",
                "#.#.###.###.#.#S#.###.#",
                "#.#.#S#.#G......#.#...#",
                "#.#S#.#S#####.v.#.#.#D#",
                "#.#...............#.#H#",
                "#.#######.###.#.#.#.###",
                "#.S.....#.#G#...#G#...#",
                "#.#.###S#.#.#.#.#####.#",
                "#S..#G#.....#.#.......#",
                "#####.#.h.###S#.h.#.#.#",
                "#G............#...#S..#",
                "#######################"
            }),

        Make("Pharaoh's Maze", 33f, 3.5f, 2.0f, 1.5f,
            new string[] {
                "#######################",
                "#E#HD....S#...........#",
                "#.#######.#.#.###.h.#.#",
                "#...S..S#.....#.....#.#",
                "###.###.#.h.#.#.#.###.#",
                "#G...G#.#...#.#S......#",
                "###.###.#.###.#####.#.#",
                "#...#.S.#..G#...S.#S..#",
                "#.v.#.#########.#.#.###",
                "#...#.#....S..#S..#.#G#",
                "#.#.#.#.#####.#.###.#.#",
                "#G#...S.#G.........S..#",
                "#######################"
            }),

        Make("Eternal Labyrinth", 32f, 3.6f, 2.0f, 1.5f,
            new string[] {
                "#######################",
                "#E#G.S.....S...S....#G#",
                "#.###.#.v.###.###.v.#.#",
                "#...#.#...#...#.....#.#",
                "###.#S#.#.#.#.#.#####.#",
                "#G#.#...#.S.#S#....S..#",
                "#.#.#D#####.#.#.#####S#",
                "#...#H#...#G#...#.S...#",
                "#S#.###S#.###.h.#.#####",
                "#.......#.#.S.........#",
                "#.#.h.###.#.###.#####.#",
                "#.....#G....#G........#",
                "#######################"
            })
    };

    public static int Count { get { return Levels.Length; } }

    public static LevelInfo Get(int index)
    {
        return Levels[Mathf.Clamp(index, 0, Levels.Length - 1)];
    }
}
