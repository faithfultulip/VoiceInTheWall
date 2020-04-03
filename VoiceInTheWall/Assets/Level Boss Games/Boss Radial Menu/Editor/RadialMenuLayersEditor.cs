using UnityEditor;
using UnityEngine;

namespace LBG.UI.Radial
{
	[CustomEditor(typeof(RadialLayer))]
	[CanEditMultipleObjects]
	public class RadialMenuLayersEditor : Editor
	{
		#region Serialized Properties

		//The serlized object
		
		//The serialized properties from the targets script

		protected SerializedProperty m_LayerName;
		protected SerializedProperty m_LayerEvent;
		protected SerializedProperty m_OverwriteTimers;
		protected SerializedProperty m_ReadyUpTime;
		protected SerializedProperty m_CleanUpTime;
		protected SerializedProperty m_OverwriteDetailPanel;
		protected SerializedProperty m_ShowLayerName;
		protected SerializedProperty m_ShowDetailText;
		protected SerializedProperty m_ShowSelectedElementSprite;
		protected SerializedProperty m_OverwriteTouchOpertation;
		protected SerializedProperty m_DragAndTouch;

		#endregion

		protected virtual void OnEnable()
		{
			//Find all of the properties and assign the serialized properties

			m_LayerName					= serializedObject.FindProperty("m_LayerName");
			m_LayerEvent				= serializedObject.FindProperty("m_LayerEvent");
			m_OverwriteTimers			= serializedObject.FindProperty("m_OverwriteTimers");
			m_ReadyUpTime				= serializedObject.FindProperty("m_OverwriteReadyUpTime");
			m_CleanUpTime				= serializedObject.FindProperty("m_OverwriteCleanUpTime");
			m_OverwriteDetailPanel		= serializedObject.FindProperty("m_OverwriteDetailPanel");
			m_ShowLayerName				= serializedObject.FindProperty("m_ShowLayerName"); 		
			m_ShowDetailText			= serializedObject.FindProperty("m_ShowDetailText");
			m_ShowSelectedElementSprite = serializedObject.FindProperty("m_ShowSelectedElementSprite");
			m_OverwriteTouchOpertation	= serializedObject.FindProperty("m_OverwriteTouchOpertation");
			m_DragAndTouch				= serializedObject.FindProperty("m_DragAndTouch");
		}

		public override void OnInspectorGUI()
		{
			//Update the object
			serializedObject.Update();

			GUILayout.Label("Base Layer Information", EditorSettings.editorHeaderStyle);

			GUILayout.Label("Layer Name", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_LayerName, EditorSettings.GetAtrributes(m_LayerName), true);

			GUILayout.Label("Layer Input Event Name", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Must be unique for each layer in a menu", MessageType.Info);

			EditorGUILayout.PropertyField(m_LayerEvent, EditorSettings.GetAtrributes(m_LayerEvent), true);

			GUILayout.Label("Timers", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Overwrites the clean up and ready up timers from the base menu for this layer", MessageType.Info);

			EditorGUILayout.PropertyField(m_OverwriteTimers, EditorSettings.GetAtrributes(m_OverwriteTimers), true);

			//if overwrite timers is true then show the ready up and clean up overwrite timers
			if (m_OverwriteTimers.boolValue)
			{
				EditorGUILayout.PropertyField(m_ReadyUpTime, EditorSettings.GetAtrributes(m_ReadyUpTime), true);
				EditorGUILayout.PropertyField(m_CleanUpTime, EditorSettings.GetAtrributes(m_CleanUpTime), true);
			}

			GUILayout.Label("Detail Panel", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Overwrites the detail panel from the base menu for this layer", MessageType.Info);

			EditorGUILayout.PropertyField(m_OverwriteDetailPanel, EditorSettings.GetAtrributes(m_OverwriteDetailPanel), true);

			//if overwrite details panel is true then show the overwrite bools
			if (m_OverwriteDetailPanel.boolValue)
			{
				EditorGUILayout.PropertyField(m_ShowLayerName, EditorSettings.GetAtrributes(m_ShowLayerName), true);
				EditorGUILayout.PropertyField(m_ShowDetailText, EditorSettings.GetAtrributes(m_ShowDetailText), true);
				EditorGUILayout.PropertyField(m_ShowSelectedElementSprite, EditorSettings.GetAtrributes(m_ShowSelectedElementSprite), true);
			}
			GUILayout.Label("Touch Operations", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_OverwriteTouchOpertation, EditorSettings.GetAtrributes(m_OverwriteTouchOpertation), true);

			if (m_OverwriteTouchOpertation.boolValue)
			{
				GUILayout.Label("Touch Confirm Style", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox("True = The User has to drag the cursor to the selected element then tap the screen \nFalse = The user can just tap on the selected element to confirm", MessageType.Info);
				EditorGUILayout.PropertyField(m_DragAndTouch, EditorSettings.GetAtrributes(m_DragAndTouch), true);
			}

			//Apply all of the changes
			serializedObject.ApplyModifiedProperties();

			GUILayout.Space(20);
		}
	}
}