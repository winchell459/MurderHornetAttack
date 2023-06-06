using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Text PlayerLoc, SpawnLoc, EndLoc;
    [SerializeField] private Text BeesMurderedText, HornetMurderedText;
    [SerializeField] private Text eggCountText, flowerCountText, royalJellyCountText;
    [SerializeField] private Text HealthMeterText;
    [SerializeField] private RawImage HealthMeterBar;

    [SerializeField] private Text PlasmaMeterText;
    [SerializeField] private RawImage PlasmaMeterBar;

    [SerializeField] private Text PlasmaPowerText, PlasmaChargeRateText, PlasmaChargeCapacityText;
    //public Text FPSText;
    [SerializeField] private InputField CameraTrackingInput, VSensInput, HSensInput, JoystickBoarderSizeInput, JoystickSensitivityInput;
    [SerializeField] private Toggle InverseReverseToggle;

    [SerializeField] private GameObject[] menuPanels;
    [SerializeField] private GameObject[] flowerPetals;

    [SerializeField] private GameObject murderPanel;
    [SerializeField] private Text murderedMessageText;
    // Start is called before the first frame update
    void Start()
    {
        DisplayMenu(false);
        HideFlowerPetals();
    }

    //LevelHandler calls after LevelSetup is complete
    public void LoadUI()
    {
        foreach(GameObject mainMenuUi in GameObject.FindGameObjectsWithTag("Main Menu"))
        {
            Destroy(mainMenuUi);
        }
    }

    public void ToggleMenu()
    {
        if (menuPanels.Length > 0)
        {
            DisplayMenu(!menuPanels[0].activeSelf);
            MiniMap.singleton.ToggleSize();
        }
    }
    private void DisplayMenu(bool on)
    {
        foreach (GameObject menuPanel in menuPanels) menuPanel.SetActive(on);
    }

    public void NewLevelButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void FlowerPickup(int flowerID)
    {
        flowerPetals[flowerID].SetActive(true);
    }

    public void HideFlowerPetals()
    {
        foreach(GameObject petal in flowerPetals)
        {
            petal.SetActive(false);
        }
    }

    public void DisplayMurderedPanel(bool display, string message)
    {
        murderedMessageText.text = message;
        murderPanel.SetActive(display);
    }

    public void MurderedPanelConfirmButton()
    {
        LevelHandler.singleton.RestartLevel();
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

    public void UpdatePlayerBuffs(float health, float maxHealth, int shotCount, int shotMax, float plasmaPower, float plasmaBuffTime, /*float chargeRate,*/ float chargeBuffRate, float chargeBuffTime, float shotMaxBuffTime, int eggCount, int flowerCount, int royalJellyCount)
    {
        float barPercent = health / maxHealth;
        HealthMeterBar.rectTransform.localScale = new Vector3(barPercent, 1, 1);
        HealthMeterText.text = health.ToString();

        float plasmaPercent = (float)shotCount / shotMax;
        PlasmaMeterBar.rectTransform.localScale = new Vector3(plasmaPercent, plasmaPercent, 1);
        PlasmaMeterText.text = shotCount.ToString();

        PlasmaPowerText.text = Utility.Utility.FormatFloat(plasmaPower,1) + " " + (int)plasmaBuffTime;
        PlasmaChargeRateText.text = Utility.Utility.FormatFloat(chargeBuffRate, 2) + " " + (int)chargeBuffTime;
        PlasmaChargeCapacityText.text = shotMax + " " + (int)shotMaxBuffTime;

        eggCountText.text = eggCount.ToString();
        flowerCountText.text = flowerCount.ToString();
        royalJellyCountText.text = royalJellyCount.ToString();
    }
    public void UpdateKillCounts(int BeesMurdered, int HornetsMurdered)
    {
        BeesMurderedText.text = BeesMurdered.ToString();
        HornetMurderedText.text = HornetsMurdered.ToString();
    }

    public void DisplayExitLocation(Vector2 location)
    {
        displayLocation(location, PlayerLoc);
    }
    public void DisplaySpawnLocation(Vector2 location)
    {
        displayLocation(location, SpawnLoc);
    }
    public void DisplayPlayerLocation(Vector2 location)
    {
        displayLocation(location, EndLoc);
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
}
