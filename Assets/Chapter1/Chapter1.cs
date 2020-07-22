using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Chapter1 : MonoBehaviour
{
    private GameObject prefab;
    private int counter;

    private IEnumerator Start()
    {
        var op = Addressables.LoadAssetAsync<GameObject>("Cube");
        yield return new WaitUntil(() => op.IsDone);
        prefab = op.Result;
    }

    private void Update()
    {
        CreateCube();
    }

    private void CreateCube()
    {
        if (prefab == null)
        {
            return;
        }

        if (counter == 120)
        {
            counter = 0;
            var go = GameObject.Instantiate(prefab);
            go.transform.position = 5f * UnityEngine.Random.insideUnitSphere;
        }
        ++counter;
    }
}
