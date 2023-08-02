using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeCityLevelHandler : LevelHandler
{
    public bool queenDead = false;
    public bool honeycombTowerBeesAlwaysSpawn = false;
    public void QueenDeath()
    {
        princess.inLove = true;
        queenDead = true;
        ExitTunnel.gameObject.SetActive(true);
    }

    protected override IEnumerator SetupComplete()
    {
        //Time.timeScale = 0;
        yield return new WaitForSeconds(1);
        uIHandler.LoadUI();
        Map.StaticMap.AddTransientChunkObject(queen);
        //queen.gameObject.SetActive(true);
        //Time.timeScale = 1;

    }

    public override bool HoneycombTowerSpawnEnemy(MapHoneycomb tower)
    {

        return honeycombTowerBeesAlwaysSpawn || PlayerHandler.royalJellyCount > 5;
    }

    public override void HandleExit()
    {
        if (queenDead || true)
        {
            //ExitPanel.SetActive(true);
            uIHandler.DisplayExitLabel(true);
            if (Player.GetComponent<HornetController>().ExitButtonPressed)
            {
                bool levelComplete = TasksComplete(LevelCompleteTasks/*, ph, uIHandler*/);

                if (levelComplete)
                {
                    uIHandler.DisplayDialoguePrompt(false, "");
                    LevelEndSequence();
                }
            }
        }
    }

    protected override void LoadNextLevel()
    {
        LoadMainMenu();
    }
}
