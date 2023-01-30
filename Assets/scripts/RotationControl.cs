using UnityEngine.EventSystems;
using UnityEngine;

public class RotationControl : MonoBehaviour {

    private Camera cam;
    [SerializeField] private float sensitivity = 0.3f;
    Vector3 mouseDownPos = Vector3.zero;
    bool canRotate = true;

    // https://www.youtube.com/watch?v=ptmum1FXiLE
    bool IsMouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Update is called once per frame
    void Update() {

        // register the point where the mouse was clicked
        if (Input.GetMouseButtonDown(0)) {
            mouseDownPos = Input.mousePosition;

            // we need to check this upon mouse button down
            // as user might be able to hover off the UI item and the rotation would begin
            canRotate = !IsMouseOverUI();

         //   print("mouseDownPos down at: " + mouseDownPos);
        }

        // and while it is down move by the current delta
        if (Input.GetMouseButton(0) && canRotate) {

            Vector3 delta = Input.mousePosition - mouseDownPos;

           // print(delta);


            transform.Rotate(-delta.y * sensitivity * Time.deltaTime,
                             delta.x * sensitivity * Time.deltaTime,
                              0);
        }
    }
}
