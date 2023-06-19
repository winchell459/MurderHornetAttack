using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : LevelLoadUI
{
    public Text TouchToPlayText;
    public AudioSource ThemeSource;
    private float TouchToPlayDelay = 2;
    private float touchToPlayDelayStart;

    public MapGeneratorParameters mapGeneratorParameters;
    public MapParameters mapParameters;
    public PerlinNoiseScriptableObject perlinNoiseParameters;
    
    private bool preloadComplete = false;

    public GameObject preloadingScreen;
    public bool randomLevels = false;
    public MapGeneratorParameters.GenerationTypes[] randomLevelList;
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 60;
        if(randomLevels && randomLevelList.Length > 0)
        {
            int index = Random.Range(0, randomLevelList.Length);
            mapGeneratorParameters.generationType = randomLevelList[index];
        }
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
            HandleLoadingNextScene();
            
        }

        if(preloadComplete) TouchToPlayText.text = "TOUCH TO CONTINUE";
    }

    public void HandleLoadingNextScene()
    {
        if (preloadComplete)
        {
            //provides a buffer for next scene's loading - Destroy in next scene after loading complete
            DontDestroyOnLoad(preloadingScreen);
            TouchToPlayText.gameObject.SetActive(false);
            PlayerHandler.ResetStats();
            GameManager.singleton.LoadStartScene();
        }
        else
        {
            TouchToPlayText.text = "LOADING";
            GameManager.singleton.LoadStartScene();
            //StartCoroutine(Preloading());
        }
    }


    public override void PreGenerationComplete()
    {
        preloadComplete = true;
    }
}

public abstract class LevelLoadUI : MonoBehaviour
{
    public abstract void PreGenerationComplete();
}