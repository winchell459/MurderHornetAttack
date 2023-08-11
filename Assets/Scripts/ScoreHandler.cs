using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : LevelLoadUI
{
    public Text TouchToPlayText;
    public Text levelText, timeText, accuracyText, diggingTimeText, percentExploredText;
    private bool preloadComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        HandleScoring(GameManager.singleton.GetLevelScore());
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!preloadComplete) TouchToPlayText.text = "LOADING...";
            GameManager.singleton.LoadNextLevel();
        }
        if (preloadComplete) TouchToPlayText.text = "TOUCH TO CONTINUE";
    }

    private void HandleScoring(GameManager.PlayerScore playerLevelScore)
    {
        levelText.text = $"Level {GameManager.singleton.level + 1} Completed";

        timeText.text = FormatTime(playerLevelScore.time);
        diggingTimeText.text = CalcDiggingTime(playerLevelScore) + "%";
        accuracyText.text = CalcAccuracy(playerLevelScore);
    }

    private string FormatTime(float time)
    {
        return time.ToString();
    }
    private string CalcAccuracy(GameManager.PlayerScore playerLevelScore)
    {
        int hits = playerLevelScore.beesHit /*+ playerLevelScore.antsKilled*/ + playerLevelScore.queenHits + playerLevelScore.pillapillarHit
            + playerLevelScore.honeycombTowerHit + playerLevelScore.terrainHit + playerLevelScore.spidersHit;
        return Utility.Utility.FormatFloat((float)hits / playerLevelScore.shotsFired, 2).ToString();
    }
    private string CalcDiggingTime(GameManager.PlayerScore playerLevelScore)
    {
        int shotsFired = playerLevelScore.shotsFired;
        int terrainHit = playerLevelScore.terrainHit;
        return Utility.Utility.FormatFloat((float)terrainHit / shotsFired, 2).ToString();
    }

    public override void PreGenerationComplete()
    {
        preloadComplete = true;
    }
}
