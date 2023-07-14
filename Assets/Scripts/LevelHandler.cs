using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{
    public static LevelHandler singleton;
    public GameObject MurderPanel;
    public GameObject ExitPanel;
    public Transform Player;
    public GameObject PlayerPrefab;

    public CameraController Cam;

    public GameObject EnemyDropPrefab;

    private PlayerHandler ph;

    private bool playerDead = false;
    private bool levelEnding = false;

    protected MapGenerator generator;

    private bool _paused = false;
    public bool paused { get { return _paused; } }

    public UIHandler uIHandler;

    public Portal PlayerSpawn { get; set; }
    public Portal Exit { get; set; }
    public Transform ExitTunnel;

    public PrincessController princess;
    public QueenController queen;

    [SerializeField] protected LevelTask[] LevelCompleteTasks;

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        MurderPanel.SetActive(false);
        //ExitPanel.SetActive(false);
        uIHandler.DisplayExitLabel(false);
        UpdatePlayerStats();
        hc = Player.GetComponent<HornetController>();
        StartCoroutine(SetupLevel());

        ControlParameters.StaticControlParams.LoadControlParameters();
        
    }
    bool generatorGenerating;
    void OnGenerationComplete()
    {
        generatorGenerating = false;
    }
    bool pregenerationComplete = false;
    void PreGenerationComplete(List<MapVoid> mapVoids)
    {
        MapGenerator.onGenerationPreloadComplete -= PreGenerationComplete;
        LevelManager.levelMapVoids = mapVoids;
        pregenerationComplete = true;
        Debug.Log("Preloading Complete");
    }

    IEnumerator SetupLevel()
    {
        generator = FindObjectOfType<MapGenerator>();
        generatorGenerating = true;
        MapGenerator.onGenerationComplete += OnGenerationComplete;
        if (LevelManager.levelMapVoids != null)
        {
            generator.GenerateMap(LevelManager.levelMapVoids);
            LevelManager.levelMapVoids = null;
        }
        else
        {
            MapGenerator.onGenerationPreloadComplete += PreGenerationComplete;
            Debug.Log("LevelHandler.SetupLevel GeneratingMap");
            pregenerationComplete = false;
            MapGenerator.GenerateMap(LevelManager.mapGeneratorParameters, LevelManager.mapParameters, LevelManager.perlinNoiseParameters);
            do { yield return null; }
            while (!pregenerationComplete);
            generator.GenerateMap(LevelManager.levelMapVoids);
            // StartCoroutine(generator.GenerateMap(/*Player*/LevelManager.mapGeneratorParameters, LevelManager.mapParameters, LevelManager.perlinNoiseParameters));
        }

        do { yield return null; }
        while (generatorGenerating);

        MapGenerator.onGenerationComplete -= OnGenerationComplete;

        Map.StaticMap.Display = true;

        Player.position = PlayerSpawn.Chamber.locations[0];
        ExitTunnel.position = Exit.Chamber.Location;

        Cam.SetCameraTarget(Player);
        ph = FindObjectOfType<PlayerHandler>();
        
        uIHandler.DisplayExitLocation(Utility.Honeycomb.WorldPointToHoneycombGrid(Exit.transform.position).vector2);
        MiniMap.singleton.StartDisplayMiniMap();
        MiniMap.singleton.SetPrincessPos(Utility.Honeycomb.WorldPointToHoneycombGrid(Exit.transform.position));
        MiniMap.singleton.DisplayPrincessMarker(true);
        StartCoroutine(SetupComplete());
        //displayLocation(Utility.WorldToHoneycomb(PlayerSpawn.transform.position), SpawnLoc);
    }

    protected virtual IEnumerator SetupComplete()
    {
        //Time.timeScale = 0;
        yield return new WaitForSeconds(1);
        uIHandler.LoadUI();
        //Time.timeScale = 1;
        
    }

    float playTime = 0;
    // Update is called once per frame
    void Update()
    {
        playTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }
        //infiniteLevel();
        if (Exit && Exit.inChamber) {
            HandleExit();
        }
        else
        {
            //ExitPanel.SetActive(false);
            uIHandler.DisplayExitLabel(false);
            uIHandler.DisplayDialoguePrompt(false, "");
        }

        if (Player)
        {
            uIHandler.DisplayPlayerLocation(Utility.Honeycomb.WorldPointToHoneycombGrid(Player.position).vector2);
            uIHandler.DisplaySpawnLocation(Map.StaticMap.GetChunkIndex( Utility.Honeycomb.GetMapChunk(Player.position)));
            //Debug.Log(Utility.GetMapChunk(Player.position).ChunkIndex + " chunkOffset: " + Utility.GetMapChunk(Player.position).mapOffset);

            //display player location on minimap
            MiniMap.singleton.SetPlayerMarker(Utility.Honeycomb.WorldPointToHoneycombGrid(Player.position), Player.up);
        }
        else if(!playerDead)
        {
            HandlePlayerDeath();
            //MurderPanel.SetActive(true);
        }
        UpdatePlayerStats();
        //FPSText.text = Utility.FormatFloat(1 / Time.deltaTime, 1);
        //if(Player)Debug.Log("Player Chunk: " + Utility.GetMapChunk(Player.transform.position).mapOffset);
    }
    public void PauseToggle()
    {
        if (_paused)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }

        _paused = !_paused;
    }

    
    HornetController hc;
    private void spawnPlayer(Portal portal)
    {
        Player = Instantiate(PlayerPrefab, /*generator.*/PlayerSpawn.Chamber.locations[0], Quaternion.identity).transform;
        playerDead = false;
        hc = Player.GetComponent<HornetController>();
        Cam.SetCameraTarget(Player);
        UpdatePlayerStats();
    }

    public bool infiniteLives = true;
    void HandlePlayerDeath()
    {
        uIHandler.DisplayDialoguePrompt(false, "");
        playerDead = true;
        
        if(infiniteLives || PlayerHandler.eggCount > 0)
        {
            PlayerHandler.eggCount = Mathf.Clamp(PlayerHandler.eggCount - 1, 0, int.MaxValue);
            uIHandler.DisplayMurderedPanel(true, "Murder Hornet is Murdered");
        }
        else
        {
            gameOver = true;
            uIHandler.DisplayMurderedPanel(true, "The Princess is in Another Hive");
        }
    }

    public GameObject towerDropPrefab;
    public void BeeuildingDestroyed(Vector2 towerPos)
    {
        Instantiate(towerDropPrefab, towerPos, Quaternion.identity);
    }

    public virtual void HandleExit()
    {
        //ExitPanel.SetActive(true);
        uIHandler.DisplayExitLabel(true);
        if (Player && Player.GetComponent<HornetController>().ExitButtonPressed && !levelEnding)
        {
            bool levelComplete = TasksComplete(LevelCompleteTasks/*, ph, uIHandler*/);
            
            if (levelComplete)
            {
                uIHandler.DisplayDialoguePrompt(false, "");
                LevelEndSequence();
            }
        }
    }

    public void SetLevelTasks(LevelTask[] tasks)
    {
        LevelCompleteTasks = tasks;
        foreach(LevelTask task in LevelCompleteTasks)
        {
            task.Completed = false;
        }
    }

    protected bool TasksComplete(LevelTask[] tasks/*, PlayerHandler ph, UIHandler uIHandler*/)
    {
        bool levelComplete = true;
        foreach (LevelTask task in tasks)
        {
            if (!task.Completed)
            {
                levelComplete = false;
                bool taskComplete = true;
                if (task.Requirements.PetalsCollect && ph.flowersFound < 5)
                {
                    taskComplete = false;
                }
                else if (task.Requirements.EggCount > 0 && ph.eggsFound < task.Requirements.EggCount)
                {
                    taskComplete = false;
                }
                else if(task.Requirements.AntMoundsTriggered && !AntMoundsTriggered())
                {
                    taskComplete = false;
                }else if(task.Requirements.QueenDefeat && queen != null)
                {
                    taskComplete = false;
                }


                task.Completed = taskComplete;
                if (task.Completed)
                {
                    uIHandler.DisplayDialoguePrompt(true, task.CompletionPrompt);
                }
                else
                {
                    uIHandler.DisplayDialoguePrompt(true, task.FailedPrompt);

                }
                break;
            }

        }
        return levelComplete;
    }


    public void EnemyDeath(GameObject enemy)
    {
        int dropCheck = 8;
        if (enemy.GetComponent<PillapillarController>()) dropCheck = 2;
        if (enemy.GetComponent<SpiderEnemy>()) dropCheck = 0;
        if(Random.Range(0,10) > dropCheck)
        {
            int dropItem = dropCheck == 0 && MapManager.singleton.UnplacedFlowerPetals()? Random.Range(0, 15) : Random.Range(0, 10); //0-2 Health 3 Power 4-6 Storage 7-9 Rapid 10 Flower
            float power;
            float duration = Random.Range(6 , 12) * 5;
            if(dropItem < 7)
            {
                power = Random.Range(10, 30);
            }
            else 
            {
                power = Random.Range(0.05f, 0.45f) * 2;
            }

            if(dropItem >= 10)
            {
                GameObject pedel = MapManager.singleton.GetFlowerPetalDrop();
                pedel.transform.position = enemy.transform.position;
                HoneycombPos hexPos = Utility.Honeycomb.WorldPointToHoneycombGrid(pedel.transform.position);
                MiniMap.singleton.SetFlower(hexPos, true);
            }
            else
            {
                ItemPickup drop = Instantiate(EnemyDropPrefab, enemy.transform.position, Quaternion.identity).GetComponent<ItemPickup>();

                if (dropItem < 3)
                {
                    drop.PickupType = ItemPickup.PickupTypes.Health;

                }
                else if (dropItem < 4) drop.PickupType = ItemPickup.PickupTypes.Power;
                else if (dropItem < 7) drop.PickupType = ItemPickup.PickupTypes.Storage;
                else if (dropItem < 10) drop.PickupType = ItemPickup.PickupTypes.Rapid;
                drop.Power = power;
                drop.Duration = duration;
                drop.SetupLetters();
            }
            

        }
    }

    private float levelEndStart;
    private float levelEndCountdown = 6.0f;
    protected void LevelEndSequence()
    {
        levelEnding = true;
        levelEndStart = Time.fixedTime;
        FindObjectOfType<PrincessController>().inLove = true;
        ExitTunnel.GetComponent<Animator>().SetTrigger("Activate");
        Map.StaticMap.Display = false;
        StartCoroutine(LevelEndCoroutine());
    }

    IEnumerator LevelEndCoroutine()
    {
        while (levelEndStart + levelEndCountdown > Time.fixedTime)
            yield return null;
        LoadNextLevel();
    }
    bool gameOver = false;
    public void RestartLevel()
    {
        if(gameOver)
        {
            LoadMainMenu();
        }
        else
        {
            MurderPanel.SetActive(false);
            spawnPlayer(/*generator.*/PlayerSpawn);
        }
        
        
    }
    protected void LoadMainMenu()
    {
        GameManager.singleton.LoadMainMenu();
    }

    protected virtual void LoadNextLevel()
    {
        GameManager.SetPlayTime(playTime);
        GameManager.singleton.LoadNextScene();
    }



    public void UpdatePlayerStats()
    {
        uIHandler.UpdateKillCounts(PlayerHandler.BeesMurderedCount, PlayerHandler.HornetMurderedCount);

        if(ph && hc)
        {
            uIHandler.UpdatePlayerBuffs(hc.Health, ph.GetMaxHealth(), hc.ShotCount, ph.GetMaxShot(), ph.GetPlasmaPower(),
                ph.GetPlasmaPowerBuffTime(), ph.GetPlasmaChargeRate(), ph.GetPlasmaChargeRateBuffTime(), ph.GetMaxShotBuffTime(),
                PlayerHandler.eggCount, PlayerHandler.flowerCount, PlayerHandler.royalJellyCount);
        }
        
    }

    public void UpdatePlayerStats(int BeesDied, int HornetDied)
    {
        PlayerHandler.BeesMurderedCount += BeesDied;
        PlayerHandler.HornetMurderedCount += HornetDied;
        UpdatePlayerStats();
    }

    public void CollectionsUpdated(int eggCount, int flowerCount, int royalJellyCount)
    {
        PlayerHandler ph = FindObjectOfType<PlayerHandler>();
        ph.AddEggs(eggCount);
        ph.AddFlowers(flowerCount);
        ph.AddRoyalJelly(royalJellyCount);
        UpdatePlayerStats();
    }

    public void SetControlParameters(float cameraSpeed, float sensitivity, float joystickBoardSize, float v, float h, bool inverseReverse)
    {
        uIHandler.SetControlParameters(cameraSpeed, sensitivity, joystickBoardSize, v, h, inverseReverse);
    }

    public virtual bool HoneycombTowerSpawnEnemy(MapHoneycomb tower) {
        
        return PlayerHandler.eggCount > 4 || ph.royalJellyFound > 5;
    }

    int pillapillarLengthMin = 2;
    public bool PillapillerSpawnTail(PillapillarController head)
    {
        int length = 0;
        while(head != null)
        {
            head = head.Tail;
            length += 1;
        }

        return length < PlayerHandler.eggCount + pillapillarLengthMin;
    }

    public virtual bool SpawnAntSqaud()
    {
        if(PlayerHandler.royalJellyCount > 0)
        {
            PlayerHandler.royalJellyCount--;
            return true;
        }
        return false;
    }

    public virtual bool AntMoundsTriggered()
    {
        bool triggered = true;
        foreach(ChamberAntFarmTrigger trigger in FindObjectsOfType<ChamberAntFarmTrigger>())
        {
            if (!trigger.Triggered)
            {
                triggered = false;
                break;
            }
        }
        return triggered;
    }
}

