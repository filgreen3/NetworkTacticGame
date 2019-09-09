using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaUnit : BasicUnit {

    public int EnergyCost;
    private int MovmentTwo;

    public override List<NodeA> MovmentArea () {

        if(EP>EnergyCost)
        MaxMovement=MovmentTwo;
        else
        MaxMovement= MovmentTwo/2;
       return base.MovmentArea();
    }

public override void SetComponents()
{
    base.SetComponents();
    MovmentTwo= MaxMovement*2;
}

    public override void EndingTurn () {
        EP -= EnergyCost;
    }

}