using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatfSub{
 public static int MPAer (int stage, bool isItx) {
        switch (stage) {
            case 0:
                if (isItx) return 1;
                else return 0;
            case 1:
                if (isItx) return -1;
                else return 0;
            case 2:
                if (!isItx) return 1;
                else return 0;
            case 3:
                if (!isItx) return -1;
                else return 0;

            default:
                return 0;
        }
    }
    public static int CrossType (int stage, bool isItx) {
        switch (stage) {
            case 0:
                if (isItx) return 1;
                else return 1;
            case 1:
                if (isItx) return -1;
                else return -1;
            case 2:
                if (!isItx) return-1;
                else return 1;
            case 3:
                if (!isItx) return 1;
                else return -1;

            default:
                return 0;
        }
    }
}
