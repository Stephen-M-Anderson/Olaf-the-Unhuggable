using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOrBullet : MonoBehaviour
{
    public bool amIBullet;
    public GameObject parentObj;

    // Start is called before the first frame update
    void Start()
    {
        parentObj = this.transform.parent.gameObject;
        //Debug.Log("parentObj for " + this.name + " is " + parentObj);
    }
}
