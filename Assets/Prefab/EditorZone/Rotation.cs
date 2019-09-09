using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    public float timer;
    private void Update()
    {
        transform.eulerAngles += new Vector3(0,  Time.deltaTime * timer, 0);
    }
}
