using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    private Vector2 offset;
    public float TrackingSpeed = 1;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        offset = Target.position - cam.transform.position;
        //Debug.Log(offset);
        transform.position = new Vector3(Target.position.x, Target.position.y, transform.position.z);
        cam.transform.localPosition = -offset;
        SetCameraTarget(Target);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(Target)transform.position = new Vector3(transform.position.x, Target.position.y + offset.y, transform.position.z);
        if (Target)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Target.position.x, Target.position.y, transform.position.z), TrackingSpeed*Time.deltaTime);
            transform.up = Target.up;
        }
        if (transform.position.z != -10) Debug.Log("Camera Error: " + transform.position.z);
    }

    public void SetCameraTarget(Transform Target)
    {
        this.Target = Target;
        transform.position = new Vector3(Target.position.x, Target.position.y, transform.position.z);
    }
}
