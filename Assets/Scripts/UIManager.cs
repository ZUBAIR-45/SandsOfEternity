using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Upar ki dark bar (SCORE / GEMS / LIVES / TIME) aur Intro, PAUSE, LEVEL COMPLETE, VICTORY, GAME OVER screens.
public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText, gemsText, timeText, bestText, objectiveText, levelText;
    public Image[] heartIcons;
    public GameObject introPanel, endPanel, pausePanel;
    public TMP_Text introNameText, endTitleText, endDetailText;

    static readonly Color Gold = new Color32(0xF2, 0xC2, 0x30, 255);
    static readonly Color Red = new Color32(0xC8, 0x3C, 0x32, 255);

    void Update()
    {
        GameManager g = GameManager.Instance;
        if (g == null) return;

        scoreText.text = "SCORE " + g.score;
        gemsText.text = "GEMS " + g.gems + "/" + g.totalGems;
        timeText.text = g.hasHeart ? "TIME " + Mathf.CeilToInt(g.timeLeft) : "TIME --";
        timeText.color = (g.hasHeart && g.timeLeft <= 10f) ? Red : Color.white;
        bestText.text = "PLAYER: " + g.playerName + "   BEST: " + g.bestScore + " (" + g.bestName + ")";
        if (levelText != null) levelText.text = "LEVEL " + g.LevelNumber + "/" + g.LevelCount;

        for (int i = 0; i < heartIcons.Length; i++)
            heartIcons[i].color = (i < g.lives) ? Color.white : new Color(1f, 1f, 1f, 0.18f);

        if (!g.doorOpen) objectiveText.text = "Collect all " + g.totalGems + " Sun Gems";
        else if (!g.hasHeart) objectiveText.text = "The Golden Door is open - take the Heart of Eternity!";
        else objectiveText.text = "RUN! Bring the Heart back to the entrance!";

        introPanel.SetActive(g.state == GameState.Intro);
        if (g.state == GameState.Intro)
        {
            bool blink = ((int)(Time.unscaledTime * 2f)) % 2 == 0;
            introNameText.text = "YOUR NAME:  " + g.typedName + (blink ? "_" : " ");
        }

        if (pausePanel != null) pausePanel.SetActive(g.state == GameState.Paused);

        bool ended = g.state == GameState.Won || g.state == GameState.Lost || g.state == GameState.LevelComplete;
        endPanel.SetActive(ended);
        if (ended)
        {
            string best = g.newBest ? "NEW BEST SCORE!" : "Best: " + g.bestScore + " by " + g.bestName;
            if (g.state == GameState.LevelComplete)
            {
                endTitleText.text = "LEVEL " + g.LevelNumber + " COMPLETE!";
                endTitleText.color = Gold;
                endDetailText.text = "Well done, " + g.playerName + "!\nScore: " + g.score + "   (time bonus +" + g.lastBonus + ")\n" + best
                    + "\n\nNext: Level " + (g.LevelNumber + 1) + " - " + g.NextLevelInfo.name + "\nPress ENTER to continue";
            }
            else if (g.state == GameState.Won)
            {
                endTitleText.text = "YOU ESCAPED!";
                endTitleText.color = Gold;
                endDetailText.text = "All " + g.LevelCount + " levels complete, " + g.playerName + "!\nFinal score: " + g.score + "   (time bonus +" + g.lastBonus + ")\n" + best
                    + "\n\nPress R to play again from Level 1";
            }
            else
            {
                endTitleText.text = "GAME OVER";
                endTitleText.color = Red;
                endDetailText.text = g.endReason + "\nScore: " + g.score + "\n" + best + "\n\nPress R to retry Level " + g.LevelNumber;
            }
        }
    }
}
