using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public int level = 0;
    public enum Stage { ready, generating, loading, playing, scoring }
    public Stage stage;

    public StageParameters[] stageParameters;
    [System.Serializable]
    public struct StageParameters
    {
        public MapGeneratorParameters mapGeneratorParameters;
        public MapParameters mapParameters;
    }

    List<PlayerScore> PlayerScores = new List<PlayerScore>();
    [System.Serializable]
    public class PlayerScore
    {
        public float time;
        public int shotsFired;
        public int beesHit;
        public int terrainHit;
        public int honeycombTowerHit;
        public int honeycombTowersDestroyed;
        public int spidersHit, spidersKilled;
        public int pillapillarHit, pillapillarLinksKilled, pillapillarsKilled;
        public int antsKilled;
        public int queenHits;
        
    }

    private void Start()
    {
        if (!singleton)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadStartScene()
    {

    }

    public void LoadMainMenu()
    {
        level = 0;
        stage = Stage.ready;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);

    }
    public void LoadNextLevel()
    {
        level++;
        if (level < stageParameters.Length)
        {
            LevelManager.mapParameters = stageParameters[level].mapParameters;
            LevelManager.mapGeneratorParameters = stageParameters[level].mapGeneratorParameters;
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
        else
        {
            LoadMainMenu();
        }
    }

    public PlayerScore GetLevelScore()
    {
        if(level >= PlayerScores.Count)
        {
            return GetNewPlayerScore();
        }
        else
        {
            return PlayerScores[level];
        }
    }

    private PlayerScore GetNewPlayerScore()
    {
        PlayerScore score = new PlayerScore();
        score.time = 0;
        score.shotsFired = 0;
        score.beesHit = 0;
        score.terrainHit = 0;
        score.honeycombTowerHit = 0;
        score.honeycombTowersDestroyed = 0;
        score.spidersHit = 0;
        score.spidersKilled = 0;
        score.pillapillarHit = 0;
        score.pillapillarLinksKilled = 0;
        score.pillapillarsKilled = 0;
        score.antsKilled = 0;
        score.queenHits = 0;
        return score;
    }
    private PlayerScore GetPlayerScore(int level)
    {
        if (PlayerScores.Count > level) return PlayerScores[level];
        else if(PlayerScores.Count == level)
        {
            PlayerScores.Add(GetNewPlayerScore());
            return PlayerScores[level];
        }
        else
        {
            Debug.LogWarning($"level: {level} is out of range.");
            return default;
        }
    }
    public static void EnemyHit(Insect.Type type)
    {
        switch (type)
        {
            case Insect.Type.bee:
                BeeHit();
                break;
            case Insect.Type.pillapillar:
                PillapillarHit();
                break;
            case Insect.Type.queen:
                QueenHit();
                break;
            case Insect.Type.spider:
                SpiderHit();
                break;
            default:
                AntsKilled();
                break;
        }
    }
    public static void ShotFired()
    {
        singleton.GetPlayerScore(singleton.level).shotsFired++;
    }
    public static void TerrainHit()
    {
        singleton.GetPlayerScore(singleton.level).terrainHit++;
    }
    public static void SpiderHit()
    {
        singleton.GetPlayerScore(singleton.level).spidersHit++;
    }
    public static void QueenHit()
    {
        singleton.GetPlayerScore(singleton.level).queenHits++;
    }
    public static void PillapillarHit()
    {
        singleton.GetPlayerScore(singleton.level).pillapillarHit++;
    }
    public static void AntsKilled()
    {
        singleton.GetPlayerScore(singleton.level).antsKilled++;
    }
    public static void BeeHit()
    {
        singleton.GetPlayerScore(singleton.level).beesHit++;
    }
    public static void SetPlayTime(float time)
    {
        singleton.GetPlayerScore(singleton.level).time = time;
    }
}
