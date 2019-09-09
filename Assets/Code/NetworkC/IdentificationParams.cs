using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IdentificationParams {

	public static string[] unitNamesArray = new string[0];
	public static BasicUnit[] unitsArray = new BasicUnit[0];

	public static string[] weaponNamesArray = new string[0];
	public static BasicWeapon[] weaponsArray = new BasicWeapon[0];

	public static void LogArray () {
		foreach (BasicUnit n in unitsArray) {
			Debug.Log (n.CorrectName);
			Debug.Log (n.MaxEP);
		}
		foreach (BasicWeapon n in weaponsArray) {
			Debug.Log (n.CorrectName);
			Debug.Log (n.Damage);
		}
	}

	static public int GetIdFromUnit (BasicUnit unit) {
		if (unitNamesArray.Length == 0) GetUnitArray ();

		for (int i = 0; i < unitNamesArray.Length; i++) {
			if (unit.CorrectName == unitNamesArray[i])
				return i;
		}
		return -1;
	}
	static public BasicUnit GetUnitFromId (int id) {
		if (unitsArray.Length == 0) GetUnitArray ();
		return unitsArray[id - 1];
	}

	static public int GetIdFromWeapon (BasicWeapon weapon) {
		if (weaponNamesArray.Length == 0) GetWeaponArray ();

		for (int i = 0; i < weaponNamesArray.Length; i++) {
			if (weapon.CorrectName == weaponNamesArray[i])
				return i;
		}
		return -1;
	}
	static public BasicWeapon GetWeaponFromId (int id) {
		if (weaponsArray.Length == 0) GetWeaponArray ();
		return weaponsArray[id - 1];
	}

	static void GetUnitArray () {
		BasicUnit[] lresult = Resources.LoadAll<BasicUnit> ("UnitData");
		unitsArray = lresult;
		string[] result = new string[lresult.Length];

		for (int i = 0; i < lresult.Length; i++) {
			result[i] = lresult[i].CorrectName;
		}
		unitNamesArray = result;
	}
	static void GetWeaponArray () {
		BasicWeapon[] lresult = Resources.LoadAll<BasicWeapon> ("WeaponData");
		weaponsArray = lresult;
		string[] result = new string[lresult.Length];

		for (int i = 0; i < lresult.Length; i++) {
			result[i] = lresult[i].CorrectName;
		}
		weaponNamesArray = result;
	}

}