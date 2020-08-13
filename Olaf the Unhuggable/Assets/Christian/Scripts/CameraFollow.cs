/* Description: The purpose of this script is to move the camera in a way that follows the movement of the player.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target; //The player's transform component set in the inspector.
    public float smoothing = 5f; //A small delay added to make the movement of the camera more smooth.
    private Vector3 offset; //The distance between the camera and the player.


    // Start is called before the first frame update
    void Start()
    {
        //Establishing the offset value as the initial distance between camera and player.
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //The position at which we want the camera to be at each FixedUpdate.
        Vector3 targetCamPosition = target.position + offset;
        //In order to get the camera to the position we want it, we use a Lerp movement. Lerp stands for linear interpretation.
        //essentially what this is doing is modifying the transform value of the camera from its current transform.position
        //to the previously established targetCamPosition above and will achieve this in an interval of our smoothing times
        //Time.deltaTime.
        transform.position = Vector3.Lerp(transform.position, targetCamPosition, smoothing * Time.deltaTime);
    }
}
