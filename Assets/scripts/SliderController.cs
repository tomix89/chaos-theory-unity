using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SliderController : MonoBehaviour {
    public Slider sliderWidget;
    public Text textWidget;

    public delegate void ValueChangedAction(SliderType type, float value);
    public static event ValueChangedAction OnSliderValueChanged;

    private float oldVal = float.NaN;

    public enum SliderType {
        SPEED,
        TRACE_DECAY,
        PARAM_SIGMA,
        PARAM_RHO,
        PARAM_BETA
    }

    [SerializeField] private SliderType sliderType;

    private struct SliderProps {
        public SliderProps(string name,
        float minVal,
        float maxVal,
        float defaultVal) {
            this.name = name;
            this.minVal = minVal;
            this.maxVal = maxVal;
            this.defaultVal = defaultVal;
        }

        public string name;
        public float minVal;
        public float maxVal;
        public float defaultVal;
    }

    Dictionary<SliderType, SliderProps> SliderTypes = new Dictionary<SliderType, SliderProps> {
         { SliderType.SPEED, new SliderProps("Speed",0,5,1) },
         { SliderType.TRACE_DECAY, new SliderProps("Trace decay",0.1f,10,5) },
         { SliderType.PARAM_SIGMA, new SliderProps("sigma",0.1f,25,10) },
         { SliderType.PARAM_RHO, new SliderProps("rho",0.1f,100,28) },
         { SliderType.PARAM_BETA, new SliderProps("beta",0.1f,15,8.0f/3.0f) }
        };


    private void Start() {
        sliderWidget.maxValue = SliderTypes[sliderType].maxVal;
        sliderWidget.minValue = SliderTypes[sliderType].minVal;
        sliderWidget.value = SliderTypes[sliderType].defaultVal;
    }


    // Update is called once per frame
    void Update() {
        float newVal = sliderWidget.value;
        if (newVal != oldVal) {
            oldVal = newVal;

            string format = (newVal < 10.0) ? "F2" : "F1";

            textWidget.text = SliderTypes[sliderType].name + " : " + newVal.ToString(format);

            if (OnSliderValueChanged != null) {
                OnSliderValueChanged(sliderType, newVal);
            }
        }
    }
}
