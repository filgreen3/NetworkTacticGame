using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : MonoBehaviour {

	public string CorrectName;
	public int valuespeedBullet;
	[Header ("Основные настройки")]
	public int Damage;
	public int Ammo, MaxAmmo;
	public AudioClip Sound;
	public Vector3 PointShoot;

	[Header ("Частота выстрелов")]
	public int BulletPerShoot;
	public float WaitTime;

	[Header ("Дистанции")]
	public int MaxDist;
	public int NormalDist;

	private GameObject _bullet;
	private BasicUnit _target;

	private NetworkElement _unit;
	int FinalDamage = 0;
	public virtual List<NodeA> AttackArea () {
		List<NodeA> result = new List<NodeA> ();
		int x, y;
		GridABS.NodeCoordinat (transform.position, out x, out y);
		for (var d = 0; d < 4; d++)
			for (var i = 1; i <= MaxDist; i++)
				try {
					var nodes = GridABS.GridOfArena[x + i * MatfSub.MPAer (d, true), y + i * MatfSub.MPAer (d, false)];
					if (nodes.TypeBloc != NodeA.NodeType.Wall) {
						if (nodes.TypeBloc == NodeA.NodeType.Walkable) result.Add (nodes);
						if (nodes.UnitOnNode != null && i > 1) break;
					} else break;

				} catch { }
		return result;
	}

	public virtual void MakeShoot (NodeA node) {
		if (_unit == null) _unit = transform.parent.GetComponent<NetworkElement> ();
		if (_bullet == null) _bullet = Resources.Load<GameObject> ("Staff/Bullet") as GameObject;
		_target = node.UnitOnNode.unit;
		StartCoroutine (MoveBullet (node));
	}
	public virtual IEnumerator MoveBullet (NodeA targetNode) {
		_unit.unit.AttackUse ();
		yield return new WaitForSeconds (0.25f);
		for (int i = 0; i < BulletPerShoot; i++) {
			_target.Manger.PlaySpecific (Sound);

			Vector3 targetPosition = targetNode.Position + Vector3.up * (Random.Range (-5, 11) / 20f) + Vector3.up * 0.6f;
			Vector3 selfPosition = transform.position + (targetPosition - (transform.position + Vector3.up * 0.3f)).normalized * 0.25f + Vector3.up * 0.3f;
			NetworkElement targetUnit = targetNode.UnitOnNode;
			GameObject localBullet = Instantiate (_bullet, selfPosition, Quaternion.identity);
			float distantion = Vector3.Distance (selfPosition, targetPosition);
			while (Vector3.Distance (localBullet.transform.position, targetPosition) > 0.05f) {
				localBullet.transform.Translate ((targetPosition - selfPosition) / valuespeedBullet);
				yield return new WaitForFixedUpdate ();
			}

			float n = Random.Range (0, 100);
			Shelter shelter;

			float DistanceTarget = Vector3.Distance (selfPosition, targetUnit.transform.position + Vector3.up * (Random.Range (-5, 11) / 20f));

			float Range = NormalDist / DistanceTarget;
			float targetShelterValue = targetUnit.GetShelterLevel (out shelter);
			if (Range > 1) Range = 1;

			if (n < Range * 100f - targetShelterValue) FinalDamage += Damage;
			else {
				if (Random.Range (0, 101) > targetShelterValue * 0.75f) {
					if (shelter != null) {
						Instantiate (shelter.FX, shelter.transform.position, Quaternion.identity);
						shelter.ShelterLevl -= Damage * 3;
						if (shelter.ShelterLevl < 1) Destroy (shelter.gameObject);
					}
				}
			}
			Ammo--;
			Destroy (localBullet);
			yield return new WaitForSeconds (WaitTime);
		}

		if (_unit.Controlled) {
			if (FinalDamage > 0) {
				ClientReciverMK1.WriteMsg ("/Damage" + _unit.unit.Manger.GetUnitId (targetNode.UnitOnNode) + ":" + FinalDamage + ":" + 0 + ":" + 0);
				_target.GetDamage (FinalDamage);
				FinalDamage = 0;
			} else {
				ClientReciverMK1.WriteMsg ("/Damage" + _unit.unit.Manger.GetUnitId (targetNode.UnitOnNode) + ":" + 0 + ":" + 0 + ":" + -1);
				_target.GetDamage (0, 0, -1);
			}
		}

	}

}