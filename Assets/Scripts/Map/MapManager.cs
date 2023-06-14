using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager singleton;
    public GameObject PortalPrefab;
    public GameObject ChamberTriggerPrefab;
    public GameObject EnemyPrefabs;
    public GameObject SnakePrefab;
    public ChamberAntFarmTrigger ChamberAntFarmTriggerPrefab;

    //public Transform ExitTunnel;
    public GameObject[] flowerPetals;
    //private List<GameObject> placedFlowerPetals = new List<GameObject>();
    public Transform SnakePit;
    public GameObject SpiderHole;
    public Transform AntSquad;

    public GameObject HoneycombPrefab;
    public GameObject HoneycombCappedPrefab;
    public GameObject HoneycombLargePrefab;
    public GameObject BeeCityPrefab;
    public GameObject HoneycombChamberFloorPrefab;

    //[SerializeField] private GameObject exitTunnelPrefab;
    //[SerializeField] private GameObject pillapillarPitPrefab;
    //[SerializeField] private GameObject spiderHolePrefab;
    //[SerializeField] private GameObject antMoundPrefab;

    private void Start()
    {
        singleton = this;
    }
}
