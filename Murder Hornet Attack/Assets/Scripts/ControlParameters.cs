using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlParameters : MonoBehaviour
{
    public float CameraSpeed = 3;
    public float FlySensitivity = 3, TurnSensitivity = 3;
    public bool InverseReverse;
    public float JoystickSensitivity = 1;
    public float JoystickBoarderSize = 4.85f;
    public static ControlParameters StaticControlParams;
    private string cameraSpeedTag = "CameraSpeedTag", flySensitivityTag = "FlySensitivity", turnSensitivityTag = "TurnSensitivity", joystickSensitivityTag = "JoystickSensitivity", joystickBoarderSizeTag = "JoystickBoarderSize", inverseReverseTag = "InverseReverse";

    private void Awake()
    {
        if (StaticControlParams) Destroy(gameObject);
        else StaticControlParams = this;
        LoadControlParameters();
        FindObjectOfType<LevelHandler>().SetControlParameters(CameraSpeed, JoystickSensitivity,JoystickBoarderSize, FlySensitivity, TurnSensitivity, InverseReverse);
    }

    public void SetControlParameters(float CameraTrackingSpeed, float JoystickSensitivity, float JoystickBoarderSize, float FlySensitivity, float TurnSensitivity, bool InverseReverse)
    {
        CameraSpeed = CameraTrackingSpeed;
        this.FlySensitivity = FlySensitivity;
        this.TurnSensitivity = TurnSensitivity;
        this.InverseReverse = InverseReverse;
        this.JoystickBoarderSize = JoystickBoarderSize;
        this.JoystickSensitivity = JoystickSensitivity;
        UpdateControllers();
        SaveControlParameters();
    }

    public void SaveControlParameters()
    {
        PlayerPrefs.SetFloat(cameraSpeedTag, CameraSpeed);
        PlayerPrefs.SetFloat(flySensitivityTag, FlySensitivity);
        PlayerPrefs.SetFloat(turnSensitivityTag, TurnSensitivity);
        PlayerPrefs.SetFloat(joystickSensitivityTag, JoystickSensitivity);
        PlayerPrefs.SetFloat(joystickBoarderSizeTag, JoystickBoarderSize);
        PlayerPrefs.SetInt(inverseReverseTag, InverseReverse ? 1 : 0);
    }

    public void LoadControlParameters()
    {
        CameraSpeed = PlayerPrefs.HasKey(cameraSpeedTag) ? PlayerPrefs.GetFloat(cameraSpeedTag) : CameraSpeed;
        FlySensitivity = PlayerPrefs.HasKey(flySensitivityTag) ? PlayerPrefs.GetFloat(flySensitivityTag) : FlySensitivity;
        TurnSensitivity = PlayerPrefs.HasKey(turnSensitivityTag) ? PlayerPrefs.GetFloat(turnSensitivityTag) : TurnSensitivity;
        JoystickSensitivity = PlayerPrefs.HasKey(joystickSensitivityTag) ? PlayerPrefs.GetFloat(joystickSensitivityTag) : JoystickSensitivity;
        JoystickBoarderSize = PlayerPrefs.HasKey(joystickBoarderSizeTag) ? PlayerPrefs.GetFloat(joystickBoarderSizeTag) : JoystickBoarderSize;
        if (PlayerPrefs.HasKey(inverseReverseTag)) InverseReverse = PlayerPrefs.GetInt(inverseReverseTag) == 1 ? true : false;
        else Debug.Log("InverseReverseTag not found");
        UpdateControllers();
    }

    public void UpdateControllers()
    {
        FindObjectOfType<TouchControlsKit.TCKJoystick>().borderSize = JoystickBoarderSize;
        FindObjectOfType<TouchControlsKit.TCKJoystick>().sensitivity = JoystickSensitivity;
    }
}
