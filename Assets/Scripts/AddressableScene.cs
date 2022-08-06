using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableScene : MonoBehaviour
{
    private UICanvas uiCanvas;
    private List<GameObject> cubeList;

    private GameObject cubePrefab;
    private Material redMat;
    private Material greenMat;
    private Material blueMat;

    private IEnumerator Start()
    {
        cubeList = new List<GameObject>();

        var op = Addressables.LoadAssetAsync<GameObject>("Assets/Addressables/UI/UICanvas.prefab");
        yield return op;
        var go = GameObject.Instantiate(op.Result);
        uiCanvas = go.GetComponent<UICanvas>();
        uiCanvas.transform.SetParent(transform);
        if (uiCanvas != null)
        {
            uiCanvas.Init(CreateCube, PushRed, PushGreen, PushBlue);
        }

        //op = Addressables.LoadAssetAsync<GameObject>("Assets/Addressables/Objects/Cube/Cube.prefab");
        op = Addressables.LoadAssetAsync<GameObject>("Cube/Cube.prefab");
        yield return op;
        cubePrefab = op.Result;

        var matOp = Addressables.LoadAssetAsync<Material>("Cube/RedMat.mat");
        yield return matOp;
        redMat = matOp.Result;

        matOp = Addressables.LoadAssetAsync<Material>("Cube/GreenMat.mat");
        yield return matOp;
        greenMat = matOp.Result;

        matOp = Addressables.LoadAssetAsync<Material>("Cube/BlueMat.mat");
        yield return matOp;
        blueMat = matOp.Result;
    }
    private void CreateCube()
    {
        if (cubePrefab != null)
        {
            var cube = GameObject.Instantiate(cubePrefab);
            cubeList.Add(cube);
            cube.transform.SetParent(transform);
            cube.transform.position = 3f * UnityEngine.Random.insideUnitSphere;
        }
    }

    private void SetMaterial(Material mat)
    {
        if (mat == null)
        {
            return;
        }
        foreach (var g in cubeList)
        {
            var r = g.GetComponent<Renderer>();
            r.material = mat;
        }
    }
    private void PushRed()
    {
        SetMaterial(redMat);
    }
    private void PushGreen()
    {
        SetMaterial(greenMat);
    }
    private void PushBlue()
    {
        SetMaterial(blueMat);
    }
}
