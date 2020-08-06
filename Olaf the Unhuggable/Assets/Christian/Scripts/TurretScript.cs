using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnLocation;
    private Vector3 spawn;
    private bool canSpawn;

    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn == true)
        {
            canSpawn = false;
            Instantiate(bulletPrefab, spawnLocation.position, spawnLocation.rotation);
            //Debug.Log("OH FUUUUUUCK BBY I'M SPAWNING!!");
            StartCoroutine(TurretWaiting());
        }
    }

    IEnumerator TurretWaiting()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits however many seconds. 
        yield return new WaitForSeconds(1);

        canSpawn = true;

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
