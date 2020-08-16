using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSHandler : MonoBehaviour
{
    public Text FPSText;
    public Toggle Toggle30FPS, Toggle60FPS, Toggle120FPS;
    private int fps = 60;
    private float valueChangedTime = float.NegativeInfinity;
    private bool valueChanged = false;
    private float calcStartTime;
    private float fpsAverage = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        ToggleFPS(fps);
        startCalc();
    }

    private void Update()
    {
        if(valueChanged && valueChangedTime + 1 < Time.fixedTime)
        {
            Application.targetFrameRate = fps;
            Debug.Log("FPS set to " + fps);
            valueChanged = false;
        }

        handleFPSCalc();
        
    }

    public void ToggleFPS(int fps)
    {

        if (!valueChanged)
        {
            valueChanged = true;
            this.fps = fps;
            if (fps == 30)
            {
                Toggle30FPS.isOn = true;
                Toggle60FPS.isOn = false;
                Toggle120FPS.isOn = false;
            }
            else if (fps == 60)
            {
                Toggle30FPS.isOn = false;
                Toggle60FPS.isOn = true;
                Toggle120FPS.isOn = false;
            }
            else
            {
                Toggle30FPS.isOn = false;
                Toggle60FPS.isOn = false;
                Toggle120FPS.isOn = true;
            }
        }
        
    }

    private void startCalc()
    {
        calcStartTime = Time.fixedTime;
        fpsAverage = 0;
    }

    private void handleFPSCalc()
    {
        fpsAverage += 1;
        if (calcStartTime + 1 < Time.fixedTime)
        {
            
            FPSText.text = Utility.FormatFloat(fpsAverage / (Time.fixedTime - calcStartTime), 1);
            startCalc();
        }
    }
}
