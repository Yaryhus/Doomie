using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField]
    GameObject parent  = null;
    GameObject player  = null;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.Player;
    }
    private void OnTriggerStay(Collider other)
    {
        //The collision is with the player
        if (other.tag == "Player")
        {
            //Debug.Log("I see the Player. Is it behind anything?");
            //Make sure it is the Player via Raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit))
            {
                //If the raycast sees the player without obstacles
                FPSMovementController target = hit.transform.GetComponent<FPSMovementController>();
                if (target != null)
                {
                    //Debug.Log("Oh boy the Player isnt behind anything. Time to ñam.");
                    parent.GetComponent<Enemy>().PlayerSpotted();
                }
                else
                {
                    //Debug.Log("Guess its nothing.");
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}