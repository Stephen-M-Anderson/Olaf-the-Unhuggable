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
    bool canParry = true; //If this bool is true then you are able to parry
    bool parryBool = false; //If this bool is true then you have begun to attempt to parry
    bool currentlyParrying = false; //If this bool is true then the parry function is currently being called
    bool cooldownIsHappening = false; //If this bool is true then the parry is currently in cooldown
    public float parryDuration; //The length of time that the parry lasts
    public float parryCooldown; //The length of time that it takes for the parry ability to cooldown

    public Collider[] parriedThings; //An array filled with the colliders of the objects being parried
    public LayerMask whatIsParryable; //The layermask that determines if an object can be parried
    public float parrySphereRadius; //The radius of this sphere determines the amount of distance an object can be parried from the player upon activating the parry

    [Header("Parry Types")]

    public bool ballManParry; //If you parried while Olaf is a ball but not zooming then it should give you double speed on your next zoom
    public bool zoomingParry; //If you parried while you were zooming to the enemy then you should yo-yo back to the enemy
    public bool bulletParry; //If you parried a bullet then you should reflect that bullet back to the enemy it belongs to
    public bool dangerZoneParry; //If you parried in an enemy's danger zone while not zooming then you should stun the enemy
    GameObject whoAttackThisIs; //The game object that the bullet being parried belongs to

    public int parryType;

    public Vector3 parryBulletDirection;
    public GameObject parryBulletPrefab;

    //Ball Man? Zooming? Bullet? Danger Zone Attack? What enemy does the attack belong to?

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire3") && canParry) //We've pressed the parry button and are currently able to parry
        {
            parryBool = true; //A parry is being attempted so we flip this bool
            canParry = false; //We flip this bool to begin the process of the parry's cooldown

            //A coroutine is called to determine the duration of the sphere
            StartCoroutine(ParryOverlapDuration());

            Debug.Log("Fuckin Pressed that parry button didn't I?");
        }

        if(parryBool == true) //A parry is being attempted so we will do the following code:
        {
            ParryFunction(); //Calling the function that the parry itself is held
            currentlyParrying = true; //We flip this bool to tell us the parry function itself has been called
        } else
        {
            currentlyParrying = false; //We flip this bool to tell us the parry function itself is not being called
            //Debug.Log("Fuckin stopped Parryin every frame dint I?");
        }

        if (currentlyParrying == false && canParry == false && cooldownIsHappening == false)
        {
            StartCoroutine(ParryCooldown()); //If we are not currently parrying, are unable to parry, and a cooldown is not happening then we start the cooldown for the parry
        }

    }

    void ParryFunction()
    {
        //Debug.Log("Fuckin Parryin every frame ain't I?");

        StartCoroutine(this.gameObject.GetComponent<playerHealth>().PostDamageInvulnerabilityWait());

        //The object holding the physics overlap sphere is called
        parriedThings = Physics.OverlapSphere(this.transform.position, parrySphereRadius, whatIsParryable);

        if (parriedThings.Length > 0) //Debug message to let us know what objects are being parried
        {
            Debug.Log("Holy fuck dude, I'm parrying: " + parriedThings[0].gameObject.name);
        }

        //Did something Parryable enter the overlap sphere? Begin a for loop for every entry of the array. For each entry gather the following
        //information: Ball Man? Zooming? Bullet? Danger Zone Attack? What enemy does the attack belong to? Then execute the correct switch case.

        ballManParry = this.gameObject.GetComponent<playerController>().ballManBool; //Are we in ball man?
        zoomingParry = this.gameObject.GetComponent<GrappleScriptEvenNewer>().isZooming; //Are we zooming?

        


        for (int i = 0; i < parriedThings.Length; i++)
        {
            bulletParry = parriedThings[i].gameObject.GetComponent<AttackOrBullet>().amIBullet; //Is the parried object a bullet?
            dangerZoneParry = !(parriedThings[i].gameObject.GetComponent<AttackOrBullet>().amIBullet); //Is the parried object a Danger Zone Attack?
            whoAttackThisIs = parriedThings[i].gameObject.GetComponent<AttackOrBullet>().parentObj; //What object is the parent of the parried thing?

            if (bulletParry == true && ballManParry == false)
            {
                Debug.Log("1.) Reflect, Destroy Bullet, Double Zoom");

                //Reflect the bullet
                parriedThings[i].gameObject.GetComponent<BulletScript>().bulletParried = true;

                //Get rid of the bullet being deflected
                //Destroy(parriedThings[i].gameObject);

                //Double Next Zoom
                this.gameObject.GetComponent<GrappleScriptEvenNewer>().DoubleZoomSpeed();

                //Stop parrying
                parryBool = false;
            }

            else if (bulletParry == false && dangerZoneParry == true && ballManParry == true && zoomingParry == false)
            {
                Debug.Log("2.) Bounce up like Cuphead Parry, give double speed to next Zoom, Stun enemy");

                //Bounce like Cuphead Parry
                this.gameObject.GetComponent<playerController>().ParryJump();

                //Double Next Zoom
                this.gameObject.GetComponent<GrappleScriptEvenNewer>().DoubleZoomSpeed();

                //Stun Enemy
                whoAttackThisIs.GetComponent<UniversalEnemyBehavior>().parryStun = true;

                //Get Rid of Danger Zone attack
                whoAttackThisIs.GetComponent<UniversalEnemyBehavior>().RemoveRed();

                //Stop parrying
                parryBool = false;
            }

            else if (bulletParry == true && dangerZoneParry == false && ballManParry == true)
            {
                Debug.Log("3.) Reflect, Bounce, Double Zoom. Also Destroy bullet");

                //Reflect the bullet
                //ReflectBullet(whoAttackThisIs, parriedThings[i].gameObject);
                parriedThings[i].gameObject.GetComponent<BulletScript>().bulletParried = true;

                //Get rid of the bullet being deflected
                //Destroy(parriedThings[i].gameObject);

                //Bounce like Cuphead Parry
                this.gameObject.GetComponent<playerController>().ParryJump();

                //Double Zoom
                this.gameObject.GetComponent<GrappleScriptEvenNewer>().DoubleZoomSpeed();

                //Stop parrying
                parryBool = false;
            }

            else if (bulletParry == false && dangerZoneParry == true && zoomingParry == true)
            {
                Debug.Log("4.) Yo-Yo Zoom");

                //Stun Enemy
                whoAttackThisIs.GetComponent<UniversalEnemyBehavior>().parryStun = true;

                //Zoom backwards
                //The plan for this is to have it be implemented in the Grapple Script with regular zooming. Essentially the idea is to set
                //up an imaginary object to zoom to in the exact opposite direction the player was zooming to begin with, then when they either
                //"hit" the invisible object or actually hit anything else (without taking damage) then they zoom back to the enemy.
                //Zoom back to enemy

                //Get Rid of Danger Zone attack
                whoAttackThisIs.GetComponent<UniversalEnemyBehavior>().RemoveRed();

                //Begin the Yo-Yo Zoom
                this.gameObject.GetComponent<GrappleScriptEvenNewer>().yoyoZoom = true;

                //Stop parrying
                parryBool = false;
            }

            //Switch Case Time (or a series of else if statements if the switch cases piss me off):
            //1.) Reflect, Destroy Bullet, Double Zoom (If bullet = true & Not Ball Man)
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

    //ReflectBullet didn't work so we now have a new function in BulletScript that handles reflection successfully.
    void ReflectBullet(GameObject whoDidThisComeFrom, GameObject theBulletBeingDeflected)
    {
        /*//We determine the direction we want the bullet to go based on where the enemy is that shot it relative to our player. I hope I didn't
        //do the math backwords...
        parryBulletDirection = whoDidThisComeFrom.transform.position;
 
        GameObject go = (GameObject)Instantiate(parryBulletPrefab, whoDidThisComeFrom.transform.position, Quaternion.identity);

        go.transform.SetParent(transform);

        //Deflecting works by detroying the enemy bullet and instantiating a new prefab in it's place called a "parry bullet". We want this
        //instantiated prefab to be a local variable so we can mess with some values in one of it's scripts.



        var parryBulletScript = go.GetComponent<ParryBulletScript>();
        parryBulletDirection = parryBulletScript.bulletForce;
        

        //When a new parry bullet is instantiated we need to tell it which direction to go.*/

        //Rigidbody arbies = theBulletBeingDeflected.GetComponent<Rigidbody>();
        //Vector3 backwards = -arbies.velocity;
        //arbies.AddForce(backwards * Time.deltaTime, ForceMode.Impulse);



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
