using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    Color color = new Color(0, 0, 0, 0);
    float fadeSpeed = 0.05f;
    WaitForSeconds wait;
    float time = 20.0f;
    bool startShirnk = false;

    void Start()
    {
        wait = new WaitForSeconds(time);
        //Le dejamos sin colision y sin poder ser re-cortado.
        wait = new WaitForSeconds(5.0f);
        //color = this.GetComponent<MeshRenderer>().material.color;
        StartCoroutine(DestroySlowly());
    }

    // Update is called once per frame
    void Update()
    {
        //Hacemos transparente el objeto con el tiempo. Actualmente no funciona con el shader del slice asi que se quedará comentado.
        //color.a -= Time.deltaTime * fadeSpeed;
        //this.GetComponent<MeshRenderer>().material.color = color;
        if (startShirnk)
        {
            if (transform.localScale.y >= 0.03f)
            {
                transform.localScale += new Vector3(0.1F, .1f, .1f) * -2.0f * Time.deltaTime;
                //transform.Rotate(new Vector3(0.0f,1.0f,0.0f),15f);
            }

        }
    }
    IEnumerator DestroySlowly()
    {
        yield return wait;
        startShirnk = true;
        Destroy(gameObject, 5.0f);
    }

    //Quitamos etiqueta sliceable despues de 5 segundos para evitar problemas de performance con objetos que ya han sido destruidos.
    IEnumerator LoseSliceableTag()
    {
        yield return wait;
        gameObject.layer = 0;
    }
}
