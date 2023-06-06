using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessController : MonoBehaviour
{
    public Transform exit;
    public float exitHoverRadius = 2.5f;
    public float playerActiveRadius = 15;
    private HornetController player;
    public Transform body;
    private Transform cam;
    public bool inLove;
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<CameraController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player && LevelHandler.singleton.Player) player = LevelHandler.singleton.Player.GetComponent<HornetController>();

        if(player && Vector2.Distance(player.transform.position, exit.position) < playerActiveRadius)
        {
            transform.up = cam.up;

            if (inLove)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed);
            }
            else
            {
                Vector2 exitDirection = (exit.position - player.transform.position).normalized;
                transform.position = exitDirection * exitHoverRadius + (Vector2)exit.position;
            }
            
        }
    }
}
