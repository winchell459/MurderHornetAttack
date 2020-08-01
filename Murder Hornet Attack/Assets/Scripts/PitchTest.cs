using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchTest : MonoBehaviour
{
    private Rigidbody2D Player;
    public float smoothing = 1;
    public float MaxSpeed, MinSpeed;
    public float MaxPitch, MinPitch;
    public AudioSource AS;
    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!Player) Player = FindObjectOfType<HornetController>().GetComponent<Rigidbody2D>();
        else
        {
            Debug.Log(Player.velocity.magnitude);
            float speed = Player.velocity.magnitude;
            float percent = (speed - MinSpeed) / (MaxSpeed - MinSpeed);
            float pitch = percent * (MaxPitch - MinPitch) + MinPitch;
            pitch = pitch < MinPitch ? MinPitch : pitch;
            AS.pitch = pitch;
        }
    }
}
