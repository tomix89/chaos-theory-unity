using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;

    Quaternion qt = Quaternion.Euler(0, 0, 0);
    float timeSum = 0;


    private void Start() {

        for (int x = -50; x < 50; x += 5) {
            for (int y = -50; y < 50; y += 5) {

                GameObject go = Instantiate(objectToSpawn, new Vector3(x, y, 1), qt);
                go.transform.position = new Vector3(x, y, 1);
                go.GetComponent<BasicMovement>().setGameObj(go);
            }

        }
    }


    private void FixedUpdate() {

       /* timeSum += Time.deltaTime;

        if (timeSum > 1.0) {
            timeSum = 0;

            float x = Random.Range(-8, 8);
            float y = Random.Range(-5.5f, 5.5f);
            GameObject go = Instantiate(objectToSpawn, new Vector3(x, y, 1), qt);
            go.GetComponent<BasicMovement>().setGameObj(go);
        }*/

    }
}
