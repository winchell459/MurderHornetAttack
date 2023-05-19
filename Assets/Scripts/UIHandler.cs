using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] menuPanels;
    [SerializeField] private GameObject[] flowerPetals;
    // Start is called before the first frame update
    void Start()
    {
        DisplayMenu(false);
        HideFlowerPetals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMenu()
    {
        if (menuPanels.Length > 0) DisplayMenu(!menuPanels[0].activeSelf);
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
}
