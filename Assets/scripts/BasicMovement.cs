using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class BasicMovement : MonoBehaviour {
    private LineRenderer m_lineRenderer;

    // this needs to be in 2 separate lists to be able to assign
    // the points to the line renderer easilly
    private List<Vector3> trailPoints = new List<Vector3>();
    private List<float> trailStamps = new List<float>();

    float x = 0;
    float y = 0;
    float z = 1.0f;

    float sigma = 10.0f;
    float rho = 28.0f;
    float beta = 8.0f / 3.5f;

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

        Color clr = gameObject.GetComponent<SpriteRenderer>().color;
        m_lineRenderer.startColor = clr;
        m_lineRenderer.endColor = clr;

        trailStamps.Add(0);
        trailPoints.Add(new Vector3(x,y,0));

        SliderController.OnSliderValueChanged += HandleSliderValueChange;
    }

    void HandleSliderValueChange(string name, float value) {
        if (name == "SliderControl-speed") {
            speed = value;
        } else if (name == "SliderControl-trace-dec") {
            trailTimeLimit_s = value;
        }
    }


    static float sqr(float val) {
        return Mathf.Pow(val, 2);
    }


    private void UpdatePositins(float dt) {
        // probably something went wrong
        if (dt < 1e-10) {
            return;
        }

        // delete the timed out trail from oldest
        while (trailStamps[trailStamps.Count - 1] - trailStamps[0] > trailTimeLimit_s) {
            trailStamps.RemoveAt(0);
            trailPoints.RemoveAt(0);
        }


        float xn = x + dt * sigma * (y - x);
        float yn = y + dt * (x * (rho - z) - y);
        float zn = z + dt * (x * y - beta * z);

        // check if the step is too big
        float step = Mathf.Sqrt(sqr(x - xn) + sqr(y - yn) + sqr(z - zn));
        if (step > 1.0) {
            // make this step in 2 halves 
            print("step:" + step + " dt: " + dt);
            UpdatePositins(dt / 2);
            UpdatePositins(dt / 2);
            return;
        }

        x = xn;
        y = yn;
        z = zn;
        if (!float.IsFinite(x) || !float.IsFinite(y) || !float.IsFinite(z)) {
            SliderController.OnSliderValueChanged -= HandleSliderValueChange;
            if (goToDestroy != null) {
                Destroy(goToDestroy);
            } else {
                Destroy(gameObject);
            }
        } else {
            transform.position = new Vector2(x, y);
            trailStamps.Add(time); // it does not make much sense to slice the time inside one update
            trailPoints.Add(transform.position);
        }
    }

    // once per fixed time tick - for physics
    private void FixedUpdate() {
        float dt = Time.fixedDeltaTime * speed;
        time += Time.fixedDeltaTime;
        UpdatePositins(dt);

        Vector3[] pts = trailPoints.ToArray();
        print(pts.Length);
        m_lineRenderer.positionCount = pts.Length;
        m_lineRenderer.SetPositions(pts);
        print(m_lineRenderer.positionCount);
    }
}
