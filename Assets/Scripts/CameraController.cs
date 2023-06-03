using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    private Vector2 offset;
    public float TrackingSpeed = 1;
    public Camera cam;

    //public Transform Layer2;
    //private float layer2Scale;
    public Transform[] Layers;
    public bool rotateTargetLocked;
    public float defaultRotation;
    private float offsetRotation = 0;
    
    private List<float> scales = new List<float>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            offsetRotation = -Target.eulerAngles.z + transform.eulerAngles.z;
            rotateTargetLocked = !rotateTargetLocked;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        offset = Target.position - cam.transform.position;
        //Debug.Log(offset);
        transform.position = new Vector3(Target.position.x, Target.position.y, transform.position.z);
        cam.transform.localPosition = -offset;
        SetCameraTarget(Target);
        //layer2Scale = Layer2.localScale.x;
        foreach(Transform layer in Layers)
        {
            scales.Add(layer.localScale.x);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(Target)transform.position = new Vector3(transform.position.x, Target.position.y + offset.y, transform.position.z);
        if (Target)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Target.position.x, Target.position.y, transform.position.z), ControlParameters.StaticControlParams.CameraSpeed*Time.deltaTime);
            //transform.up = Target.up;
            if(rotateTargetLocked)transform.eulerAngles = Target.eulerAngles + offsetRotation * Vector3.forward;
            for(int i = 1; i < Layers.Length; i+=1)
            {
                Transform layer = Layers[i];
                layer.position = transform.position - scales[i] * scales[i] * transform.position;
            }
            //Layer2.position = transform.position - layer2Scale * layer2Scale * transform.position;
        }
        if (transform.position.z != -10) Debug.Log("Camera Error: " + transform.position.z);
    }

    public void SetCameraTarget(Transform Target)
    {
        this.Target = Target;
        transform.position = new Vector3(Target.position.x, Target.position.y, transform.position.z);
    }
}
