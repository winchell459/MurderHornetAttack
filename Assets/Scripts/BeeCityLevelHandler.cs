using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeCityLevelHandler : LevelHandler
{
    public bool queenDead = false;
    public void QueenDeath()
    {
        FindObjectOfType<PrincessController>().inLove = true;
        queenDead = true;
        generator.ExitTunnel.gameObject.SetActive(true);
    }

    public override bool HoneycombTowerSpawnEnemy(MapHoneycomb tower)
    {

        return true;
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
