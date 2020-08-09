using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text TouchToPlayText;
    private float TouchToPlayDelay = 2;
    private float touchToPlayDelayStart;
    // Start is called before the first frame update
    void Start()
    {
        touchToPlayDelayStart = Time.fixedTime;
        TouchToPlayText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (TouchToPlayDelay + touchToPlayDelayStart < Time.fixedTime) TouchToPlayText.gameObject.SetActive(true);

        if (Input.GetMouseButtonDown(0)) SceneManager.LoadScene(1);
    }
}
