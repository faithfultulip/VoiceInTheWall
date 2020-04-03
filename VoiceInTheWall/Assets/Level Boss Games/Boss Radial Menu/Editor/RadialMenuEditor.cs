using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace LBG.UI.Radial
{
	[CustomEditor(typeof(RadialBase))]
	public class RadialMenuEditor : Editor
	{
		#region Target Variables

		/// <summary>
		/// Target object
		/// </summary>
		RadialBase m_Target;

		/// <summary>
		/// List of layer names 
		/// </summary>
		List<string> m_LayerNames;

		/// <summary>
		/// Index of the currently selected layer
		/// </summary>
		int m_SelectedLayer;

		#endregion

		#region Serialized Properties

		//The serlized object

		SerializedObject m_SerializedObject;

		//The serialized properties from the targets script

		SerializedProperty m_Input;
		SerializedProperty m_InputHorizontal;
		SerializedProperty m_InputVertical;
		SerializedProperty m_InputJoystickConfirm;
		SerializedProperty m_InputJoystickCancel;

		SerializedProperty m_AllInputEnabled;
		SerializedProperty m_TouchInputEnabled;
		SerializedProperty m_JoystickInputEnabled;
		SerializedProperty m_MouseInputEnabled;

		SerializedProperty m_ShowCancelButton;

		SerializedProperty m_DragAndTouch;
		SerializedProperty m_TouchTime;

		SerializedProperty m_DefaultReadyUpTime;
		SerializedProperty m_DefaultCleanUpTime;

		SerializedProperty m_ElementGrowthFactor;
		SerializedProperty m_ElementMaxSize;

		SerializedProperty m_ShowLayerName;
		SerializedProperty m_ShowDetailText;
		SerializedProperty m_ShowSelectedElement;

		SerializedProperty m_StartLayer;
		SerializedProperty m_Layers;
		SerializedProperty m_PanelOffset;
		SerializedProperty m_InputLeeway;
		SerializedProperty m_InputDeadzone;

		SerializedProperty m_InputManagerName;

		SerializedProperty m_ElementSnapping;
		#endregion


		void OnEnable()
		{
			//Set the target
			m_Target = target as RadialBase;

			//Set the serielized object
			m_SerializedObject = new SerializedObject(target);

			//Find all of the properties and assign the serialized properties

			m_Input					= m_SerializedObject.FindProperty("m_InputManager");
			m_InputHorizontal		= m_SerializedObject.FindProperty("m_InputHorizontal");
			m_InputVertical			= m_SerializedObject.FindProperty("m_InputVertical");
			m_InputJoystickConfirm	= m_SerializedObject.FindProperty("m_InputJoystickConfirm");
			m_InputJoystickCancel	= m_SerializedObject.FindProperty("m_InputJoystickCancel");

			m_AllInputEnabled		= m_SerializedObject.FindProperty("m_AllInputEnabled");
			m_TouchInputEnabled		= m_SerializedObject.FindProperty("m_TouchInputEnabled");
			m_JoystickInputEnabled	= m_SerializedObject.FindProperty("m_JoystickInputEnabled");
			m_MouseInputEnabled		= m_SerializedObject.FindProperty("m_MouseInputEnabled");

			m_ShowCancelButton		= m_SerializedObject.FindProperty("m_ShowCancelButton");

			m_DragAndTouch			= m_SerializedObject.FindProperty("m_DragAndTouch");
			m_TouchTime				= m_SerializedObject.FindProperty("m_TouchTime");

			m_DefaultReadyUpTime	= m_SerializedObject.FindProperty("m_DefaultReadyUpTime");
			m_DefaultCleanUpTime	= m_SerializedObject.FindProperty("m_DefaultCleanUpTime");

			m_ElementGrowthFactor	= m_SerializedObject.FindProperty("m_ElementGrowthFactor");
			m_ElementMaxSize		= m_SerializedObject.FindProperty("m_ElementMaxSize");

			m_ShowLayerName			= m_SerializedObject.FindProperty("m_ShowLayerName");
			m_ShowDetailText		= m_SerializedObject.FindProperty("m_ShowDetailText");
			m_ShowSelectedElement	= m_SerializedObject.FindProperty("m_ShowSelectedElementSprite");

			m_StartLayer			= m_SerializedObject.FindProperty("m_StartLayer");
			m_Layers				= m_SerializedObject.FindProperty("m_Layers");
			m_PanelOffset			= m_SerializedObject.FindProperty("m_PanelOffset");
			m_InputLeeway			= m_SerializedObject.FindProperty("m_InputLeeway");
			m_InputDeadzone			= m_SerializedObject.FindProperty("m_InputDeadzone");

			m_InputManagerName      = m_SerializedObject.FindProperty("m_InputManagerName");

			m_ElementSnapping		= m_SerializedObject.FindProperty("m_ElementSnapping");
			//Initalize the layer list
			m_LayerNames = new List<string>();
		}

		public override void OnInspectorGUI()
		{
			//Clear the layer names
			m_LayerNames.Clear();

			//How many layers are on the radial menu
			int layersCount = m_Target.m_Layers.Count;

			//Loop through all of the layers and assign their names to the names list. If a layer has no name then "No Layer" is put in to the names list
			for (int i = 0; i < layersCount; i++)
			{
				if (m_Target.m_Layers[i] != null)
				{
					m_LayerNames.Add(i + " - " + m_Target.m_Layers[i].m_LayerName);
				}
				else
				{
					m_LayerNames.Add(i + " - No Layer");
				}
			}

			//Update the object
			m_SerializedObject.Update();


			GUILayout.Label("Radial Base Information", EditorSettings.editorHeaderStyle);

			GUILayout.Label("Input Manager", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_Input, EditorSettings.GetAtrributes(m_Input), true);

            GUILayout.Label("Create Input Manager", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_InputManagerName, EditorSettings.GetAtrributes(m_InputManagerName), true);

            if(GUILayout.Button("Create Input Manager"))
            {
               CreateInputManager();
            }


			GUILayout.Label("Enabled Input Types", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_AllInputEnabled, EditorSettings.GetAtrributes(m_AllInputEnabled), true);

			//Show input bools in all inpute enabled is false
			if (!m_Target.m_AllInputEnabled)
			{
				EditorGUILayout.PropertyField(m_TouchInputEnabled, EditorSettings.GetAtrributes(m_TouchInputEnabled), true);
				EditorGUILayout.PropertyField(m_JoystickInputEnabled, EditorSettings.GetAtrributes(m_JoystickInputEnabled), true);
				EditorGUILayout.PropertyField(m_MouseInputEnabled, EditorSettings.GetAtrributes(m_MouseInputEnabled), true);
			}

			//If all input enabled or touch input enabled then show the bool for showing the cancel button
			if (m_Target.m_AllInputEnabled || m_Target.m_TouchInputEnabled)
			{
				GUILayout.Label("Cancel Button Used For Touch", EditorStyles.boldLabel);

				EditorGUILayout.PropertyField(m_ShowCancelButton, EditorSettings.GetAtrributes(m_ShowCancelButton), true);

				GUILayout.Label("Touch Confirm Style", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox("True = The User has to drag the cursor to the selected element then tap the screen \nFalse = The user can just tap on the selected element to confirm", MessageType.Info);
				EditorGUILayout.PropertyField(m_DragAndTouch, EditorSettings.GetAtrributes(m_DragAndTouch), true);

				GUILayout.Label("Confirm Touch Timer", EditorStyles.boldLabel);

				EditorGUILayout.PropertyField(m_TouchTime, EditorSettings.GetAtrributes(m_TouchTime), true);
			}

			//If all input enabled or joystick input enabled is true then show the property fields for the axis names
			if (m_Target.m_JoystickInputEnabled || m_Target.m_AllInputEnabled)
			{
				GUILayout.Label("Joystick Input Names", EditorStyles.boldLabel);

				EditorGUILayout.PropertyField(m_InputHorizontal, EditorSettings.GetAtrributes(m_InputHorizontal), true);
				EditorGUILayout.PropertyField(m_InputVertical, EditorSettings.GetAtrributes(m_InputVertical), true);
				EditorGUILayout.PropertyField(m_InputJoystickConfirm, EditorSettings.GetAtrributes(m_InputJoystickConfirm), true);
				EditorGUILayout.PropertyField(m_InputJoystickCancel, EditorSettings.GetAtrributes(m_InputJoystickCancel), true);

				EditorGUILayout.PropertyField(m_InputDeadzone, EditorSettings.GetAtrributes(m_InputDeadzone), true);
			}

			GUILayout.Label("Layers", EditorStyles.boldLabel);

			m_SelectedLayer = m_StartLayer.intValue;
			m_StartLayer.intValue = EditorGUILayout.Popup("Start Layer", m_SelectedLayer, m_LayerNames.ToArray());

			EditorGUILayout.PropertyField(m_Layers, EditorSettings.GetAtrributes(m_Layers), true);

			GUILayout.Label("Ready Up and Clean Up Times for the Layers", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_DefaultReadyUpTime, EditorSettings.GetAtrributes(m_DefaultReadyUpTime), true);
			EditorGUILayout.PropertyField(m_DefaultCleanUpTime, EditorSettings.GetAtrributes(m_DefaultCleanUpTime), true);

			
			GUILayout.Label("Element Size", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(m_ElementMaxSize, EditorSettings.GetAtrributes(m_ElementMaxSize), true);


			GUILayout.Label("Element Growth", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_ElementGrowthFactor, EditorSettings.GetAtrributes(m_ElementGrowthFactor), true);


			GUILayout.Label("Centre Detail Panel", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_ShowLayerName, EditorSettings.GetAtrributes(m_ShowLayerName), true);
			EditorGUILayout.PropertyField(m_ShowSelectedElement, EditorSettings.GetAtrributes(m_ShowSelectedElement), true);
			EditorGUILayout.PropertyField(m_ShowDetailText, EditorSettings.GetAtrributes(m_ShowDetailText), true);


			GUILayout.Label("Attributes", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_PanelOffset, EditorSettings.GetAtrributes(m_PanelOffset), true);
			EditorGUILayout.PropertyField(m_InputLeeway, EditorSettings.GetAtrributes(m_InputLeeway), true);

			GUILayout.Label("Snapping", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_ElementSnapping, EditorSettings.GetAtrributes(m_ElementSnapping), true);

			//Apply all of the changes
			m_SerializedObject.ApplyModifiedProperties();
		}

        void CreateInputManager()
        {

            string filePath = Application.dataPath;
            string fileName = m_InputManagerName.stringValue.RemoveSpaces();
            string completePath = filePath + "/" + fileName + ".cs";

            if (fileName == "")
            {
                Debug.LogError("Input Manager name is empty");
                return;
            }

            string[] assetsWithSameName = AssetDatabase.FindAssets(fileName);
            if (assetsWithSameName.Length > 0)
            {
                string foundAssetPath = (AssetDatabase.GUIDToAssetPath(assetsWithSameName[0]));
                foundAssetPath = foundAssetPath.Replace(fileName + ".cs", "");

                Debug.LogError("A Radial Menu Input Manager called " + fileName + " already exists in " + foundAssetPath);
                return;
            }

            FileStream file = File.Create(completePath);
            file.Close();
            TextAsset template = Resources.Load("InputManagerTemplate") as TextAsset;
            string templateData = template.text.Replace("#SCRIPTNAME#", fileName);
            File.WriteAllText(completePath, templateData);

            AssetDatabase.Refresh();
        }
    }
}