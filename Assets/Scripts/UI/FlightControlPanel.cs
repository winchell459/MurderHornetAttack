using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlightControlPanel : MonoBehaviour
{
    public UISliderAndInputFieldPanel cameraTracking, turnRate, acceleration, sensitivity, maxSpeed;
    public Toggle invertOnReverseToggle;

    private void Start()
    {
        cameraTracking.Controller = gameObject;
        turnRate.Controller = gameObject;
        acceleration.Controller = gameObject;
        sensitivity.Controller = gameObject;
        maxSpeed.Controller = gameObject;
    }

    public void DefaultsButton()
    {

    }
    public void OnToggleChanged(bool value)
    {
        UpdateControlParameters();
    }
    public void UIElementChanged(UISliderAndInputFieldPanel uiPanel)
    {
        //switch (uiPanel)
        //{
        //    case cameraTracking:

        //}
        UpdateControlParameters();
    }

    private void UpdateControlParameters()
    {
        ControlParameters.StaticControlParams.SetControlParameters(cameraTracking.GetValue(), sensitivity.GetValue(), maxSpeed.GetValue(), acceleration.GetValue(), turnRate.GetValue(), invertOnReverseToggle.isOn);
        ControlParameters.StaticControlParams.UpdateControllers();
    }

    public void SetControlParameters(float cameraSpeed, float sensitivity, float joystickBoardSize, float v, float h, bool inverseReverse)
    {
        cameraTracking.SetValue(cameraSpeed);
        turnRate.SetValue(h);
        acceleration.SetValue(v);
        invertOnReverseToggle.SetIsOnWithoutNotify(inverseReverse);
        this.sensitivity.SetValue(sensitivity);
        maxSpeed.SetValue(joystickBoardSize);
    }
}
