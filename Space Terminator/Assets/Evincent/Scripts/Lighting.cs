using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    public bool flickering = false;
    public float time;

    IEnumerator lightingBlinking()
    {
        //flickering = true;
        this.gameObject.GetComponent<Light>().enabled = true;
        time = Random.Range(0.05f, 0.2f);
        yield return new WaitForSeconds(time);
        this.gameObject.GetComponent<Light>().enabled = false;
        time = Random.Range(0.1f, 0.2f);
        yield return new WaitForSeconds(time);
        this.gameObject.GetComponent<Light>().enabled = true;
        time = Random.Range(0.05f, 0.2f);
        yield return new WaitForSeconds(time);
        this.gameObject.GetComponent<Light>().enabled = false;
        time = Random.Range(0.1f, 0.2f);
        yield return new WaitForSeconds(time);
        this.gameObject.GetComponent<Light>().enabled = true;
        time = Random.Range(0.05f, 0.2f);
        yield return new WaitForSeconds(time);
        this.gameObject.GetComponent<Light>().enabled = false;
        time = Random.Range(0.1f, 0.2f);
        yield return new WaitForSeconds(time);
        this.gameObject.GetComponent<Light>().enabled = true;
        time = Random.Range(0.05f, 0.2f);
        yield return new WaitForSeconds(time);
        this.gameObject.GetComponent<Light>().enabled = false;
        time = Random.Range(0.1f, 0.2f);
        yield return new WaitForSeconds(time);
        this.gameObject.GetComponent<Light>().enabled = true;
        time = Random.Range(0.2f, 05f);
        yield return new WaitForSeconds(time);
        flickering = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && flickering == false)
        {
            Debug.Log("Hi");
            StartCoroutine(lightingBlinking());
        }
    }
}
