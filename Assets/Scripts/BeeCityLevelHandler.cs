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
        queen.gameObject.SetActive(true);
        //Time.timeScale = 1;

    }

    public override bool HoneycombTowerSpawnEnemy(MapHoneycomb tower)
    {

        return honeycombTowerBeesAlwaysSpawn || PlayerHandler.royalJellyCount > 5;
    }

    public override void HandleExit()
    {
        if (queenDead)
        {
            ExitPanel.SetActive(true);
            if (Player.GetComponent<HornetController>().ExitButtonPressed) LevelEndSequence();
        }
    }

    protected override void LoadNextLevel()
    {
        LoadMainMenu();
    }
}
