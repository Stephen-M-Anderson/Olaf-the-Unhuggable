using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatMoveDetect : MonoBehaviour
{
    public bool isPlatformed;
    public bool platCheck;
    public Transform player;
    private RaycastHit platProbe;
    private RaycastHit testProbe;


    // Start is called before the first frame update
    void Start()
    {

    }

        // Update is called once per frame
        void Update()
    {

        if (isPlatformed != true)
        {

            platCheck = false;
            if (Input.GetKeyDown("space"))
            {
                player.SetParent(null);
                Debug.Log("Yeeting parent because of controls");
            }
        }

        if (platCheck != true)
        {

            //platProbe = Physics2D.Raycast(transform.position, Vector2.down, .130f);
            Ray platRay = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(platRay, out platProbe, .130f))
            {
                if (platProbe.collider.CompareTag("Platform"))
                {
                    player.SetParent(platProbe.transform);
                    Debug.Log("Parent is Set");

                }
                else
                {
                    Debug.Log("parent is lost.");
                    player.SetParent(null);
                    isPlatformed = false;
                }
                platCheck = true;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        ProcessCollision(collision.gameObject);
        //Debug.Log("collision detected");
    }

    private void OnCollisionEnter(Collision collision)
    {
        ProcessCollision(collision.gameObject);
        //Debug.Log("collision detected");
    }
    void ProcessCollision(GameObject collider)
    {
        if (collider.CompareTag("Platform"))
        {
            //platProbe = Physics2D.Raycast(transform.position, Vector2.down, .130f);
            Ray testRay = new Ray(transform.position, Vector3.down);
            //Debug.Log("Platform collision potentially detected");
            if (Physics.Raycast(testRay, out testProbe, .130f))
            {
                if (testProbe.collider.CompareTag("Platform"))
                {
                    isPlatformed = true;
                    Debug.Log("PLAYER SHOULD BE ON PLATFORM");
                }
            }
            else
            {

            }
        }
    }
}
