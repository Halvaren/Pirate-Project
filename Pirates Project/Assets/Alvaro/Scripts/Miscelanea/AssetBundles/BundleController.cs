using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace DefinitiveScript
{
    public class BundleController : MonoBehaviour
    {
        private List<Object[]> listRequestedAssets;
        private int numRequests;

        void Start()
        {
            listRequestedAssets = new List<Object[]>();
            numRequests = 0;
        }

        public void SendAudioRequest(string bundleName, AudioController audioController)
        {
            StartCoroutine(LoadAudioAssetBundle(bundleName, audioController));
        }

        IEnumerator LoadAudioAssetBundle(string bundleName, AudioController audioController)
        {
            int currentRequest = numRequests;
            numRequests++;

            yield return StartCoroutine(LoadAssetBundle(bundleName));

            numRequests--;

            AudioClip[] audioClips = new AudioClip[listRequestedAssets[currentRequest].Length];
            for(int i = 0; i < audioClips.Length; i++)
            {
                audioClips[i] = listRequestedAssets[currentRequest][i] as AudioClip;
            }

            audioController.ResolveAudioRequest(bundleName, audioClips);
        }

        IEnumerator LoadAssetBundle(string bundleName)
        {
            AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, bundleName));
            yield return bundleLoadRequest;

            AssetBundle myLoadedAssetBundle = bundleLoadRequest.assetBundle;
            if (myLoadedAssetBundle == null)
            {
                print("Failed to load AssetBundle!");
                yield break;
            }

            AssetBundleRequest assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync<Object>();
            yield return assetLoadRequest;

            listRequestedAssets.Add(assetLoadRequest.allAssets);

            myLoadedAssetBundle.Unload(false);
        }
    }
}
