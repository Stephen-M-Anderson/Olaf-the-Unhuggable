/* Description: This script is meant to be attached to an object that you would like to be capable of spawning bullets. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public GameObject bulletPrefab; //In the inspector we drag the bullet prefab for this gameObject
    public Transform spawnLocation; //The location we want the bullet to spawn from
    private bool canSpawn; //A bool that determines whether or not a bullet can be spawned.

    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true; //By default the turret can spawn a bullet on its first frame.
    }

    // Update is called once per frame
    void Update()
    {
        //We only want to run this code if we've allowed the turret to spawn a bullet.
        if (canSpawn == true)
        {
            //We set this bool to false right away so that bullets only spawn on the frames we want them to.
            canSpawn = false;
            //This code spawns the bullet prefab into existance at the location and rotation we specified.
            Instantiate(bulletPrefab, spawnLocation.position, spawnLocation.rotation);
            //Debug.Log("OH FUUUUUUCK BBY I'M SPAWNING!!");

            //This coroutine is called so that we can delay how often bullets spawn.
            StartCoroutine(TurretWaiting());
        }
    }

    //This is the coroutine that delays how often bullets spawn.
    IEnumerator TurretWaiting()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits however many seconds. 
        yield return new WaitForSeconds(1);

        //After we've waited a second (the amount of delay we've input) we flip the bool back to true to 
        //allow bullets to spawn again.
        canSpawn = true;

        //After we have waited x second(s) print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
