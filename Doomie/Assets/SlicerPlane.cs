using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SlicerPlane : MonoBehaviour
{
    [SerializeField]
    Transform cutPlane;
    [SerializeField]
    GameObject cam;
    public PostProcessVolume volume;
    public PostProcessProfile bladeModePP;
    public PostProcessProfile standartPP;
    public LayerMask layerMask;
    Vector3 cutPlaneDefault;

    // Start is called before the first frame update
    void Start()
    {
        cutPlaneDefault = cutPlane.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        //If holding Right click, we slow time and activate "Blade mode"
        if (Input.GetMouseButton(1))
        {
            //Debug.Log("Tiempo lentooo");
            //Slow time code
            Time.timeScale = 0.7f;

            //Block vertical movement of camera
            cam.GetComponent<FPSLookController>().LockVerticalCamera(true);

            //Change postprocess volume
            ChangePostProcessVolume(bladeModePP);

            //Rotate slice plane
            RotateSlicePlane();

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Slice!");
                Slice();
            }
        }
        //Disable "Blade Mode"
        else if (Input.GetMouseButtonUp(1))
        {
            //Debug.Log("Se acabó el tiempo lentooo");
            //Reset everything: Time, unlock vertical view, restore postprocess, reset the slice plane.
            Time.timeScale = 1.0f;
            cam.GetComponent<FPSLookController>().LockVerticalCamera(false);
            ChangePostProcessVolume(standartPP);
            ResetSlicePlane();

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
    void ChangePostProcessVolume(PostProcessProfile outPP)
    {
        //Weight do nothing
        volume.weight = Mathf.Lerp(1.0f, 0.0f, 5f);
        volume.profile = outPP;
        volume.weight = Mathf.Lerp(0.0f, 1.0f, 5f);
    }

    public void Slice()
    {
        Collider[] hits = Physics.OverlapBox(cutPlane.position, new Vector3(10, 0.1f, 10), cutPlane.rotation, layerMask);

        if (hits.Length <= 0)
            return;


        for (int i = 0; i < hits.Length; i++)
        {
            SlicedHull hull = SliceObject(hits[i].gameObject, null);
            
            //Enemy damage
            EnemyBehavior enemy = hits[i].transform.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.TakeDamage(500);
            
            }

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

    public void AddHullComponents(GameObject go)
    {
        //Sliceable label
        go.layer = 13;
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(100, go.transform.position, 10);
    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        //Slice
        return obj.Slice(cutPlane.position, cutPlane.up, crossSectionMaterial);
    }


}
