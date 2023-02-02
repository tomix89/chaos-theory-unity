using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownController : MonoBehaviour
{

    public delegate void ValueChangedAction(int index);
    public static event ValueChangedAction OnDropdownValueChanged;

    private void Start() {
        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate { ItemSelected(dropdown); });
    }

    // https://www.youtube.com/watch?v=URS9A4V_yLc
    void ItemSelected(Dropdown dropdown) { 
         
        if (OnDropdownValueChanged != null) {
            OnDropdownValueChanged(dropdown.value);
        }

    }

}
