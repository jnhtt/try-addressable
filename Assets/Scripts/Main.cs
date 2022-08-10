using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        var op = Addressables.LoadSceneAsync("AddressableScene.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        op.WaitForCompletion();
        //yield return op;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
