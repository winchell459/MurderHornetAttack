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
        public string name;
        public PerlinNoiseScriptableObject perlinNoiseParameters;
        public MapGeneratorParameters mapGeneratorParameters;
        public MapParameters mapParameters;
        public List<MapVoid> stageMapVoids;
        public LevelCompleteParameters levelCompleteParameters;
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
        public int queenHits;

        public int antsSaved;
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
        LevelTask[] levelTasks = stageParameters[level].levelCompleteParameters.levelTasks;

        if(perlinNoiseParameters) perlinNoiseParameters.parameters.seed = mapGeneratorParameters.seed;

        LevelManager.perlinNoiseParameters = perlinNoiseParameters;
        LevelManager.mapParameters = mapParameters;
        LevelManager.mapGeneratorParameters = mapGeneratorParameters;
        LevelManager.levelTasks = levelTasks;

        MapGenerator.onGenerationPreloadComplete += PreGenerationComplete;

        MapGenerator.GenerateMap(mapGeneratorParameters, mapParameters, perlinNoiseParameters);
        while (!preloadComplete) yield return null;
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
        score.queenHits = 0;

        score.antsSaved = 0;
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
    public static void SpiderKilled()
    {
        singleton.GetPlayerScore(singleton.level).spidersKilled++;
        DisplayPlayerStats(singleton.level);
    }
    public static void QueenHit()
    {
        singleton.GetPlayerScore(singleton.level).queenHits++;
    }

    // -------------------- Pillapillar -------------------------
    public static void PillapillarHit()
    {
        singleton.GetPlayerScore(singleton.level).pillapillarHit++;
    }
    public static void PillapillarLinkKilled()
    {
        singleton.GetPlayerScore(singleton.level).pillapillarLinksKilled++;
        DisplayPlayerStats(singleton.level);
    }
    public static void PillapillarKilled()
    {
        singleton.GetPlayerScore(singleton.level).pillapillarsKilled++;
    }

    // ---------------------- Ants ------------------------------
    public static void AntsSaved()
    {
        singleton.GetPlayerScore(singleton.level).antsSaved++;
    }

    public static int TotalAntsSaved()
    {
        int total = 0;
        foreach(PlayerScore ps in singleton.PlayerScores)
        {
            total += ps.antsSaved;
        }
        return total;
    }

    public static int AntSavedCount()
    {
        return AntSavedCount(singleton.level);
    }

    public static int AntSavedCount(int level)
    {
        return singleton.GetPlayerScore(level).antsSaved;
    }

    // ---------------------- Bees ------------------------------
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
        Debug.Log($"kiaBees: {ps.beesHit} kiaSpiders: {ps.spidersKilled}/{ps.spidersHit} kiaPill: {ps.pillapillarLinksKilled}/{ps.pillapillarsKilled}/{ps.pillapillarHit} time: {ps.time} ");

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
