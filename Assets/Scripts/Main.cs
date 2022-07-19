using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        var op = Addressables.LoadSceneAsync("Assets/Addressables/Scenes/AddressableScene.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        yield return op;
    }

    // Update is called once per frame
    void Update()
    {

    }
}