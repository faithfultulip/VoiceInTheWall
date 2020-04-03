using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	public abstract class RadialLayerHandler
	{
        #region private variables

        /// <summary>
        /// Reference to the current ScriptableObject that is used to get data
        /// </summary>
        private RadialLayer m_RefLayer;

        #endregion

        #region protected variables

        /// <summary>
        /// Radial Menu that this layer belongs to
        /// </summary>
        protected RadialBase			m_RadialBase;

		/// <summary>
		/// True if the clean up function is finished with the setup stage
		/// </summary>
		protected bool					m_bCleanUpSetup;
		/// <summary>
		/// True if the ready up function is finished with the setup stage
		/// </summary>
		protected bool					m_bReadyUpSetup;
		/// <summary>
		/// Time used for the current cleanup process
		/// </summary>
		protected float					m_CurrentCleanUp;
		/// <summary>
		/// how long the layer will take to be removed from the screen
		/// </summary>
		protected float					m_CleanUpTime;
		/// <summary>
		/// Time used for the current readyup process
		/// </summary>
		protected float					m_CurrentReadyUp;
		/// <summary>
		/// how long the layer will take to load on to the screen
		/// </summary>
		protected float					m_ReadyUpTime = 1.0f;

		/// <summary>
		/// The cursors game object
		/// </summary>
		protected GameObject			m_Cursor;
		/// <summary>
		/// Image component on the main cursor object
		/// </summary>
		protected Image					m_CursorImage;
		/// <summary>
		/// RectTransform of the main cursor object
		/// </summary>
		protected RectTransform			m_CursorRect;
		/// <summary>
		/// RectTransform of the icon layer on the cursor
		/// </summary>
		protected RectTransform			m_CursorIconRect;
		/// <summary>
		/// Image Component of the image on the icon layer for the cursor
		/// </summary>
		protected Image					m_CursorIconImage;
		/// <summary>
		/// RectTransform of the image on the icon layer for the cursor
		/// </summary>
		protected RectTransform			m_CursorIconImageRect;
		/// <summary>
		/// The cursor rotation at the start of either a cleanup or a readyup, nullable so that it is only set once per transition
		/// </summary>
		protected float?				m_SavedCursorRotation;
		/// <summary>
		/// The cursor fill amount at the start of a cleanup or a readyup, it is nullable so that it is only set once per transition
		/// </summary>
		protected float?				m_SavedCursorFill;

        #endregion

        /// <summary>
        /// Main initialisation function
        /// </summary>
        /// <param name="menu">the reference to the RadialBase script that is associated with this layer</param>
        /// <param name="layer">the scriptable object that is providing this layer with data</param>
        /// <param name="customElements">the custom list of elements that can be passed at run time</param>
        public virtual void Init(RadialBase menu, RadialLayer layer, List<RadialMenuObject> customElements = null)
		{
			//initialise all of the protected and private variables
			m_RadialBase = menu;
			m_RefLayer = layer;
            //Set the active layer in the radial base
            m_RadialBase.ActiveLayer = this;

            m_CurrentCleanUp = 0.0f;
			m_CurrentReadyUp = 0.0f;

			//if the override bool is false then get these values from the base else get them from the layer
			if(!m_RefLayer.m_OverwriteTimers)
			{
				m_ReadyUpTime = m_RadialBase.m_DefaultReadyUpTime;
				m_CleanUpTime = m_RadialBase.m_DefaultCleanUpTime;
			}
			else
			{
				m_ReadyUpTime = m_RefLayer.m_OverwriteReadyUpTime;
				m_CleanUpTime = m_RefLayer.m_OverwriteCleanUpTime;
			}


			m_SavedCursorRotation = null;
			m_SavedCursorFill = null;
			m_bReadyUpSetup = m_bCleanUpSetup = false;
			m_Cursor = menu.transform.Find("RadialPanel").Find("Cursor").gameObject;
			m_CursorImage = m_Cursor.GetComponent<Image>();
			Populate();

			m_CursorRect = m_Cursor.transform as RectTransform;
			m_CursorIconRect = m_CursorRect.Find("Icon").GetComponent<RectTransform>();
			m_CursorIconImageRect = m_CursorIconRect.Find("Image").GetComponent<RectTransform>();
			m_CursorIconImage = m_CursorIconImageRect.GetComponent<Image>();
		}

		/// <summary>
		/// Overload initialisation for the slider layer
		/// </summary>
		/// <param name="menu">the reference to the RadialBase script that is associated with this layer</param>
		/// <param name="layer">the scriptable object that is providing this layer with data</param>
		/// <param name="sliderValue">The starting value for the slider</param>
		public virtual void Init(RadialBase menu, RadialLayer layer, float sliderValue)
		{
			Init(menu, layer);
		}

		/// <summary>
		/// Overload initialisation for the selection layer
		/// </summary>
		/// <param name="menu">the reference to the RadialBase script that is associated with this layer</param>
		/// <param name="layer">the scriptable object that is providing this layer with data</param>
		/// <param name="currentIndex">the starting index in the list of items to be selected through</param>
		/// <param name="selectionValue">the values for the selection layer to select from</param>
		public virtual void Init(RadialBase menu, RadialLayer layer, int currentIndex, string[] selectionValue)
		{
			Init(menu, layer);
		}

		/// <summary>
		/// Updates the text that is used to display information about the radial menu
		/// </summary>
		/// <param name="detailText">Text that is shown to give detail about the current element</param>
		protected void UpdateDisplayedText(string detailText)
		{
			//if overwrite details panel is false check the radial base to see which details can be shown
			//else use the layers values
			if(!m_RefLayer.m_OverwriteDetailPanel)
			{
				if(m_RadialBase.m_ShowLayerName)
					m_RadialBase.LayerNameText.text = m_RefLayer.m_LayerName;

				if(m_RadialBase.m_ShowDetailText)
					m_RadialBase.DetailText.text = detailText;
			}
			else
			{
				if(m_RefLayer.m_ShowLayerName)
					m_RadialBase.LayerNameText.text = m_RefLayer.m_LayerName;

				if(m_RefLayer.m_ShowDetailText)
					m_RadialBase.DetailText.text = detailText;
			}
		}

		/// <summary>
		/// Updates the text that is used to display information about the radial menu
		/// </summary>
		/// <param name="elementSprite">The sprite that is displayed in the information about the radial menu</param>
		/// <param name="spriteColour">The colour of the spirte that is displayed</param>
		protected void UpdateDisplayedText(Sprite elementSprite, Color spriteColour)
		{
			//if overwrite details panel is false check the radial base to see which details can be shown
			//else use the layers values
			if(!m_RefLayer.m_OverwriteDetailPanel)
			{
				if(m_RadialBase.m_ShowLayerName)
					m_RadialBase.LayerNameText.text = m_RefLayer.m_LayerName;

				if(m_RadialBase.m_ShowSelectedElementSprite)
				{
					m_RadialBase.ElementSprite.sprite = elementSprite;
					m_RadialBase.ElementSprite.color = spriteColour;
				}
			}
			else
			{
				if(m_RefLayer.m_ShowLayerName)
					m_RadialBase.LayerNameText.text = m_RefLayer.m_LayerName;

				if(m_RefLayer.m_ShowSelectedElementSprite)
				{
					m_RadialBase.ElementSprite.sprite = elementSprite;
					m_RadialBase.ElementSprite.color = spriteColour;
				}
			}

			//Set the image to active false if there is no sprite
			m_RadialBase.ElementSprite.gameObject.SetActive((m_RadialBase.ElementSprite.sprite != null));
		}

		/// <summary>
		/// Updates the text that is used to display information about the radial menu
		/// </summary>
		/// <param name="detailText">Text that is shown to give detail about the current element</param>
		/// <param name="elementSprite">The sprite that is displayed in the information about the radial menu</param>
		/// <param name="spriteColour">The colour of the spirte that is displayed</param>
		protected void UpdateDisplayedText(string detailText, Sprite elementSprite, Color spriteColour)
		{
			//if overwrite details panel is false check the radial base to see which details can be shown
			//else use the layers values
			if(!m_RefLayer.m_OverwriteDetailPanel)
			{
				if(m_RadialBase.m_ShowLayerName)
					m_RadialBase.LayerNameText.text = m_RefLayer.m_LayerName;

				if(m_RadialBase.m_ShowDetailText)
					m_RadialBase.DetailText.text = detailText;

				if(m_RadialBase.m_ShowSelectedElementSprite)
				{
					m_RadialBase.ElementSprite.sprite = elementSprite;
					m_RadialBase.ElementSprite.color = spriteColour;
				}
			}
			else
			{
				if(m_RefLayer.m_ShowLayerName)
					m_RadialBase.LayerNameText.text = m_RefLayer.m_LayerName;

				if(m_RefLayer.m_ShowDetailText)
					m_RadialBase.DetailText.text = detailText;

				if(m_RefLayer.m_ShowSelectedElementSprite)
				{
					m_RadialBase.ElementSprite.sprite = elementSprite;
					m_RadialBase.ElementSprite.color = spriteColour;
				}

			}

			//Set the image to active false if there is no sprite
			m_RadialBase.ElementSprite.gameObject.SetActive((m_RadialBase.ElementSprite.sprite != null));
		}

		/// <summary>
		/// Populates the layer
		/// </summary>
		protected abstract void Populate();

		/// <summary>
		/// The Main Update call for the layer
		/// </summary>
		/// <param name="targetAngle">angle provided by the input method</param>
		/// <param name="method">type of input</param>
		public abstract void UpdateLayer(float? targetAngle, ControlMethod method);

		/// <summary>
		/// The Update function for transition to out of this layer
		/// </summary>
		public abstract bool CleanUp();

		/// <summary>
		/// The Update function for transitioning into this layer
		/// </summary>
		public abstract bool ReadyUp();

		/// <summary>
		/// Event call to process the currently selected element
		/// </summary>
		public abstract void Confirm();

		/// <summary>
		/// Event call to cancel the layer and transition back up the breadcrumb
		/// </summary>
		public abstract void Cancel();

		/// <summary>
		/// Returned each value on a new line
		/// </summary>
		/// <param name="values">String values that will be returned</param>
		/// <returns></returns>
		protected string MultiLineText(params string[] values)
		{
			string tempString = "";
			int stringLength = values.Length;

			for(int i = 0; i < stringLength; i++)
			{

				tempString += values[i];

				if(i < stringLength - 1)
				{
					tempString += Environment.NewLine;
				}
			}

#if UNITY_5_3
			return tempString + Environment.NewLine;
#else
			return tempString;
#endif
		}

        /// <summary>
        /// Redraw the layer in the correct aspect ratio
        /// </summary>
        /// <param name="targetAngle">Angle of the cursor</param>
        /// <param name="method">Input type that is being used</param>
		public abstract void Redraw(float? targetAngle, ControlMethod method);

        /// <summary>
        /// Skips the current layer transition
        /// </summary>
		public abstract void Skip();
	}
}