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

    private Portal PlayerSpawn;
    private Portal Exit;

    public Text PlayerLoc, SpawnLoc, EndLoc;

    public CameraController Cam;

    // Start is called before the first frame update
    void Start()
    {
        MurderPanel.SetActive(false);
        ExitPanel.SetActive(false);
        //createVoids();
        createRandomMap(7);

        Map.StaticMap.DisplayChunks();
        Map.StaticMap.Display = true;
    }

    // Update is called once per frame
    void Update()
    {
        //infiniteLevel();
        if (Exit && Exit.inPortal) {
            ExitPanel.SetActive(true);
            if( Input.GetKeyDown(KeyCode.E)) ReloadLevel();
        }
        else
        {
            ExitPanel.SetActive(false);
        }

        if(Player) displayLocation(Map.StaticMap.WorldToHoneycomb(Player.position), PlayerLoc);
        else
        {
            // spawnPlayer(PlayerSpawn);
            MurderPanel.SetActive(true);
        }
        displayLocation(Map.StaticMap.WorldToHoneycomb(Exit.transform.position), EndLoc);
        displayLocation(Map.StaticMap.WorldToHoneycomb(PlayerSpawn.transform.position), SpawnLoc);
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
        addPortal(PlayerSpawn, spawnChamber);
        newVoids.Add(spawnChamber);
        connected.Add(false);

        Map map = Map.StaticMap;
        Vector2 origin = new Vector2(map.MapOrigin.x * map.HorizontalSpacing, map.MapOrigin.y * map.VerticalSpacing);
        Vector2 mapMin = origin + new Vector2(15, 15);
        Vector2 mapMax = origin + new Vector2(map.MapWidth, map.MapHeight) - new Vector2(15, 15);

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
        Exit = Instantiate(PortalPrefab, endChamber.Location, Quaternion.identity).GetComponent<Portal>();
        addPortal(Exit, endChamber);

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
    }

    private void addPortal(Portal portal, MapChamber chamber)
    {
        portal.Chamber = chamber;
        foreach(Vector2 loc in chamber.locations)
        {
            portal.gameObject.AddComponent<CircleCollider2D>();
            
        }
        CircleCollider2D[] colliders = portal.gameObject.GetComponents<CircleCollider2D>();
        for (int i = 0; i < colliders.Length; i+=1)
        {
            CircleCollider2D collider = colliders[i];
            collider.isTrigger = true;
            collider.radius = chamber.widths[i] / 2;
            collider.offset = chamber.locations[i] - (Vector2)portal.transform.position;

            GameObject circle = Instantiate(portal.CirclePrefab, chamber.locations[i], Quaternion.identity);
            circle.transform.localScale = new Vector2(chamber.widths[i], chamber.widths[i]);
            circle.GetComponent<SpriteRenderer>().color = Color.black;
            //Debug.Log(chamber.locations[i] + " " + chamber.widths[i]);
        }
    }
}
