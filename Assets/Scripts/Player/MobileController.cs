using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;


public class MobileController : MonoBehaviour
{
    HornetController hornetController;
    //public float FlySensitivity = 1, TurnSensitivity = 1;
    ControlParameters cp;
    // Start is called before the first frame update
    void Start()
    {
        hornetController = GetComponent<HornetController>();
        cp = FindObjectOfType<ControlParameters>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TCKInput.GetAction("fireBtn", EActionEvent.Down))
        {
            hornetController.FirePlasma();
        }

        if(TCKInput.GetAction("tunnelBtn", EActionEvent.Down))
        {
            hornetController.ExitButtonPressed = true;
        }
        else
        {
            hornetController.ExitButtonPressed = false;
        }
    }
    private void FixedUpdate()
    {
        if (hornetController.MobileControls)
        {
            Vector2 move = TCKInput.GetAxis("Joystick");
            if (cp.InverseReverse && move.y < 0) hornetController.MotionControl(move.y * cp.FlySensitivity, -move.x * cp.TurnSensitivity);
            else hornetController.MotionControl(move.y * cp.FlySensitivity, move.x * cp.TurnSensitivity);
        }
        
    }

    public void SetSensitivityParameters()
    {

    }
}
