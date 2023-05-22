using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text TouchToPlayText;
    public AudioSource ThemeSource;
    private float TouchToPlayDelay = 2;
    private float touchToPlayDelayStart;

    public MapParameters mapParameters;
    public PerlinNoise perlinNoise;
    private bool preloadComplete = false;

    public GameObject preloadingScreen;
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 60;
        touchToPlayDelayStart = Time.fixedTime;
        TouchToPlayText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (TouchToPlayDelay + touchToPlayDelayStart < Time.fixedTime)
        {
            TouchToPlayText.gameObject.SetActive(true);
            ThemeSource.Play();
            touchToPlayDelayStart = float.PositiveInfinity;
        }

        if (Input.GetMouseButtonDown(0))
        {

            if (preloadComplete)
            {
                //provides a buffer for next scene's loading - Destroy in next scene after loading complete
                DontDestroyOnLoad(preloadingScreen);
                TouchToPlayText.gameObject.SetActive(false);
                SceneManager.LoadScene(2);
            }
            else
            {
                TouchToPlayText.text = "LOADING";
                StartCoroutine(Preloading());
            }
        }

    }

    
    IEnumerator Preloading()
    {
        if (perlinNoise)
        {
            MapGenerator.PregeneratePerlineNoiseVoid(perlinNoise, mapParameters);
            while (MapGenerator.pregenerated.generating) yield return null;
        }
        preloadComplete = true;
        TouchToPlayText.text = "TOUCH TO CONTINUE";
    }
}

