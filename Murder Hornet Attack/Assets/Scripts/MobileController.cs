using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;

public class MobileController : MonoBehaviour
{
    HornetController hornetController;
    // Start is called before the first frame update
    void Start()
    {
        hornetController = GetComponent<HornetController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TCKInput.GetAction("fireBtn", EActionEvent.Down))
        {
            hornetController.FirePlasma();
        }

        if(TCKInput.GetAction("tunnelBtn", EActionEvent.Press))
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
        Vector2 move = TCKInput.GetAxis("Joystick");
        hornetController.MotionControl(move.y, move.x);
    }
}
