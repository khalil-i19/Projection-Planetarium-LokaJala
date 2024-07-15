using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleGroupExtension : MonoBehaviour
{
    public void ToggleAll(bool value)
    {
        foreach(var toggle in GetComponentsInChildren<Toggle>())
        {
            toggle.isOn = value;
        }
    }
}
