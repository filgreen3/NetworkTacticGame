using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworcVisualisation : MonoBehaviour {

	public MangerNetwork Manger;

	public void VisualGranade (int x, int y, int granadeType) {
		Vector3 point = GridABS.GridOfArena[x, y].Position;
		StartCoroutine (CreateBoomFx (granadeType, point));
		Vector3 unitVector = Manger.CorrectUnit.transform.position;

		ParticleSystem _granadelineFx = (Resources.Load ("FX/PercGranade/GranadeLineFX") as GameObject).GetComponent<ParticleSystem> ();
		var granadeLineFx = Instantiate (_granadelineFx, unitVector, Quaternion.identity);
		granadeLineFx.Stop ();
		var fxvel = granadeLineFx.velocityOverLifetime;

		fxvel.x = (point - unitVector).x;
		fxvel.z = (point - unitVector).z;

		granadeLineFx.gameObject.SetActive (true);
		granadeLineFx.Play ();

	}
	IEnumerator CreateBoomFx (int fxType, Vector3 point) {
		yield return new WaitForSeconds (1f);
		Instantiate (GranadeMaster.ImportVisualEffect (fxType), point, Quaternion.identity);
	}
}