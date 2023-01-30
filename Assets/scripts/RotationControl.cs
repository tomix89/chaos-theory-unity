using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationControl : MonoBehaviour {

    private Camera cam;
    [SerializeField] private float sensitivity = 0.3f;
    Vector3 mouseDownPos = Vector3.zero;

    // Update is called once per frame
    void Update() {

        // register the point where the mouse was clicked
        if (Input.GetMouseButtonDown(0)) {
            mouseDownPos = Input.mousePosition;
         //   print("mouseDownPos down at: " + mouseDownPos);
        }

        // and while it is down move by the current delta
        if (Input.GetMouseButton(0)) {

            Vector3 delta = Input.mousePosition - mouseDownPos;

           // print(delta);


            transform.Rotate(-delta.y * sensitivity * Time.deltaTime,
                             delta.x * sensitivity * Time.deltaTime,
                              0);
        }
    }
}
