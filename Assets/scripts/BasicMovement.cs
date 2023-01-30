using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class BasicMovement : MonoBehaviour {
    private LineRenderer m_lineRenderer;

    // for center calculation for camera rotator
    public delegate void PositionUpdateAction(string name, Vector3 value);
    public static event PositionUpdateAction OnPositionUpdateAction;

    // this needs to be in 2 separate lists to be able to assign
    // the points to the line renderer easilly
    private List<Vector3> trailPoints = new List<Vector3>();
    private List<float> trailStamps = new List<float>();

    const float MAX_STEP = 0.5f;

    float x = 0;
    float y = 0;
    float z = 0;

    float sigma = 10.0f; // will be overwritten from sliders 
    float rho = 28.0f;  // will be overwritten from sliders 
    float beta = 8.0f / 3.0f;  // will be overwritten from sliders 

    float speed = 1; // simualtion speed
    float time = 0; // for debugging it is better to use our own time
    float trailTimeLimit_s = 5; // trail persistence in real world seconds

    GameObject goToDestroy = null; // as we are instantiated we can't delete self
    public void setGameObj(GameObject goToDestroy) {
        this.goToDestroy = goToDestroy;
    }

    // Start is called before the first frame update
    void Start() {
        m_lineRenderer = GetComponent<LineRenderer>();

        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;

        Color clr = gameObject.GetComponent<Renderer>().material.color;
        m_lineRenderer.startColor = clr;
        m_lineRenderer.endColor = clr;

        trailStamps.Add(0);
        trailPoints.Add(new Vector3(x, y, z));

        SliderController.OnSliderValueChanged += HandleSliderValueChange;
    }

    void HandleSliderValueChange(SliderController.SliderType type, float value) {
        switch (type) {
            case SliderController.SliderType.SPEED:
                speed = value;
                break;
            case SliderController.SliderType.TRACE_DECAY:
                trailTimeLimit_s = value;
                break;
            case SliderController.SliderType.PARAM_BETA:
                beta = value;
                break;
            case SliderController.SliderType.PARAM_RHO:
                rho = value;
                break;
            case SliderController.SliderType.PARAM_SIGMA:
                sigma = value;
                break;
            default:
                throw new System.NotImplementedException();
        }
    }


    static float sqr(float val) {
        return Mathf.Pow(val, 2);
    }


    private void UpdatePositins(float dt) {
        // probably something went wrong
        if (dt < 1e-12) {
            return;
        }

        float xn = x + dt * sigma * (y - x);
        float yn = y + dt * (x * (rho - z) - y);
        float zn = z + dt * (x * y - beta * z);

        // check if the step is too big
        float step = Mathf.Sqrt(sqr(x - xn) + sqr(y - yn) + sqr(z - zn));
        if (step > MAX_STEP) {
            // make this step in 2 halves 
         //   print("step:" + step + " dt: " + dt);
            UpdatePositins(dt / 2);
            UpdatePositins(dt / 2);
            return;
        }

        x = xn;
        y = yn;
        z = zn;

        // trails needs to be upadted in each iteration
        trailStamps.Add(time); // it does not make much sense to slice the time inside one update
        trailPoints.Add(new Vector3(x, y, z));
    }

    // once per fixed time tick - for physics
    private void FixedUpdate() {
        float dt = Time.fixedDeltaTime * speed;
        time += Time.fixedDeltaTime;

        // claculate the new positions
        UpdatePositins(dt);

        // actual position of the circle needs to be updated only once per update 
        if (!float.IsFinite(x) || !float.IsFinite(y) || !float.IsFinite(z)) {
            SliderController.OnSliderValueChanged -= HandleSliderValueChange;
            if (goToDestroy != null) {
                Destroy(goToDestroy);
            } else {
                Destroy(gameObject);
            }
        } else {
            Vector3 newPos = new Vector3(x, y, z);
            transform.position = newPos;

            if (OnPositionUpdateAction != null) {
                OnPositionUpdateAction(name, newPos);
            }

            // delete the timed out trail from oldest
            while (trailStamps[trailStamps.Count - 1] - trailStamps[0] > trailTimeLimit_s) {
                trailStamps.RemoveAt(0);
                trailPoints.RemoveAt(0);
            }

            // draw the new trail
            Vector3[] pts = trailPoints.ToArray();
            m_lineRenderer.positionCount = pts.Length;
            m_lineRenderer.SetPositions(pts);
        }
    }
}
