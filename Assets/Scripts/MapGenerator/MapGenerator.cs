using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private List<MapVoid> mapVoids = new List<MapVoid>();

    public GameObject PortalPrefab;
    public GameObject ChamberTriggerPrefab;
    public GameObject EnemyPrefabs;
    public GameObject SnakePrefab;
    public ChamberAntFarmTrigger ChamberAntFarmTriggerPrefab;

    public Transform ExitTunnel;
    public GameObject[] flowerPetals;
    private List<GameObject> placedFlowerPetals = new List<GameObject>();
    public Transform SnakePit;
    public GameObject SpiderHole;
    public Transform AntSquad;
    public bool createAntFarm = false;

    public Portal PlayerSpawn { get; set; }
    public Portal Exit { get; set; }


    public PerlinNoise perlinNoise;
    public enum GenerationTypes { randomVoids, perlinNoise, pillapillarPit}
    public GenerationTypes generationType;

    public bool generating = false;

    public IEnumerator GenerateMap(Transform Player)
    {
        generating = true;
        //createVoids();
        float start = Utility.Utility.GetTime();
        if(generationType == GenerationTypes.randomVoids)
        {
            createRandomMap(Player, 10);
            
            Debug.Log("createRandomMap Time: " + (Utility.Utility.GetTime() - start) + " seconds.");
        }   
        else if(generationType == GenerationTypes.perlinNoise)
        {
            StartCoroutine(CreatePerlinNoiseMap(Player));
            do { yield return null; }
            while (perlinNoiseGenerating);
        }else if(generationType == GenerationTypes.pillapillarPit)
        {
            createPillapillarPit(Player, 1);
        }


        
        start = Utility.Utility.GetTime();
        Map.StaticMap.DisplayChunks();
        Debug.Log("DisplayChunks Time: " + (Utility.Utility.GetTime() - start) + " seconds.");

        //setup enemies in Paths
        addPathEnemies();
        generating = false;
    }

    private void createVoids(Transform Player)
    {
        //Debug.Log("magnitude of zero vector: " + Vector2.zero.normalized);
        MapPath newPath = MapPath.CreateJoggingPath(Player.position, new Vector2(35, 3f), -5, 5, 1, 3, 2, 4);
        Map.StaticMap.AddVoid(newPath);

        newPath = MapPath.CreateJoggingPath(Player.position, new Vector2(-10, -10f), -5, 5, 1, 3, 2, 4);
        Map.StaticMap.AddVoid(newPath);

        MapChamber newChamber = new MapChamber(new Vector2(35, 5));
        newChamber.AddChamber(new Vector2(35, 5), 10);
        newChamber.AddChamber(new Vector2(30, 3), 6);
        Map.StaticMap.AddVoid(newChamber);


        Map.StaticMap.AddVoid(MapChamber.RandomChamber(new Vector2(-20, 20), 10));
    }
    List<MapVoid> newVoids = new List<MapVoid>();

    bool perlinNoiseGenerating = false;

    public static PerlineNoiseVoid pregenerated;
    public static void PregeneratePerlineNoiseVoid(PerlinNoise perlinNoise, MapParameters mapParameters)
    {
        int mapWidth = (int)(mapParameters.MapWidth / mapParameters.HorizontalSpacing);
        int mapHeight = (int)(mapParameters.MapHeight / mapParameters.VerticalSpacing) / 2;
        pregenerated = new PerlineNoiseVoid(perlinNoise, mapWidth, mapHeight);
        //return pregenerated;
    }
    private IEnumerator CreatePerlinNoiseMap(Transform player)
    {
        perlinNoiseGenerating = true;
        int mapWidth = (int)(Map.StaticMap.MapWidth / Map.StaticMap.HorizontalSpacing);
        int mapHeight = (int)(Map.StaticMap.MapHeight / Map.StaticMap.VerticalSpacing)/2;
        PerlineNoiseVoid perlineNoiseVoid = pregenerated != null? pregenerated : new PerlineNoiseVoid(perlinNoise, mapWidth, mapHeight);
        pregenerated = null;
        while (perlineNoiseVoid.generating) yield return null;

        if (true)
        {
            System.Random random = new System.Random(perlinNoise.seed);
            

            HoneycombPos exitHexPos = perlineNoiseVoid.GetAreaPos(8, false);
            Exit = CreateExitTunnel(PortalPrefab, Utility.Honeycomb.HoneycombGridToWorldPostion(exitHexPos));
            ExitTunnel.position = Exit.Chamber.Location;

            //Added player spawn point
            HoneycombPos playerHexSpawn = exitHexPos;// perlineNoiseVoid.GetAreaPos(3, true);
            PlayerSpawn = CreatePlayerSpawn(PortalPrefab, Utility.Honeycomb.HoneycombGridToWorldPostion(playerHexSpawn));
            player.position = PlayerSpawn.Chamber.locations[0];

            bool areasFilled = false;
            int count = 0;
            while (!areasFilled && count < 20)
            {
                count++;
                //create snake Chamber
                int pillapillarPitSize = 10;
                HoneycombPos pillapillarHexPos = perlineNoiseVoid.GetAreaPos(pillapillarPitSize, true); 
                
                if (pillapillarHexPos == new HoneycombPos(-1, -1)) break;
                pillapillarHexPos = perlineNoiseVoid.GetPerlinNoiseArea(pillapillarHexPos).parentArea.pos;
                Vector2 snakeChamberLoc = Utility.Honeycomb.HoneycombGridToWorldPostion(pillapillarHexPos);
                //CreateCaterpillarGarden(ChamberTriggerPrefab, snakeChamberLoc, false);
                FillArea(perlineNoiseVoid.GetPerlinNoiseArea(pillapillarHexPos), pillapillarPitSize, SnakePit.gameObject);
                perlineNoiseVoid.SetAreaType(pillapillarHexPos, HoneycombTypes.Areas.Garden);


                int spiderHoleSize = 5;
                HoneycombPos SpiderNestHexPos = perlineNoiseVoid.GetAreaPos(spiderHoleSize, true);
                if (SpiderNestHexPos == new HoneycombPos(-1, -1)) break;
                //CreateSpiderNest(Utility.Honeycomb.HoneycombGridToWorldPostion(SpiderNestHexPos));
                FillArea(perlineNoiseVoid.GetPerlinNoiseArea(SpiderNestHexPos).parentArea, spiderHoleSize, SpiderHole);
                perlineNoiseVoid.SetAreaType(SpiderNestHexPos, HoneycombTypes.Areas.Nest);
                

                Debug.Log($"pillapillarHexPos: {pillapillarHexPos} | spiderNestHexPos: {SpiderNestHexPos}");
                //createRandomMap(player,10);
            }

            // ---------------------- Place Flower Petals -----------------------------
            List<PerlinNoiseChamber> unusedChambers = perlineNoiseVoid.GetUnusedChambers();
            while(UnplacedFlowerPetals() && unusedChambers.Count > 0)
            {
                PerlinNoiseChamber chamber = unusedChambers[0];
                unusedChambers.RemoveAt(0);
                HoneycombPos petalHex = perlineNoiseVoid.GetAreaPos(2, chamber);
                if(petalHex != new HoneycombPos(-1, -1))
                {
                    GetFlowerPetalDrop().transform.position = Utility.Honeycomb.HoneycombGridToWorldPostion(petalHex);
                    MiniMap.singleton.SetFlower(petalHex, true);
                }
            }

            if(createAntFarm)CreateAntFarm(Utility.Honeycomb.WorldPointToHoneycombGrid(AntSquad.position));
        }



        mapVoids = newVoids;
        mapVoids.Add(perlineNoiseVoid);
        Map.StaticMap.AddVoid(mapVoids);
        perlinNoiseGenerating = false;
    }
    public bool UnplacedFlowerPetals() { return flowerPetals.Length > placedFlowerPetals.Count; }
    public GameObject GetFlowerPetalDrop()
    {
        if (UnplacedFlowerPetals())
        {
            GameObject flowerPetal = flowerPetals[placedFlowerPetals.Count];
            placedFlowerPetals.Add(flowerPetals[placedFlowerPetals.Count]);
            return flowerPetal;
        }
        else return null;
    }

    private void createPillapillarPit(Transform Player, float voidCount)
    {

        newVoids.Clear();

        PlayerSpawn = CreatePlayerSpawn(PortalPrefab, Player.position);
        Player.position = PlayerSpawn.Chamber.locations[0];

        Map map = Map.StaticMap;


        //create snake Chamber
        Vector2 snakeChamberLoc = Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(150, 80));
        CreateCaterpillarGarden(ChamberTriggerPrefab, snakeChamberLoc, true);


        //Exit = CreateExitTunnel(PortalPrefab, Player.position);
        Exit = CreateExitTunnel(PortalPrefab, Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(200, 200)));
        ExitTunnel.position = Exit.Chamber.Location;



        //connect chambers
        connectChambers(newVoids);

        map.AddVoid(newVoids);

        mapVoids = newVoids;
        //Debug.Log("void wall count: " + newVoids[newVoids.Count - 1].GetVoidWalls().Count);




    }

    private void createRandomMap(Transform Player, float voidCount)
    {

        newVoids.Clear();
        //newConnected.Clear();
        //newLocations.Clear();

        //Added player spawn point
        PlayerSpawn = CreatePlayerSpawn(PortalPrefab, Player.position);
        Player.position = PlayerSpawn.Chamber.locations[0];


        CreateSpiderNest(Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(75, 105)));
        CreateAntFarm(Utility.Honeycomb.WorldPointToHoneycombGrid(AntSquad.position));


        Map map = Map.StaticMap;
        Vector2 origin = new Vector2(map.MapOrigin.x * map.HorizontalSpacing, map.MapOrigin.y * map.VerticalSpacing);
        Vector2 mapMin = origin + new Vector2(15, 15);
        Vector2 mapMax = origin + new Vector2(map.MapWidth, map.MapHeight) - new Vector2(15, 15);

        //create snake Chamber
        Vector2 snakeChamberLoc = Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(150, 80));
        CreateCaterpillarGarden(ChamberTriggerPrefab, snakeChamberLoc, true);

        //create random chambers
        //for (int i = 0; i < voidCount; i += 1)
        //{
        //    float xLoc = Random.Range(mapMin.x, mapMax.x);
        //    float yLoc = Random.Range(mapMin.y, mapMax.y);
        //    float radius = Random.Range(5, 15);
        //    newVoids.Add(MapChamber.RandomChamber(new Vector2(xLoc, yLoc), radius));
        //    //newConnected.Add(false);

        //    //newLocations.Add(new Vector2(xLoc, yLoc));
        //}

        //MapChamber endChamber = (MapChamber)newVoids[newVoids.Count - 1];
        //for(int i = 1; i < voidCount - 1; i+=1)
        //{
        //    if(Vector2.Distance(spawnChamber.Location, endChamber.Location) < Vector2.Distance(spawnChamber.Location, ((MapChamber)newVoids[i]).Location)) {
        //        endChamber = (MapChamber)newVoids[i];
        //    }
        //}



        //Exit = CreateExitTunnel(PortalPrefab, Player.position);
        Exit = CreateExitTunnel(PortalPrefab, Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(200, 200)));
        ExitTunnel.position = Exit.Chamber.Location;



        //connect chambers
        connectChambers(newVoids);

        map.AddVoid(newVoids);

        mapVoids = newVoids;
        //Debug.Log("void wall count: " + newVoids[newVoids.Count - 1].GetVoidWalls().Count);




    }



    private void addPathEnemies()
    {
        foreach (MapVoid mv in mapVoids)
        {
            Debug.Log(mv.VoidType);
            if (mv.VoidType == HoneycombTypes.Variety.Path)
            {
                List<MapHoneycomb> walls = mv.GetVoidWalls();
                Debug.Log(walls.Count);
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
    public  Portal CreatePlayerSpawn(GameObject PortalPrefab, Vector2 position)
   {
        MapChamber spawnChamber = MapChamber.RandomChamber(position, 3);
        Portal PlayerSpawn = (Portal)ChamberTrigger.SetupChamberTrigger(PortalPrefab, spawnChamber);

        //newLocations.Add(position);
        newVoids.Add(PlayerSpawn.Chamber);
        //newConnected.Add(false);
        return PlayerSpawn;
    }

    public  Portal CreateExitTunnel(GameObject PortalPrefab, Vector2 position)
    {
        MapChamber endChamber = MapChamber.EndChamberTunnel(position, 8);

        newVoids.Add(endChamber);
        //newConnected.Add(false);
        //newLocations.Add(Utility.WorldPointToHoneycombPos(position));
        return (Portal)ChamberTrigger.SetupChamberTrigger(PortalPrefab, endChamber);
    }

    public void CreateCaterpillarGarden(GameObject ChamberTriggerPrefab, Vector2 position, bool random)
    {

        //MapChamber snakeChamber = MapChamber.RandomChamber(position, 15);
        ////ChamberTrigger snakeChamberTrigger = Instantiate(ChamberTriggerPrefab, snakeChamberLoc, Quaternion.identity).GetComponent<ChamberTrigger>();
        ////addChamberTrigger(snakeChamberTrigger, snakeChamber);
        //ChamberTrigger.SetupChamberTrigger(ChamberTriggerPrefab, snakeChamber);
        //newVoids.Add(snakeChamber);
        if (random)
        {
            MapGarden garden = MapGarden.CreateRandomGarden(position, 15, ChamberTriggerPrefab);
            newVoids.Add(garden);
        }
        
        Transform newPit = Instantiate(SnakePit);
        newPit.position = position;
        newPit.gameObject.SetActive(true);
        SnakePit.gameObject.SetActive(false);
        //SnakePit.position = position;
    }

    void FillArea(PerlinNoiseArea area, float unitSize, GameObject unitPrefab)
    {
        List<PerlinNoiseArea> nonIntersecting = new List<PerlinNoiseArea>();
        nonIntersecting.Add(area);
        for(int i = 1; i < area.mergedAreas.Count; i++)
        {
            bool valid = true;
            PerlinNoiseArea validCheck = area.mergedAreas[i];
            foreach(PerlinNoiseArea validArea in nonIntersecting)
            {
                if (validArea.maxRadius < unitSize || Utility.Honeycomb.DistanceBetweenHoneycomb(validArea.pos, validCheck.pos) < unitSize * 2) valid = false;
            }
            if (valid) nonIntersecting.Add(validCheck);
        }
        foreach(PerlinNoiseArea valid in nonIntersecting)
        {
            GameObject newUnit = Instantiate(unitPrefab);
            newUnit.transform.position = Utility.Honeycomb.HoneycombGridToWorldPostion(valid.pos);
            newUnit.SetActive(true);
        }
    }

    public void CreateSpiderNest(Vector2 position)
    {
        MapNest nest = MapNest.CreateRandomNest(position, 5, 10, SpiderHole);
        connectChambers(nest);
        newVoids.Add(nest);
    }

    public void CreateAntFarm(HoneycombPos position)
    {
        MapFarm farm = MapFarm.CreateRandomMaze(position, position + new HoneycombPos(50, 50), 2, 7, ChamberAntFarmTriggerPrefab.gameObject);
        newVoids.Add(farm);
    }

    private void connectChambers(List<MapVoid> chambers)
    {
        int voidCount = chambers.Count ;
        for (int i = 0; i < voidCount; i += 1)
        {
            //Debug.Log("newConnected: " + newConnected.Count + " voidCount: " + voidCount);
            while (!((MapChamber)chambers[i]).Connected)
            {
                int connecting = (int)Random.Range(0, voidCount - 1);
                if (connecting != i)
                {
                    //chambers.Add(MapPath.CreateJoggingPath(((MapChamber)chambers[i]).ClosestEntrancePoint(newLocations[connecting]), newLocations[connecting], -2, 2, 2, 6, 2, 2));
                    chambers.Add(MapPath.CreateJoggingPath(((MapChamber)chambers[i]).ClosestEntrancePoint(((MapChamber)chambers[connecting]).Location), ((MapChamber)chambers[connecting]).Location, -2, 2, 2, 6, 3, 6));
                    ((MapChamber)chambers[i]).Connected = true;
                }
            }
        }
    }

    private void connectChambers(MapNest nest)
    {
        int voidCount = nest.chambers.Count - 1;
        for (int i = 0; i < voidCount; i += 1)
        {
            //Debug.Log("newConnected: " + newConnected.Count + " voidCount: " + voidCount);
            while (!((MapChamber)nest.chambers[i]).Connected)
            {
                int connecting = (int)Random.Range(0, voidCount - 1);
                if (connecting != i)
                {
                    //chambers.Add(MapPath.CreateJoggingPath(((MapChamber)chambers[i]).ClosestEntrancePoint(newLocations[connecting]), newLocations[connecting], -2, 2, 2, 6, 2, 2));
                    nest.paths.Add(MapPath.CreateJoggingPath((nest.chambers[i]).ClosestEntrancePoint((nest.chambers[connecting]).Location), ((MapChamber)nest.chambers[connecting]).Location, -2, 2, 2, 6, 2, 2));
                    ((MapChamber)nest.chambers[i]).Connected = true;
                }
            }
        }
    }
    
}
