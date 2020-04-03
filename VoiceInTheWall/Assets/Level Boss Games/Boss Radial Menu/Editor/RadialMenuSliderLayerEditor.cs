using UnityEditor;
using UnityEngine;

namespace LBG.UI.Radial
{
	[CustomEditor(typeof(RadialLayerSlider), true)]
	[CanEditMultipleObjects]
	public class RadialMenuSliderLayerEditor : RadialMenuLayersEditor 
	{
		#region Serialized Properties

		protected SerializedProperty m_CursorSprite;
		protected SerializedProperty m_CursorSize;
		protected SerializedProperty m_FillImage;
		protected SerializedProperty m_FillColour;
		protected SerializedProperty m_BackgroundImage;
		protected SerializedProperty m_BackgroundColour;
		protected SerializedProperty m_LayerMin;
		protected SerializedProperty m_LayerMax;
		protected SerializedProperty m_DisplayedTextDecimalPoint;

		#endregion

		protected override void OnEnable()
		{
			base.OnEnable();

			m_CursorSprite				= serializedObject.FindProperty("m_CursorSprite"); 
			m_CursorSize				= serializedObject.FindProperty("m_CursorSize");
			m_FillImage					= serializedObject.FindProperty("m_FillImage");
			m_FillColour				= serializedObject.FindProperty("m_FillColour");
			m_BackgroundImage			= serializedObject.FindProperty("m_BackgroundImage");
			m_BackgroundColour			= serializedObject.FindProperty("m_BackgroundColour");
			m_LayerMin					= serializedObject.FindProperty("m_LayerMin");
			m_LayerMax					= serializedObject.FindProperty("m_LayerMax");
			m_DisplayedTextDecimalPoint = serializedObject.FindProperty("m_DisplayedTextDecimalPoint");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			GUILayout.Label("Slider Information", EditorSettings.editorHeaderStyle);

			GUILayout.Label("Min and Max Values", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_LayerMin, EditorSettings.GetAtrributes(m_LayerMin), true);
			EditorGUILayout.PropertyField(m_LayerMax, EditorSettings.GetAtrributes(m_LayerMax), true);

			GUILayout.Label("Cursor Details", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_CursorSize, EditorSettings.GetAtrributes(m_CursorSize), true);
			EditorGUILayout.PropertyField(m_CursorSprite, EditorSettings.GetAtrributes(m_CursorSprite), true);

			GUILayout.Label("Radial Image Values", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_FillImage, EditorSettings.GetAtrributes(m_FillImage), true);
			EditorGUILayout.PropertyField(m_FillColour, EditorSettings.GetAtrributes(m_FillColour), true);
			EditorGUILayout.PropertyField(m_BackgroundImage, EditorSettings.GetAtrributes(m_BackgroundImage), true);
			EditorGUILayout.PropertyField(m_BackgroundColour, EditorSettings.GetAtrributes(m_BackgroundColour), true);

			GUILayout.Label("Number of Decimal Places Displayed", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(m_DisplayedTextDecimalPoint, EditorSettings.GetAtrributes(m_DisplayedTextDecimalPoint), true);

			serializedObject.ApplyModifiedProperties();
		}
	}
}