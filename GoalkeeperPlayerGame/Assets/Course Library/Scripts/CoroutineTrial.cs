using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTrial : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(coroutineA());
    }

    IEnumerator coroutineA()
    {
        // wait for 1 second
        yield return new WaitForSeconds(1.0f);
        Debug.Log("coroutineA() started: " + Time.time);

        // wait for another 1 second and then create b
        yield return new WaitForSeconds(1.0f);
        Coroutine b = StartCoroutine(coroutineB());

        yield return new WaitForSeconds(2.0f);
        Debug.Log("coroutineA() finished " + Time.time);

        // B() was expected to run for 10 seconds
        // but was shut down here after 3.0f
        StopCoroutine(b);
        yield return null;
    }

    IEnumerator coroutineB()
    {
        float f = 0.0f;
        float start = Time.time;

        Debug.Log("coroutineB() started " + start);

        while (f < 10.0f)
        {
            Debug.Log("coroutineB(): " + f);
            yield return new WaitForSeconds(1.0f);
            f = f + 1.0f;
        }

        // Intended to handling exit of the this coroutine.
        // However coroutineA() shuts coroutineB() down. This
        // means the following lines are not called.
        float t = Time.time - start;
        Debug.Log("coroutineB() finished " + t);
        yield return null;
    }
}
