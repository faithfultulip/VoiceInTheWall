using UnityEditor;
using UnityEngine;

namespace LBG.UI.Radial
{
	[CustomEditor(typeof(RadialLayerSelection), true)]
	[CanEditMultipleObjects]
	public class RadialMenuSelectionLayerEditor : RadialMenuElementLayerEditor
	{
		#region Serialized Properties

		//The serialized properties from the targets script

		protected SerializedProperty m_Left;
		protected SerializedProperty m_Right;
		protected SerializedProperty m_BottomLeft;
		protected SerializedProperty m_BottomMiddle;
		protected SerializedProperty m_BottomRight;
		protected SerializedProperty m_UpdateOnSwitch;

        #endregion

        protected override void OnEnable()
		{
			base.OnEnable();

			//Find all of the properties and assign the serialized properties

			m_Left			    = serializedObject.FindProperty("m_Left");
			m_Right			    = serializedObject.FindProperty("m_Right");
			m_BottomLeft	    = serializedObject.FindProperty("m_BottomLeft");
			m_BottomMiddle	    = serializedObject.FindProperty("m_BottomMiddle");
			m_BottomRight	    = serializedObject.FindProperty("m_BottomRight");
			m_UpdateOnSwitch	= serializedObject.FindProperty("m_UpdateOnSwitch");
        }

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			//Update the object
			serializedObject.Update();

			GUILayout.Label("Selection Layer Information", EditorSettings.editorHeaderStyle);

			EditorGUILayout.PropertyField(m_UpdateOnSwitch, EditorSettings.GetAtrributes(m_UpdateOnSwitch), true);
			EditorGUILayout.PropertyField(m_Left, EditorSettings.GetAtrributes(m_Left), true);
            EditorGUILayout.PropertyField(m_Right, EditorSettings.GetAtrributes(m_Right), true);
			EditorGUILayout.PropertyField(m_BottomLeft, EditorSettings.GetAtrributes(m_BottomLeft), true);
			EditorGUILayout.PropertyField(m_BottomMiddle, EditorSettings.GetAtrributes(m_BottomMiddle), true);
			EditorGUILayout.PropertyField(m_BottomRight, EditorSettings.GetAtrributes(m_BottomRight), true);


			//Apply all of the changes
			serializedObject.ApplyModifiedProperties();

			GUILayout.Space(20);
		}
	}
}