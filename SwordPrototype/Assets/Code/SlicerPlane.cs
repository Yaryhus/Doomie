using EZCameraShake;
using EzySlice;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SlicerPlane : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField]
    Transform cutPlane;
    [SerializeField]
    GameObject cam;
    public Volume volume;
    public VolumeProfile bladeModePP;
    public VolumeProfile standartPP;
    public LayerMask layerMask;
    Vector3 cutPlaneDefault;
    public Material sliceMaterial;

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

    [Header("HUD")]
    [SerializeField]
    TextMeshProUGUI text;
    Image slowMoBar;

    bool isInBladeMode;

    // Start is called before the first frame update
    void Start()
    {
        cutPlaneDefault = cutPlane.eulerAngles;
        cutPlane.gameObject.SetActive(false);
        idleSound.Init();
        SliceSound.Init();
        rechargeSliceSound.Init();
    }

    // Update is called once per frame
    void Update()
    {
        //If holding Right click, we slow time and activate "Blade mode"
        if (Input.GetMouseButton(1))
        {
            //Empezamos a contar tiempo en slowmo hasta que se le gaste.
            isInBladeMode = true;

            //Debug.Log("Tiempo lentooo");
            //Slow time code
            Time.timeScale = slowmotion;
            Time.fixedDeltaTime = Time.timeScale * (slowmotion / 10f);
            cutPlane.gameObject.SetActive(true);
            //Block vertical movement of camera
            cam.GetComponent<FPSLookController>().LockVerticalCamera(true);

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
            cam.GetComponent<FPSLookController>().LockVerticalCamera(false);
            ChangePostProcessVolume(standartPP);
            ResetSlicePlane();
            cutPlane.gameObject.SetActive(false);
            isInBladeMode = false;
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
            if(timeRemaining < timeAvailable)
            {
                timeRemaining += Time.deltaTime;
                if()
            }

        }
    }
    void RotateSlicePlane()
    {
        cutPlane.eulerAngles += new Vector3(0, 0, -Input.GetAxis("Mouse X") * 5);

    }
    void ResetSlicePlane()
    {
        //Debug.Log("Resetting from " + cutPlane.rotation);
        cutPlane.rotation = Quaternion.Euler(-5.0f, 0.0f, 0.0f);
        cutPlane.localRotation = Quaternion.Euler(-5.0f, 0.0f, 0.0f);
        //Debug.Log("To... " + cutPlane.rotation);

        //cutPlane = cutPlaneDefault;
        //cutPlane.eulerAngles = cutPlaneDefault;
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
            CameraShaker.Instance.ShakeOnce(shakeMagnitude, shakeRoughness, shakeFadeInTime, shakeFadeOutTime);

            //Hits air or nonSlashable object
            if (hits.Length <= 0)
            {
                //hits air sound
                SliceSound.SetParameter("SliceMaterial", 0);
                SliceSound.PlayOneShot(transform);
                return;
            }

            for (int i = 0; i < hits.Length; i++)
            {
                SlicedHull hull = SliceObject(hits[i].gameObject, null);

                //Enemy damage
                EnemyBehavior enemy = hits[i].transform.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    SliceSound.SetParameter("SliceMaterial", 1);
                    enemy.TakeDamage(500);

                }
                else
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
        go.layer = 13;
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
        //Slice
        return obj.Slice(cutPlane.position, cutPlane.up, crossSectionMaterial);
    }


}
