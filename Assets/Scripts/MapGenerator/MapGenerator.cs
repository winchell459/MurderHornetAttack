using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private List<MapVoid> mapVoids = new List<MapVoid>();

    public GameObject EnemyPrefabs;

    public GameObject[] flowerPetals;
    private List<GameObject> placedFlowerPetals = new List<GameObject>();

    public Transform AntSquad;
    public bool createAntFarm = false;

    public PerlinNoise perlinNoise;
    public MapGeneratorParameters parameters;

    public bool generating = false;

    public IEnumerator GenerateMap(Transform Player)
    {
        generating = true;
        //createVoids();
        float start = Utility.Utility.GetTime();
        if(parameters.generationType == MapGeneratorParameters.GenerationTypes.randomVoids)
        {
            createRandomMap(Player/*, 10*/);
            
            Debug.Log("createRandomMap Time: " + (Utility.Utility.GetTime() - start) + " seconds.");
        }   
        else if(parameters.generationType == MapGeneratorParameters.GenerationTypes.perlinNoise)
        {
            StartCoroutine(CreatePerlinNoiseMap(Player));
            do { yield return null; }
            while (perlinNoiseGenerating);
        }else if(parameters.generationType == MapGeneratorParameters.GenerationTypes.pillapillarPit)
        {
            createPillapillarPit(Player, 1);
        }else if(parameters.generationType == MapGeneratorParameters.GenerationTypes.beeCity)
        {
            Player.position = new Vector3(65, 85);

            mapVoids.Add(new MapBeeCity(Player.position, Player.position + new Vector3(50,0), 4, 15,30));
            
            CreatePlayerSpawn(Player.position);
            
            LevelHandler.singleton.queen.transform.position = Player.position + new Vector3(50, 0);
            addPathEnemies();

            CreateExitTunnel(Player.position + new Vector3(50, 0));
            LevelHandler.singleton.ExitTunnel.gameObject.SetActive(false);

            Map.StaticMap.AddVoid(mapVoids);
            Map.StaticMap.AddVoid(newVoids);

            Debug.Log("createBeeCity Time: " + (Utility.Utility.GetTime() - start) + " seconds.");

            //Player.position = PlayerSpawn.transform.position + new Vector3(-5, 0);
        }


        
        start = Utility.Utility.GetTime();
        Map.StaticMap.SetupChunks();
        Debug.Log("DisplayChunks Time: " + (Utility.Utility.GetTime() - start) + " seconds.");

        //setup enemies in Paths
        addPathEnemies();
        generating = false;
    }


    List<MapVoid> newVoids = new List<MapVoid>();

    bool perlinNoiseGenerating = false;

    public static PerlineNoiseVoid pregenerated;
    public static void PregeneratePerlineNoiseVoid(PerlinNoise perlinNoise, MapParameters mapParameters)
    {
        int mapWidth = (int)(mapParameters.MapWidth / mapParameters.HorizontalSpacing);
        int mapHeight = (int)(mapParameters.MapHeight / mapParameters.VerticalSpacing) / 2;
        pregenerated = new PerlineNoiseVoid(perlinNoise, mapWidth, mapHeight);
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
            CreateExitTunnel( Utility.Honeycomb.HoneycombGridToWorldPostion(exitHexPos));
            

            //Added player spawn point
            HoneycombPos playerHexSpawn = exitHexPos;
            CreatePlayerSpawn(Utility.Honeycomb.HoneycombGridToWorldPostion(playerHexSpawn));
            

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
               
                FillArea(perlineNoiseVoid,pillapillarHexPos, pillapillarPitSize, Map.MapObjects.pillapillarPit);
                perlineNoiseVoid.SetAreaType(pillapillarHexPos, HoneycombTypes.Areas.Garden);


                int spiderHoleSize = 5;
                HoneycombPos SpiderNestHexPos = perlineNoiseVoid.GetAreaPos(spiderHoleSize, true);
                if (SpiderNestHexPos == new HoneycombPos(-1, -1)) break;
                FillArea(perlineNoiseVoid,SpiderNestHexPos, spiderHoleSize, Map.MapObjects.spiderHole);
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

        CreatePlayerSpawn(Player.position);

        Map map = Map.StaticMap;


        //create snake Chamber
        Vector2 snakeChamberLoc = Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(150, 80));
        CreateCaterpillarGarden(snakeChamberLoc, true);

        CreateExitTunnel( Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(200, 200)));



        //connect chambers
        connectChambers(newVoids);

        map.AddVoid(newVoids);

        mapVoids = newVoids;
        //Debug.Log("void wall count: " + newVoids[newVoids.Count - 1].GetVoidWalls().Count);




    }

    private void createRandomMap(Transform Player/*, int voidCount*/)
    {

        newVoids.Clear();
        //newConnected.Clear();
        //newLocations.Clear();

        Map map = Map.StaticMap;
        Vector2 origin = new Vector2(map.MapOrigin.x * map.HorizontalSpacing, map.MapOrigin.y * map.VerticalSpacing);
        Vector2 mapMin = origin + new Vector2(15, 15);
        Vector2 mapMax = origin + new Vector2(map.MapWidth, map.MapHeight) - new Vector2(15, 15);

        Debug.Log($"map origin: {origin} mapMin: {mapMin} mapMax:{mapMax}");

        int antMounds = parameters.antMoundCount;
        int voidCount = parameters.gardenCount + parameters.nestCount + parameters.antMoundCount;
        Vector2[] voidLocations = new Vector2[voidCount + 2 ];
        float[] locationRadius = new float[voidCount + 2];
        locationRadius[0] = 5;
        locationRadius[1] = 20;
        for(int i = 2; i < voidCount + 2; i++)
        {
            locationRadius[i] = 30;
        }

        for(int i = voidCount + 2; i < locationRadius.Length; i++)
        {
            locationRadius[i] = 5;
        }



        GetRandomLocation(voidLocations, locationRadius, mapMin, mapMax);

        //Added player spawn point
        CreatePlayerSpawn(voidLocations[0],5);

        HoneycombPos exitHex = Utility.Honeycomb.WorldPointToHoneycombGrid(voidLocations[1]);
        CreateExitTunnel(voidLocations[1]);

        if(antMounds > 0)
        {
            AntSquad.position = voidLocations[voidLocations.Length - antMounds];
            HoneycombPos[] antMoundLocations = new HoneycombPos[antMounds];

            for (int i = 0; i < antMounds; i++)
            {
                antMoundLocations[i] = Utility.Honeycomb.WorldPointToHoneycombGrid(voidLocations[i + voidLocations.Length - antMounds]);
                //Debug.Log($"ant mound location: {antMoundLocations[i].worldPos} index:{i + voidLocations.Length - antMounds} count:{voidLocations.Length}");
            }
            CreateAntFarm(Utility.Honeycomb.WorldPointToHoneycombGrid(AntSquad.position), exitHex, antMoundLocations);

            for (int i = 1; i < antMoundLocations.Length; i++)
            {
                if (UnplacedFlowerPetals())
                {
                    GetFlowerPetalDrop().transform.position = Utility.Honeycomb.HoneycombGridToWorldPostion(antMoundLocations[i]);
                    MiniMap.singleton.SetFlower(antMoundLocations[i], true);
                }
            }
        }
        
        
        
        for(int i = 0; i < parameters.nestCount; i++)
        {
            CreateSpiderNest(voidLocations[i + 2]);
        }

        for (int i = 0; i < parameters.gardenCount; i++)
        {
            Vector2 snakeChamberLoc = voidLocations[i +parameters.nestCount +2];//Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(150, 80));
            CreateCaterpillarGarden(snakeChamberLoc, true);
        }

        

        string locationstr = "";
        foreach(Vector2 location in voidLocations)
        {
            locationstr += location + " ";
        }
        Debug.Log(locationstr);

        locationstr = "";
        foreach (float radius in locationRadius)
        {
            locationstr += radius + " ";
        }
        Debug.Log(locationstr);

        //connect chambers
        connectChambers(newVoids);

        map.AddVoid(newVoids);

        mapVoids = newVoids;
        //Debug.Log("void wall count: " + newVoids[newVoids.Count - 1].GetVoidWalls().Count);




    }

    void GetRandomLocation(Vector2[] locations, float[] locationRadius, Vector2 min, Vector2 max)
    {
        int breakCounter = 1000;
        bool done = false;
        while(!done && breakCounter > 0)
        {
            for(int i = 0; i < locations.Length; i++)
            {
                bool valid = true;
                bool done2 = false;
                Vector2 newLoc = Vector2.zero;
                int breakCounter2 = 10000;
                while (!done2 && breakCounter2 > 0)
                {
                    float x = Random.Range(min.x, max.x);
                    float y = Random.Range(min.y, max.y);
                    newLoc = new Vector2(x, y);
                    
                    for (int j = 0; j < i; j++)
                    {
                        if (Vector2.Distance(newLoc, locations[j]) < Mathf.Max(locationRadius[i],locationRadius[j]))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        done2 = true;
                    }
                    breakCounter2--;
                }

                if (valid)
                {
                    locations[i] = newLoc;
                    if (i == locations.Length - 1) done = true;
                }
                else break;
            }

            breakCounter--;
        }
        if(breakCounter <= 0)Debug.LogWarning($"breakCounter");
    }



    private void addPathEnemies()
    {
        foreach (MapVoid mv in mapVoids)
        {
            //Debug.Log(mv.VoidType);
            if (mv.VoidType == HoneycombTypes.Variety.Path)
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
    public void CreatePlayerSpawn(Vector2 position, float radius)
    {
        MapSpawn spawnChamber = MapChamber.PlayerSpawnChamber(position, radius);
        newVoids.Add(spawnChamber);
    }
    public void CreatePlayerSpawn(Vector2 position)
   {
        CreatePlayerSpawn(position, 3);
    }

    public void CreateExitTunnel(Vector2 position)
    {
        MapExit endChamber = MapChamber.EndChamberTunnel(position, 8);

        newVoids.Add(endChamber);
    }

    public void CreateCaterpillarGarden(Vector2 position, bool random)
    {
        if (random)
        {
            MapGarden garden = MapGarden.CreateRandomGarden(position, 15);
            newVoids.Add(garden);
        }
        
    }

    void FillArea(PerlineNoiseVoid perlineNoiseVoid, HoneycombPos hexPos, float unitSize, Map.MapObjects mapObject)
    {
        PerlinNoiseArea area = perlineNoiseVoid.GetPerlinNoiseArea(hexPos).parentArea;
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
            perlineNoiseVoid.AddMapObject(mapObject, valid.pos);
            
        }
    }

    public void CreateSpiderNest(Vector2 position)
    {
        MapNest nest = MapNest.CreateRandomNest(position, 5, 10);
        connectChambers(nest);
        newVoids.Add(nest);
    }

    
    public void CreateAntFarm(HoneycombPos position)
    {
        //Debug.Log($"ant farm start {position}");
        MapFarm farm = MapFarm.CreateRandomMaze(position, position + new HoneycombPos(50, 50), 2, 7/*, ChamberAntFarmTriggerPrefab.gameObject*/);
        newVoids.Add(farm);
    }
    public void CreateAntFarm(HoneycombPos position, HoneycombPos end, HoneycombPos[] moundLocations)
    {

        MapFarm farm = MapFarm.CreateRandomMazeWithPoints(position, end, 2, moundLocations/*, ChamberAntFarmTriggerPrefab.gameObject*/);
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
                    //Debug.Log($"{((MapChamber)chambers[connecting]).GetType()}");
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
