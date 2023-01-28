using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SliderController : MonoBehaviour
{
    public string baseText;
    public Slider sliderWidget;
    public Text textWidget;

    public delegate void ValueChangedAction(string name, float value);
    public static event ValueChangedAction OnSliderValueChanged;

    private float oldVal = float.NaN;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float newVal = sliderWidget.value;
        if (newVal != oldVal) {
            oldVal = newVal;

            string format = (newVal < 10.0) ? "F2" : "F1"; 

            textWidget.text = baseText + " : " + newVal.ToString(format);

            if (OnSliderValueChanged != null) {
                OnSliderValueChanged(gameObject.name, newVal);
            }
        }
    }
}
