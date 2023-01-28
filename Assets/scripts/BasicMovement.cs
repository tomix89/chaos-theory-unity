using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class BasicMovement : MonoBehaviour {
    private Rigidbody2D m_rigidbody2D;
    private TrailRenderer m_trailRenderer;

    float x = 0;
    float y = 0;
    float z = 1.0f;

    float sigma = 10.0f;
    float rho = 28.0f;
    float beta = 8.0f / 3.5f;

    float timeRate = 1;

    GameObject goToDestroy = null; // as we are instantiated we can't delete self
    public void setGameObj(GameObject goToDestroy) {
        this.goToDestroy = goToDestroy;
    }

    float camXmax, camYmax;

    // Start is called before the first frame update
    void Start() {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_trailRenderer = GetComponent<TrailRenderer>();

        camXmax = Camera.main.orthographicSize * Camera.main.aspect;
        camYmax = Camera.main.orthographicSize;

        x = m_rigidbody2D.position.x;
        y = m_rigidbody2D.position.y;

        SliderController.OnSliderValueChanged += HandleSliderValueChange;
    }

    void HandleSliderValueChange(string name, float value) {

        if (name == "SliderControl-tick") {
            timeRate = value;
        } else if (name == "SliderControl-trace-dec") {
            m_trailRenderer.time = value;
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
            m_rigidbody2D.position = new Vector2(x, y);
        }
    }


    // once per fixed time tick - for physics
    private void FixedUpdate() {
        float dt = Time.fixedDeltaTime * timeRate;
        UpdatePositins(dt);
    }
}
