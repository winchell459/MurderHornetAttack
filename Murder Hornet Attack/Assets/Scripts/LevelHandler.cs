using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{
    public GameObject MurderPanel;
    public GameObject ExitPanel;
    public Transform Player;
    public GameObject PlayerPrefab;
    public GameObject PortalPrefab;
    public GameObject ChamberTriggerPrefab;

    public Transform ExitTunnel;
    public Transform SnakePit;

    private Portal PlayerSpawn;
    private Portal Exit;

    public Text PlayerLoc, SpawnLoc, EndLoc;
    public Text BeesMurderedText, HornetMurderedText;
    public Text HealthMeterText;
    public RawImage HealthMeterBar;

    public Text PlasmaMeterText;
    public RawImage PlasmaMeterBar;

    public Text PlasmaPowerText, PlasmaChargeRateText, PlasmaChargeCapacityText;

    public CameraController Cam;

    public GameObject EnemyPrefabs;
    public GameObject SnakePrefab;

    public GameObject EnemyDropPrefab;

    private List<MapVoid> mapVoids = new List<MapVoid>();
    private PlayerHandler ph;

    private bool levelEnding = false;

    
    

    // Start is called before the first frame update
    void Start()
    {
        MurderPanel.SetActive(false);
        ExitPanel.SetActive(false);
        UpdatePlayerStats();


        //createVoids();
        createRandomMap(10);

        Map.StaticMap.DisplayChunks();

        //setup enemies in Paths
        addPathEnemies();

        Map.StaticMap.Display = true;

        ph = FindObjectOfType<PlayerHandler>();

        //Debug.Log("honeycomb (0,0) " + Utility.HoneycombGridToWorldPostion(new Vector2(0, 0)));
        //Debug.Log("honeycomb (0,6) " + Utility.HoneycombGridToWorldPostion(new Vector2(0, 6)));
        //Debug.Log("honeycomb (3,0) " + Utility.HoneycombGridToWorldPostion(new Vector2(3, 0)));
        //Debug.Log("honeycomb (3,6) " + Utility.HoneycombGridToWorldPostion(new Vector2(3, 6)));
        //Debug.Log("honeycomb (6,6) " + Utility.HoneycombGridToWorldPostion(new Vector2(6, 6)));

        displayLocation(Utility.WorldPointToHoneycombGrid(Exit.transform.position), EndLoc);
        //displayLocation(Utility.WorldToHoneycomb(PlayerSpawn.transform.position), SpawnLoc);
    }

    // Update is called once per frame
    void Update()
    {
        //infiniteLevel();
        if (Exit && Exit.inChamber) {
            ExitPanel.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) && !levelEnding) LevelEndSequence();
        }
        else
        {
            ExitPanel.SetActive(false);
        }

        if (Player)
        {
            displayLocation(Utility.WorldPointToHoneycombGrid(Player.position), PlayerLoc);
            displayLocation(Map.StaticMap.GetChunkIndex( Utility.GetMapChunk(Player.position)), SpawnLoc);
            //Debug.Log(Utility.GetMapChunk(Player.position).ChunkIndex + " chunkOffset: " + Utility.GetMapChunk(Player.position).mapOffset);
        }
        else
        {
            // spawnPlayer(PlayerSpawn);
            MurderPanel.SetActive(true);
        }
        UpdatePlayerStats();
        //if(Player)Debug.Log("Player Chunk: " + Utility.GetMapChunk(Player.transform.position).mapOffset);
    }

    private void displayLocation(Vector2 loc, Text text)
    {
        int decimals = 2;
        float factor = Mathf.Pow(10, decimals);
        float x = (int)loc.x * factor;
        x = (float)x / factor;
        float y = (int)loc.y * factor;
        y /= factor;
        text.text = x + " " + y;

    }

    private void spawnPlayer(Portal portal)
    {
        Player = Instantiate(PlayerPrefab, PlayerSpawn.Chamber.locations[0], Quaternion.identity).transform;
        Cam.SetCameraTarget(Player);
        UpdatePlayerStats();
    }

    public void EnemyDeath(GameObject enemy)
    {
        int dropCheck = 8;
        if (enemy.GetComponent<SnakeController>()) dropCheck = 2;
        if(Random.Range(0,10) > dropCheck)
        {
            int dropItem = Random.Range(0, 10); //0-2 Health 3 Power 4-6 Storage 7-9 Rapid
            float power;
            float duration = Random.Range(1 , 7) * 5;
            if(dropItem < 7)
            {
                power = Random.Range(10, 30);
            }
            else
            {
                power = Random.Range(0.05f, 0.45f) * 2;
            }

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

    private float levelEndStart;
    private float levelEndCountdown = 6.0f;
    private void LevelEndSequence()
    {
        levelEnding = true;
        levelEndStart = Time.fixedTime;
        ExitTunnel.GetComponent<Animator>().SetTrigger("Activate");
        Map.StaticMap.Display = false;
        StartCoroutine("LevelEndCoroutine");
    }

    IEnumerator LevelEndCoroutine()
    {
        while (levelEndStart + levelEndCountdown > Time.fixedTime)
            yield return null;
        ReloadLevel();
    }

    public void RestartLevel()
    {
        MurderPanel.SetActive(false);
        spawnPlayer(PlayerSpawn);
        
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
        
        BeesMurderedText.text = PlayerHandler.BeesMurderedCount.ToString();
        HornetMurderedText.text = PlayerHandler.HornetMurderedCount.ToString();

        PlayerHandler ph = FindObjectOfType<PlayerHandler>();
        HornetController hc = FindObjectOfType<HornetController>();

        if(ph && hc)
        {
            float barPercent = hc.Health / ph.GetMaxHealth();
            HealthMeterBar.rectTransform.localScale = new Vector3(barPercent, 1, 1);
            HealthMeterText.text = hc.Health.ToString();

            float plasmaPercent = (float)hc.ShotCount / ph.GetMaxShot();
            PlasmaMeterBar.rectTransform.localScale = new Vector3(plasmaPercent, plasmaPercent, 1);
            PlasmaMeterText.text = hc.ShotCount.ToString();

            PlasmaPowerText.text = (int)ph.GetPlasmaPower() + " " + (int)ph.GetPlasmaPowerBuffTime();
            PlasmaChargeRateText.text = ph.GetPlasmaChargeRate() + " " + (int)ph.GetPlasmaChargeRateBuffTime();
            PlasmaChargeCapacityText.text = ph.GetMaxShot() + " " + (int)ph.GetMaxShotBuffTime();
        }
        
    }

    public void UpdatePlayerStats(int BeesDied, int HornetDied)
    {
        PlayerHandler.BeesMurderedCount += BeesDied;
        PlayerHandler.HornetMurderedCount += HornetDied;
        UpdatePlayerStats();
    }

    private void createVoids()
    {
        //Debug.Log("magnitude of zero vector: " + Vector2.zero.normalized);
        MapPath newPath = MapPath.CreateJoggingPath(Player.position, new Vector2(35, 3f), -5, 5, 1, 3, 2, 4);
        Map.StaticMap.AddVoid(newPath);

        newPath = MapPath.CreateJoggingPath(Player.position, new Vector2(-10, -10f), -5, 5, 1, 3, 2, 4);
        Map.StaticMap.AddVoid(newPath);

        MapChamber newChamber = new MapChamber(new Vector2(35,5));
        newChamber.AddChamber(new Vector2(35, 5), 10);
        newChamber.AddChamber(new Vector2(30, 3), 6);
        Map.StaticMap.AddVoid(newChamber);

        
        Map.StaticMap.AddVoid(MapChamber.RandomChamber(new Vector2(-20, 20), 10));
    }

    private void createRandomMap(float voidCount)
    {
        List<MapVoid> newVoids = new List<MapVoid>();
        List<bool> connected = new List<bool>();

        List<Vector2> locations = new List<Vector2>();

        //Added player spawn point
        locations.Add(Player.position);
        MapChamber spawnChamber = MapChamber.RandomChamber(Player.position, 3);
        PlayerSpawn = Instantiate(PortalPrefab, spawnChamber.Location, Quaternion.identity).GetComponent<Portal>();
        Player.position = spawnChamber.locations[0];
        //addChamberTrigger(PlayerSpawn, spawnChamber);
        PlayerSpawn = (Portal)ChamberTrigger.SetupChamberTrigger(PortalPrefab, spawnChamber);
        newVoids.Add(spawnChamber);
        connected.Add(false);

        Map map = Map.StaticMap;
        Vector2 origin = new Vector2(map.MapOrigin.x * map.HorizontalSpacing, map.MapOrigin.y * map.VerticalSpacing);
        Vector2 mapMin = origin + new Vector2(15, 15);
        Vector2 mapMax = origin + new Vector2(map.MapWidth, map.MapHeight) - new Vector2(15, 15);

        //create snake Chamber
        Vector2 snakeChamberLoc = Utility.HoneycombGridToWorldPostion(new Vector2(150, 100));
        MapChamber snakeChamber = MapChamber.RandomChamber(snakeChamberLoc, 15);
        //ChamberTrigger snakeChamberTrigger = Instantiate(ChamberTriggerPrefab, snakeChamberLoc, Quaternion.identity).GetComponent<ChamberTrigger>();
        //addChamberTrigger(snakeChamberTrigger, snakeChamber);
        ChamberTrigger.SetupChamberTrigger(ChamberTriggerPrefab, snakeChamber);
        newVoids.Add(snakeChamber);
        SnakePit.position = snakeChamberLoc;

        //create random chambers
        for(int i = 0; i < voidCount; i += 1)
        {
            float xLoc = Random.Range(mapMin.x, mapMax.x);
            float yLoc = Random.Range(mapMin.y, mapMax.y);
            float radius = Random.Range(5, 15);
            newVoids.Add(MapChamber.RandomChamber(new Vector2(xLoc, yLoc), radius));
            connected.Add(false);

            locations.Add(new Vector2(xLoc, yLoc));
        }

        //MapChamber endChamber = (MapChamber)newVoids[newVoids.Count - 1];
        //for(int i = 1; i < voidCount - 1; i+=1)
        //{
        //    if(Vector2.Distance(spawnChamber.Location, endChamber.Location) < Vector2.Distance(spawnChamber.Location, ((MapChamber)newVoids[i]).Location)) {
        //        endChamber = (MapChamber)newVoids[i];
        //    }
        //}
        MapChamber endChamber = MapChamber.EndChamberTunnel(Player.position, 8);
        newVoids.Add(endChamber);
        connected.Add(false);
        locations.Add(Utility.WorldPointToHoneycombPos(Player.position));
        //setup Exit tunnel
        //Exit = Instantiate(PortalPrefab, endChamber.Location, Quaternion.identity).GetComponent<Portal>();
        //addChamberTrigger(Exit, endChamber);
        Exit = (Portal)ChamberTrigger.SetupChamberTrigger(PortalPrefab, endChamber);
        ExitTunnel.position = Exit.Chamber.Location;



        //connect chambers
        for(int i = 0; i < voidCount; i += 1)
        {
            while (!connected[i])
            {
                int connecting = (int)Random.Range(0, voidCount - 1); 
                if(connecting != i)
                {
                    newVoids.Add(MapPath.CreateJoggingPath(((MapChamber)newVoids[i]).ClosestEntrancePoint(locations[connecting]), locations[connecting], -2, 2, 2, 6, 2, 2));
                    connected[i] = true;
                }
            }
        }

        map.AddVoid(newVoids);

        mapVoids = newVoids;
        //Debug.Log("void wall count: " + newVoids[newVoids.Count - 1].GetVoidWalls().Count);

        


    }

    //private void addChamberTrigger(ChamberTrigger trigger, MapChamber chamber)
    //{
    //    trigger.Chamber = chamber;
    //    foreach (Vector2 loc in chamber.locations)
    //    {
    //        trigger.gameObject.AddComponent<CircleCollider2D>();

    //    }
    //    CircleCollider2D[] colliders = trigger.gameObject.GetComponents<CircleCollider2D>();
    //    for (int i = 0; i < colliders.Length; i += 1)
    //    {
    //        CircleCollider2D collider = colliders[i];
    //        collider.isTrigger = true;
    //        collider.radius = chamber.widths[i] / 2;
    //        collider.offset = chamber.locations[i] - (Vector2)trigger.transform.position;

    //        GameObject circle = Instantiate(trigger.CirclePrefab, chamber.locations[i], Quaternion.identity);
    //        circle.transform.localScale = new Vector2(chamber.widths[i], chamber.widths[i]);
    //        circle.GetComponent<SpriteRenderer>().color = Color.black;
    //        //Debug.Log(chamber.locations[i] + " " + chamber.widths[i]);
    //    }
    //}

    private void addPathEnemies()
    {
        foreach (MapVoid mv in mapVoids)
        {
            if (mv.VoidType == MapHoneycomb.LocationTypes.Path)
            {
                List<MapHoneycomb> walls = mv.GetVoidWalls();
                //Debug.Log(walls.Count);
                foreach (MapHoneycomb mhc in walls)
                {
                    if (Random.Range(0, 10) < 1)
                    {
                        mhc.AddEnemy(EnemyPrefabs);
                        //Debug.Log("Enemy Added");
                    }
                }
            }
        }
    }
}
