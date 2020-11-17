using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    Color color = new Color(0,0,0,0);
    float fadeSpeed = 0.05f;

    void Start()
    {
        color = this.GetComponent<MeshRenderer>().material.color;
        Destroy(gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        color.a -= Time.deltaTime * fadeSpeed;
        this.GetComponent<MeshRenderer>().material.color = color;
    }
}
