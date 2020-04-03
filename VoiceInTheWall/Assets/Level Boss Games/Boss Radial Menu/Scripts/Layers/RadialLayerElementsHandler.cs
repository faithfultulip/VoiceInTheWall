using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	public abstract class RadialLayerElementsHandler : RadialLayerHandler
	{
		#region private variables

		/// <summary>
        /// Reference to the current ScriptableObject that is used to get data
        /// </summary>
		private RadialLayerElements									m_RefLayer;

		/// <summary>
		/// Used in the ready up and clean up to keep track of how many elemntes have been spawned / destroyed
		/// </summary>
		private int													m_CurrentElementIndex;

		/// <summary>
		/// Used to store and transfer the scale of the icon under the cursor when the cursor changes sibling index
		/// </summary>
		private float?												m_SavedIconScale;

		#endregion

		#region protected variables

		/// <summary>
		/// List of all the elements in the layer
		/// </summary>
		protected List<RadialMenuElement>							m_ElementsScript = new List<RadialMenuElement>();

		/// <summary>
		/// List of all the objects to be turned into elements on the layer in clockwise order
		/// </summary>
		protected List<RadialMenuObject>							m_MenuObjects = new List<RadialMenuObject>();

		/// <summary>
		/// Anglebetween each element in degrees
		/// </summary>
		protected float												m_ElementAngleDeg = 0;

		/// <summary>
		/// Angle between each element in degrees
		/// </summary>
		protected float												m_ElementAngleRad;

		/// <summary>
		/// The length of the arc that each element has
		/// </summary>
		protected float												m_ElementArcSize;

		/// <summary>
		/// Index of the currently selected element
		/// </summary>
		protected int												m_SelectedElement = -1;

		/// <summary>
		/// Amount of elements in the layer
		/// </summary>
		protected int												m_ElementCount = 0;

		/// <summary>
        /// Triggers to true if the layer contains elements
        /// </summary>
        protected bool                                              m_ContaintsElements = false;

				/// <summary>
		/// The angle in degrees that the cursor has to be rotated so that the middle falls where the input is aiming
		/// </summary>
		protected float												m_CursorOffset;

		#endregion

		/// <summary>
		/// Main initialisation function
		/// </summary>
		/// <param name="menu">the reference to the RadialBase script that is associated with this layer</param>
		/// <param name="layer">the scriptable object that is providing this layer with data</param>
		public override void Init(RadialBase menu, RadialLayer layer, List<RadialMenuObject> customElements = null)
		{
			m_RefLayer = layer as RadialLayerElements;

			if(m_RefLayer != null)
			{
				//clear all the lists
				m_ElementsScript.Clear();
				m_MenuObjects.Clear();

				m_ElementAngleDeg = 0.0f;

				//if a header has been specified
				if(m_RefLayer.m_MenuHeader != null)
				{
					//add the header to the list of objects to be created
					m_MenuObjects.Add(m_RefLayer.m_MenuHeader);
				}
				//Add the elements to the list of objects to be created
				m_MenuObjects.AddRange(AddElements().ToArray());

				//run the parent initialsation
				base.Init(menu, layer);

				//Update the text in the details panel
				if(m_MenuObjects.Count > 0)
				{
					UpdateDisplayedText(m_MenuObjects[0].GetName(), (m_RefLayer.m_MenuHeader != null) ? null : m_MenuObjects[0].m_ElementSprite, (m_RefLayer.m_MenuHeader != null) ? Color.clear : m_MenuObjects[0].m_SpriteColour);
				}

				//get a reference to the cursor transform
				m_CursorRect = m_Cursor.transform as RectTransform;
				//get a reference to the Icon layer transform on the cursor
				m_CursorIconRect = m_CursorRect.Find("Icon").GetComponent<RectTransform>();
				//get a reference to the Image transform on the icon layer of the cursor
				m_CursorIconImageRect = m_CursorIconRect.Find("Image").GetComponent<RectTransform>();
				//get a reference to the Image component of the image on the cursor
				m_CursorIconImage = m_CursorIconImageRect.GetComponent<Image>();
				//set the offset variable to the correct amout of degrees
				m_CursorOffset = m_ElementAngleDeg / 2.0f;
				//initalise the index variables
				m_CurrentElementIndex = 0;
				m_SelectedElement = 0;
			}
		}

		/// <summary>
		/// Add elements to the m_Objects list in the base class
		/// </summary>
		protected abstract List<RadialMenuObject> AddElements();

		/// <summary>
		/// Create and return a menu element as a GameObject with the specified sprite and colour
		/// </summary>
		/// <param name="sprite">Image to be displayed as the element</param>
		/// <param name="spriteColour">Tint to be applied to the sprite</param>
		protected GameObject CreateElementObject(Sprite sprite, Color spriteColour)
		{
			//Create a new GameObject
			GameObject go = new GameObject("Radial Elemenet", typeof(RectTransform));
			//Set the parent as the parent reference from the RadialBase
			go.transform.SetParent(m_RadialBase.ElementsParent, false);
			//Add and store the element script to the gameObject
			RadialMenuElement tempElement = go.AddComponent<RadialMenuElement>();
			//Initialise the script
			tempElement.Init(sprite, spriteColour);
			//Add the reference to the list of element scripts
			m_ElementsScript.Add(tempElement);
			//return the GameObject
			return go;
		}
		
		/// <summary>
		/// Used for generating any gameObjects needed for the layer
		/// </summary>
		protected override void Populate()
		{
            if (m_MenuObjects.Count <= 0)
            {
                Debug.LogError("There are no elements in the layer " + m_RefLayer.m_LayerName);
                return;
            }

            //initialise the needed variable
            m_ElementCount = m_MenuObjects.Count;
			m_ElementAngleDeg = 360.0f / m_ElementCount;
			m_ElementAngleRad = m_ElementAngleDeg * Mathf.Deg2Rad;
			m_ElementArcSize = (2.0f * Mathf.PI * m_RadialBase.m_ScreenSize) * (m_ElementAngleDeg / 360.0f);

			//for each object in the m_MenuObjects list
			for (int i = 0; i < m_ElementCount; i++)
			{
				//Create the Element
				Sprite tempSprite = null;
				Color tempSpriteColour = Color.white;

				if(m_MenuObjects[i].GetType() == typeof(RadialMenuToggleButton))
				{
					RadialMenuToggleButton tempToggle = m_MenuObjects[i] as RadialMenuToggleButton;

					if (m_RadialBase.m_InputManager.ProcessReturnValues<bool>(m_RefLayer.m_LayerEvent, tempToggle.m_ElementEvent))
					{
						tempSprite = tempToggle.m_ElementSprite;
						tempSpriteColour = tempToggle.m_SpriteColour;
					}
					else
					{
						tempSprite = tempToggle.m_FalseElementSprite;
						tempSpriteColour = tempToggle.m_FalseSpriteColour;
					}
				}
				else
				{
					tempSprite = m_MenuObjects[i].m_ElementSprite;
					tempSpriteColour = m_MenuObjects[i].m_SpriteColour;
				}

				GameObject go = CreateElementObject(tempSprite, tempSpriteColour);
				//turn it off so that the ready up function can just turn it on during the animation
				go.SetActive(false);

				//work out it's position based on it's index number
				Vector2 tempPosition = new Vector2();
				float tempAngle = i * m_ElementAngleRad;
				tempPosition.x = m_RadialBase.m_ScreenSize * Mathf.Sin(tempAngle);
				tempPosition.y = m_RadialBase.m_ScreenSize * Mathf.Cos(tempAngle);
				//move it into position
				RectTransform tempRect = go.GetComponent<RectTransform>();
				tempRect.Resize(m_RadialBase.m_ElementMaxSize, m_ElementArcSize);
				tempRect.localPosition = tempPosition;
                //resize the element to fit the screen it is on
                //Move it's achor points to it's corners
                tempRect.AnchorsToCorners();
			}
		}

        public virtual void Reload(float? targetAngle, ControlMethod method, List<RadialMenuObject> customElements = null)
        {
        }

		/// <summary>
		/// In the event that the resolution of the screen changes, the elements need to be redrawn in new positions
		/// </summary>
		/// <param name="targetAngle">the current angle of the cursor</param>
		/// <param name="method">the current control method being used</param>
		public override void Redraw(float? targetAngle, ControlMethod method)
		{
            //initialise the needed variable
            m_ElementsScript.Clear();

			m_ElementCount = m_MenuObjects.Count;
			m_ElementAngleDeg = 360.0f / m_ElementCount;
			m_ElementAngleRad = m_ElementAngleDeg * Mathf.Deg2Rad;
			m_ElementArcSize = (2.0f * Mathf.PI * m_RadialBase.m_ScreenSize) * (m_ElementAngleDeg / 360.0f);

			//for each object in the m_MenuObjects list
			for (int i = 0; i < m_ElementCount; i++)
			{
				//Create the Element
				Sprite tempSprite = null;
				Color tempSpriteColour = Color.white;

				if (m_MenuObjects[i].GetType() == typeof(RadialMenuToggleButton))
				{
					RadialMenuToggleButton tempToggle = m_MenuObjects[i] as RadialMenuToggleButton;

					if (m_RadialBase.m_InputManager.ProcessReturnValues<bool>(m_RefLayer.m_LayerEvent, tempToggle.m_ElementEvent))
					{
						tempSprite = tempToggle.m_ElementSprite;
						tempSpriteColour = tempToggle.m_SpriteColour;
					}
					else
					{
						tempSprite = tempToggle.m_FalseElementSprite;
						tempSpriteColour = tempToggle.m_FalseSpriteColour;
					}
				}
				else
				{
					tempSprite = m_MenuObjects[i].m_ElementSprite;
					tempSpriteColour = m_MenuObjects[i].m_SpriteColour;
				}

				GameObject go = CreateElementObject(tempSprite, tempSpriteColour);

				//ensure that it's on as we don't need to do an animation
				go.SetActive(true);

				//work out it's position based on it's index number
				Vector2 tempPosition = new Vector2();
				float tempAngle = i * m_ElementAngleRad;
				tempPosition.x = m_RadialBase.m_ScreenSize * Mathf.Sin(tempAngle);
				tempPosition.y = m_RadialBase.m_ScreenSize * Mathf.Cos(tempAngle);
				//move it into position
				RectTransform tempRect = go.GetComponent<RectTransform>();
				tempRect.localPosition = tempPosition;
				//resize the element to fit the screen it is on
				tempRect.Resize(m_RadialBase.m_ElementMaxSize, m_ElementArcSize);
				//Move it's achor points to it's corners
				tempRect.AnchorsToCorners();
			}
			
			//set the size of the image on the cursaor
			m_CursorIconImageRect.Resize(m_RadialBase.m_ElementMaxSize, m_ElementArcSize);
			
			//get the position of the selected element
			Vector2 tempCPosition = new Vector2();
			float tempCAngle = m_SelectedElement * m_ElementAngleRad;//(360.0f - (m_Cursor.transform.localRotation.eulerAngles.z - (m_CursorImage.fillAmount * 180.0f))) * Mathf.Deg2Rad;
			tempCPosition.x = m_RadialBase.m_ScreenSize * Mathf.Sin(tempCAngle);
			tempCPosition.y = m_RadialBase.m_ScreenSize * Mathf.Cos(tempCAngle);
			//set teh image on the cursor to that position
			m_CursorIconImageRect.localPosition = tempCPosition;
			
			if(m_CurrentCleanUp != 0.0f)
			{
				//show the icon on the cursor so that nothing visibly changes
				m_CursorIconRect.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// The Main Update call for the layer
		/// </summary>
		/// <param name="targetAngle">angle provided by the input method</param>
		/// <param name="method">type of input</param>
		abstract public override void UpdateLayer(float? targetAngle, ControlMethod method);

		/// <summary>
		/// Moves the cursor and returns the selected element
		/// </summary>
		/// <param name="targetAngle">The angle of the cursor</param>
		protected virtual int HandleCursor(float targetAngle)
		{
			//work out what the selected element is based on the target angle
			int selectedElement = (int)Mathf.Round((targetAngle) / m_ElementAngleDeg);

			//make sure the selected element is a zero based index reference
			if (selectedElement >= m_ElementCount)
				selectedElement = 0;

			if (m_RefLayer.m_ElementSnapOverwrite)
			{
				if (m_RefLayer.m_ElementSnap)
				{
					//Set the cursor to the target rotation 
					m_CursorRect.localRotation = Quaternion.Euler(0.0f, 0.0f, 360.0f - ((selectedElement * m_ElementAngleDeg) - m_CursorOffset));
				}
				else
				{
					//Set the cursor to the target rotation 
					m_CursorRect.localRotation = Quaternion.Euler(0.0f, 0.0f, 360.0f - (targetAngle - m_CursorOffset));
				}
			}
			else if (m_RadialBase.m_ElementSnapping)
			{
				//Set the cursor to the target rotation 
				m_CursorRect.localRotation = Quaternion.Euler(0.0f, 0.0f, 360.0f - ((selectedElement * m_ElementAngleDeg) - m_CursorOffset));
			}
			else
			{
				//Set the cursor to the target rotation 
				m_CursorRect.localRotation = Quaternion.Euler(0.0f, 0.0f, 360.0f - (targetAngle - m_CursorOffset));
			}

			//return the selected element
			return selectedElement;
		}

		/// <summary>
		/// Increased the selected elements scale 
		/// </summary>
		/// <param name="angle">angle of the element</param>
		protected void SetSelectedElementSize(float angle)
		{
			//loop through all the elements
			for (int i = 0; i < m_ElementCount; i++)
			{
				//Work out the degrees that this element is at round the circle
				float elementDegrees = i * m_ElementAngleDeg;
				//Work out the difference between the cursor angle and this element
				float angleDifference = Mathf.Abs(angle - elementDegrees);

				//if the difference is greater than 180 degrees then it is the wrong side of he circle and must be corrected
				if (angleDifference > 180.0f)
				{
					angleDifference = 360.0f - angleDifference;
				}

				//if this is the selected element then it needs its scale setting based on the difference from the cursor
				if (i == m_SelectedElement)
				{
					m_ElementsScript[i].SetScale(Mathf.Lerp(m_RadialBase.m_ElementGrowthFactor, 1.0f, angleDifference / m_ElementAngleDeg));
				}
				//otherwise it needs to be reset to its starting scale
				else
				{
					m_ElementsScript[i].ResetScale();
				}
			}
		}
		
		/// <summary>
		/// The Update function for transition to out of this layer
		/// </summary>
		public override bool CleanUp()
		{
			//if it is the beginning of the cleanup process
			if (m_SavedCursorRotation == null)
			{
				//save the cursors current rotation
				m_SavedCursorRotation = m_Cursor.transform.localRotation.eulerAngles.z;
				//save the currently selected elements icon scale
				m_SavedIconScale = m_ElementsScript[m_SelectedElement].transform.GetChild(0).localScale.x;
			}

			//if the cursor is not on top of the elements
			if (m_Cursor.transform.GetSiblingIndex() != 1)
			{
				//reset the scale of the selected element
				m_ElementsScript[m_SelectedElement].ResetScale();

				//get the position of the selected element
				Vector2 tempPosition = new Vector2();
				float tempAngle = m_SelectedElement * m_ElementAngleRad;
				tempPosition.x = m_RadialBase.m_ScreenSize * Mathf.Sin(tempAngle);
				tempPosition.y = m_RadialBase.m_ScreenSize * Mathf.Cos(tempAngle);
				//set teh image on the cursor to that position
				m_CursorIconImageRect.localPosition = tempPosition;
				//set the size of the image on the cursaor
				m_CursorIconImageRect.Resize(m_RadialBase.m_ElementMaxSize, m_ElementArcSize);
				//set the scale to match the icon that it is replacing
				m_CursorIconImageRect.localScale = new Vector3(m_SavedIconScale.GetValueOrDefault(1.0f), m_SavedIconScale.GetValueOrDefault(1.0f), m_SavedIconScale.GetValueOrDefault(1.0f));
				//Copy the sprite from the selected element onto the image on the cursor
				m_CursorIconImage.sprite = m_MenuObjects[m_SelectedElement].m_ElementSprite;
                //Copy the sprite colour from the selected element onto the image on the cursor
                m_CursorIconImage.color = m_MenuObjects[m_SelectedElement].m_SpriteColour;
				//hide the selected element so that it doesn't fly round with the other elements during clean up
				m_ElementsScript[m_SelectedElement].gameObject.SetActive(false);

				//set the cursor to be on top of the elements 
				m_Cursor.transform.SetSiblingIndex(1);
				//show the icon on the cursor so that nothing visibly changes
				m_CursorIconRect.gameObject.SetActive(true);
			}

			//create the variable to hold the progress through the clean up process
			float percent;
			#region skip
			if(m_CurrentCleanUp >= m_CleanUpTime * 2.0f)
			{
				if(!m_bCleanUpSetup)
				{
					//reset the tracker variable
					m_CurrentCleanUp = 0.0f;
					//set the percent variable to its maximum
					percent = 1.0f;
					//set the index to the count of elements
					m_CurrentElementIndex = m_ElementCount;
					//set the flag to move onto the main clean up state
					m_bCleanUpSetup = true;

					
					//work out the traget rotation for the cursor to move to the selected element
					float targetRotation = (360.0f - (m_SelectedElement * (360.0f / m_ElementCount))) + (m_CursorImage.fillAmount * 180.0f);

					//if the unsigned target rotation is greater than 180 degrees then minus 360 to reference the correct side of the angle
					if (Mathf.Abs(targetRotation - m_SavedCursorRotation.GetValueOrDefault(0.0f)) > 180)
					{
						targetRotation -= 360.0f;
					}

					if(m_CleanUpTime != 0.0f)
					{
						//lerp the cursor round to the selected element position
						m_Cursor.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, targetRotation);
					}
					//keep the icon layer on the cursor at world z rotation 0
					m_CursorIconRect.localRotation = Quaternion.Euler(0.0f, 0.0f, -m_Cursor.transform.localRotation.eulerAngles.z);
					//lerp the scale of the selected element back down to 1
					m_CursorIconImageRect.localScale = Vector3.one;
				}

				foreach(RadialMenuElement element in m_ElementsScript)
				{
					if(element != null)
					{
						MonoBehaviour.Destroy(element.gameObject);
					}
				}
				
				//reset the tracker variable
				m_CurrentCleanUp = 0.0f;
				//clear the list of the elements
				m_ElementsScript.Clear();
				//reset the saved rotation holder
				m_SavedCursorRotation = null;
				//return true to exit the loop
				return true;
			}
			#endregion
			//if clean up is in the setup state
			if (!m_bCleanUpSetup)
			{
				//save the fill amount of the cursor
				float savedFill = m_CursorImage.fillAmount;
				//work out the percentage completion of the clean up multiplied by 2 to effectively doube the speed
				percent = Mathf.Min(m_CurrentCleanUp / m_CleanUpTime, 0.5f) * 2.0f;
				//increment the clean up tracker variable by the time since the last frame
				m_CurrentCleanUp += Time.deltaTime;

				//if the setup is finished
				if (percent >= 0.99f)
				{
					//reset the tracker variable
					m_CurrentCleanUp = 0.0f;
					//set the percent variable to its maximum
					percent = 1.0f;
					//set the index to the count of elements
					m_CurrentElementIndex = m_ElementCount;
					//set the flag to move onto the main clean up state
					m_bCleanUpSetup = true;
				}

				//work out the traget rotation for the cursor to move to the selected element
				float targetRotation = (360.0f - (m_SelectedElement * (360.0f / m_ElementCount))) + (savedFill * 180.0f);

				//if the unsigned target rotation is greater than 180 degrees then minus 360 to reference the correct side of the angle
				if (Mathf.Abs(targetRotation - m_SavedCursorRotation.GetValueOrDefault(0.0f)) > 180)
				{
					targetRotation -= 360.0f;
				}
				//lerp the cursor round to the selected element position
				m_Cursor.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(m_SavedCursorRotation.GetValueOrDefault(0.0f), targetRotation, percent));
				//keep the icon layer on the cursor at world z rotation 0
				m_CursorIconRect.localRotation = Quaternion.Euler(0.0f, 0.0f, -m_Cursor.transform.localRotation.eulerAngles.z);
				//lerp the scale of the selected element back down to 1
				m_CursorIconImageRect.localScale = Vector3.Lerp(new Vector3(m_SavedIconScale.GetValueOrDefault(1.0f), m_SavedIconScale.GetValueOrDefault(1.0f), m_SavedIconScale.GetValueOrDefault(1.0f)), Vector3.one, percent);
			}
			else
			{
				//work out the percent progress through the clean up
				percent = Mathf.Min(m_CurrentCleanUp / m_CleanUpTime, 1.0f);
				//increment the clean up tracker by the time since the last frame
				m_CurrentCleanUp += Time.deltaTime;

				//translate the percent into a rotation that will make 1 full turn in the clockwise direction
				float zRot = Mathf.Lerp(0.0f, -360.0f, percent);
				//apply the rotation to the elements
				m_RadialBase.ElementsParent.transform.localRotation = Quaternion.Euler(0.0f, 360.0f, zRot);
				//while the index of the target element to be destroyed is greater than the element count minus the element indexer
				while ((int)(percent / (1.0f / m_ElementCount)) > (m_ElementCount - m_CurrentElementIndex))
				{
					//decrement the indexer
					m_CurrentElementIndex--;
					//shift the idex round the circle by the selected element index so that it becomes zero
					int index = m_CurrentElementIndex + m_SelectedElement;
					//correct the index so it stays int he correct bounds
					if (index >= m_ElementCount)
					{
						index -= m_ElementCount;
					}

					//destroy the element at the created index
					MonoBehaviour.Destroy(m_ElementsScript[index].gameObject);
				}

				//if the clean up is finished
				if (percent >= 1.0f)
				{
					//clear the list of the elements
					m_ElementsScript.Clear();
					//reset the saved rotation holder
					m_SavedCursorRotation = null;
					//return true to exit the loop
					return true;
				}
			}
			//return false to stay in the loop
			return false;
		}
		
		/// <summary>
		/// The Update function for transitioning into this layer
		/// </summary>
		public override bool ReadyUp()
		{
			//if it is the beginning of the ready up process
			if (m_SavedCursorRotation == null)
			{
				//save the cursor z rotation
				m_SavedCursorRotation = m_Cursor.transform.localRotation.eulerAngles.z;
			}
			if (m_SavedCursorFill == null)
			{
				//save the cursor image fill amount
				m_SavedCursorFill = m_Cursor.GetComponent<Image>().fillAmount;
			}

			//create the variable to hold the percent progress
			float percent;

			#region skip
			if(m_CurrentReadyUp >= m_ReadyUpTime * 2.0f)
			{
				if(!m_bReadyUpSetup)
				{
					float theta;

					if(m_ReadyUpTime != 0.0f)
					{
						m_Cursor.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, m_ElementAngleDeg / 2.0f);
					}
					else
					{
						//work out the angle to the center of the cursor in radians
						theta = (m_Cursor.transform.localRotation.eulerAngles.z - (m_CursorImage.fillAmount * 180.0f));
						m_Cursor.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, theta + (m_ElementAngleDeg / 2.0f));
					}
					m_CursorImage.fillAmount = m_ElementAngleDeg / 360.0f;
					//work out the angle to the center of the cursor in radians
					theta = (360.0f - (m_Cursor.transform.localRotation.eulerAngles.z - (m_CursorImage.fillAmount * 180.0f))) * Mathf.Deg2Rad;
					//position the image on the middle of the cursor in local space
					m_CursorIconImageRect.localPosition = new Vector2(m_RadialBase.m_ScreenSize * Mathf.Sin(theta), m_RadialBase.m_ScreenSize * Mathf.Cos(theta));
					//correct the icon layer so that it has a z rotation of 0
					m_CursorIconRect.localRotation = Quaternion.Euler(0.0f, 0.0f, -m_Cursor.transform.localRotation.eulerAngles.z);
				}

				m_RadialBase.ElementsParent.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

				foreach(RadialMenuElement elementScript in m_ElementsScript)
				{
					elementScript.gameObject.SetActive(true);
				}

				//reset the tracker variable
				m_CurrentReadyUp = 0.0f;
				m_Cursor.transform.SetSiblingIndex(0);
				//reset the store variables
				m_SavedCursorRotation = null;
				m_SavedCursorFill = null;
				//hide the icon on the cursor
				m_CursorIconRect.gameObject.SetActive(false);

				UpdateLayer(360.0f - (m_Cursor.transform.localRotation.eulerAngles.z - (m_ElementAngleDeg / 2.0f)), ControlMethod.Mouse);

				//return true to exit the loop
				return true;
			}
			#endregion

			//if in the setup state
			if (!m_bReadyUpSetup)
			{
				//work out the percent progress as double the tracker variable over the limit effectively doubling the speed of the process
				percent = Mathf.Min(m_CurrentReadyUp / m_ReadyUpTime, 0.5f) * 2.0f;
				//increment the ready up tracker by the tuime since the last frame
				m_CurrentReadyUp += Time.deltaTime;

				//if at the end of the setup
				if (percent >= 0.99f)
				{
					//reset the tracker
					m_CurrentReadyUp = 0.0f;
					//set the percent to its limit
					percent = 1.0f;
					//initialise the current index
					m_CurrentElementIndex = 0;
					//set the flag to move into the main state
					m_bReadyUpSetup = true;
				}
				//lerp the cursor image fill amount to the new size denoted by the amount of elements
				m_CursorImage.fillAmount = Mathf.Lerp(m_SavedCursorFill.GetValueOrDefault(0.0f), m_ElementAngleDeg / 360.0f, percent);
				//lerp the cursor to the top of the menu
				m_Cursor.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(m_SavedCursorRotation.GetValueOrDefault(0.0f), m_ElementAngleDeg / 2.0f, percent));
				//work out the angle to the center of the cursor in radians
				float tempTheta = (360.0f - (m_Cursor.transform.localRotation.eulerAngles.z - (m_CursorImage.fillAmount * 180.0f))) * Mathf.Deg2Rad;
				//position the image on the middle of the cursor in local space
				m_CursorIconImageRect.localPosition = new Vector2(m_RadialBase.m_ScreenSize * Mathf.Sin(tempTheta), m_RadialBase.m_ScreenSize * Mathf.Cos(tempTheta));
				//correct the icon layer so that it has a z rotation of 0
				m_CursorIconRect.localRotation = Quaternion.Euler(0.0f, 0.0f, -m_Cursor.transform.localRotation.eulerAngles.z);
			}
			//otherwise in the main state
			else
			{
				//work out the percent progress through the ready up
				percent = Mathf.Min(m_CurrentReadyUp / m_ReadyUpTime, 1.0f);
				//increment the tracker by the time since the last frame
				m_CurrentReadyUp += Time.deltaTime;

				//translate the percent progess into a rotation aeound the z axis to move in a clockwise direction
				float zRot = Mathf.Lerp(360.0f, 0.0f, percent);
				//rotate the elements by this angle
				m_RadialBase.ElementsParent.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, zRot);
				//while the index of the target element to be shown is greater than the element indexer
				while ((int)(percent / (1.0f / m_ElementCount)) >= m_CurrentElementIndex)
				{
					//if it is the first element then set it as visible and increment the indexer
					if (m_CurrentElementIndex == 0)
					{
						m_CurrentElementIndex++;
					}
					//if it is not the last element then reverse the index against the list of elements and show the element at the new index
					//and increment the indexer
					else if (m_CurrentElementIndex != m_ElementCount)
					{
						m_ElementsScript[m_ElementCount - m_CurrentElementIndex].gameObject.SetActive(true);
						m_CurrentElementIndex++;
					}
					else
					{
						//increment the indexer
						m_CurrentElementIndex++;
					}
				}

				//if at the end of the ready up process
				if (percent >= 1.0f)
				{
                    //Set element 0 to visible if one exists
                    if (m_ElementsScript.Count > 0)
                    {
                        m_ElementsScript[0].gameObject.SetActive(true);
                    }

					//if the cursor is infront of the elements
					if (m_Cursor.transform.GetSiblingIndex() != 0)
					{
						//set the cursor as behind the elements
						m_Cursor.transform.SetSiblingIndex(0);
					}
					//reset the store variables
					m_SavedCursorRotation = null;
					m_SavedCursorFill = null;
					//hide the icon on the cursor
					m_CursorIconRect.gameObject.SetActive(false);
					//return true to exit the loop
					return true;
				}
			}
			//return false to stay in the loop
			return false;
		}
		
		/// <summary>
		/// Event call to process the currently selected element
		/// </summary>
		public override void Confirm()
		{
			m_MenuObjects[m_SelectedElement].Interact(m_RadialBase, m_RefLayer.m_LayerEvent);
		}
		
		/// <summary>
		/// Event call to cancel the layer and transition back up the breadcrumb
		/// </summary>
		public override void Cancel()
		{
			//if there is a header then set that as the selected element
			//this will move the cursor to the top of the menu to transition out
			if(m_RefLayer.m_MenuHeader != null)
			{
				m_SelectedElement = 0;
			}
			m_RadialBase.TransitionToPreviousLayer();
		}

		/// <summary>
		/// Event called to skip the current transition by doubling the current transition timer
		/// </summary>
		public override void Skip()
		{
			if(m_CurrentReadyUp != 0.0f)
			{
				m_CurrentReadyUp = m_ReadyUpTime * 2.0f;
			}
			if(m_CurrentCleanUp != 0.0f)
			{
				m_CurrentCleanUp = m_CleanUpTime * 2.0f;
			}
		}
	}
}