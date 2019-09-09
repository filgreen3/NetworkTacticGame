using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpInfo : MonoBehaviour {
    public TextMeshPro TextMesh;

    float timetolive = 0;

    public void DestroyInfo () {
        Destroy (gameObject);
    }

    // Use this for initialization
    public void HitInfoMetod () {
        timetolive =(Mathf.Pow (TextMesh.text.Length, 3)*0.005f)+3;
        StartCoroutine (UpFor ());

        Destroy (gameObject, timetolive);
    }
    public IEnumerator UpFor () {
        while(true)
        {
        yield return new WaitForEndOfFrame ();
        transform.position = Vector3.MoveTowards (transform.position, transform.position + Vector3.up * 30/timetolive, 0.017f);
        TextMesh.color = new Color (TextMesh.color.r, TextMesh.color.g, TextMesh.color.b, TextMesh.color.a - 0.02f/timetolive);
        }
    }

}