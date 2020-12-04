using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleProjectile : MonoBehaviour
{
    Transform cam;
    GameObject player;
    [Header("Stats")]
    [SerializeField]
    float damage = 10;
    [SerializeField]
    float speed = 2;

    GameObject owner;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        player = PlayerManager.instance.Player;
        
        Vector3 dir = new Vector3(owner.transform.forward.x,owner.transform.forward.y,owner.transform.forward.z).normalized;
        this.GetComponent<Rigidbody>().AddForce(dir * speed, ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other)
    {
        //If the player is hit
        PlayerHealth player = other.transform.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
        //If collides with anything else

            Destroy(this.gameObject);

    }

    public GameObject getOwner()
    {
        return owner;
    }

    public void setOwner(GameObject own)
    {
        owner = own;
    }
   
}
