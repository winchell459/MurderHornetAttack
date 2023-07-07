using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private List<MapVoid> mapVoids = new List<MapVoid>();

    public bool createAntFarm = false;

    //public PerlinNoise perlinNoise;
    //public MapGeneratorParameters parameters;

    private bool generating = false;

    public delegate void OnGenerationComplete();
    public static event OnGenerationComplete onGenerationComplete;
    public delegate void OnGenerationPreloadComplete(List<MapVoid> mapVoids);
    public static event OnGenerationPreloadComplete onGenerationPreloadComplete;

    public IEnumerator GenerateMap(MapGeneratorParameters parameters, MapParameters mapParameters, PerlinNoiseScriptableObject perlinNoiseParameters)
    {
        generating = true;
        Utility.Utility.seed = parameters.seed;
        
        float start = Utility.Utility.GetTime();
        if(parameters.generationType == MapGeneratorParameters.GenerationTypes.randomVoids)
        {
            mapVoids = createRandomMap(parameters, mapParameters);
            
            Debug.Log("createRandomMap Time: " + (Utility.Utility.GetTime() - start) + " seconds.");
        }   
        else if(parameters.generationType == MapGeneratorParameters.GenerationTypes.perlinNoise)
        {
            StartCoroutine(CreatePerlinNoiseMap( new PerlinNoise(perlinNoiseParameters), mapParameters,parameters));
            do { yield return null; }
            while (perlinNoiseGenerating);
        }else if(parameters.generationType == MapGeneratorParameters.GenerationTypes.pillapillarPit)
        {
            mapVoids = createPillapillarPit(new Vector2(0,0), 1, mapParameters);
        }else if(parameters.generationType == MapGeneratorParameters.GenerationTypes.beeCity)
        {
            Vector2 playerPos = new Vector3(65, 85);
            List<MapVoid> beeCity = CreateBeeCity(playerPos, mapParameters);

            StartCoroutine(CreatePerlinNoiseMap(new PerlinNoise(perlinNoiseParameters), mapParameters, parameters));
            do { yield return null; }
            while (perlinNoiseGenerating);

            

            foreach(MapVoid mapVoid in beeCity)
            {
                mapVoids.Add(mapVoid);
            }
            //mapVoids = CreateBeeCity(playerPos, mapParameters);

            Debug.Log("createBeeCity Time: " + (Utility.Utility.GetTime() - start) + " seconds.");

            //Player.position = PlayerSpawn.transform.position + new Vector3(-5, 0);
        }

        GenerateMap(mapVoids);
    }

    static MapGeneratorParameters parametersThread;
    static MapParameters mapParametersThread;
    

    public static void GenerateMap(MapGeneratorParameters parameters, MapParameters mapParameters/*, PerlinNoiseScriptableObject perlinNoiseParameters*/)
    {
        parametersThread = parameters;
        mapParametersThread = mapParameters;
        

        Thread generateMapThread = new Thread(GenerateMapThread);

        generateMapThread.Start();
    }
    private static void GenerateMapThread()
    {
        List<MapVoid> mapVoids = new List<MapVoid>();
        Utility.Utility.seed = parametersThread.seed;
        float start = Utility.Utility.GetTime();
        if (parametersThread.generationType == MapGeneratorParameters.GenerationTypes.randomVoids)
        {
            mapVoids = createRandomMap(parametersThread, mapParametersThread);

            Debug.Log("createRandomMap Time: " + (Utility.Utility.GetTime() - start) + " seconds.");
        }
        //else if (parameters.generationType == MapGeneratorParameters.GenerationTypes.perlinNoise)
        //{
        //    StartCoroutine(CreatePerlinNoiseMap(new PerlinNoise(perlinNoiseParameters), mapParameters));
        //    do { yield return null; }
        //    while (perlinNoiseGenerating);
        //}
        else if (parametersThread.generationType == MapGeneratorParameters.GenerationTypes.pillapillarPit)
        {
            Debug.Log(mapParametersThread == null);
            mapVoids = createPillapillarPit(new Vector2(0, 0), 1, mapParametersThread);
        }
        else if (parametersThread.generationType == MapGeneratorParameters.GenerationTypes.beeCity)
        {
            Vector2 playerPos = new Vector3(65, 85);

            mapVoids = CreateBeeCity(playerPos, mapParametersThread);

            Debug.Log("createBeeCity Time: " + (Utility.Utility.GetTime() - start) + " seconds.");

            //Player.position = PlayerSpawn.transform.position + new Vector3(-5, 0);
        }

        Debug.Log("Map Generated");
        onGenerationPreloadComplete?.Invoke(mapVoids);
    }

    public void GenerateMap(List<MapVoid> mapVoids)
    {
        Map.StaticMap.AddVoid(mapVoids);

        float start = Utility.Utility.GetTime();
        Map.StaticMap.SetupChunks();
        Debug.Log("DisplayChunks Time: " + (Utility.Utility.GetTime() - start) + " seconds.");

        //setup enemies in Paths
        //must be done after Map.StaticMap.SetupChunks(); 
        addPathEnemies(mapVoids);

        generating = false;
        onGenerationComplete?.Invoke();
    }
    


    public static List<MapVoid> CreateBeeCity(Vector2 playerPos, MapParameters mapParameters)
    {
        List<MapVoid> newVoids = new List<MapVoid>();
        newVoids.Add(new MapBeeCity(playerPos, playerPos + new Vector2(50, 0), 4, 15, 30, mapParameters));

        newVoids.Add(CreatePlayerSpawn(playerPos));

        


        newVoids.Add(CreateExitTunnel(playerPos + new Vector2(50, 0), mapParameters));
        

        return newVoids;
    }


    bool perlinNoiseGenerating = false;

    public static PerlineNoiseVoid pregenerated;
    public static void PregeneratePerlineNoiseVoid(PerlinNoise perlinNoise, MapParameters mapParameters)
    {
        int mapWidth = (int)(mapParameters.MapWidth / mapParameters.HorizontalSpacing);
        int mapHeight = (int)(mapParameters.MapHeight / mapParameters.VerticalSpacing) / 2;
        pregenerated = new PerlineNoiseVoid(perlinNoise, mapWidth, mapHeight);
    }
    private IEnumerator CreatePerlinNoiseMap(/*Transform player*/PerlinNoise perlinNoise, MapParameters mapParameters, MapGeneratorParameters parameters)
    {
        List<MapVoid> newVoids = new List<MapVoid>();

        perlinNoiseGenerating = true;
        int mapWidth = (int)(mapParameters.MapWidth / mapParameters.HorizontalSpacing);
        int mapHeight = (int)(mapParameters.MapHeight / mapParameters.VerticalSpacing)/2;
        PerlineNoiseVoid perlineNoiseVoid = pregenerated != null? pregenerated : new PerlineNoiseVoid(perlinNoise, mapWidth, mapHeight);
        pregenerated = null;
        while (perlineNoiseVoid.generating) yield return null;

        if (true)
        {
            //System.Random random = new System.Random(perlinNoise.seed);
            

            HoneycombPos exitHexPos = perlineNoiseVoid.GetAreaPos(8, false);
            newVoids.Add(CreateExitTunnel( Utility.Honeycomb.HoneycombGridToWorldPostion(exitHexPos), mapParameters));
            

            //Added player spawn point
            HoneycombPos playerHexSpawn = exitHexPos;
            newVoids.Add(CreatePlayerSpawn(Utility.Honeycomb.HoneycombGridToWorldPostion(playerHexSpawn)));
            

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
            int petalCount = 5;
            petalCount = parameters.antMoundCount;
            while(petalCount > 0 && unusedChambers.Count > 0)
            {
                PerlinNoiseChamber chamber = unusedChambers[0];
                unusedChambers.RemoveAt(0);
                HoneycombPos petalHex = perlineNoiseVoid.GetAreaPos(2, chamber);
                if(petalHex != new HoneycombPos(-1, -1))
                {
                    //GetFlowerPetalDrop().transform.position = Utility.Honeycomb.HoneycombGridToWorldPostion(petalHex);
                    //MiniMap.singleton.SetFlower(petalHex, true);
                    newVoids.Add(new MapPedal(petalHex));
                    petalCount--;
                }
            }

            //if(createAntFarm)CreateAntFarm(Utility.Honeycomb.WorldPointToHoneycombGrid(AntSquad.position));
        }



        mapVoids = newVoids;
        mapVoids.Add(perlineNoiseVoid);
        //Map.StaticMap.AddVoid(mapVoids);
        perlinNoiseGenerating = false;
    }


    private static List<MapVoid> createPillapillarPit(Vector2 playerPos, float voidCount, MapParameters mapParameters)
    {

        List<MapVoid> newVoids = new List<MapVoid>();

        newVoids.Add(CreatePlayerSpawn(playerPos));

        //Map map = Map.StaticMap;


        //create snake Chamber
        Vector2 snakeChamberLoc = Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(150, 80), mapParameters);
        newVoids.Add(CreatePillapillarGarden(snakeChamberLoc));

        newVoids.Add(CreateExitTunnel( Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(200, 200), mapParameters), mapParameters)); 

        //connect chambers
        connectChambers(newVoids);

        //map.AddVoid(newVoids);

        //mapVoids = newVoids;
        //Debug.Log("void wall count: " + newVoids[newVoids.Count - 1].GetVoidWalls().Count);


        return newVoids;

    }

    private static List<MapVoid> createRandomMap(MapGeneratorParameters parameters, MapParameters mapParameters)
    {

        List<MapVoid> newVoids = new List<MapVoid>();

        
        
        Vector2 origin = new Vector2(mapParameters.MapOrigin.x * mapParameters.HorizontalSpacing, mapParameters.MapOrigin.y * mapParameters.VerticalSpacing);
        Vector2 mapMin = origin + new Vector2(15, 15);
        Vector2 mapMax = origin + new Vector2(mapParameters.MapWidth, mapParameters.MapHeight) - new Vector2(15, 15);

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
        newVoids.Add(CreatePlayerSpawn(voidLocations[0],5));

        HoneycombPos exitHex = Utility.Honeycomb.WorldPointToHoneycombGrid(voidLocations[1], mapParameters);
        newVoids.Add(CreateExitTunnel(voidLocations[1], mapParameters));

        if(antMounds > 0)
        {
            //AntSquad.position = voidLocations[voidLocations.Length - antMounds];
            HoneycombPos[] antMoundLocations = new HoneycombPos[antMounds];

            for (int i = 0; i < antMounds; i++)
            {
                antMoundLocations[i] = Utility.Honeycomb.WorldPointToHoneycombGrid(voidLocations[i + voidLocations.Length - antMounds],mapParameters);
                //Debug.Log($"ant mound location: {antMoundLocations[i].worldPos} index:{i + voidLocations.Length - antMounds} count:{voidLocations.Length}");
            }
            Vector2 worldPos = voidLocations[voidLocations.Length - antMounds];
            newVoids.Add(CreateAntFarm(worldPos, Utility.Honeycomb.WorldPointToHoneycombGrid(worldPos, mapParameters), exitHex, antMoundLocations,mapParameters));

            for (int i = 1; i < antMoundLocations.Length; i++)
            {
                newVoids.Add(new MapPedal(antMoundLocations[i]));
            }
        }
        
        
        
        for(int i = 0; i < parameters.nestCount; i++)
        {
            newVoids.Add(CreateSpiderNest(voidLocations[i + 2], mapParameters));
        }

        for (int i = 0; i < parameters.gardenCount; i++)
        {
            Vector2 snakeChamberLoc = voidLocations[i +parameters.nestCount +2];//Utility.Honeycomb.HoneycombGridToWorldPostion(new HoneycombPos(150, 80));
            newVoids.Add(CreatePillapillarGarden(snakeChamberLoc));
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

        //map.AddVoid(newVoids);

        //mapVoids = newVoids;
        //Debug.Log("void wall count: " + newVoids[newVoids.Count - 1].GetVoidWalls().Count);



        return newVoids;
    }

    private static void GetRandomLocation(Vector2[] locations, float[] locationRadius, Vector2 min, Vector2 max)
    {
        //int seed = 0;
        //System.Random random = new System.Random(seed);
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
                    float x = Utility.Utility.Random(min.x, max.x);
                    float y = Utility.Utility.Random(min.y, max.y);
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



    private void addPathEnemies(List<MapVoid> mapVoids)
    {
        //System.Random random = new System.Random(seed);
        foreach (MapVoid mv in mapVoids)
        {
            //Debug.Log(mv.VoidType);
            if (mv.VoidType == HoneycombTypes.Variety.Path)
            {
                List<MapHoneycomb> walls = mv.GetVoidWalls();
                //Debug.Log(walls.Count);
                foreach (MapHoneycomb mhc in walls)
                {
                    if (Utility.Utility.Random(0, 10) < 1)
                    {
                        mhc.AddEnemy(true);
                        //Debug.Log("Enemy Added");
                    }
                }
            }
        }
    }
    public static MapVoid CreatePlayerSpawn(Vector2 position, float radius)
    {
        MapSpawn spawnChamber = MapChamber.PlayerSpawnChamber(position, radius);
        return spawnChamber;
    }
    public static MapVoid CreatePlayerSpawn(Vector2 position)
   {
        return CreatePlayerSpawn(position, 3);
    }

    public static MapVoid CreateExitTunnel(Vector2 position, MapParameters map)
    {
        MapExit endChamber = MapChamber.EndChamberTunnel(position, 8,map);

        return endChamber;
    }

    public static MapVoid CreatePillapillarGarden(Vector2 position/*, bool random*/)
    {
        MapGarden garden = MapGarden.CreateRandomGarden(position, 15);
        return garden;

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

    public static MapVoid CreateSpiderNest(Vector2 position, MapParameters mapParameters)
    {
        MapNest nest = MapNest.CreateRandomNest(position, 5, 10,mapParameters);
        connectChambers(nest);
        return nest;
    }

    
    //public static MapVoid CreateAntFarm(HoneycombPos position, MapParameters mapParameters)
    //{
    //    //Debug.Log($"ant farm start {position}");
    //    MapFarm farm = MapFarm.CreateRandomMaze(position, position + new HoneycombPos(50, 50), 2, 7, mapParameters);
    //    return farm;
    //}
    public static MapVoid CreateAntFarm(Vector2 worldPos, HoneycombPos position, HoneycombPos end, HoneycombPos[] moundLocations, MapParameters mapParameters)
    {

        MapFarm farm = MapFarm.CreateRandomMazeWithPoints(worldPos, position, end, 2, moundLocations, mapParameters);
        return farm;
    }

    private static void connectChambers(List<MapVoid> chambers)
    {
        //System.Random random = new System.Random(seed);
        int voidCount = chambers.Count ;
        for (int i = 0; i < voidCount; i += 1)
        {
            if (chambers[i].VoidType != HoneycombTypes.Variety.Chamber) continue;
            while (!((MapChamber)chambers[i]).Connected)
            {
                int connecting = Utility.Utility.Random(0, voidCount - 1);
                if (connecting != i && chambers[connecting].VoidType == HoneycombTypes.Variety.Chamber)
                {
                    Vector2 start = ((MapChamber)chambers[i]).ClosestEntrancePoint(((MapChamber)chambers[connecting]).Location);
                    Vector2 end = ((MapChamber)chambers[connecting]).Location;

                    chambers.Add(MapPath.CreateJoggingPath(start, end, -2, 2, 2, 6, 3, 6));
                    ((MapChamber)chambers[i]).Connected = true;
                }
                //Debug.Log("looping");
            }
        }
    }

    private static void connectChambers(MapNest nest)
    {
        //System.Random random = new System.Random(seed);
        int voidCount = nest.chambers.Count - 1;
        for (int i = 0; i < voidCount; i += 1)
        {
            //Debug.Log("newConnected: " + newConnected.Count + " voidCount: " + voidCount);
            while (!((MapChamber)nest.chambers[i]).Connected)
            {
                int connecting = Utility.Utility.Random(0, voidCount - 1);
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
