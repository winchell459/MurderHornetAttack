using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
    public Transform[] Layers;
    private List<Vector2> layersPos = new List<Vector2>();
    public float[] LayerWeights;
    public Transform PlayerCamera;
    // Start is called before the first frame update
    void Start()
    {
        PlayerCamera = FindObjectOfType<CameraController>().transform;
        foreach(Transform layer in Layers)
        {
            layersPos.Add(layer.localPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCamera)
        {
            Vector2 distance = PlayerCamera.position - transform.position;
            for(int i = 0; i < Layers.Length; i += 1)
            {
                Layers[i].localPosition = layersPos[i] + distance * LayerWeights[i];
            }
        }
    }
}
