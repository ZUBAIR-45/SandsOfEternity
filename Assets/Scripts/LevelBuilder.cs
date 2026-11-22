using UnityEngine;

// Runtime par current level ka maze khud bana deta hai (LevelData.cs ke map se).
// Sprites / Player / Camera ke references Editor builder (Sands of Eternity > Build Game Scene) bhar deta hai.
[DefaultExecutionOrder(-100)]
public class LevelBuilder : MonoBehaviour
{
    public Sprite wall, floor, door, entrance, gem, glow, heart, boulder, spikes, cover;
    public Transform player;
    public CameraFit cameraFit;

    static readonly Color Gold = new Color32(0xF2, 0xC2, 0x30, 255);
    static readonly Color Teal = new Color32(0x28, 0xC8, 0xBE, 255);

    string[] map;
    int W, H;
    LevelInfo info;

    void Awake()
    {
        info = LevelData.Get(GameManager.CurrentLevel);
        map = info.map;
        W = info.Width;
        H = info.Height;
        Build();
    }

    Vector3 Pos(int r, int c)
    {
        return new Vector3(c - (W - 1) / 2f, (H - 1) / 2f - r, 0f);
    }

    void Build()
    {
        if (cameraFit != null)
        {
            cameraFit.mazeWidth = W;
            cameraFit.mazeHeight = H;
        }

        Transform mazeRoot = new GameObject("Maze").transform;
        Transform floors = NewChild("Floors", mazeRoot);
        Transform walls = NewChild("Walls", mazeRoot);
        Transform items = NewChild("Items", mazeRoot);
        Transform hazards = NewChild("Hazards", mazeRoot);

        Vector3 spawn = Vector3.zero;
        GameObject shadow = null;
        DoorController doorCtrl = null;
        int spikeCount = 0;

        for (int r = 0; r < H; r++)
        {
            for (int c = 0; c < W; c++)
            {
                char ch = map[r][c];
                Vector3 p = Pos(r, c);

                if (ch == '#')
                {
                    GameObject w = MakeSprite("Wall", wall, p, 1, walls, 1.02f);
                    BoxCollider2D bc = w.AddComponent<BoxCollider2D>();
                    bc.size = Vector2.one;
                    continue;
                }

                MakeSprite("Floor", floor, p, 0, floors, 1.02f);

                switch (ch)
                {
                    case 'E':
                    {
                        spawn = p;
                        GameObject e = MakeSprite("Entrance", entrance, p, 1, items, 1f);
                        BoxCollider2D tc = e.AddComponent<BoxCollider2D>();
                        tc.isTrigger = true;
                        tc.size = new Vector2(0.8f, 0.8f);
                        e.AddComponent<ExitZone>();
                        break;
                    }
                    case 'G':
                    {
                        GameObject g = MakeSprite("SunGem", gem, p, 3, items, 0.85f);
                        CircleCollider2D cc = g.AddComponent<CircleCollider2D>();
                        cc.isTrigger = true;
                        cc.radius = 0.5f;
                        g.AddComponent<Collectible>().type = CollectibleType.Gem;
                        AddGlow(g.transform, p, Teal);
                        break;
                    }
                    case 'H':
                    {
                        GameObject h = MakeSprite("HeartOfEternity", heart, p, 3, items, 0.8f);
                        CircleCollider2D cc = h.AddComponent<CircleCollider2D>();
                        cc.isTrigger = true;
                        cc.radius = 0.5f;
                        h.AddComponent<Collectible>().type = CollectibleType.Heart;
                        AddGlow(h.transform, p, Gold);
                        shadow = MakeSprite("HeartRoomShadow", cover, p, 6, items, 1.02f);
                        break;
                    }
                    case 'D':
                    {
                        GameObject d = MakeSprite("GoldenDoor", door, p, 3, items, 1f);
                        BoxCollider2D bc = d.AddComponent<BoxCollider2D>();
                        bc.size = Vector2.one;
                        doorCtrl = d.AddComponent<DoorController>();
                        break;
                    }
                    case 'S':
                    {
                        GameObject s = MakeSprite("SpikeTrap", spikes, p, 2, hazards, 1f);
                        BoxCollider2D tc = s.AddComponent<BoxCollider2D>();
                        tc.isTrigger = true;
                        tc.size = new Vector2(0.7f, 0.7f);
                        s.AddComponent<Hazard>();
                        SpikeTrap trap = s.AddComponent<SpikeTrap>();
                        trap.sprite = s.GetComponent<SpriteRenderer>();
                        trap.upTime = info.spikeUp;
                        trap.downTime = info.spikeDown;
                        trap.startOffset = spikeCount * 0.8f;
                        spikeCount++;
                        break;
                    }
                    case 'h':
                    case 'v':
                        MakeBoulder(r, c, ch == 'h', hazards);
                        break;
                }
            }
        }
        if (doorCtrl != null) doorCtrl.shadowCover = shadow;

        if (player != null)
        {
            player.position = spawn;
            Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
            if (prb != null) prb.position = new Vector2(spawn.x, spawn.y);
        }
    }

    // ================= helpers =================

    static Transform NewChild(string name, Transform parent)
    {
        Transform t = new GameObject(name).transform;
        t.SetParent(parent, false);
        return t;
    }

    static GameObject MakeSprite(string name, Sprite s, Vector3 pos, int order, Transform parent, float scale)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(scale, scale, 1f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = order;
        return go;
    }

    void AddGlow(Transform parent, Vector3 pos, Color color)
    {
        GameObject g = MakeSprite("Glow", glow, pos, 2, parent, 1.1f);
        g.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0.6f);
        g.AddComponent<Pulse>();
    }

    bool IsOpen(int r, int c)
    {
        if (r < 0 || r >= H || c < 0 || c >= W) return false;
        return "#DEH".IndexOf(map[r][c]) < 0;
    }

    void MakeBoulder(int r, int c, bool horizontal, Transform parent)
    {
        Vector3 p = Pos(r, c);
        GameObject root = new GameObject("Boulder");
        root.transform.SetParent(parent, false);
        root.transform.position = p;

        Rigidbody2D rb = root.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        CircleCollider2D cc = root.AddComponent<CircleCollider2D>();
        cc.isTrigger = true;
        cc.radius = 0.4f;
        root.AddComponent<Hazard>();

        GameObject visual = MakeSprite("Visual", boulder, p, 4, root.transform, 0.9f);

        int pos = horizontal ? c : r;
        int a = pos;
        int b = pos;
        while (IsOpen(horizontal ? r : a - 1, horizontal ? a - 1 : c) && pos - (a - 1) <= 3) a--;
        while (IsOpen(horizontal ? r : b + 1, horizontal ? b + 1 : c) && (b + 1) - pos <= 3) b++;

        BoulderPatrol patrol = root.AddComponent<BoulderPatrol>();
        patrol.pointA = horizontal ? (Vector2)Pos(r, a) : (Vector2)Pos(a, c);
        patrol.pointB = horizontal ? (Vector2)Pos(r, b) : (Vector2)Pos(b, c);
        patrol.speed = info.boulderSpeed;
        patrol.visual = visual.transform;
    }
}
