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

    MapGenerator generator;

    private bool _paused = false;
    public bool paused { get { return _paused; } }

    public UIHandler uIHandler;


    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        MurderPanel.SetActive(false);
        ExitPanel.SetActive(false);
        UpdatePlayerStats();
        hc = Player.GetComponent<HornetController>();
        StartCoroutine(SetupLevel());

        ControlParameters.StaticControlParams.LoadControlParameters();
        
    }

    IEnumerator SetupLevel()
    {
        generator = FindObjectOfType<MapGenerator>();
        StartCoroutine(generator.GenerateMap(Player));
        do { yield return null; }
        while (generator.generating);

        Map.StaticMap.Display = true;
        Cam.SetCameraTarget(Player);
        ph = FindObjectOfType<PlayerHandler>();
        
        uIHandler.DisplayExitLocation(Utility.Honeycomb.WorldPointToHoneycombGrid(generator.Exit.transform.position).vector2);
        MiniMap.singleton.StartDisplayMiniMap();
        MiniMap.singleton.SetPrincessPos(Utility.Honeycomb.WorldPointToHoneycombGrid(generator.Exit.transform.position));
        MiniMap.singleton.DisplayPrincessMarker(true);
        StartCoroutine(SetupComplete());
        //displayLocation(Utility.WorldToHoneycomb(PlayerSpawn.transform.position), SpawnLoc);
    }

    IEnumerator SetupComplete()
    {
        //Time.timeScale = 0;
        yield return new WaitForSeconds(1);
        uIHandler.LoadUI();
        //Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }
        //infiniteLevel();
        if (generator.Exit && generator.Exit.inChamber) {
            ExitPanel.SetActive(true);
            if (Player.GetComponent<HornetController>().ExitButtonPressed && !levelEnding && ph.flowersFound >= 5) LevelEndSequence();
        }
        else
        {
            ExitPanel.SetActive(false);
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
        Player = Instantiate(PlayerPrefab, generator.PlayerSpawn.Chamber.locations[0], Quaternion.identity).transform;
        playerDead = false;
        hc = Player.GetComponent<HornetController>();
        Cam.SetCameraTarget(Player);
        UpdatePlayerStats();
    }

    public bool infiniteLives = true;
    void HandlePlayerDeath()
    {
        playerDead = true;
        
        if(infiniteLives || PlayerHandler.eggCount > 0)
        {
            PlayerHandler.eggCount = Mathf.Clamp(PlayerHandler.eggCount - 1, 0, int.MaxValue);
            FindObjectOfType<UIHandler>().DisplayMurderedPanel(true, "Murder Hornet is Murdered");
        }
        else
        {
            gameOver = true;
            FindObjectOfType<UIHandler>().DisplayMurderedPanel(true, "The Princess is in Another Hive");
        }
    }

    public GameObject towerDropPrefab;
    public void BeeuildingDestroyed(Vector2 towerPos)
    {
        Instantiate(towerDropPrefab, towerPos, Quaternion.identity);
    }

    public void QueenDeath()
    {
        FindObjectOfType<PrincessController>().inLove = true;
    }

    public void EnemyDeath(GameObject enemy)
    {
        int dropCheck = 8;
        if (enemy.GetComponent<PillapillarController>()) dropCheck = 2;
        if (enemy.GetComponent<SpiderEnemy>()) dropCheck = 0;
        if(Random.Range(0,10) > dropCheck)
        {
            int dropItem = dropCheck == 0 && generator.UnplacedFlowerPetals()? Random.Range(0, 11) : Random.Range(0, 10); //0-2 Health 3 Power 4-6 Storage 7-9 Rapid 10 Flower
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

            if(dropItem == 10)
            {
                generator.GetFlowerPetalDrop().transform.position = enemy.transform.position;
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
    private void LevelEndSequence()
    {
        levelEnding = true;
        levelEndStart = Time.fixedTime;
        FindObjectOfType<PrincessController>().inLove = true;
        generator.ExitTunnel.GetComponent<Animator>().SetTrigger("Activate");
        Map.StaticMap.Display = false;
        StartCoroutine("LevelEndCoroutine");
    }

    IEnumerator LevelEndCoroutine()
    {
        while (levelEndStart + levelEndCountdown > Time.fixedTime)
            yield return null;
        ReloadLevel();
    }
    bool gameOver = false;
    public void RestartLevel()
    {
        if(gameOver)
        {
            ReloadLevel();
        }
        else
        {
            MurderPanel.SetActive(false);
            spawnPlayer(generator.PlayerSpawn);
        }
        
        
    }

    private void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void infiniteLevel()
    {
        if (Player)
        {
            //if (Player.transform.position.y > honeycombToWorldPostion(new Vector2(0, honeycombHeight - ChunkHeight)).y)
            //{
            //    createChunk(new Vector2(0, honeycombHeight), ChunkWidth, ChunkHeight);
            //    honeycombHeight += ChunkHeight;
            //    float randX = Random.Range(-2.3f, 2.3f);
            //    path = new MapPath(path.Start(0), new Vector2(randX, path.End(0).y + 16), 2);
            //    honeycombChunks[honeycombChunks.Count - 1].AddVoid(path);
            //    honeycombChunks[honeycombChunks.Count - 1].DisplayChunk();
            //}
        }
        else
        {
            MurderPanel.SetActive(true);
        }
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

    public bool HoneycombTowerSpawnEnemy(MapHoneycomb tower) {
        
        return PlayerHandler.eggCount > 4 || PlayerHandler.royalJellyCount > 5;
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
}
