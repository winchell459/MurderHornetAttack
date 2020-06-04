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
        createVoids();

        Map.StaticMap.DisplayChunks();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        
        MapPath newPath = MapPath.CreateJoggingPath(Player.position, new Vector2(40, -5f), -5, 5, 1, 3, 2, 4);
        Map.StaticMap.AddVoid(newPath);
        MapChamber newChamber = new MapChamber();
        newChamber.AddChamber(new Vector2(40, 5), 10);
        newChamber.AddChamber(new Vector2(30, 3), 6);
        Map.StaticMap.AddVoid(newChamber);
    }
}
