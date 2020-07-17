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

    private Portal PlayerSpawn;
    private Portal Exit;

    public Text PlayerLoc, SpawnLoc, EndLoc;
    public Text BeesMurderedText, HornetMurderedText;
    public Text HealthMeterText;
    public RawImage HealthMeterBar;

    public Text PlasmaMeterText;
    public RawImage PlasmaMeterBar;

    public CameraController Cam;

    public GameObject EnemyPrefabs;
    public GameObject SnakePrefab;

    private List<MapVoid> mapVoids = new List<MapVoid>();
    private PlayerHandler ph;

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

        displayLocation(Utility.WorldToHoneycomb(Exit.transform.position), EndLoc);
        //displayLocation(Utility.WorldToHoneycomb(PlayerSpawn.transform.position), SpawnLoc);
    }

    // Update is called once per frame
    void Update()
    {
        //infiniteLevel();
        if (Exit && Exit.inChamber) {
            ExitPanel.SetActive(true);
            if( Input.GetKeyDown(KeyCode.E)) ReloadLevel();
        }
        else
        {
            ExitPanel.SetActive(false);
        }

        if (Player)
        {
            displayLocation(Utility.WorldToHoneycomb(Player.position), PlayerLoc);
            displayLocation(Map.StaticMap.GetChunkIndex( Utility.GetMapChunk(Player.position)), SpawnLoc);
            //Debug.Log(Utility.GetMapChunk(Player.position).ChunkIndex + " chunkOffset: " + Utility.GetMapChunk(Player.position).mapOffset);
        }
        else
        {
            // spawnPlayer(PlayerSpawn);
            MurderPanel.SetActive(true);
        }

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

    public void RestartLevel()
    {
        MurderPanel.SetActive(false);
        spawnPlayer(PlayerSpawn);
        
    }

    private void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
        PlayerHandler ph = FindObjectOfType<PlayerHandler>();
        BeesMurderedText.text = PlayerHandler.BeesMurderedCount.ToString();
        HornetMurderedText.text = PlayerHandler.HornetMurderedCount.ToString();

        HornetController hc = FindObjectOfType<HornetController>();

        float barPercent = hc.Health / ph.MaxHealth;
        HealthMeterBar.rectTransform.localScale = new Vector3(barPercent, 1, 1);
        HealthMeterText.text = hc.Health.ToString();

        float plasmaPercent = (float) hc.ShotCount / ph.MaxShot;
        PlasmaMeterBar.rectTransform.localScale = new Vector3(plasmaPercent, plasmaPercent, 1);
        PlasmaMeterText.text = hc.ShotCount.ToString();
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
        Vector2 snakeChamberLoc = Utility.HoneycombGridToWorldPostion(new Vector2(40, 40));
        MapChamber snakeChamber = MapChamber.RandomChamber(snakeChamberLoc, 20);
        //ChamberTrigger snakeChamberTrigger = Instantiate(ChamberTriggerPrefab, snakeChamberLoc, Quaternion.identity).GetComponent<ChamberTrigger>();
        //addChamberTrigger(snakeChamberTrigger, snakeChamber);
        ChamberTrigger.SetupChamberTrigger(ChamberTriggerPrefab, snakeChamber);
        newVoids.Add(snakeChamber);

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

        MapChamber endChamber = (MapChamber)newVoids[newVoids.Count - 1];
        for(int i = 1; i < voidCount - 1; i+=1)
        {
            if(Vector2.Distance(spawnChamber.Location, endChamber.Location) < Vector2.Distance(spawnChamber.Location, ((MapChamber)newVoids[i]).Location)) {
                endChamber = (MapChamber)newVoids[i];
            }
        }

        //setup Exit tunnel
        //Exit = Instantiate(PortalPrefab, endChamber.Location, Quaternion.identity).GetComponent<Portal>();
        //addChamberTrigger(Exit, endChamber);
        Exit = (Portal)ChamberTrigger.SetupChamberTrigger(PortalPrefab, endChamber);

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
