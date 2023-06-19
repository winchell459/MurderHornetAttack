using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public int level = 0;
    public enum Stage { ready, generating, loading, playing, scoring }
    [SerializeField]private Stage stage;

    public StageParameters[] stageParameters;
    [System.Serializable]
    public struct StageParameters
    {
        public PerlinNoiseScriptableObject perlinNoiseParameters;
        public MapGeneratorParameters mapGeneratorParameters;
        public MapParameters mapParameters;
        public List<MapVoid> stageMapVoids;
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

    public void LoadStartScene() //MainMenu calls when player clicks on screen
    {
        LoadLevel();
    }

    public void LoadMainMenu() //LevelHandler when gameover and GameManager when no more levels
    {
        level = 0;
        stage = Stage.ready;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void LoadNextScene() //LevelHandler calls at end of level
    {
        stage = Stage.scoring;
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        
    }
    public void LoadNextLevel() //ScoreHandler called when player clicks screen
    {
        if(stage == Stage.scoring || stage == Stage.ready)
            level++;
        LoadLevel();
    }
    private void LoadLevel()
    {
        if (level < stageParameters.Length)
        {
            if (stage == Stage.loading)
            {
                stage = Stage.playing;
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            }
            else if (stage != Stage.generating)
            {
                stage = Stage.generating;
                StartCoroutine(Preloading());
            }
            
        }
        else
        {
            LoadMainMenu();
        }
    }
    private bool preloadComplete = false;
    IEnumerator Preloading()
    {
        preloadComplete = false;

        MapGeneratorParameters mapGeneratorParameters = stageParameters[level].mapGeneratorParameters;
        MapParameters mapParameters = stageParameters[level].mapParameters;
        PerlinNoiseScriptableObject perlinNoiseParameters = stageParameters[level].perlinNoiseParameters;

        LevelManager.perlinNoiseParameters = perlinNoiseParameters;
        LevelManager.mapParameters = mapParameters;
        LevelManager.mapGeneratorParameters = mapGeneratorParameters;
        
        if (mapGeneratorParameters.generationType == MapGeneratorParameters.GenerationTypes.perlinNoise)
        {
            MapGenerator.PregeneratePerlineNoiseVoid(new PerlinNoise(perlinNoiseParameters), mapParameters);
            while (MapGenerator.pregenerated.generating) yield return null;
            preloadComplete = true;

        }
        else
        {
            MapGenerator.onGenerationPreloadComplete += PreGenerationComplete;

            MapGenerator.GenerateMap(mapGeneratorParameters, mapParameters);
            while (!preloadComplete) yield return null;

        }
        stage = Stage.loading;
        FindObjectOfType<LevelLoadUI>().PreGenerationComplete();
    }

    void PreGenerationComplete(List<MapVoid> mapVoids)
    {
        MapGenerator.onGenerationPreloadComplete -= PreGenerationComplete;
        LevelManager.levelMapVoids = mapVoids;

        preloadComplete = true;
        Debug.Log("Preloading Complete");
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
        DisplayPlayerStats(singleton.level);
    }
    public static void SetPlayTime(float time)
    {
        singleton.GetPlayerScore(singleton.level).time = time;
        DisplayPlayerStats(singleton.level);
    }
    public static void DisplayPlayerStats(int level)
    {
        PlayerScore ps = singleton.PlayerScores[level];
        Debug.Log($"kiaBees: {ps.beesHit} kiaSiders: {ps.spidersKilled}/{ps.spidersHit} kiaPill: {ps.pillapillarLinksKilled}/{ps.pillapillarsKilled}/{ps.pillapillarHit} time: {ps.time} ");

    //public float time;
    //public int shotsFired;
    //public int beesHit;
    //public int terrainHit;
    //public int honeycombTowerHit;
    //public int honeycombTowersDestroyed;
    //public int spidersHit, spidersKilled;
    //public int pillapillarHit, pillapillarLinksKilled, pillapillarsKilled;
    //public int antsKilled;
    //public int queenHits;
}
}
