using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class SpawnObjectAddressables : MonoBehaviour
{

    [SerializeField] private AssetReference assetReference;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            assetReference.LoadAssetAsync<GameObject>().Completed +=
            (asyncOperationHandle) =>
            {
                if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Instantiate(asyncOperationHandle.Result);
                }
                else
                {
                    Debug.Log("Error loading addressables.");
                }
            };
        }
    }




}
