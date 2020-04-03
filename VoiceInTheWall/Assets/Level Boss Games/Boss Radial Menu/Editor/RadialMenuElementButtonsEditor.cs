using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace LBG.UI.Radial
{
	[CustomEditor(typeof(RadialLayerButtons), true)]
	[CanEditMultipleObjects]
	public class RadialLayerButtonsEditor : RadialMenuElementLayerEditor
	{
		#region Target Variables

		RadialLayerButtons m_Target;
		/// <summary>
		/// Reorderable list of elements
		/// </summary>
		ReorderableList m_RElementsList;

		#endregion

		#region Serialized Properties

		//The serialized properties from the targets script

		SerializedProperty m_Elements;

		#endregion

		protected override void OnEnable()
		{
			base.OnEnable();

			m_Target = target as RadialLayerButtons;
			//Gets the m_MenuElements field
			m_Elements = serializedObject.FindProperty("m_MenuElements");

			//Creates the reorderable list
			m_RElementsList = new ReorderableList(serializedObject, m_Elements, true, true, true, true);

			//Gives the list a header
			m_RElementsList.drawHeaderCallback = (Rect rect) =>
			{
				//Header string
				EditorGUI.LabelField(rect, "Elements");
			};

			//Adds a menu when the + button is pressed under the list
			m_RElementsList.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
			{
				//Creates a menu
				GenericMenu menu = new GenericMenu();

				//The folder the current selected asset is in
				string assetFolderPath = AssetDatabase.GetAssetPath(target).TrimEnd((target.name + ".asset").ToCharArray());
				assetFolderPath = assetFolderPath.TrimEnd('/');
				//Gets all assets in the correct folder
				string[] assetPath = AssetDatabase.FindAssets("", new[] { assetFolderPath });

				//Create a blank item on the list
				menu.AddItem(new GUIContent("New Item"), false, ClickHandler, "");

				//for each index of elements with no name
				int NoNameIndex = 0;
				//loop through each asset path and create a menu listing for it if it's an ElementObject and is not a Header
				foreach (string s in assetPath)
				{
					//The path to the object
					string path = AssetDatabase.GUIDToAssetPath(s);
					
					//Make sure that only files from the correct folder are chosen
					string testPath = path.Remove(0, assetFolderPath.Length + 1);
					
					if (!testPath.Contains("/"))
					{
						//Gets the target object from the path
						ScriptableObject tempSO = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as ScriptableObject;

						Debug.Log("a");

						//Ensures that only the correct objects appear in the menu
						if (tempSO as RadialMenuObject != null && tempSO.GetType() != typeof(RadialMenuHeader))
						{
							//Gets the folder name
							string folderName = tempSO.GetType().ToString().RemoveNameSpaces();
							folderName = folderName.SpaceBeforeCapitals();



							//Gets the asset name
							string assetName = (tempSO as RadialMenuObject).GetName();

							//Make sure it has a name
							if (assetName == string.Empty)
							{
								assetName = "Asset Has No Name " + NoNameIndex;
								NoNameIndex++;
							}

							//Create a menu for this item
							menu.AddItem(new GUIContent(folderName + "/" + assetName), false, ClickHandler, path);
						}
					}
				}

				menu.ShowAsContext();
			};
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Label("Button Layer Information", EditorSettings.editorHeaderStyle);

			if (targets.Length == 1)
			{

				//Draw all elements in the list on screen
				m_RElementsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
				//Gets the correct element in the list
				SerializedProperty element = m_RElementsList.serializedProperty.GetArrayElementAtIndex(index);

				GUIContent guiContent;

				//If the element isn't empty
				if ((m_Target as RadialLayerButtons).m_MenuElements[index] != null)
					{
					//if the name is not nothing then get the name from the element else set it to no name
					if ((m_Target as RadialLayerButtons).m_MenuElements[index].GetName() != "")
						{
							guiContent = new GUIContent((m_Target as RadialLayerButtons).m_MenuElements[index].GetName());
						}
						else
						{
							guiContent = new GUIContent("No Name");
						}
					}
				//If the element is empty set the gui content to No Element
				else
					{
						guiContent = new GUIContent("No Element");
					}

					rect.y += 2;

				//Draw the element on screen
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, guiContent);
				};

				GUILayout.Label("Element list", EditorStyles.boldLabel);

				//Update the object
				serializedObject.Update();

				//Compile the list
				m_RElementsList.DoLayoutList();

				//Apply all of the changes
				serializedObject.ApplyModifiedProperties();
			}
			else
			{
				EditorGUILayout.HelpBox("Can not edit multiple menu elements", MessageType.Warning);
			}
			GUILayout.Space(20);
		}

		/// <summary>
		/// Handles the click events for the menu items
		/// </summary>
		/// <param name="target">The target previded from the menu</param>
		private void ClickHandler(object target)
		{
			//Set the target to a string
			string data = (string)target;

			//Get the last index in the array
			int index = m_RElementsList.serializedProperty.arraySize;

			//Add one to the index
			m_RElementsList.serializedProperty.arraySize++;
			//Set the reorderable list index
			m_RElementsList.index = index;

			//If the path is == to nothing then create a copy of the last element else load one from the path
			if (data == "")
			{
				(m_Target as RadialLayerButtons).m_MenuElements.Add(null);
			}
			else
			{
				(m_Target as RadialLayerButtons).m_MenuElements.Add(AssetDatabase.LoadAssetAtPath(data, typeof(RadialMenuObject)) as RadialMenuObject);
			}
		}
	}
}