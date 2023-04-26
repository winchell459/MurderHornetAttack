using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] menuPanels;
    // Start is called before the first frame update
    void Start()
    {
        DisplayMenu(false);
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
}
