using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static PerlinNoiseScriptableObject perlinNoiseParameters;
    public static MapGeneratorParameters mapGeneratorParameters;
    public static MapParameters mapParameters;
    public static List<MapVoid> levelMapVoids;

    [SerializeField] private PerlinNoiseScriptableObject perlinNoiseParametersDebug;
    [SerializeField] private MapGeneratorParameters mapGeneratorParametersDebug;
    //[SerializeField] private MapParameters mapParametersDebug;
    //[SerializeField] private static MapParameters mapParametersDebug;

    public Map map;
    public MapGenerator mapGenerator;

    public LevelHandler randomLevelHandler;
    public BeeCityLevelHandler beeCityLevelHandler;

    // Start is called before the first frame update
    void Start()
    {
        if (mapParameters) map.parameters = mapParameters;
        else
        {
            mapParameters = map.parameters;
        }
        map.SetupMap();

        if (!perlinNoiseParameters) perlinNoiseParameters = perlinNoiseParametersDebug;
        if (mapGeneratorParameters)
        {
            //mapGenerator.parameters = mapGeneratorParameters;
            SetupLevelHandler(mapGeneratorParameters);
            
        }
        else
        {
            mapGeneratorParameters = mapGeneratorParametersDebug;
            SetupLevelHandler(mapGeneratorParametersDebug);
        }

    }

    private void SetupLevelHandler(MapGeneratorParameters mapGeneratorParameters)
    {
        switch (mapGeneratorParameters.generationType)
        {
            case MapGeneratorParameters.GenerationTypes.beeCity:
                beeCityLevelHandler.enabled = true;
                break;
            default:
                randomLevelHandler.enabled = true;
                break;
        }
    }
    
}
