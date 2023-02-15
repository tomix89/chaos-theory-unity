using System;
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

    BasicMovement.ChaosType chaosType;

    public enum SliderType {
        SPEED,
        TRACE_DECAY,
        PARAM_SIGMA_A, // lorenz calls this sigma rossler a
        PARAM_RHO_B, // lorenz calls this rho rossler b
        PARAM_BETA_C // lorenz calls this beta rossler c
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

    Dictionary<SliderType, SliderProps> SliderTypesLorenz = new Dictionary<SliderType, SliderProps> {
         { SliderType.SPEED, new SliderProps("Speed",0,5,1) },
         { SliderType.TRACE_DECAY, new SliderProps("Trace decay",0.1f,10,5) },
         { SliderType.PARAM_SIGMA_A, new SliderProps("sigma",0.1f,25,10) },
         { SliderType.PARAM_RHO_B, new SliderProps("rho",0.1f,100,28) },
         { SliderType.PARAM_BETA_C, new SliderProps("beta",0.1f,15,8.0f/3.0f) }
        };

    Dictionary<SliderType, SliderProps> SliderTypesRossler = new Dictionary<SliderType, SliderProps> {
         { SliderType.SPEED, new SliderProps("Speed",0,10,5.0f) }, // rossler seems to be a bit slower
         { SliderType.TRACE_DECAY, new SliderProps("Trace decay",0.1f,10,5) },
         { SliderType.PARAM_SIGMA_A, new SliderProps("a",0.1f,0.4f,0.1f) },
         { SliderType.PARAM_RHO_B, new SliderProps("b",0.1f,0.4f,0.1f) },
         { SliderType.PARAM_BETA_C, new SliderProps("c",0.1f,45,14.0f) }
        };

    private void initSliders(BasicMovement.ChaosType chaosType) {

        switch (chaosType) {

            case BasicMovement.ChaosType.LORENZ:
                sliderWidget.maxValue = SliderTypesLorenz[sliderType].maxVal;
                sliderWidget.minValue = SliderTypesLorenz[sliderType].minVal;
                sliderWidget.value = SliderTypesLorenz[sliderType].defaultVal;
                break;

            case BasicMovement.ChaosType.ROSSLER:
                sliderWidget.maxValue = SliderTypesRossler[sliderType].maxVal;
                sliderWidget.minValue = SliderTypesRossler[sliderType].minVal;
                sliderWidget.value = SliderTypesRossler[sliderType].defaultVal;
                break;

            default:
                throw new NotImplementedException();
        }

    }

    string getSliderText(BasicMovement.ChaosType chaosType) {
        switch (chaosType) {

            case BasicMovement.ChaosType.LORENZ:
                return SliderTypesLorenz[sliderType].name;

            case BasicMovement.ChaosType.ROSSLER:
                return SliderTypesRossler[sliderType].name;

            default:
                throw new NotImplementedException();
        }
    }

    private void Start() {
        chaosType = BasicMovement.ChaosType.LORENZ;
        initSliders(chaosType);

        DropdownController.OnDropdownValueChanged += HandleChaosTypeChange;
    }


    void HandleChaosTypeChange(int type) {
        chaosType = (BasicMovement.ChaosType)type;
        initSliders(chaosType);
    }

    // Update is called once per frame
    void Update() {
        float newVal = sliderWidget.value;
        if (newVal != oldVal) {
            oldVal = newVal;

            string format = (newVal < 10.0) ? "F2" : "F1";

            textWidget.text = getSliderText(chaosType) + " : " + newVal.ToString(format);

            if (OnSliderValueChanged != null) {
                OnSliderValueChanged(sliderType, newVal);
            }
        }
    }
}
