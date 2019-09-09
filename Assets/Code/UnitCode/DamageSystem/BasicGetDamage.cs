using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGetDamage : MonoBehaviour {

	public const int effectCount = 6;

	public int[] _durations = new int[effectCount];
	public int[] _damages = new int[effectCount];
	public GameObject[] _effectObjects = new GameObject[effectCount];

	public BasicUnit ThisUnit;

	void GetbasicUnit () {
		ThisUnit = GetComponent<BasicUnit> ();
	}

	public void CompleadArray () {
		for (int i = 0; i < effectCount; i++) {
			_effectObjects[i] = ImportVisualEffect (i);
			_effectObjects[i].SetActive (false);
		}
	}

	void Awake()
	{
		CompleadArray();
	}

	public GameObject ImportVisualEffect (int typeGranade) {

		string pathGanadeFx = "FX/PercEffect/";
		switch (typeGranade) {
			case 0:
				pathGanadeFx += "StunEffect";
				break;
			case 1:
				pathGanadeFx += "FireEffect";
				break;
			case 2:
				pathGanadeFx += "PoisonEffect";
				break;
			case 3:
				pathGanadeFx += "ElectricEffect";
				break;
			case 4:
				pathGanadeFx += "StunEffect";
				break;
			case 5:
				pathGanadeFx += "HealEffect";
				break;
		}
		var result = Instantiate (Resources.Load (pathGanadeFx) as GameObject, transform.position, Quaternion.identity,transform);
		result.transform.eulerAngles= transform.eulerAngles;
		return result;

	}
}