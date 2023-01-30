using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomControl : MonoBehaviour {
    private Camera cam;
    [SerializeField] private float scrollSpeed = 35;

    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        Vector3 pos = cam.transform.localPosition;
        pos.z += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cam.transform.localPosition = pos;
    }
}
