using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVelocityLagController : MonoBehaviour
{
    ControlParameters cp;
    public AnimationCurve weight;
    
    public void VelocityLagSlider(float value)
    {
        if (cp)
        {
            cp.SetVelocityLag(weight.Evaluate(value));
        }
        else
        {
            cp = FindObjectOfType<ControlParameters>();
        }
        
    }
}
