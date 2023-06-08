using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderAndInputFieldPanel : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private Slider slider;
    private GameObject controller;
    public GameObject Controller { set { controller = value; } }

    float value;

    public void OnInputFieldValueChanged(string value)
    {
        try
        {
            this.value = Mathf.Clamp(int.Parse(value), slider.minValue, slider.maxValue);
            slider.SetValueWithoutNotify(this.value);
            controller.SendMessage("UIElementChanged", this);
        }
        catch
        {
            inputField.SetTextWithoutNotify(this.value.ToString());
        }
        

    }
    public void OnSliderValueChanged(float value)
    {
        this.value = value;
        inputField.SetTextWithoutNotify(this.value.ToString());
        controller.SendMessage("UIElementChanged", this);
    }

    public void SetValue(float value)
    {
        this.value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
        slider.SetValueWithoutNotify(this.value);
        inputField.SetTextWithoutNotify(this.value.ToString());
    }

    public float GetValue()
    {
        return value;
    }
}
