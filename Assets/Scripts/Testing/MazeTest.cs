using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Vector2 p1_1 = new Vector2(1, 1);
        //Vector2 p1_2 = new Vector2(0, 2);
        //Vector2 p2_1 = new Vector2(0, 3);
        //Vector2 p2_2 = new Vector2(3, 0);
        //Debug.Log(Utility.Utility.CheckIntersecting(p1_1, p1_2, p2_1, p2_2));

        HoneycombPos p1 = new HoneycombPos(1, 1);
        //HoneycombPos p2 = new HoneycombPos(1, 1);
        //HoneycombPos p3 = new HoneycombPos(1, -1);

        //Debug.Log(p1 == p2);
        //Debug.Log(p1 == p3);
        //Debug.Log(p1.Equals(p2));

        //List<HoneycombPos> honeycombPos = new List<HoneycombPos>();
        //honeycombPos.Add(p1);
        //Debug.Log(honeycombPos.Contains(p2));
        //Debug.Log(honeycombPos.Contains(p3));

        //List<HoneycombPos> neighbors = p1.GetAdjecentHoneycomb(1);
        //Debug.Log(neighbors.Count);
        //Debug.Log(p1.GetAdjecentHoneycomb(2).Count);
        //Debug.Log(p1.GetAdjecentHoneycomb(3).Count);
        //Debug.Log(p1.GetAdjecentHoneycomb(4).Count);

        for(int i = 0; i < 10; i += 1)
        {
            Debug.Log($"{p1.GetAdjecentHoneycomb(i).Count} =? {Utility.Honeycomb.GetHoneycombRadius(p1.GetAdjecentHoneycomb(i).Count)}");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
