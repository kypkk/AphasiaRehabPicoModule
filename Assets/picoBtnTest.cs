using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class picoBtnTest : MonoBehaviour
{
    public Text msg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PicoInput.GetButtonDown(PicoInput.PicoButton.A))
        {
            Debug.Log("A");
        }
        else if (PicoInput.GetButtonDown(PicoInput.PicoButton.GripR))
        {
            Debug.Log("GripR");
        }
        else if (PicoInput.GetButtonDown(PicoInput.PicoButton.JoystickR))
        {
            Debug.Log("JoystickR");
        }
        else if (PicoInput.GetButtonDown(PicoInput.PicoButton.TriggerR))
        {
            Debug.Log("TriggerR");
        }
        else if (PicoInput.GetButtonDown(PicoInput.PicoButton.MenuR))
        {
            Debug.Log("MenuR");
        }
        else if (PicoInput.GetJoystickValue(1) != null)
        {
            Debug.Log("GetJoystickValue: " + PicoInput.GetJoystickValue(1));
        }
    }
}
