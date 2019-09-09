using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShildUserDamageSystem : BasicGetDamage, IGetingDamage {

    public void PhysicalDamage (int damage) {
          if (ThisUnit.EP > 0) {
            ThisUnit.EP -= damage;
            return;
        }
        ThisUnit.HP -= damage;
    }

    public void ElectricDamage (int damage, int duration) {

    }

    public void FireDamage (int damage, int duration) {

        if (ThisUnit.EP > 0) {
            ThisUnit.EP -= damage;
            return;
        }

        ThisUnit.HP -= damage;
        _durations[1] = duration;
        _damages[1] = damage;
        _effectObjects[1].SetActive (true);
        _effectObjects[1].transform.position = transform.position + Vector3.up;
    }

    public void GetHeal (int heal) {
        throw new System.NotImplementedException ();
    }

    public void GetStun (int duration) {
        _durations[4] = duration;
        _effectObjects[4].SetActive (true);
        _effectObjects[4].transform.position = transform.position + Vector3.up;
    }

    public bool IsHaveEffect (BasicUnit.DamageType damage) {
        if (_durations[(int) damage] > 0) return true;
        return false;
    }

    public void PoisonDamage (int damage, int duration) {
          if (ThisUnit.EP > 0) {
            ThisUnit.EP -= damage;
            return;
        }
        throw new System.NotImplementedException ();
    }
    public void GetEnergyDamage (int damage) {        
        ThisUnit.EP -= damage;
        if (GetComponent<EnergyRobot> () != null && ThisUnit.EP < 1) GetComponent<EnergyRobot> ().EndingTurn ();
    }

    public void GetMesg (string msg) {
        ThisUnit.Manger.ShowInfoText (msg, Color.white, transform.position);
    }
    public void CheckEffect () {
        if (_durations[4] > 0) {
            ThisUnit.Manger.EndTurn ();
            ThisUnit.Manger.ShowInfoText ("Stan", Color.white, transform.position);

        }
        for (int i = 0; i < effectCount; i++) {
            if (_damages[i] != 0 && _durations[i] > 0) {
                ThisUnit.HP -= _damages[i];
            }
            _durations[i]--;
            if (_durations[i] < 0 && _effectObjects[i].activeInHierarchy) {
                _effectObjects[i].SetActive (false);
                _damages[i] = 0;
            }

        }
    }
}