using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2 p1_1 = new Vector2(1, 1);
        Vector2 p1_2 = new Vector2(0, 2);
        Vector2 p2_1 = new Vector2(0, 3);
        Vector2 p2_2 = new Vector2(3, 0);
        Debug.Log(Utility.CheckIntersecting(p1_1, p1_2, p2_1, p2_2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
