using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public BundleData[] bundles;
}

[System.Serializable]
public class BundleData
{
    public string name;
    public ClipData[] clips;
}

[System.Serializable]
public class ClipData
{
    public string name;
}
