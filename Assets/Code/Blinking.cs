using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour
{

    public GameObject lights;
    public int MaxSecondPass, MinSecondPass;
    public float BlincTime;
    void Start()
    {
        StartCoroutine(ReLight());
    }

    IEnumerator ReLight()
    {
        while (true)
        {
            float second = Random.Range(MinSecondPass, MaxSecondPass) / 2f;
            yield return new WaitForSeconds(second);
            lights.SetActive(false);

            yield return new WaitForSeconds(BlincTime);
            lights.SetActive(true);
        }


    }
}
