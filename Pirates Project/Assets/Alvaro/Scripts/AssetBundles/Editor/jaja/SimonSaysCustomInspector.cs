using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace DefinitiveScript
{
    [CustomEditor(typeof(SimonSays))]
    public class SimonSaysCustomInspector : Editor
    {
        SoundData soundData;
        string fileName = "soundData.json";

        /* void OnEnable()
        {
            LoadSoundData();
        }*/

        /* public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SimonSays script = (SimonSays) target;

            GUIContent soundBundleLabel = new GUIContent("Sound Bundle");

            string[] nameArray = new string[soundData.bundles.Length];
            for(int i = 0; i < nameArray.Length; i++)
            {
                nameArray[i] = soundData.bundles[i].name;
            }

            script.soundBundle = soundData.bundles[EditorGUILayout.Popup(0, nameArray)];

            GUIContent clipsLabel = new GUIContent("Selected Clips");

            nameArray = new string[script.soundBundle.clips.Length];
            for(int i = 0; i < nameArray.Length; i++)
            {
                nameArray[i] = script.soundBundle.clips[i].name;
            }

            for(int i = 0; i < script.selectedClips.Length; i++)
            {
                script.selectedClips[i] = script.soundBundle.clips[EditorGUILayout.Popup(0, nameArray)];
            }
        }*/

        private void LoadSoundData()
        {
            string filePath = Application.streamingAssetsPath + fileName;

            if(File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                soundData = JsonUtility.FromJson<SoundData>(dataAsJson);
            }
            else
            {
                soundData = null;
            }
        }
    }
}

