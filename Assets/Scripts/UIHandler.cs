using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
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
}
