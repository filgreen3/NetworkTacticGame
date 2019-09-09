using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyRobot : BasicUnit {

    public int EnergyCost;
    private BasicWeapon _weapon;
    private float _lateTime,_standartWait;
    private int _localEnergyCost;
    public override List<NodeA> MovmentArea () {
        List<NodeA> result = new List<NodeA> ();
        int x = 0, y = 0;
        x = GridABS.NodeFromWorldPoint (transform.position).x;
        y = GridABS.NodeFromWorldPoint (transform.position).y;
        for (var d = 0; d < 4; d++) {
            for (var i = 0; i <= MaxMovement; i++) {
                try {
                    var nodes = GridABS.GridOfArena[x + i * MatfSub.CrossType (d, true), y + i * MatfSub.CrossType (d, false)];
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

    public override void SetComponents () {
        base.SetComponents ();
        _weapon = transform.parent.GetComponent<NetworkElement> ().weapon;
        _weapon.Damage *= 2;
        _lateTime = _weapon.BulletPerShoot * _weapon.WaitTime + 1f;
    }

    public override void EndingTurn () {
        try {
            StartCoroutine (EnergySpend ());
        } catch { }
    }
    public override void MoveUse () {
        _standartWait = .7f;
        _localEnergyCost = EnergyCost * 2;
    }
    public override void AttackUse () {
        _standartWait = _lateTime;
        _localEnergyCost = EnergyCost * 3;
    }
    IEnumerator EnergySpend () {
        yield return new WaitForSeconds (_standartWait);
        EP -= _localEnergyCost;
        if (EP < 1) {
            Alive = false;
            transform.parent.gameObject.SetActive (false);
            transform.parent.GetComponent<NetworkElement> ().CreateMarker (Vector3.one * 100);
            GridABS.NodeFromWorldPoint (transform.position).UnitOnNode = null;
        }
        _localEnergyCost = EnergyCost;
        _standartWait =0;
    }

}