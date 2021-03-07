/* Description: This script is meant to handle the behavior of bullets. So far it propels bullets horizontally until
 * it collides with something, where it then despawns.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryBulletScript : MonoBehaviour
{
    [Header("Movement Variables")]

    private Rigidbody bulletRB; //The bullet's rigidbody
    public Vector3 bulletForce; 
    private Vector3 newBulletForce; //The direction and force applied to move the bullet. This needs to be set by the parry script
    public float bulletSpeed; //The speed at which the bullet moves, this influences the bulletForce Vector3.


    [Header("Dealing Damage")]

    public float damage = 1; //The amount of damage the bullet deals.

    [Header("Hitbox")]
    public Collider[] BulletTargets; //An array of colliders to store information on anything the bullet hits
    public float bulletSphereRadius; //The radius of the spherical hurtbox of this attack
    public LayerMask whatIsPlayer; //A layermask only for the player

    // Start is called before the first frame update
    void Start()
    {
        //getting the rigidbody attached to the bullet prefab
        bulletRB = GetComponent<Rigidbody>();

        newBulletForce = new Vector3(bulletForce.x * bulletSpeed, bulletForce.y * bulletSpeed, 0);

        // A vector 3 representing the magnitude in the x direction the bullet will move
        //bulletForce = new Vector3(bulletSpeed, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //We make the bullet move by giving it the velocity of the bulletForce Vector3
        bulletRB.velocity = newBulletForce;

        //BulletHitbox();
    }
    void BulletHitbox()
    {
        //This sets elements of our array as objects with the player tag that enter the overlap sphere
        BulletTargets = Physics.OverlapSphere(this.transform.position, bulletSphereRadius);

        if (BulletTargets.Length == 1)
        {
            if (BulletTargets[0].gameObject.tag == "Enemy")
            {
                //Stun enemy goes here
                BulletTargets[0].gameObject.GetComponent<UniversalEnemyBehavior>().stunnedBool = true;

                //Debug.Log("Oh shit THESE bullets hurt enemies oh fuck!");
            }

            Destroy(this.gameObject);
        }
    }

    // OnCollisionEnter is called on the frame that the attached object's collider makes contact with another collider
    void OnCollisionEnter(Collision collision)
    {
        //If the collision that occurred is with a gameObject with the "Player" tag AND the player is capable of taking
        //damage (not in the "post damage invulnerability" state) then this code is executed.
        if (collision.gameObject.tag == "Enemy")
        {
            //Stun enemy goes here
            BulletTargets[0].gameObject.GetComponent<UniversalEnemyBehavior>().stunnedBool = true;

            //Debug.Log("Oh shit THESE bullets hurt enemies oh fuck!");
        }

        //the bullet destroys itself upon any collision.
        Destroy(this.gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, bulletSphereRadius);
        //Gizmos.DrawSphere(sphereTwo.position, area);
    }

}