using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeExplode : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(WaitForDisable());
    }

    private IEnumerator WaitForDisable()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject.Destroy(this);
    }
}
