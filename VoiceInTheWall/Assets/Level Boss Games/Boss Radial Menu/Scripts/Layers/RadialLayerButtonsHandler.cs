using UnityEngine;
using System;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	public class RadialLayerButtonsHandler : RadialLayerElementsHandler
	{
        #region private variables

        /// <summary>
        /// Reference to the current ScriptableObject that is used to get data
        /// </summary>
        private RadialLayerButtons								m_RefLayer;

		#endregion

		/// <summary>
		/// Override initialisation for the button layer
		/// </summary>
		/// <param name="menu">the reference to the RadialBase script that is associated with this layer</param>
		/// <param name="layer">the scriptable object that is providing this layer with data</param>
		public override void Init(RadialBase menu, RadialLayer layer, List<RadialMenuObject> customElements = null)
		{
			//Cast the scriptable object to one that is compatible with this script
			m_RefLayer = layer as RadialLayerButtons;
            m_RefLayer.m_CustomMenuElements = customElements;
			base.Init(menu, layer);
		}

		/// <summary>
		/// Add elements to the m_Objects list in the base class - called in base.Init()
		/// </summary>
		/// <returns></returns>
		protected override List<RadialMenuObject> AddElements()
		{
            if(m_RefLayer.m_CustomMenuElements != null)
            {
                if (m_RefLayer.m_CustomMenuElements.Count != 0)
                {
                    m_ContaintsElements = true;

                    return m_RefLayer.m_CustomMenuElements;
                }
            }

            if(m_RefLayer.m_MenuElements.Count == 0)
            {
                m_ContaintsElements = false;
            }
            else
            {
                m_ContaintsElements = true;
            }

			return m_RefLayer.m_MenuElements;
		}

		/// <summary>
		/// Used for generating any gameObjects needed for the layer
		/// </summary>
		protected override void Populate()
		{
			base.Populate();
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
					Type buttonType = m_MenuObjects[m_SelectedElement].GetType();
					//Check to see if the selected element is a toggle button
					if (buttonType == typeof(RadialMenuToggleButton))
					{
						RadialMenuToggleButton tempButton = m_MenuObjects[m_SelectedElement] as RadialMenuToggleButton;

						//Check to see if the current toggle value is true or false 
						if (m_RadialBase.m_InputManager.ProcessReturnValues<bool>(m_RefLayer.m_LayerEvent, tempButton.m_ElementEvent))
						{
							//Add the true value to the desplayed text on screen
							UpdateDisplayedText(MultiLineText(m_MenuObjects[m_SelectedElement].GetName(), tempButton.m_TrueValue) , tempButton.m_ElementSprite, tempButton.m_SpriteColour);
						}
						else
						{
							//Add the false value to the desplayed text on screen
							UpdateDisplayedText(MultiLineText(m_MenuObjects[m_SelectedElement].GetName(), tempButton.m_FalseValue), tempButton.m_FalseElementSprite, tempButton.m_FalseSpriteColour);
						}
					}
					//If the button is a slider button get the value of the slider
					else if(buttonType == typeof(RadialMenuSliderButton))
					{
						RadialMenuSliderButton tempButton = m_MenuObjects[m_SelectedElement] as RadialMenuSliderButton;

						float sliderValue = m_RadialBase.m_InputManager.ProcessReturnValues<float>(m_RefLayer.m_LayerEvent, tempButton.m_ElementEvent);

						UpdateDisplayedText(MultiLineText(m_MenuObjects[m_SelectedElement].GetName(), sliderValue.RoundToDP(tempButton.m_DisplayedTextDecimalPoint).ToString()), tempButton.m_ElementSprite, tempButton.m_SpriteColour);
					}
					//If the button is a selection button get the value of the selection
					else if(buttonType == typeof(RadialMenuSelectionButton))
					{
						RadialMenuSelectionButton tempButton = m_MenuObjects[m_SelectedElement] as RadialMenuSelectionButton;

						string selectionValue = m_RadialBase.m_InputManager.ProcessReturnValues<string>(m_RefLayer.m_LayerEvent, tempButton.m_ElementEvent);

						UpdateDisplayedText(MultiLineText(m_MenuObjects[m_SelectedElement].GetName(), selectionValue), tempButton.m_ElementSprite, tempButton.m_SpriteColour);
					}
					else //If it's an ordinary button
					{
						//display the button name
						UpdateDisplayedText(m_MenuObjects[m_SelectedElement].GetName(), (m_SelectedElement == 0 && m_RefLayer.m_MenuHeader != null) ? null : m_MenuObjects[m_SelectedElement].m_ElementSprite, (m_SelectedElement == 0 && m_RefLayer.m_MenuHeader != null) ? Color.clear : m_MenuObjects[m_SelectedElement].m_SpriteColour);
					}
				}

				//Change the size of the element if it is selected
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
            //Press the button and get it's type
            Type buttonType = m_MenuObjects[m_SelectedElement].Interact(m_RadialBase, m_RefLayer.m_LayerEvent);

			//If the button type is a radial menu update the displayed text 
			if (buttonType == typeof(RadialMenuToggleButton))
			{
				RadialMenuToggleButton tempButton = m_MenuObjects[m_SelectedElement] as RadialMenuToggleButton;
				bool toggleResult = m_RadialBase.m_InputManager.ProcessReturnValues<bool>(m_RefLayer.m_LayerEvent, tempButton.m_ElementEvent);

				if (toggleResult)
				{
					m_ElementsScript[m_SelectedElement].UpdateSprite(tempButton.m_ElementSprite, tempButton.m_SpriteColour);
					UpdateDisplayedText(MultiLineText(m_MenuObjects[m_SelectedElement].GetName(), tempButton.m_TrueValue), tempButton.m_ElementSprite, tempButton.m_SpriteColour);
				}
				else
				{
					m_ElementsScript[m_SelectedElement].UpdateSprite(tempButton.m_FalseElementSprite, tempButton.m_FalseSpriteColour);
					UpdateDisplayedText(MultiLineText(m_MenuObjects[m_SelectedElement].GetName(), tempButton.m_FalseValue), tempButton.m_FalseElementSprite, tempButton.m_FalseSpriteColour);
				}

			}
		}

		/// <summary>
		/// Event call to cancel the layer and transition back up the breadcrumb
		/// </summary>
		public override void Cancel()
		{
			base.Cancel();
		}

        public override void Reload(float? targetAngle, ControlMethod method, List<RadialMenuObject> customElements = null)
        {
            m_MenuObjects.Clear();
            //if a header has been specified
            if (m_RefLayer.m_MenuHeader != null)
            {
                //add the header to the list of objects to be created
                m_MenuObjects.Add(m_RefLayer.m_MenuHeader);
            }
            //Add the elements to the list of objects to be created
            m_RefLayer.m_CustomMenuElements = customElements;
            m_MenuObjects.AddRange(AddElements().ToArray());
            Redraw(targetAngle, method);
        }

    }
}