using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Menu: Sands of Eternity > Build Game Scene
// Ye script maze, player, gems, door, traps, UI, audio sab khud bana deti hai.
public static class SandsBuilder
{
    const string SprDir = "Assets/Sprites/";
    const string AudDir = "Assets/Audio/";
    const string ScenePath = "Assets/Scenes/Main.unity";

    static Material spriteMat;
    static Color Night = new Color32(0x10, 0x26, 0x2B, 255);
    static Color Gold = new Color32(0xF2, 0xC2, 0x30, 255);
    static Color Teal = new Color32(0x28, 0xC8, 0xBE, 255);
    static Color Sand = new Color32(0xDE, 0xC4, 0x8C, 255);

    [MenuItem("Sands of Eternity/Build Game Scene")]
    public static void BuildGame()
    {
        if (TMP_Settings.instance == null)
        {
            EditorUtility.DisplayDialog("TextMeshPro chahiye",
                "Pehle ye karo: Window > TextMeshPro > Import TMP Essential Resources\n(Import button dabao), phir ye menu dobara chalao.", "OK");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        FixSpriteImportSettings();
        spriteMat = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");

        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Sprite sprWall = Spr("wall"), sprFloor = Spr("floor"), sprDoor = Spr("door"), sprEntrance = Spr("entrance");
        Sprite sprGem = Spr("gem"), sprGlow = Spr("glow"), sprHeart = Spr("heart_of_eternity"), sprBoulder = Spr("boulder");
        Sprite sprSpikes = Spr("spikes"), sprExplorer = Spr("explorer"), sprCover = Spr("cover"), sprLife = Spr("life_heart");

        // ---------- Camera ----------
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6.85f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Night;
        camGO.transform.position = new Vector3(0f, 1.15f, -10f);
        camGO.AddComponent<AudioListener>();
        var fit = camGO.AddComponent<CameraFit>();

        // ---------- Maze (runtime par LevelBuilder level ke hisaab se banata hai) ----------
        // ---------- Player ----------
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = Vector3.zero;
        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        var pc = player.AddComponent<CircleCollider2D>();
        pc.radius = 0.32f;
        player.AddComponent<PlayerController>();
        MakeSprite("Visual", sprExplorer, Vector3.zero, 5, player.transform, 0.8f);

        // ---------- Level builder ----------
        var lb = new GameObject("LevelBuilder").AddComponent<LevelBuilder>();
        lb.wall = sprWall; lb.floor = sprFloor; lb.door = sprDoor; lb.entrance = sprEntrance;
        lb.gem = sprGem; lb.glow = sprGlow; lb.heart = sprHeart; lb.boulder = sprBoulder;
        lb.spikes = sprSpikes; lb.cover = sprCover;
        lb.player = player.transform;
        lb.cameraFit = fit;

        // ---------- Managers ----------
        new GameObject("GameManager").AddComponent<GameManager>();

        var am = new GameObject("AudioManager").AddComponent<AudioManager>();
        am.wind = Clip("wind_loop");
        am.drumsSlow = Clip("drums_slow");
        am.drumsFast = Clip("drums_fast");
        am.gem = Clip("sfx_gem");
        am.hit = Clip("sfx_hit");
        am.door = Clip("sfx_door");
        am.heart = Clip("sfx_heart");
        am.victory = Clip("sfx_victory");
        am.gameOver = Clip("sfx_gameover");
        am.slam = Clip("sfx_slam");

        BuildUI(sprLife);

        // ---------- Save ----------
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        Selection.activeGameObject = player;

        EditorUtility.DisplayDialog("Done!", "Scene ban gaya: " + ScenePath + "\n(" + LevelData.Count + " levels ready)\nAb upar Play button dabao.", "OK");
    }

    [MenuItem("Sands of Eternity/Reset Saved Name and Best Score")]
    public static void ResetPrefs()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("BestScore");
        PlayerPrefs.DeleteKey("BestName");
        PlayerPrefs.Save();
        EditorUtility.DisplayDialog("Reset", "Saved name aur best score delete ho gaye.", "OK");
    }

    // ================= EXE (Windows) =================

    [MenuItem("Sands of Eternity/Build Windows EXE")]
    public static void BuildWindowsExe()
    {
        if (!File.Exists(ScenePath))
        {
            EditorUtility.DisplayDialog("Scene nahi mila",
                "Pehle menu se Sands of Eternity > Build Game Scene chalao, phir ye dobara.", "OK");
            return;
        }
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
        {
            EditorUtility.DisplayDialog("Windows Build Support chahiye",
                "Unity Hub > Installs > (apna Unity version) > Add modules > \"Windows Build Support (Mono)\" install karo, phir dobara try karo.", "OK");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        PlayerSettings.productName = "Sands of Eternity";
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.runInBackground = true;

        string projectDir = Directory.GetParent(Application.dataPath).FullName;
        string outDir = Path.Combine(projectDir, "Build");
        Directory.CreateDirectory(outDir);
        string exePath = Path.Combine(outDir, "SandsOfEternity.exe");

        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = exePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
        {
            EditorUtility.DisplayDialog("EXE ban gayi!",
                "Ye file bani: " + exePath + "\n\nPoora \"Build\" folder zip karke bhejo (exe akeli nahi chalegi, saath wale folders/dll chahiye).", "OK");
            EditorUtility.RevealInFinder(exePath);
        }
        else
        {
            EditorUtility.DisplayDialog("Build fail", "Console window dekho (Window > General > Console) - wahan error likha hai.", "OK");
        }
    }

    // ================= helpers =================

    static void FixSpriteImportSettings()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) continue;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = 64f;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.filterMode = FilterMode.Bilinear;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.SaveAndReimport();
        }
        AssetDatabase.Refresh();
    }

    static Sprite Spr(string name)
    {
        var s = AssetDatabase.LoadAssetAtPath<Sprite>(SprDir + name + ".png");
        if (s == null) throw new System.Exception("Sprite nahi mila: " + SprDir + name + ".png  (Assets/Sprites folder check karo)");
        return s;
    }

    static AudioClip Clip(string name)
    {
        return AssetDatabase.LoadAssetAtPath<AudioClip>(AudDir + name + ".wav");
    }

    static Transform NewChild(string name, Transform parent)
    {
        var t = new GameObject(name).transform;
        t.SetParent(parent, false);
        return t;
    }

    static GameObject MakeSprite(string name, Sprite s, Vector3 pos, int order, Transform parent, float scale)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(scale, scale, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = order;
        if (spriteMat != null) sr.sharedMaterial = spriteMat;
        return go;
    }

    // ================= UI =================

    static RectTransform MakeRect(string name, Transform parent, Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }

    static TextMeshProUGUI MakeText(string name, Transform parent, string text, float size, Color color,
        TextAlignmentOptions align, Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 pos, Vector2 sz)
    {
        var rt = MakeRect(name, parent, aMin, aMax, pivot, pos, sz);
        var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.fontStyle = FontStyles.Bold;
        t.raycastTarget = false;
        return t;
    }

    static Image MakeImage(string name, Transform parent, Color color, Sprite sprite,
        Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 pos, Vector2 sz)
    {
        var rt = MakeRect(name, parent, aMin, aMax, pivot, pos, sz);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        img.sprite = sprite;
        img.preserveAspect = sprite != null;
        img.raycastTarget = false;
        return img;
    }

    static void BuildUI(Sprite lifeSprite)
    {
        var canvasGO = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        var ui = canvasGO.AddComponent<UIManager>();
        Transform cv = canvasGO.transform;

        Vector2 half = new Vector2(0.5f, 0.5f);
        Color barColor = new Color32(0x05, 0x0F, 0x14, 242);

        // --- top bar ---
        var bar = MakeImage("TopBar", cv, barColor, null, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, 90));
        Transform b = bar.transform;

        ui.scoreText = MakeText("ScoreText", b, "SCORE 0", 48, Gold, TextAlignmentOptions.Left,
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(40, 0), new Vector2(500, 80));
        ui.gemsText = MakeText("GemsText", b, "GEMS 0/3", 48, Teal, TextAlignmentOptions.Center,
            new Vector2(0.38f, 0.5f), new Vector2(0.38f, 0.5f), half, Vector2.zero, new Vector2(420, 80));
        MakeText("LivesLabel", b, "LIVES", 48, Color.white, TextAlignmentOptions.Right,
            new Vector2(0.55f, 0.5f), new Vector2(0.55f, 0.5f), half, Vector2.zero, new Vector2(220, 80));
        ui.heartIcons = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            ui.heartIcons[i] = MakeImage("Heart" + (i + 1), b, Color.white, lifeSprite,
                new Vector2(0.66f, 0.5f), new Vector2(0.66f, 0.5f), half, new Vector2((i - 1) * 70f, 0f), new Vector2(56, 56));
        }
        ui.timeText = MakeText("TimeText", b, "TIME --", 48, Color.white, TextAlignmentOptions.Right,
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-40, 0), new Vector2(420, 80));

        // --- second row (below bar) ---
        ui.bestText = MakeText("BestText", cv, "", 28, Sand, TextAlignmentOptions.Left,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -105), new Vector2(620, 50));
        ui.objectiveText = MakeText("ObjectiveText", cv, "", 36, Color.white, TextAlignmentOptions.Center,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(120, -105), new Vector2(1000, 60));

        ui.levelText = MakeText("LevelText", cv, "", 34, Gold, TextAlignmentOptions.Right,
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-40, -105), new Vector2(300, 50));

        // --- intro panel ---
        var intro = MakeImage("IntroPanel", cv, new Color32(0x10, 0x26, 0x2B, 245), null, Vector2.zero, Vector2.one, half, Vector2.zero, Vector2.zero);
        Transform ip = intro.transform;
        MakeText("Title", ip, "SANDS OF ETERNITY", 120, Gold, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.78f), new Vector2(0.5f, 0.78f), half, Vector2.zero, new Vector2(1700, 160));
        MakeText("Subtitle", ip, "Escape the Forgotten Pyramid", 48, Color.white, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.66f), new Vector2(0.5f, 0.66f), half, Vector2.zero, new Vector2(1500, 70));
        MakeText("HowToPlay", ip,
            "WASD / Arrow keys = move      ESC = pause\nCollect all Sun Gems  >  the Golden Door opens\nTake the Heart of Eternity  >  the escape timer starts\nRun back to the entrance to escape!\nBoulders and spikes cost 1 life (you only have 3)\nClear all " + LevelData.Count + " levels to escape the pyramid",
            34, Sand, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), half, Vector2.zero, new Vector2(1500, 260));
        ui.introNameText = MakeText("NameText", ip, "YOUR NAME:  _", 56, Teal, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.24f), new Vector2(0.5f, 0.24f), half, Vector2.zero, new Vector2(1500, 80));
        MakeText("Hint", ip, "Type your name, then press ENTER to start", 32, Color.white, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.15f), new Vector2(0.5f, 0.15f), half, Vector2.zero, new Vector2(1500, 60));
        ui.introPanel = intro.gameObject;

        // --- end panel ---
        var end = MakeImage("EndPanel", cv, new Color(0f, 0f, 0f, 0.78f), null, Vector2.zero, Vector2.one, half, Vector2.zero, Vector2.zero);
        Transform ep = end.transform;
        ui.endTitleText = MakeText("EndTitle", ep, "VICTORY!", 150, Gold, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.64f), new Vector2(0.5f, 0.64f), half, Vector2.zero, new Vector2(1700, 200));
        ui.endDetailText = MakeText("EndDetail", ep, "", 44, Color.white, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.36f), new Vector2(0.5f, 0.36f), half, Vector2.zero, new Vector2(1500, 360));
        ui.endPanel = end.gameObject;
        end.gameObject.SetActive(false);

        // --- pause panel (ESC dabane par) ---
        var pause = MakeImage("PausePanel", cv, new Color(0f, 0f, 0f, 0.82f), null, Vector2.zero, Vector2.one, half, Vector2.zero, Vector2.zero);
        Transform pp = pause.transform;
        MakeText("PauseTitle", pp, "PAUSED", 150, Gold, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.66f), new Vector2(0.5f, 0.66f), half, Vector2.zero, new Vector2(1700, 200));
        MakeText("PauseHelp", pp, "ESC  =  Resume\nR  =  Restart this level\nQ  =  Quit game", 52, Color.white, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.36f), new Vector2(0.5f, 0.36f), half, Vector2.zero, new Vector2(1500, 360));
        ui.pausePanel = pause.gameObject;
        pause.gameObject.SetActive(false);
    }
}
