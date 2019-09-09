using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUnit : MonoBehaviour {

	public string CorrectName;
	public string Info;

	public int HP {
		get { return _iHP; }
		set {
			if (Manger != null) Manger.ShowInfoText ((-(_iHP - value)).ToString (), Color.red, transform.position);
			_iHP = Mathf.Clamp (value, 0, MaxHP);
			if (HP == 0) {
				Alive = false;
				transform.parent.gameObject.SetActive (false);
				transform.parent.GetComponent<NetworkElement> ().CreateMarker (Vector3.one * 100);
				GridABS.NodeFromWorldPoint (transform.position).UnitOnNode = null;
			}
			SetValueToBar ();
		}
	}
	public int EP {
		get { return _iEP; }
		set {
			if (Manger != null) Manger.ShowInfoText ((-(_iEP - value)).ToString (), Color.magenta, transform.position);
			_iEP = Mathf.Clamp (value, 0, MaxEP);
			SetValueToBar ();
		}
	}
	private int _iHP, _iEP;

	public int MaxHP, MaxEP;

	public int MaxMovement;

	public UnityEngine.UI.Text HPtext, EPtext;
	public Transform HPBar, EPBar;

	public bool Alive;
	private bool _isPointed;

	public MangerNetwork Manger;
	public IPercActivity PercSystem;
	public IGetingDamage DamageSystem;

	public enum DamageType {
		Physical,
		Fire,
		Poison,
		Electric,
		Stan,
		Heal,
		Energy
	}

	public virtual void EndingTurn () {

	}

	private void SetValueToBar () {
		HPtext.text = HP.ToString ();
		EPtext.text = EP.ToString ();
		HPBar.localScale = Vector3.up * (HP / (float) MaxHP) + Vector3.right + Vector3.forward;
		EPBar.localScale = Vector3.up * (EP / ((float) MaxEP + 1)) + Vector3.right + Vector3.forward;
	}

	public int TimeReusePerc;

	public virtual List<NodeA> MovmentArea () {
		List<NodeA> result = new List<NodeA> ();
		int x = 0, y = 0;
		x = GridABS.NodeFromWorldPoint (transform.position).x;
		y = GridABS.NodeFromWorldPoint (transform.position).y;
		for (var d = 0; d < 4; d++) {
			for (var i = 0; i <= MaxMovement; i++) {
				try {
					var nodes = GridABS.GridOfArena[x + i * MatfSub.MPAer (d, true), y + i * MatfSub.MPAer (d, false)];
					if (nodes.TypeBloc == NodeA.NodeType.Walkable &&
						nodes.UnitOnNode == null) {

						result.Add (nodes);

					}
					if (nodes.TypeBloc == NodeA.NodeType.Wall)
						break;
				} catch { }

			}
		}
		return result;

	}

	public virtual void SetComponents () {
		HP = MaxHP;
		EP = MaxEP;
		SetValueToBar ();
		PercSystem = (IPercActivity) GetComponent<IPercActivity> ();
		DamageSystem = (IGetingDamage) GetComponent<IGetingDamage> ();
	}

	public virtual void GetDamage (int damage = 0, int duration = 0, int typeDamage = 0) {
		switch ((int) typeDamage) {
			case -1:
				DamageSystem.GetMesg ("Miss");
				break;
			case 0:
				DamageSystem.PhysicalDamage (damage);
				break;
			case 1:
				DamageSystem.FireDamage (damage, duration);
				break;
			case 2:
				DamageSystem.PoisonDamage (damage, duration);
				break;
			case 3:
				DamageSystem.ElectricDamage (damage, duration);
				break;
			case 4:
				DamageSystem.GetStun (duration);
				break;
			case 5:
				DamageSystem.GetHeal (damage);
				break;
			case 6:
				DamageSystem.GetEnergyDamage (damage);
				break;
		}
	}

	private void OnMouseEnter () {
		if ((int) Manger.TurnMode == 0) {
			if (this != Manger.CorrectUnit.unit) {
				GetComponent<SpriteRenderer> ().material.color *= new Color (1, 1, 1, 0.3f);
				transform.parent.GetChild (1).GetComponent<SpriteRenderer> ().material.color *= new Color (1, 1, 1, 0.3f);
				_isPointed = true;
			}
		} else {

			SetValueToBar ();
			transform.GetChild (0).gameObject.SetActive (true);
			StartCoroutine (ReSizeBar ());
		}

	}
	private void OnMouseExit () {
		transform.GetChild (0).gameObject.SetActive (false);
		if (_isPointed) {
			GetComponent<SpriteRenderer> ().material.color = new Color (1, 1, 1, 1);
			transform.parent.GetChild (1).GetComponent<SpriteRenderer> ().material.color = new Color (1, 1, 1, 1);
			_isPointed = false;
		}
	}

	private IEnumerator ReSizeBar () {
		float timer = 0;
		while (timer < 0.01f) {
			timer += Time.deltaTime / 20;
			transform.GetChild (0).localScale = 1.36f * Vector3.one * timer;
			yield return new WaitForEndOfFrame ();
		}
	}

	public virtual void MoveUse () { }
	public virtual void AttackUse () { }
	public virtual void PercUse () { }

}
public interface IPercActivity {
	void PercAction ();
	void Reset();
	void Clean ();
}
public interface IGetingDamage {
	void PhysicalDamage (int damage);
	void FireDamage (int damage, int duration);
	void ElectricDamage (int damage, int duration);
	void PoisonDamage (int damage, int duration);
	void GetStun (int duration);
	void GetHeal (int heal);
	void GetMesg (string msg);

	void GetEnergyDamage (int energy);

	bool IsHaveEffect (BasicUnit.DamageType damageType = BasicUnit.DamageType.Stan);
	void CheckEffect ();

}