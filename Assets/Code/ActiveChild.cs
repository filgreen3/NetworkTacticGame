using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveChild : MonoBehaviour {

	public void DoActive()
	{
		GameObject gm = transform.GetChild(0).gameObject;
		gm.SetActive(!gm.activeInHierarchy);
		gm.transform.localScale = Vector3.zero;
		StartCoroutine(_reSize(gm.transform));
	}
	private IEnumerator _reSize(Transform transformM)
	{
		 
        while (Vector3.Distance(transformM.localScale,Vector3.one)>0.001f)
        {
            transformM.localScale = Vector3.Lerp(transformM.localScale,Vector3.one,0.4f);
            yield return new WaitForFixedUpdate();
        }
	}
}
