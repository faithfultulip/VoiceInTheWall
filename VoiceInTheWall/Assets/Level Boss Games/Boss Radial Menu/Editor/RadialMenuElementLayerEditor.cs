using UnityEditor;
using UnityEngine;

namespace LBG.UI.Radial
{
	[CustomEditor(typeof(RadialLayerElements), true)]
	[CanEditMultipleObjects]
	public class RadialMenuElementLayerEditor : RadialMenuLayersEditor 
	{

		#region Serialized Properties

		//The serialized properties from the targets script

		protected SerializedProperty m_Header;
		protected SerializedProperty m_ElementSnapOverwrite;
		protected SerializedProperty m_ElementSnap;

		#endregion

		protected override void OnEnable()
		{
			base.OnEnable();

			//Find all of the properties and assign the serialized properties

			m_Header = serializedObject.FindProperty("m_MenuHeader");
			m_ElementSnapOverwrite = serializedObject.FindProperty("m_ElementSnapOverwrite");
			m_ElementSnap = serializedObject.FindProperty("m_ElementSnap");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			//Update the object
			serializedObject.Update();

			GUILayout.Label("Element Layer Information", EditorSettings.editorHeaderStyle);

			GUILayout.Label("Layer Header", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_Header, EditorSettings.GetAtrributes(m_Header), true);

			GUILayout.Label("Settings", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_ElementSnapOverwrite, EditorSettings.GetAtrributes(m_ElementSnapOverwrite), true);

			if (m_ElementSnapOverwrite.boolValue)
			{
				EditorGUILayout.PropertyField(m_ElementSnap, EditorSettings.GetAtrributes(m_ElementSnap), true);
			}

			//Apply all of the changes
			serializedObject.ApplyModifiedProperties();

			GUILayout.Space(20);
		}
	}
}