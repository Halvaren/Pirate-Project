using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DefinitiveScript
{
    public class BuildAssetBundle : Editor
    {
        [MenuItem("Assets/ Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        }
    }
}

