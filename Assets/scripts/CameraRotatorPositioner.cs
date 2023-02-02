using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotatorPositioner : MonoBehaviour {

    private float decayCoeff = 0.00005f;
    private float maxCamSpeed = 0.05f;
    bool wasUpdate = false;

    private struct MMItem {
        private string tag;
        public float max;
        public float min;

        public void setTag(string tag) {
            this.tag = tag;
        }

        // this creates disturbing camera jumps when being assinged direclty
        public void SetLimits(float value) {
            if (value < min) {
                min = value;
            }
            if (value > max) {
                max = value;
            }
        }

        public void applyDecay(float amount) {
            float oldMin = min;

            min += max*amount;
            max += oldMin*amount;
        }

        public float getAvg() {
            print(tag + "  " + min + "  " + max);
            return (max + min) / 2.0f;
        }
    }

    private struct Limits {
        public MMItem mmx;
        public MMItem mmy;
        public MMItem mmz;

        public void SetLimits(Vector3 pos) {
            mmx.SetLimits(pos.x);
            mmy.SetLimits(pos.y);
            mmz.SetLimits(pos.z);
        }

        public void applyDecays(float amount) {
            mmx.applyDecay(amount);
            mmy.applyDecay(amount);
            mmz.applyDecay(amount);
        }

        public Vector3 getAvg() {
            return new Vector3(mmx.getAvg(), mmy.getAvg(), mmz.getAvg());
        }
    }

    Limits limits;

    void Start() {
        limits = new Limits();
        limits.mmx.setTag("x");
        limits.mmy.setTag("y");
        limits.mmz.setTag("z");
        BasicMovement.OnPositionUpdateAction += HandlePositionUpdateAction;
    }

    void HandlePositionUpdateAction(string name, Vector3 pos) {
        wasUpdate = true;
        limits.SetLimits(pos);
    }


    Vector3 limitedDelta(Vector3 val) {
        val.x = Mathf.Clamp(val.x, -maxCamSpeed, maxCamSpeed);
        val.y = Mathf.Clamp(val.y, -maxCamSpeed, maxCamSpeed);
        val.z = Mathf.Clamp(val.z, -maxCamSpeed, maxCamSpeed);

        print("limitedDelta: " + val);

        return val;
    } 

    void Update() {
        if (wasUpdate) {
            wasUpdate = false;

            limits.applyDecays(decayCoeff);
            Vector3 finalAvg = limits.getAvg();
            print("final avg: " + finalAvg);

            Vector3 delta = finalAvg - transform.position;
            print("delta: " + delta);

            // this alters all axis, maybe it will be nice to compensate for the camera to rotator distance (z)
            transform.position += limitedDelta(delta); 
        }
    }
}
