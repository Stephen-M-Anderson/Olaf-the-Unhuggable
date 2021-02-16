/* Description: The purpose of this script is to be able to do a parry. When parrying, the player will get an overlap sphere around them and
 * any attack on the "parryable" layer that enters the overlap sphere while it's active will be nullified and the player will receive a bonus.
 * If the player is on the ground then the bonus is to stun the enemy, if the player is in the air (in ball form) then their next zoom will
 * be twice as fast.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour
{
    public bool parrySuccessful = false;

    bool canParry = true;
    bool parryBool = false;
    bool currentlyParrying = false;
    bool cooldownIsHappening = false;
    public float parryDuration;
    public float parryCooldown;

    public Collider[] parriedThings;
    public LayerMask whatIsParryable;
    public float parrySphereRadius;

    bool ballManParry;
    bool zoomingParry;
    bool bulletParry;
    bool dangerZoneParry;
    GameObject whoAttackThisIs;

    //Ball Man? Zooming? Bullet? Danger Zone Attack? What enemy does the attack belong to?

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire3") && canParry)
        {
            parryBool = true;
            canParry = false;

            //A coroutine is called to determine the duration of the sphere
            StartCoroutine(ParryOverlapDuration());

            //Debug.Log("Fuckin Pressed that parry button didn't I?");
        }

        if(parryBool)
        {
            ParryFunction();
            currentlyParrying = true;
        } else
        {
            currentlyParrying = false;
            //Debug.Log("Fuckin stopped Parryin every frame dint I?");
        }

        if (currentlyParrying == false && canParry == false && cooldownIsHappening == false)
        {
            StartCoroutine(ParryCooldown());
        }

    }

    void ParryFunction()
    {
        //Debug.Log("Fuckin Parryin every frame ain't I?");

        //The object holding the physics overlap sphere is called
        parriedThings = Physics.OverlapSphere(this.transform.position, parrySphereRadius, whatIsParryable);

        //Did something Parryable enter the overlap sphere? Begin a for loop for every entry of the array. For each entry gather the following
        //information: Ball Man? Zooming? Bullet? Danger Zone Attack? What enemy does the attack belong to? Then execute the correct switch case.

        ballManParry = this.gameObject.GetComponent<playerController>().ballManBool; //Are we in ball man?
        zoomingParry = this.gameObject.GetComponent<GrappleScriptEvenNewer>().isZooming; //Are we zooming?

        for (int i = 0; i < parriedThings.Length; i++)
        {
            bulletParry = parriedThings[i].gameObject.GetComponent<AttackOrBullet>().amIBullet; //Is the parried object a bullet?
            dangerZoneParry = !(parriedThings[i].gameObject.GetComponent<AttackOrBullet>().amIBullet); //Is the parried object a Danger Zone Attack?
            whoAttackThisIs = parriedThings[i].gameObject.GetComponent<AttackOrBullet>().parentObj; //What object is the parent of the parried thing?

            //Switch Case Time:
            //1.) Reflect & Destroy Bullet (If bullet = true & Not Ball Man)
            //2.) Bounce up like Cuphead Parry, give double speed to next Zoom, Stun enemy (if Bullet = false, Danger Zone = True, Ball man = True)
            //3.) Reflect, Bounce, Double Zoom. Also Destroy bullet (if Bullet = true, Danger Zone = false, Ball man = True)
            //4.) Yo-Yo Zoom (if Bullet = false, Danger Zone = True, and Zooming = true)
        }

        //Are we ball man? 
        //Yes we are ball man.
        //Are We zooming?
        //Yes we are zooming
        //if close range then Yo-Yo. If bullet then reflect
        //No We are not zooming
        //If close range then stun and double zoom speed. If Bullet then reflect
        //No we aren't ball man.
        //Is it a bullet?
        //Yes it is a bullet so reflect it
        //No it is not a bullet so stun and double zoom speed.

        //That seems complicated but let's break down what we need to know: 
        // - Ball man or Not?
        // - Zooming or Not?
        // - Bullet or Danger Zone Attack?
        // - What enemy does the attack belong to?
        //As long as we have that we could have these Switch Cases:
        //1.) Reflect & Destroy Bullet (If bullet = true & Not Ball Man)
        //2.) Bounce up like Cuphead Parry, give double speed to next Zoom, Stun enemy (if Bullet = false, Danger Zone = True, Ball man = True)
        //3.) Reflect, Bounce, Double Zoom. Also Destroy bullet (if Bullet = true, Danger Zone = false, Ball man = True)
        //4.) Yo-Yo Zoom (if Bullet = false, Danger Zone = True, and Zooming = true)
    }
    void ReflectBullet()
    {
        //Who did this bullet come from?
        //Whoever they are send it flying towards them
    }

    IEnumerator ParryOverlapDuration()
    {
        yield return new WaitForSeconds(parryDuration);

        //Get rid of overlap sphere object
        parryBool = false;
    }

    IEnumerator ParryCooldown()
    {
        cooldownIsHappening = true;
        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
        cooldownIsHappening = false; 

        //Debug.Log("Guess what fucker you can parry again");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.transform.position, parrySphereRadius);
    }
}
