using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WormholeGenerator))]
public class WormholeGeneratorInspector : Editor
{
    

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WormholeGenerator generator = (WormholeGenerator)target;

       


        if (GUILayout.Button("Rebuild Wormhole"))
        {
            generator.CreateWormhole();
        }
    }
}
