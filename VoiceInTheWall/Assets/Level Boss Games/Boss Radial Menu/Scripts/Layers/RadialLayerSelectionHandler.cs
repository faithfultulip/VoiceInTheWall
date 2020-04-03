using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LBG.UI.Radial
{
	public class RadialLayerSelectionHandler : RadialLayerElementsHandler
	{
        #region private variables

        /// <summary>
        /// Reference to the current ScriptableObject that is used to get data
        /// </summary>
        private RadialLayerSelection							m_RefLayer;

		/// <summary>
		/// The values that are cycled through by the layer
		/// </summary>
		string[]												m_Values;

		/// <summary>
		/// Index that is currently selected
		/// </summary>
		int														m_CurrentIndex = 0;

		/// <summary>
		/// The original index that was set when the layer was constructed
		/// </summary>
		int														m_OriginalIndex = 0;

		/// <summary>
		/// List of all of the elements used for snapping
		/// </summary>
		List<int>												m_SnappingChecks;

		#endregion

		/// <summary>
		/// Override initialisation for the selection layer
		/// </summary>
		/// <param name="menu">the reference to the RadialBase script that is associated with this layer</param>
		/// <param name="layer">the scriptable object that is providing this layer with data</param>
		/// <param name="currentIndex">Index that will be displayed first from the selection values when the layer constructs</param>
		/// <param name="selectionValues">Array of the values that will be displayed on screen</param>
		public override void Init(RadialBase menu, RadialLayer layer, int currentIndex, string[] selectionValues)
		{
			m_RefLayer = layer as RadialLayerSelection;

			m_SnappingChecks = new List<int>();

			//Call the base init
			base.Init(menu, layer);

			//Set the values for the layer
			m_Values = selectionValues;
			
			//Set the current index
			m_CurrentIndex = currentIndex;

			//Set the original Index
			m_OriginalIndex = currentIndex;

			//Update the displayed text so that it shows the correct value
			UpdateDisplayedText(m_Values[currentIndex]);
		}

		/// <summary>
		/// Used for generating any gameObjects needed for the layer
		/// </summary>
		protected override void Populate()
		{
			//Call the base populate
			base.Populate();
		}

		/// <summary>
		/// Add elements to the m_Objects list in the base class - called in base.Init()
		/// </summary>
		/// <returns></returns>
		protected override List<RadialMenuObject> AddElements()
		{
			//Create a new list 
			List<RadialMenuObject> tempElementList = new List<RadialMenuObject>();

			//Create an empty instance of a radial button
			RadialMenuObject fillerOBJ = ScriptableObject.CreateInstance<RadialMenuButton>() as RadialMenuObject;

			//name the newly created radial button
			fillerOBJ.name = "fillerOBJ";

			//make the newly created radial button invisible 
			fillerOBJ.m_SpriteColour = Color.clear;

			//If there is no header add an empty button to the list
			if(m_RefLayer.m_MenuHeader == null)
			{
				tempElementList.Add(fillerOBJ);
			}

			//add multiple empty buttons to the list
			tempElementList.Add(fillerOBJ);
			tempElementList.Add(fillerOBJ);

            if (m_RefLayer.m_Right != null)
            {
                tempElementList.Add(m_RefLayer.m_Right);
                m_ContaintsElements = true;
				m_SnappingChecks.Add(3);
            }
            else
            {
                m_ContaintsElements = false;
                Debug.LogError("There is no right selection arrow");
            }

			tempElementList.Add(fillerOBJ);

			//if there is no bottom right button then add an empty button
			if (m_RefLayer.m_BottomRight != null)
			{
				tempElementList.Add(m_RefLayer.m_BottomRight);
				m_SnappingChecks.Add(5);
			}
			else
				tempElementList.Add(fillerOBJ);

			//if there is no bottom button then add an empty button
			if (m_RefLayer.m_BottomMiddle != null)
			{
				tempElementList.Add(m_RefLayer.m_BottomMiddle);
				m_SnappingChecks.Add(6);
			}
			else
				tempElementList.Add(fillerOBJ);

			//if there is no bottom left button then add an empty button
			if (m_RefLayer.m_BottomLeft != null)
			{
				tempElementList.Add(m_RefLayer.m_BottomLeft);
				m_SnappingChecks.Add(7);
			}
			else
				tempElementList.Add(fillerOBJ);

			//add multiple empty buttons to the list
			tempElementList.Add(fillerOBJ);
            if (m_RefLayer.m_Left != null)
            {
                m_ContaintsElements = true;
                tempElementList.Add(m_RefLayer.m_Left);
				m_SnappingChecks.Add(9);
			}
			else
            {
                m_ContaintsElements = false;
                Debug.LogError("There is no left selection arrow");
            }
            tempElementList.Add(fillerOBJ);
			tempElementList.Add(fillerOBJ);

			return tempElementList;
		}

		/// <summary>
		/// The Main Update call for the layer
		/// </summary>
		/// <param name="targetAngle">angle provided by the input method</param>
		/// <param name="method">type of input</param>
		public override void UpdateLayer(float? targetAngle, ControlMethod method)
		{
            if (!m_ContaintsElements)
                return;

            //if we have recieved an input angle
            if (targetAngle != null)
			{
				//currently hovered over element
				int tempSelected = HandleCursor(targetAngle.GetValueOrDefault(0.0f));

				//The hovered over the element is different from the one that was selected the previous frame
				if (tempSelected != m_SelectedElement)
				{
					//Set the selected element
					m_SelectedElement = tempSelected;

					//Get the name of the element
					string tempName = m_MenuObjects[m_SelectedElement].GetName();

					//if the name is nothing don't display it else 
					if (tempName == "" || tempName == null)
					{
						//Don't show the sprite that is on the temp elements but show the current string value from the m_Values array
						UpdateDisplayedText(m_Values[m_CurrentIndex], m_MenuObjects[0].m_ElementSprite, m_MenuObjects[0].m_SpriteColour);
					}
					else
					{
						//Show the sprite of the button, the name of the button and the current value string value from the m_Values array
						UpdateDisplayedText(MultiLineText(m_Values[m_CurrentIndex], m_MenuObjects[m_SelectedElement].GetName()), (m_SelectedElement == 0 && m_RefLayer.m_MenuHeader != null) ? null : m_MenuObjects[m_SelectedElement].m_ElementSprite, m_MenuObjects[m_SelectedElement].m_SpriteColour);
					}
				}

				//Increase the size of the highlighted element
				SetSelectedElementSize(targetAngle.GetValueOrDefault(0.0f));
			}
		}

		/// <summary>
		/// The Update function for transition to out of this layer
		/// </summary>
		public override bool CleanUp()
		{
			return base.CleanUp();
		}

		/// <summary>
		/// The Update function for transitioning into this layer
		/// </summary>
		public override bool ReadyUp()
		{
			return base.ReadyUp();
		}

		/// <summary>
		/// Event call to process the currently selected element
		/// </summary>
		public override void Confirm()
		{
			//Right
			if (m_SelectedElement == 3)
			{
				//cycle the values up
				CycleValue(true);
                if(m_RefLayer.m_UpdateOnSwitch)
                {
                    m_RadialBase.m_InputManager.ProcessSelection(m_RefLayer.m_LayerEvent, "Update", m_CurrentIndex, m_OriginalIndex);
                }
			}
			//Left
			else if (m_SelectedElement == 9)
			{
				//cycle the values down
				CycleValue(false);
                if (m_RefLayer.m_UpdateOnSwitch)
                {
                    m_RadialBase.m_InputManager.ProcessSelection(m_RefLayer.m_LayerEvent, "Update", m_CurrentIndex, m_OriginalIndex);
                }
            }
			//Bottom right button
			else if(m_SelectedElement == 5 && m_RefLayer.m_BottomRight != null)
			{
				//Process this button through the input manager
				m_RadialBase.m_InputManager.ProcessSelection(m_RefLayer.m_LayerEvent, m_RefLayer.m_BottomRight.m_ElementEvent, m_CurrentIndex, m_OriginalIndex);
			}
			//Bottom middle button
			else if(m_SelectedElement == 6 && m_RefLayer.m_BottomMiddle != null)
			{
				//Process this button through the input manager
				m_RadialBase.m_InputManager.ProcessSelection(m_RefLayer.m_LayerEvent, m_RefLayer.m_BottomMiddle.m_ElementEvent, m_CurrentIndex, m_OriginalIndex);
			}
			//Bottom left button
			else if(m_SelectedElement == 7 && m_RefLayer.m_BottomLeft != null)
			{
				//Process this button through the input manager
				m_RadialBase.m_InputManager.ProcessSelection(m_RefLayer.m_LayerEvent, m_RefLayer.m_BottomLeft.m_ElementEvent, m_CurrentIndex, m_OriginalIndex);
			}
		}

		/// <summary>
		/// Event call to cancel the layer and transition back up the breadcrumb. Moves the cursor to the header if one exists else it goes to the closest element
		/// </summary>
		public override void Cancel()
		{
			//If there is no header
			if (m_RefLayer.m_MenuHeader == null)
			{
				//Create a dictionary to hold the element index and it's angle
				Dictionary<int, float> angleDictionary = new Dictionary<int, float>();
				//The nearest element to the cursor
				int nearestElement = 0;
				//smallest angle that has been found
				float smallestAngle = 999;

				//Check to see what elements exist and add them to the dictionary via there index and angle
				angleDictionary.Add(3, m_ElementAngleDeg * 3);

				if (m_RefLayer.m_BottomRight != null)
				{
					angleDictionary.Add(5, m_ElementAngleDeg * 5);
				}

				if (m_RefLayer.m_BottomMiddle != null)
				{
					angleDictionary.Add(6, m_ElementAngleDeg * 6);
				}

				if (m_RefLayer.m_BottomLeft != null)
				{
					angleDictionary.Add(7, m_ElementAngleDeg * 7);
				}

				angleDictionary.Add(9, m_ElementAngleDeg * 9);

				//Get the dictionary size
				int dictionaryCount = angleDictionary.Count;
				//Loop through the dictionary to find the closest element
				for (int i = 0; i < dictionaryCount; i++)
				{
					//The current angle to this element in the loop
					float currentAngle = Mathf.Abs(((m_Cursor.gameObject.transform.localRotation.eulerAngles.z - (m_CursorImage.fillAmount * 180.0f)) - (360.0f - angleDictionary[angleDictionary.Keys.ElementAt(i)])));

					//Make the value an absolute value of -180 - 180
					if (currentAngle > 180.0f)
						currentAngle = Mathf.Abs(currentAngle - 360.0f);

					//If this angle is the smallest one set set the smallestAngle variable and the nearest elements as the index of this element
					if (smallestAngle > currentAngle)
					{
						smallestAngle = currentAngle;
						nearestElement = angleDictionary.Keys.ElementAt(i);
					}
				}

				//Set the currently selected element to be the nearest one
				m_SelectedElement = nearestElement;
			}
			else
			{
				//If a header exisits just set the selected element to the header
				m_SelectedElement = 0;
			}

            //Set the value back to the original one
            if (m_RefLayer.m_UpdateOnSwitch)
            {
                m_RadialBase.m_InputManager.ProcessSelection(m_RefLayer.m_LayerEvent, "Update", m_OriginalIndex, m_OriginalIndex);
            }
            //transition back up the breadcrumb
            m_RadialBase.TransitionToPreviousLayer();
		}

		/// <summary>
		/// Cycle throuh m_Values 
		/// </summary>
		/// <param name="up">If true, m_Values is cycled up else it cycles down</param>
		void CycleValue(bool up)
		{
			if(up)
			{
				//Plus one to the current index
				m_CurrentIndex++;

				//If the current index is greater than the amount of values in m_Values then we set current index to 0
				if(m_CurrentIndex > m_Values.Length - 1)
				{
					m_CurrentIndex = 0;
				}
			}
			else
			{
				//Minues one from the current index
				m_CurrentIndex--;

				//If the current index is less than 0 then we set current index to the amount of values in m_Values
				if(m_CurrentIndex < 0)
				{
					m_CurrentIndex = m_Values.Length - 1;
				}
			}

			//Update the desplayed text based on the current index
			UpdateDisplayedText(m_Values[m_CurrentIndex]);
		}

		/// <summary>
		/// Moves the cursor and returns the selected element
		/// </summary>
		/// <param name="targetAngle">The angle of the cursor</param>
		protected override int HandleCursor(float targetAngle)
		{
			//3 9 5 6 7

			//work out what the selected element is based on the target angle
			int selectedElement = (int)Mathf.Round((targetAngle) / m_ElementAngleDeg);

			//make sure the selected element is a zero based index reference
			if (selectedElement >= m_ElementCount)
				selectedElement = 0;
			

			int tempNum = int.MaxValue;
			int checkID = int.MaxValue;

			for (int i = 0; i < m_SnappingChecks.Count; i++)
			{
				int difference = Mathf.Abs(m_SnappingChecks[i] - selectedElement);

				if (tempNum > difference)
				{
					tempNum = difference;
					checkID = m_SnappingChecks[i];
				}
			}
			if (m_RefLayer.m_ElementSnapOverwrite)
			{
				if (m_RefLayer.m_ElementSnap)
				{
					//Set the cursor to the target rotation 
					m_CursorRect.localRotation = Quaternion.Euler(0.0f, 0.0f, 360.0f - ((checkID * m_ElementAngleDeg) - m_CursorOffset));
				}
				else
				{
					//Set the cursor to the target rotation 
					m_CursorRect.localRotation = Quaternion.Euler(0.0f, 0.0f, 360.0f - (targetAngle - m_CursorOffset));
				}
			}
			else if(m_RadialBase.m_ElementSnapping)
			{
				//Set the cursor to the target rotation 
				m_CursorRect.localRotation = Quaternion.Euler(0.0f, 0.0f, 360.0f - ((checkID * m_ElementAngleDeg) - m_CursorOffset));
			}
			else
			{
				//Set the cursor to the target rotation 
				m_CursorRect.localRotation = Quaternion.Euler(0.0f, 0.0f, 360.0f - (targetAngle - m_CursorOffset));
			}

			//return the selected element
			return selectedElement;
		}
	}
}