using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleProjectile : MonoBehaviour
{
    Transform cam;
    GameObject playerGO;
    [Header("Stats")]
    [SerializeField]
    float damage = 10;
    [SerializeField]
    float speed = 2;
    [SerializeField]
    LayerMask bounceableLayer;
    public bool isBounceable;

    [Header("Sonido")]
    [SerializeField]
    Sound impactPlayerSound = null;
    [SerializeField]
    Sound impactNoneSound = null;
    [SerializeField]
    Sound trayectorySound = null;
    GameObject owner;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        playerGO = PlayerManager.instance.Player;

        Vector3 dir = new Vector3(owner.transform.forward.x, owner.transform.forward.y, owner.transform.forward.z).normalized;
        this.GetComponent<Rigidbody>().AddForce(new Vector3(dir.x * speed, 0f, dir.z * speed), ForceMode.Impulse);
        trayectorySound.Play(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("colisiono con " + other.name);

        //If the player is hit
        FPSCameraAndMovController player = other.transform.GetComponent<FPSCameraAndMovController>();
        if (player != null)
        {
            player.TakeDamage(damage);
            impactPlayerSound.Play(other.transform);
            Destroy(this.gameObject);
            trayectorySound.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        //Hits another enemy
        EnemyBehavior enemy = other.transform.GetComponent<EnemyBehavior>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            impactPlayerSound.Play(other.transform);
            Destroy(this.gameObject);
            trayectorySound.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        //Hits a turret
        TurretBehavior turret = other.transform.GetComponent<TurretBehavior>();
        if (turret != null)
        {
            turret.TakeDamage(damage);
            impactPlayerSound.Play(other.transform);
            Destroy(this.gameObject);
            trayectorySound.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }


        //Bounce if possible
        if (other.gameObject.layer.Equals(14) && isBounceable)
        {
            //Debug.Log("rebote");
            //Bounce in the opposite direction and double velocity if we are in blademode and we swing
            playerGO.GetComponent<FPSCameraAndMovController>().PlayBounce();
                GetComponent<Rigidbody>().velocity = -GetComponent<Rigidbody>().velocity * 2.0f;
        }

        //If collides with anything else

        else
        {
            impactNoneSound.Play(other.transform);
            Destroy(this.gameObject);
            trayectorySound.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }


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
