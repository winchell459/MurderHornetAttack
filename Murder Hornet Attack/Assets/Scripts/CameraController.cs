using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    private Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - Target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Target)transform.position = new Vector3(transform.position.x, Target.position.y + offset.y, transform.position.z);
    }
}
