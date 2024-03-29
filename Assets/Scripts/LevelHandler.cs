﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{
    public GameObject MurderPanel;
    public GameObject ExitPanel;
    public Transform Player;
    public GameObject PlayerPrefab;
    

    public Text PlayerLoc, SpawnLoc, EndLoc;
    public Text BeesMurderedText, HornetMurderedText;
    public Text HealthMeterText;
    public RawImage HealthMeterBar;

    public Text PlasmaMeterText;
    public RawImage PlasmaMeterBar;

    public Text PlasmaPowerText, PlasmaChargeRateText, PlasmaChargeCapacityText;
    //public Text FPSText;
    public InputField CameraTrackingInput, VSensInput, HSensInput, JoystickBoarderSizeInput, JoystickSensitivityInput;
    public Toggle InverseReverseToggle;

    public CameraController Cam;

    

    public GameObject EnemyDropPrefab;

    
    private PlayerHandler ph;

    private bool levelEnding = false;

    MapGenerator generator;
    

    // Start is called before the first frame update
    void Start()
    {
        MurderPanel.SetActive(false);
        ExitPanel.SetActive(false);
        UpdatePlayerStats();

        generator = GetComponent<MapGenerator>();
        generator.GenerateMap(Player);
        

        Map.StaticMap.Display = true;

        ph = FindObjectOfType<PlayerHandler>();

        //Debug.Log("honeycomb (0,0) " + Utility.HoneycombGridToWorldPostion(new Vector2(0, 0)));
        //Debug.Log("honeycomb (0,6) " + Utility.HoneycombGridToWorldPostion(new Vector2(0, 6)));
        //Debug.Log("honeycomb (3,0) " + Utility.HoneycombGridToWorldPostion(new Vector2(3, 0)));
        //Debug.Log("honeycomb (3,6) " + Utility.HoneycombGridToWorldPostion(new Vector2(3, 6)));
        //Debug.Log("honeycomb (6,6) " + Utility.HoneycombGridToWorldPostion(new Vector2(6, 6)));

        displayLocation(Utility.WorldPointToHoneycombGrid(generator.Exit.transform.position).vector2, EndLoc);
        //displayLocation(Utility.WorldToHoneycomb(PlayerSpawn.transform.position), SpawnLoc);

        ControlParameters.StaticControlParams.LoadControlParameters();
    }

    // Update is called once per frame
    void Update()
    {
        //infiniteLevel();
        if (generator.Exit && generator.Exit.inChamber) {
            ExitPanel.SetActive(true);
            if (Player.GetComponent<HornetController>().ExitButtonPressed && !levelEnding) LevelEndSequence();
        }
        else
        {
            ExitPanel.SetActive(false);
        }

        if (Player)
        {
            displayLocation(Utility.WorldPointToHoneycombGrid(Player.position).vector2, PlayerLoc);
            displayLocation(Map.StaticMap.GetChunkIndex( Utility.GetMapChunk(Player.position)), SpawnLoc);
            //Debug.Log(Utility.GetMapChunk(Player.position).ChunkIndex + " chunkOffset: " + Utility.GetMapChunk(Player.position).mapOffset);
        }
        else
        {
            // spawnPlayer(PlayerSpawn);
            MurderPanel.SetActive(true);
        }
        UpdatePlayerStats();
        //FPSText.text = Utility.FormatFloat(1 / Time.deltaTime, 1);
        //if(Player)Debug.Log("Player Chunk: " + Utility.GetMapChunk(Player.transform.position).mapOffset);
    }

    private void displayLocation(Vector2 loc, Text text)
    {
        int decimals = 2;
        float factor = Mathf.Pow(10, decimals);
        float x = (int)loc.x * factor;
        x = (float)x / factor;
        float y = (int)loc.y * factor;
        y /= factor;
        text.text = x + " " + y;

    }

    private void spawnPlayer(Portal portal)
    {
        Player = Instantiate(PlayerPrefab, generator.PlayerSpawn.Chamber.locations[0], Quaternion.identity).transform;
        Cam.SetCameraTarget(Player);
        UpdatePlayerStats();
    }

    public void EnemyDeath(GameObject enemy)
    {
        int dropCheck = 8;
        if (enemy.GetComponent<SnakeController>()) dropCheck = 2;
        if (enemy.GetComponent<SpiderEnemy>()) dropCheck = 0;
        if(Random.Range(0,10) > dropCheck)
        {
            int dropItem = Random.Range(0, 10); //0-2 Health 3 Power 4-6 Storage 7-9 Rapid
            float power;
            float duration = Random.Range(6 , 12) * 5;
            if(dropItem < 7)
            {
                power = Random.Range(10, 30);
            }
            else
            {
                power = Random.Range(0.05f, 0.45f) * 2;
            }

            ItemPickup drop = Instantiate(EnemyDropPrefab, enemy.transform.position, Quaternion.identity).GetComponent<ItemPickup>();

            if (dropItem < 3)
            {
                drop.PickupType = ItemPickup.PickupTypes.Health;
                
            }
            else if (dropItem < 4) drop.PickupType = ItemPickup.PickupTypes.Power;
            else if (dropItem < 7) drop.PickupType = ItemPickup.PickupTypes.Storage;
            else if (dropItem < 10) drop.PickupType = ItemPickup.PickupTypes.Rapid;
            drop.Power = power;
            drop.Duration = duration;
            drop.SetupLetters();

        }
    }

    private float levelEndStart;
    private float levelEndCountdown = 6.0f;
    private void LevelEndSequence()
    {
        levelEnding = true;
        levelEndStart = Time.fixedTime;
        generator.ExitTunnel.GetComponent<Animator>().SetTrigger("Activate");
        Map.StaticMap.Display = false;
        StartCoroutine("LevelEndCoroutine");
    }

    IEnumerator LevelEndCoroutine()
    {
        while (levelEndStart + levelEndCountdown > Time.fixedTime)
            yield return null;
        ReloadLevel();
    }

    public void RestartLevel()
    {
        MurderPanel.SetActive(false);
        spawnPlayer(generator.PlayerSpawn);
        
    }

    private void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
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

    public void UpdatePlayerStats()
    {
        
        BeesMurderedText.text = PlayerHandler.BeesMurderedCount.ToString();
        HornetMurderedText.text = PlayerHandler.HornetMurderedCount.ToString();

        PlayerHandler ph = FindObjectOfType<PlayerHandler>();
        HornetController hc = FindObjectOfType<HornetController>();

        if(ph && hc)
        {
            float barPercent = hc.Health / ph.GetMaxHealth();
            HealthMeterBar.rectTransform.localScale = new Vector3(barPercent, 1, 1);
            HealthMeterText.text = hc.Health.ToString();

            float plasmaPercent = (float)hc.ShotCount / ph.GetMaxShot();
            PlasmaMeterBar.rectTransform.localScale = new Vector3(plasmaPercent, plasmaPercent, 1);
            PlasmaMeterText.text = hc.ShotCount.ToString();

            PlasmaPowerText.text = (int)ph.GetPlasmaPower() + " " + (int)ph.GetPlasmaPowerBuffTime();
            PlasmaChargeRateText.text = Utility.FormatFloat(ph.GetPlasmaChargeRate(),2) + " " + (int)ph.GetPlasmaChargeRateBuffTime();
            PlasmaChargeCapacityText.text = ph.GetMaxShot() + " " + (int)ph.GetMaxShotBuffTime();
        }
        
    }

    public void UpdatePlayerStats(int BeesDied, int HornetDied)
    {
        PlayerHandler.BeesMurderedCount += BeesDied;
        PlayerHandler.HornetMurderedCount += HornetDied;
        UpdatePlayerStats();
    }

    

    public void SetControlParameters()
    {
        try
        {
            float cameraSpeed = float.Parse(CameraTrackingInput.text);
            float v = float.Parse(VSensInput.text);
            float h = float.Parse(HSensInput.text);
            float sensitivity = float.Parse(JoystickSensitivityInput.text);
            float boarderSize = float.Parse(JoystickBoarderSizeInput.text);
            //Debug.Log(h);
            bool inverseReverse = InverseReverseToggle.isOn;
            ControlParameters.StaticControlParams.SetControlParameters(cameraSpeed, sensitivity, boarderSize, v, h, inverseReverse);
        }
        catch
        {
            Debug.Log("SetControlParameters Error");
        }

    }

    public void SetControlParameters(float cameraSpeed, float sensitivity, float joystickBoardSize, float v, float h, bool inverseReverse)
    {
        CameraTrackingInput.text = cameraSpeed.ToString();
        VSensInput.text = v.ToString();
        HSensInput.text = h.ToString();
        InverseReverseToggle.isOn = inverseReverse;
        JoystickBoarderSizeInput.text = joystickBoardSize.ToString();
        JoystickSensitivityInput.text = sensitivity.ToString();
    }

    
}
