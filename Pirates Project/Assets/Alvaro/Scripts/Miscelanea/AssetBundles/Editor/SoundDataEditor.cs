using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SoundDataEditor : EditorWindow
{
    public SoundData soundData;
    private string fileName = "soundData.json";

    [MenuItem("Window/Sound Data Editor")]
    static void Init()
    {
        SoundDataEditor window = (SoundDataEditor)EditorWindow.GetWindow(typeof(SoundDataEditor));
        window.Show();
    }

    void OnGUI()
    {
        if(soundData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("soundData");

            //EditorGUILayout.PropertyField(bundleProperty, true);
            //EditorGUILayout.PropertyField(clipProperty, true);
            EditorGUILayout.PropertyField(serializedProperty, true);

            serializedObject.ApplyModifiedProperties();

            if(GUILayout.Button("Save data"))
                SaveSoundData();
        }

        if(GUILayout.Button("Load data"))
                LoadSoundData();
    }

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
            soundData = new SoundData();
        }
    }

    private void SaveSoundData()
    {
        string dataAsJson = JsonUtility.ToJson(soundData);

        string filePath = Application.streamingAssetsPath + fileName;
        File.WriteAllText(filePath, dataAsJson);
    }
}
