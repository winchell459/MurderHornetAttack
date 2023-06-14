using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static MapGeneratorParameters mapGeneratorParameters;
    public static MapParameters mapParameters;

    public Map map;
    public MapGenerator mapGenerator;

    public LevelHandler randomLevelHandler;
    public BeeCityLevelHandler beeCityLevelHandler;

    // Start is called before the first frame update
    void Start()
    {
        if (mapParameters) map.parameters = mapParameters;
        if (mapGeneratorParameters)
        {
            mapGenerator.parameters = mapGeneratorParameters;
            SetupLevelHandler(mapGeneratorParameters);
            
        }
        else
        {
            SetupLevelHandler(mapGenerator.parameters);
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
