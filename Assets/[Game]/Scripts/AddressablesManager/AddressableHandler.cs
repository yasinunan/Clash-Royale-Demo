using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UnityEngine.UI;

namespace UOP1.Helper
{
    public class AddressableHandler : MonoBehaviour
    {

        [SerializeField] Transform groundHolder;
        [SerializeField] TextMeshProUGUI popUptext;
        [SerializeField] private AssetReference decorationAddresable;
        private AsyncOperationHandle m_decorationsLoadingHandle;

        void Start()
        {
            Caching.ClearCache();

            var downloadSizeCall = Addressables.GetDownloadSizeAsync(decorationAddresable.RuntimeKey);
            downloadSizeCall.Completed += handle =>
            {
                var sizeInBytes = handle.Result;
                var sizeInMb = sizeInBytes / (1024f * 1024f);
                var popupString = $"Additional ({sizeInMb:F1} MB) file needs to be download.";
                popUptext.text = popupString;
            };

        }

        public void InstantiateDecoration()
        {
            m_decorationsLoadingHandle = Addressables.InstantiateAsync(decorationAddresable, groundHolder, false);

            m_decorationsLoadingHandle.Completed += OnDecorationsInstantiated;
        }

        private void OnDecorationsInstantiated(AsyncOperationHandle obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                LevelManager.Instance.StartGame();
                Debug.Log("Decorations instantiated succesfully");
            }
        }
    }
}

