using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NetworkElement : MonoBehaviour {
	public SpriteRenderer Marker;
	private SpriteRenderer _localmarker;
	public BasicUnit unit;
	public BasicWeapon weapon;

	public bool Controlled;

	public void SetElements () {
		transform.position = GridABS.NodeFromWorldPoint (transform.position).UnitPosition;
		GridABS.NodeFromWorldPoint (transform.position).UnitOnNode = this;
		unit = GetComponentInChildren<BasicUnit> ();
		weapon = GetComponentInChildren<BasicWeapon> ();

		unit.gameObject.name = unit.CorrectName;
		weapon.gameObject.name = weapon.CorrectName;
		unit.SetComponents ();
	}
	public void CreateMarker (Vector3 position) {
		if (_localmarker != null)
			Destroy (_localmarker.gameObject);

		_localmarker = Instantiate (Marker, position, Quaternion.identity);
		_localmarker.color = Controlled ? new Color (0, 244 / 255f, 1,0.4f) : new Color (1, 30 / 255f, 0,0.4f);
		_localmarker.transform.eulerAngles = new Vector3 (90, 45);
	}

	public void MoveToCord (int x, int y) {
		GridABS.NodeFromWorldPoint (transform.position).UnitOnNode = null;
		_localmarker.gameObject.SetActive (false);
		StartCoroutine (LerpMove (GridABS.GridOfArena[x, y]));
	}
	public void ShootToСord (int x, int y) {
		weapon.MakeShoot (GridABS.GridOfArena[x, y]);
	}

	public int GetShelterLevel (out Shelter shelter) {
		shelter = null;
		RaycastHit raycast;
		Physics.Raycast (transform.position, unit.Manger.CorrectUnit.transform.position - transform.position, out raycast, 1);

		if (raycast.collider != null && raycast.collider.GetComponent<Shelter> () != null) {
			shelter = raycast.collider.GetComponent<Shelter> ();
			return raycast.collider.GetComponent<Shelter> ().ShelterLevl;
		}

		return 0;
	}

	public IEnumerator LerpMove (NodeA targetNode) {
		unit.MoveUse ();
		Vector3 finalePose = targetNode.UnitPosition;
		float timer = 0;
		while (Vector3.Distance (transform.position, finalePose) > 0.1f && timer < 1f) {
			timer += Time.deltaTime;
			transform.position = Vector3.Lerp (transform.position, finalePose, 0.1f);
			yield return new WaitForEndOfFrame ();
		}
		transform.position = finalePose;
		targetNode.UnitOnNode = this;
		
	}
}