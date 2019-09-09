using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMaster : MonoBehaviour, IPercActivity {

    [Header ("Настройки энергетика")]

    private ParticleSystem _lineEffect, _granadeFx, _granadelineFx;
    public int PowerPerc;
    public int Reuse;
    public int MaxDistantion;
    public int EnergyCost;
    public GameObject PrefabMarcer;

    private bool isCreatedMarcer;
    private GameObject marker {
        get {
            if (imarker == null) imarker = Instantiate (PrefabMarcer);
            return imarker;
        }
    }

    private GameObject imarker;
    private Vector3 _point = Vector3.zero;
    BasicUnit ThisUnit;

    public BasicUnit targetVapireEnergy, targetGiveEnergy;
    bool isVampireListCreate, isGiveListCreate;

    public void PercAction () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        Physics.Raycast (ray, out hit, Mathf.Infinity);

        if (ThisUnit == null) ThisUnit = gameObject.GetComponent<BasicUnit> ();
        if (ThisUnit.EP < EnergyCost) {
            ThisUnit.Manger.ShowInfoText ("No energy", Color.white, transform.position);
            ThisUnit.Manger.SetTurnMode (-1);
            return;
        }

        if (!isVampireListCreate) {
            foreach (NetworkElement item in ThisUnit.Manger.ElementsInGame) {
                if (Vector3.Distance (item.transform.position, transform.position) < MaxDistantion && item.unit.Alive) {
                    SpriteRenderer localmarcer = ThisUnit.Manger.PoolElement;
                    localmarcer.transform.position = item.transform.position - Vector3.right * .25f - Vector3.back * .1f;
                    localmarcer.color = Color.magenta;
                    ThisUnit.Manger.TrashNode.Add (localmarcer);
                }

            }
            isVampireListCreate = true;
        }
        if (Input.GetKeyDown (KeyCode.Mouse0) && targetVapireEnergy == null && hit.rigidbody != null) {
            BasicUnit target = hit.rigidbody.GetComponent<BasicUnit> ();
            targetVapireEnergy = target;
            marker.SetActive (true);
            SpriteRenderer localmarcer = marker.GetComponent<SpriteRenderer> ();
            localmarcer.transform.position = target.transform.position - Vector3.right * .25f - Vector3.back * .1f;
            localmarcer.color = Color.magenta;

        }
        if (targetVapireEnergy == null) return;

        if (!isGiveListCreate) {
            ThisUnit.Manger.ClearGrid ();
            foreach (NetworkElement item in ThisUnit.Manger.ElementsInGame) {
                if (Vector3.Distance (item.transform.position, transform.position) < MaxDistantion && item.unit != targetVapireEnergy && item.unit.Alive) {
                    SpriteRenderer localmarcer = ThisUnit.Manger.PoolElement;
                    localmarcer.transform.position = item.transform.position - Vector3.right * .25f - Vector3.back * .1f;
                    localmarcer.color = Color.yellow;
                    ThisUnit.Manger.TrashNode.Add (localmarcer);
                }

            }
            isGiveListCreate = true;
        }
        if (Input.GetKeyDown (KeyCode.Mouse0) && targetGiveEnergy == null && hit.rigidbody != null) {
            BasicUnit target = hit.rigidbody.GetComponent<BasicUnit> ();
            if (targetVapireEnergy != target)
                targetGiveEnergy = target;
        }
        if (targetGiveEnergy == null) return;

        GetFinalAction ();
    }
    public void Reset () {
        marker.SetActive (false);
        targetVapireEnergy = null;
        targetGiveEnergy = null;
        isVampireListCreate = false;
        isGiveListCreate = false;

    }
    void GetFinalAction () {
        int giveValue = (targetVapireEnergy.EP - PowerPerc) > 0 ? (-PowerPerc / 2) : (-PowerPerc - (targetVapireEnergy.EP - PowerPerc)) / 2;
        marker.SetActive (false);
        targetVapireEnergy.GetDamage (PowerPerc, 0, (int) BasicUnit.DamageType.Energy);
        ClientReciverMK1.WriteMsg ("/Damage" + ThisUnit.Manger.GetUnitId (targetVapireEnergy.transform.parent.GetComponent<NetworkElement> ()) + ":" + PowerPerc + ":" + 0 + ":" + (int) BasicUnit.DamageType.Energy);

        if (ThisUnit == targetGiveEnergy) giveValue += PowerPerc + 5;
        else {
            ThisUnit.GetDamage (PowerPerc, 0, (int) BasicUnit.DamageType.Energy);
            ClientReciverMK1.WriteMsg ("/Damage" + ThisUnit.Manger.GetUnitId (transform.parent.GetComponent<NetworkElement> ()) + ":" + PowerPerc + ":" + 0 + ":" + (int) BasicUnit.DamageType.Energy);
        }

        targetGiveEnergy.GetDamage (giveValue, 0, (int) BasicUnit.DamageType.Energy);
        ClientReciverMK1.WriteMsg ("/Damage" + ThisUnit.Manger.GetUnitId (targetGiveEnergy.transform.parent.GetComponent<NetworkElement> ()) + ":" + giveValue + ":" + 0 + ":" + (int) BasicUnit.DamageType.Energy);

        ThisUnit.Manger.ClearGrid ();

        ThisUnit.Manger.EndTurn ();
        ClientReciverMK1.WriteMsg ("/EndTurn");
    }

    public void Clean () {
        if (targetVapireEnergy != null && targetGiveEnergy != null) {
            targetVapireEnergy = null;
            targetGiveEnergy = null;
            isVampireListCreate = false;
            isGiveListCreate = false;
        }
    }

}