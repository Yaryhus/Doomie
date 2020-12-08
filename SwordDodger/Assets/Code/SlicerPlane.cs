using EZCameraShake;
using EzySlice;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Cinemachine;


public class SlicerPlane : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField]
    Transform cutPlane;
    //[SerializeField]
    //GameObject cam;
    public Volume volume;
    public VolumeProfile bladeModePP;
    public VolumeProfile standartPP;
    public LayerMask layerMask;
    Vector3 cutPlaneDefault;
    public Material sliceMaterial;
    public Animator anim;
    //Target to see and slice
    public Transform target;
    //Target inside the plane
    public Transform planeTarget;
    //Camera
    //public CinemachineExternalCamera cam;
    public CinemachineImpulseSource impulseSource;
    public ParticleSystem[] particles;

    [Header("Variables")]
    [SerializeField]
    int slicesAvailable = 3;
    int slicesUsed = 0;
    [SerializeField]
    float timeAvailable = 5.0f;
    float timeRemaining = 0;
    [SerializeField]
    float timeToRecoverSlice = 1.0f;
    float currentRecoverTime = 0f;

    [SerializeField]
    [Range(0, 1)]
    float slowmotion = 0.5f;

    [Header("Sounds")]
    [SerializeField]
    [Tooltip("Slice Sound variables: 1 - Air Slash, 2: Flesh Impact, 3: Object Impact, 4: Non Slashable object")]
    Sound SliceSound = null;
    [SerializeField]
    Sound idleSound = null;
    [SerializeField]
    Sound rechargeSliceSound = null;
    
    /* Shake causa bloqueo de la coordenada Y
    [Header("Shake")]
    [SerializeField]
    [Tooltip("Suavidad del tiemble. Valores bajos son mas suaves")]
    float shakeRoughness = 1.0f;
    [SerializeField]
    [Tooltip("Intensidad del tiemble")]
    float shakeMagnitude = 1.0f;
    [SerializeField]
    [Tooltip("Cuanto tarda en ocurrir")]
    float shakeFadeInTime = 0.1f;
    [SerializeField]
    [Tooltip("Cuanto tarda en irse")]
    float shakeFadeOutTime = 1f;
    */

    [Header("HUD")]
    [SerializeField]
    TextMeshProUGUI text;
    Image slowMoBar;

    bool isInBladeMode;
    Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        cutPlaneDefault = cutPlane.eulerAngles;
        cutPlane.gameObject.SetActive(false);
        idleSound.Init();
        SliceSound.Init();
        rechargeSliceSound.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInBladeMode)
        {
            anim.SetFloat("X", Mathf.Clamp(target.localPosition.x + 0.3f, -1, 1));
            anim.SetFloat("Y", Mathf.Clamp(target.localPosition.y + .18f, -1, 1));
        }
        else
        {
            ResetSlicePlane();
        }
        //If holding Right click, we slow time and activate "Blade mode"
        if (Input.GetMouseButton(1))
        {
            //Empezamos a contar tiempo en slowmo hasta que se le gaste.
            isInBladeMode = true;
            anim.SetBool("bladeMode", isInBladeMode);
            //Debug.Log("Tiempo lentooo");
            //Slow time code
            Time.timeScale = slowmotion;
            Time.fixedDeltaTime = Time.timeScale * (slowmotion / 10f);
            cutPlane.gameObject.SetActive(true);
            //Block vertical movement of camera
            parent.GetComponent<FPSCameraAndMovController>().LockVerticalCamera(true);

            //Change postprocess volume
            ChangePostProcessVolume(bladeModePP);

            //Rotate slice plane
            RotateSlicePlane();

            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Slice!");
                Slice();
            }
        }
        //Disable "Blade Mode" voluntarily
        else if (Input.GetMouseButtonUp(1))
        {
            //Debug.Log("Se acabó el tiempo lentooo");
            //Reset everything: Time, unlock vertical view, restore postprocess, reset the slice plane.
            //Time.timeScale = 1.0f;
            parent.GetComponent<FPSCameraAndMovController>().LockVerticalCamera(false);
            ChangePostProcessVolume(standartPP);
            ResetSlicePlane();
            cutPlane.gameObject.SetActive(false);
            isInBladeMode = false;
            anim.SetBool("bladeMode", isInBladeMode);
        }
        //update text
        text.SetText(slicesAvailable - slicesUsed + " / " + slicesAvailable);

        //Recover from bulletTime
        Time.timeScale += (1f / 1.1f) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);


        if (!isInBladeMode)
        {
            //Recargar espadazos
            if (slicesUsed > 0)
            {
                currentRecoverTime += Time.deltaTime;
                if (currentRecoverTime >= timeToRecoverSlice)
                {
                    slicesUsed--;
                    currentRecoverTime = 0f;
                    rechargeSliceSound.PlayOneShot(transform);
                }
            }
            //recargar tiempo de Slowmo
            if (timeRemaining < timeAvailable)
            {
                //timeRemaining += Time.deltaTime;
                //if()
            }

        }
    }
    void RotateSlicePlane()
    {
        cutPlane.eulerAngles += new Vector3(0, 0, -Input.GetAxis("Mouse X") * 6);

    }
    void ResetSlicePlane()
    {
        //Debug.Log("Resetting from " + cutPlane.rotation);
        cutPlane.rotation = Quaternion.Euler(-4.0f, 3.0f, -45.0f);
        cutPlane.localRotation = Quaternion.Euler(-4.0f, 3.0f, -45.0f);
        
        anim.SetFloat("X", 1f);
        anim.SetFloat("Y", -1f);
        
        //planeTarget.localPosition = new Vector3(1.3f,0.17f,0f);
        //target.localPosition = new Vector3(0f,0f,9.65f);        
               
    }
    void ChangePostProcessVolume(VolumeProfile outPP)
    {
        //Weight do nothing
        volume.weight = Mathf.Lerp(1.0f, 0.0f, 5f);
        volume.profile = outPP;
        volume.weight = Mathf.Lerp(0.0f, 1.0f, 5f);
    }

    public void Slice()
    {
        //If we already sliced maximun ammount of times
        if (slicesUsed >= slicesAvailable)
            return;
        else
        {
            //Increment slice and do the rest
            slicesUsed++;
            Collider[] hits = Physics.OverlapBox(cutPlane.position, new Vector3(10, 0.1f, 10), cutPlane.rotation, layerMask);
            //CameraShaker.Instance.ShakeOnce(shakeMagnitude, shakeRoughness, shakeFadeInTime, shakeFadeOutTime);
            ShakeCamera();

            planeTarget.localPosition = new Vector3(planeTarget.localPosition.x * -1, planeTarget.localPosition.y * -0.5f);
            //Hits air or nonSlashable object
            if (hits.Length <= 0)
            {
                //hits air sound
                SliceSound.SetParameter("SliceMaterial", 0);
                SliceSound.PlayOneShot(transform);
                particles[0].Play();
                return;
            }

            for (int i = 0; i < hits.Length; i++)
            {
                SlicedHull hull = SliceObject(hits[i].gameObject, sliceMaterial);
                /*
                particles[1].transform.position = hits[i].transform.position;
                particles[1].transform.forward = cam.transform.forward;
                particles[1].Play();*/

                /*
                //Enemy damage
                EnemyBehavior enemy = hits[i].transform.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    SliceSound.SetParameter("SliceMaterial", 1);
                    enemy.TakeDamage(500);

                }
                else*/


                SliceSound.SetParameter("SliceMaterial", 2);
                SliceSound.PlayOneShot(transform);

                if (hull != null)
                {
                    GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, null);
                    GameObject top = hull.CreateUpperHull(hits[i].gameObject, null);
                    AddHullComponents(bottom);
                    AddHullComponents(top);
                    Destroy(hits[i].gameObject);
                }
            }
        }

    }

    public void AddHullComponents(GameObject go)
    {
        //Sliceable label
        go.layer = 12;
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(150, go.transform.position, 10);

        //Adding Self destruct script.
        go.AddComponent<FadeAndDestroy>();

    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(cutPlane.position, cutPlane.up, crossSectionMaterial);
    }

    public void ShakeCamera()
    {
        //cam.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        impulseSource.GenerateImpulse();
        
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
    }

}
