using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public GameObject MurderPanel;
    public Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        MurderPanel.SetActive(false);
        //createVoids();
        createRandomMap(7);

        Map.StaticMap.DisplayChunks();
        Map.StaticMap.Display = true;
    }

    // Update is called once per frame
    void Update()
    {
        infiniteLevel();
    }

    public void RestartLevel()
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

        MapChamber newChamber = new MapChamber();
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
        locations.Add(Player.position);
        newVoids.Add(MapChamber.RandomChamber(Player.position, 3));
        connected.Add(false);

        Map map = Map.StaticMap;
        Vector2 origin = new Vector2(map.MapOrigin.x * map.HorizontalSpacing, map.MapOrigin.y * map.VerticalSpacing);
        Vector2 mapMin = origin + new Vector2(15, 15);
        Vector2 mapMax = origin + new Vector2(map.MapWidth, map.MapHeight) - new Vector2(15, 15);

        
        for(int i = 0; i < voidCount; i += 1)
        {
            float xLoc = Random.Range(mapMin.x, mapMax.x);
            float yLoc = Random.Range(mapMin.y, mapMax.y);
            float radius = Random.Range(5, 15);
            newVoids.Add(MapChamber.RandomChamber(new Vector2(xLoc, yLoc), radius));
            connected.Add(false);

            locations.Add(new Vector2(xLoc, yLoc));
        }

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
}
