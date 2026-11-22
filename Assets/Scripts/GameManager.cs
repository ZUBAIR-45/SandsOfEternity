using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Intro, Playing, Paused, LevelComplete, Won, Lost }

// Game ka "dimagh": score, gems, lives, timer, levels, pause, win/lose sab yahan hai.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Kaun sa level chal raha hai (0 = Level 1). Scene reload hone par bhi yaad rehta hai.
    public static int CurrentLevel;
    static int carriedScore;    // pichhle levels ka score
    static bool skipIntro;      // restart / next level par name screen dobara na aaye

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        CurrentLevel = 0;
        carriedScore = 0;
        skipIntro = false;
    }

    [Header("Rules (Inspector mai change kar sakte ho)")]
    public int maxLives = 3;
    public int gemScore = 10;
    public int hitPenalty = 5;
    public int heartScore = 50;
    public int timeBonusPerSecond = 2;
    public float invulnerableSeconds = 1.5f;

    [Header("Level se aate hain (LevelData.cs mai badlo)")]
    public int totalGems = 3;
    public float escapeTime = 30f;

    [Header("Live values (sirf dekhne ke liye)")]
    public GameState state = GameState.Intro;
    public int score;
    public int gems;
    public int lives;
    public bool hasHeart;
    public bool doorOpen;
    public float timeLeft;
    public int lastBonus;
    public bool newBest;
    public string endReason = "";

    [HideInInspector] public string typedName = "";
    [HideInInspector] public string playerName = "Explorer";
    [HideInInspector] public int bestScore;
    [HideInInspector] public string bestName = "-";

    public event System.Action OnDoorOpened;

    float invulnerableUntil;

    public bool Invulnerable { get { return Time.time < invulnerableUntil; } }
    public LevelInfo Level { get { return LevelData.Get(CurrentLevel); } }
    public int LevelNumber { get { return CurrentLevel + 1; } }
    public int LevelCount { get { return LevelData.Count; } }
    public bool IsLastLevel { get { return CurrentLevel >= LevelData.Count - 1; } }
    public LevelInfo NextLevelInfo { get { return LevelData.Get(CurrentLevel + 1); } }

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        CurrentLevel = Mathf.Clamp(CurrentLevel, 0, LevelData.Count - 1);
        LevelInfo info = LevelData.Get(CurrentLevel);
        totalGems = info.GemCount;
        escapeTime = info.escapeTime;

        lives = maxLives;
        score = carriedScore;
        playerName = PlayerPrefs.GetString("PlayerName", "Explorer");
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bestName = PlayerPrefs.GetString("BestName", "-");
        typedName = PlayerPrefs.HasKey("PlayerName") ? playerName : "";
    }

    void Start()
    {
        if (skipIntro)
        {
            typedName = playerName;
            BeginPlay();
        }
        else
        {
            state = GameState.Intro;
            Time.timeScale = 0f;
        }
    }

    void Update()
    {
        switch (state)
        {
            case GameState.Intro:
                HandleNameTyping();
                break;

            case GameState.Playing:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGame();
                    break;
                }
                if (hasHeart)
                {
                    timeLeft -= Time.deltaTime;
                    if (timeLeft <= 0f)
                    {
                        timeLeft = 0f;
                        endReason = "Time ran out!";
                        GameOver();
                    }
                }
                break;

            case GameState.Paused:
                if (Input.GetKeyDown(KeyCode.Escape)) ResumeGame();
                else if (Input.GetKeyDown(KeyCode.R)) RestartLevel();
                else if (Input.GetKeyDown(KeyCode.Q)) QuitGame();
                break;

            case GameState.LevelComplete:
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
                    NextLevel();
                break;

            case GameState.Won:
                if (Input.GetKeyDown(KeyCode.R)) RestartAll();
                break;

            case GameState.Lost:
                if (Input.GetKeyDown(KeyCode.R)) RestartLevel();
                break;
        }
    }

    void HandleNameTyping()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (typedName.Length > 0) typedName = typedName.Substring(0, typedName.Length - 1);
            }
            else if (c == '\n' || c == '\r')
            {
                BeginPlay();
                return;
            }
            else if ((char.IsLetterOrDigit(c) || c == ' ' || c == '_' || c == '-') && typedName.Length < 12)
            {
                typedName += c;
            }
        }
    }

    void BeginPlay()
    {
        string n = typedName.Trim();
        if (n.Length == 0) n = "Explorer";
        playerName = n;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        skipIntro = true;
        state = GameState.Playing;
        Time.timeScale = 1f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySlam();   // "entrance door slams shut"
            AudioManager.Instance.StartMusic();
        }
    }

    // ---------- Pause ----------

    public void PauseGame()
    {
        if (state != GameState.Playing) return;
        state = GameState.Paused;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        if (state != GameState.Paused) return;
        state = GameState.Playing;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ---------- Events jo doosre scripts call karte hain ----------

    public void CollectGem()
    {
        if (state != GameState.Playing) return;
        gems++;
        score += gemScore;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGem();
        if (gems >= totalGems && !doorOpen)
        {
            doorOpen = true;
            if (AudioManager.Instance != null) AudioManager.Instance.PlayDoor();
            if (OnDoorOpened != null) OnDoorOpened();
        }
    }

    public void CollectHeart()
    {
        if (state != GameState.Playing || hasHeart) return;
        hasHeart = true;
        score += heartScore;
        timeLeft = escapeTime;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHeart();
            AudioManager.Instance.FastDrums();
        }
    }

    public void TakeHit()
    {
        if (state != GameState.Playing || Invulnerable) return;
        invulnerableUntil = Time.time + invulnerableSeconds;
        lives--;
        score = Mathf.Max(0, score - hitPenalty);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
        if (lives <= 0)
        {
            lives = 0;
            endReason = "You lost all your lives.";
            GameOver();
        }
    }

    public void ReachExit()
    {
        if (state == GameState.Playing && hasHeart) Victory();
    }

    void Victory()
    {
        lastBonus = Mathf.CeilToInt(timeLeft) * timeBonusPerSecond;
        score += lastBonus;
        state = IsLastLevel ? GameState.Won : GameState.LevelComplete;
        SaveBest();
        Time.timeScale = 0f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayVictory();
        }
    }

    void GameOver()
    {
        state = GameState.Lost;
        SaveBest();
        Time.timeScale = 0f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayGameOver();
        }
    }

    void SaveBest()
    {
        newBest = false;
        if (score > bestScore)
        {
            bestScore = score;
            bestName = playerName;
            newBest = true;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.SetString("BestName", bestName);
            PlayerPrefs.Save();
        }
    }

    // ---------- Level / restart ----------

    void NextLevel()
    {
        carriedScore = score;
        CurrentLevel++;
        Reload();
    }

    // Isi level ko dobara shuru karo (score us level ke start wala wapas aa jata hai)
    void RestartLevel()
    {
        Reload();
    }

    // Level 1 se poori game dobara
    void RestartAll()
    {
        CurrentLevel = 0;
        carriedScore = 0;
        Reload();
    }

    void Reload()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
