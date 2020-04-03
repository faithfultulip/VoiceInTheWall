using UnityEngine;
using UnityEditor;

namespace LBG.UI.Radial
{
	public class EditorSettings : MonoBehaviour
	{
		private static GUIStyle m_EditorHeader;

		public static GUIStyle editorHeaderStyle
		{
			get
			{
				if (m_EditorHeader == null)
				{
					m_EditorHeader = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
					m_EditorHeader.fontSize = 20;
				}
				return m_EditorHeader;
			}
			set
			{
				m_EditorHeader = value;
			}
		}

		public static GUIContent GetAtrributes(SerializedProperty property)
		{
			return new GUIContent(property.name.VarNameToScreenName(), property.tooltip);
		}
	}
}
