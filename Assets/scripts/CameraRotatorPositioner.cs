using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotatorPositioner : MonoBehaviour {

    private Dictionary<string, Vector3> averages = null;
    private float coeff = 0.9999f;
    bool wasUpdate = false;

    void Start() {
        averages = new Dictionary<string, Vector3>();
        BasicMovement.OnPositionUpdateAction += HandlePositionUpdateAction;
    }

    void HandlePositionUpdateAction(string name, Vector3 pos) {

        wasUpdate = true;
        if (averages.ContainsKey(name)) {
            Vector3 oldAvg = averages[name];
            Vector3 newAvg = oldAvg * coeff + (1 - coeff) * pos;
            averages[name] = newAvg;

            print(name + " " + newAvg);
        } else {
            averages.Add(name, pos);
        }

    }

    // Update is called once per frame
    void Update() {

        if (wasUpdate) {
            wasUpdate = false;

            Vector3 sum = Vector3.zero;
            foreach (var item in averages.Values) {
                sum += item;
            }

            Vector3 finalAvg = sum / averages.Count;
            print("final avg: " + finalAvg);
            transform.position = finalAvg;
        }
    }
}
