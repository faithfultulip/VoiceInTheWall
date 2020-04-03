using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	public class RadialLayerSliderHandler : RadialLayerHandler
	{
        #region private variables

        /// <summary>
        /// Reference to the current ScriptableObject that is used to get data
        /// </summary>
        private RadialLayerSlider					m_RefLayer;

		/// <summary>
        /// GameObject reference to the background element
        /// </summary>
        private GameObject							m_SliderBaseGO;
        /// <summary>
        /// RectTransform of the background element
        /// </summary>
        private RectTransform						m_SliderBaseRect;
        /// <summary>
        /// Image component of the background element
        /// </summary>
        private Image								m_SliderBaseImage;
        /// <summary>
        /// GameObject reference to the fill element
        /// </summary>
        private GameObject							m_SliderFillGO;
        /// <summary>
        /// RectTransform of the fill element
        /// </summary>
        private RectTransform						m_SliderFillRect;
        /// <summary>
        /// Image component of the fill element
        /// </summary>
        private Image								m_SliderFillImage;

        /// <summary>
        /// Current angle of the center of the cursor in degrees (anti-clockwise)
        /// </summary>
        private float								m_CursorAngle;

        /// <summary>
        /// Current value for the fill element Image fill amount
        /// </summary>
        private float								m_SliderValue;
        /// <summary>
        /// Maximum value that m_SliderValue can show to line up with the cursor
        /// </summary>
        private float								m_SliderMax;

        /// <summary>
        /// Actual value that the slider currently represents
        /// </summary>
        private float								m_LayerValue;
        /// <summary>
        /// Original that the slider loaded with
        /// </summary>
        private float								m_OriginalLayerValue;

        /// <summary>
        /// True if the current value of m_LayerValue should be saved
        /// </summary>
        private bool								m_ValueSaved;

		#endregion

		/// <summary>
        /// Override initialisation for the slider layer
        /// </summary>
        /// <param name="menu">the reference to the RadialBase script that is associated with this layer</param>
		/// <param name="layer">the scriptable object that is providing this layer with data</param>
        /// <param name="sliderValue">The starting value for the slider</param>
        public override void Init(RadialBase menu, RadialLayer layer, float sliderValue)
        {
			m_RefLayer = layer as RadialLayerSlider;

            base.Init(menu, layer);

			///Starting value for the slider tis applied and saved
			m_LayerValue = sliderValue;
            m_OriginalLayerValue = sliderValue;

            //slider max is the size of the cursor in percent taken from 1
            m_SliderMax = (360.0f - (m_RefLayer.m_CursorSize - 1.0f)) / 360.0f;

            //slider value is worked out from the value passed through alpha'd between the layer min and max
            //that value is then used as the alpha between the slider min and max
            m_SliderValue = Mathf.Lerp(0.0f, m_SliderMax, Mathf.InverseLerp(m_RefLayer.m_LayerMin, m_RefLayer.m_LayerMax, m_LayerValue));
            //the actual cursor angle is the cursor element rotation minus half the length of the fill amount in degrees
            //this is because the cursor is filling from the top in a clockwise direction and we are looking down the positive z so positive angles go anti-clockwse
            m_CursorAngle = m_Cursor.transform.localRotation.eulerAngles.z - ((m_Cursor.GetComponent<Image>().fillAmount * 360.0f) / 2);

            //run the text update function once to set up the text with initial values
            UpdateDisplayedText(m_LayerValue.RoundToDP(m_RefLayer.m_DisplayedTextDecimalPoint).ToString());

			//Ensures the cursor has an image
			if (m_CursorIconImage.sprite == null)
			{
				m_CursorIconImage.sprite = m_RefLayer.m_CursorSprite;
				m_CursorIconRect.gameObject.SetActive(true);
				ResizeCursorImage();
			}
		}

        /// <summary>
        /// Used for generating any gameObjects needed for the layer
        /// </summary>
        protected override void Populate()
        {
            //Background object
            m_SliderBaseGO = new GameObject("Background Object", typeof(RectTransform));
            m_SliderBaseGO.transform.SetParent(m_RadialBase.ElementsParent, false);
            m_SliderBaseRect = m_SliderBaseGO.GetComponent<RectTransform>();
            m_SliderBaseGO.AddComponent<CanvasRenderer>();
            //Background object image
            m_SliderBaseImage = m_SliderBaseGO.AddComponent<Image>();
            m_SliderBaseImage.preserveAspect = true;
            m_SliderBaseImage.sprite = m_RefLayer.m_BackgroundImage;
            m_SliderBaseImage.color = m_RefLayer.m_BackgroundColour;
            m_SliderBaseImage.type = Image.Type.Filled;
            m_SliderBaseImage.fillMethod = Image.FillMethod.Radial360;
            m_SliderBaseImage.fillClockwise = true;
            m_SliderBaseImage.fillOrigin = (int)Image.Origin360.Top;
            m_SliderBaseImage.fillAmount = 0.0f;
            //Background object transform / anchors
            m_SliderBaseRect.anchorMin = Vector2.zero;
            m_SliderBaseRect.anchorMax = Vector2.one;
            m_SliderBaseRect.CornersToAnchors();

            //Fill object
            m_SliderFillGO = new GameObject("Fill Object" , typeof(RectTransform));
            m_SliderFillGO.transform.SetParent(m_SliderBaseGO.transform, false);
            m_SliderFillRect = m_SliderFillGO.GetComponent<RectTransform>();
            m_SliderFillGO.AddComponent<CanvasRenderer>();
            //Fill object image
            m_SliderFillImage = m_SliderFillGO.AddComponent<Image>();
            m_SliderFillImage.preserveAspect = true;
            m_SliderFillImage.sprite = m_RefLayer.m_FillImage;
            m_SliderFillImage.color = m_RefLayer.m_FillColour;
            m_SliderFillImage.type = Image.Type.Filled;
            m_SliderFillImage.fillMethod = Image.FillMethod.Radial360;
            m_SliderFillImage.fillClockwise = true;
            m_SliderFillImage.fillOrigin = (int)Image.Origin360.Top;
            m_SliderFillImage.fillAmount = 0.0f;
            //Fill object transform / anchors
            m_SliderFillRect.anchorMin = Vector2.zero;
            m_SliderFillRect.anchorMax = Vector2.one;
            m_SliderFillRect.CornersToAnchors();
		}

		/// <summary>
		/// In the event that the resolution of the screen changes, the elements need to be redrawn in new positions
		/// </summary>
		/// <param name="targetAngle">the current angle of the cursor</param>
		/// <param name="method">the current control method being used</param>
        public override void Redraw(float? targetAngle, ControlMethod method)
        {
			//Re-create the elements
            Populate();
            m_SliderBaseImage.fillAmount = m_SliderMax;
            m_SliderFillImage.fillAmount = m_SliderValue;
            m_SliderBaseRect.localRotation = Quaternion.Euler(0.0f, 0.0f, m_Cursor.transform.localRotation.eulerAngles.z - m_RefLayer.m_CursorSize);

			ResizeCursorImage();
        }

        /// <summary>
        /// The Main Update call for the layer
        /// </summary>
        /// <param name="targetAngle">angle provided by the input method</param>
        /// <param name="method">type of input</param>
        public override void UpdateLayer(float? targetAngle, ControlMethod method)
        {
            //initialise the value saved bool
            m_ValueSaved = false;
            //if the method is a valid input method
            if (method == ControlMethod.Joystick || method == ControlMethod.Mouse || method == ControlMethod.Touch)
            {
                //if we have recieved an input angle
                if (targetAngle != null)
                {
                    //translate the target angle to be clockwise degrees from the cursor angle
                    targetAngle = 360.0f + (targetAngle.GetValueOrDefault(0.0f) - (360.0f - m_CursorAngle));
                    if (targetAngle.GetValueOrDefault(0.0f) > 360)
                    {
                        targetAngle = targetAngle.GetValueOrDefault(0.0f) - 360.0f;
                    }
                    targetAngle -= m_RefLayer.m_CursorSize / 2.0f;

                    //clamp the slider value so that it stays between the min and max
                    m_SliderValue = Mathf.Clamp(targetAngle.GetValueOrDefault(0.0f) / 360.0f, 0.0f, m_SliderMax);
                    //set the fill amount of the fill element to the new clamped slider value
                    m_SliderFillImage.fillAmount = m_SliderValue;
                    //set the layer value between the the layer min and max using the slider value as an alpha
                    m_LayerValue = Mathf.Lerp(m_RefLayer.m_LayerMin, m_RefLayer.m_LayerMax, Mathf.InverseLerp(0.0f, m_SliderMax, m_SliderValue));

                    //Process the current value of the slider in the input manager
                    m_RadialBase.m_InputManager.ProcessSlider(m_RefLayer.m_LayerEvent, m_LayerValue, false);

                    //Run the update for the text in the middle of the menu for the new value
                    UpdateDisplayedText(m_LayerValue.RoundToDP(m_RefLayer.m_DisplayedTextDecimalPoint).ToString());
                }
            }
        }

        /// <summary>
        /// The Update function for transition to out of this layer
        /// </summary>
        public override bool CleanUp()
        {
            //if at the start of the cleanup
            if (m_CurrentCleanUp == 0.0f)
            {
                //if we are not saving the value
                if (!m_ValueSaved)
                {
                    //Run the event in the input manager for not saving the value
                    m_RadialBase.m_InputManager.ProcessSlider(m_RefLayer.m_LayerEvent, m_OriginalLayerValue, false);
                }
                else
                {
                    //Run the event in the input manager for saving the value
                    m_RadialBase.m_InputManager.ProcessSlider(m_RefLayer.m_LayerEvent, m_LayerValue, true);
                }
            }
            //work out the percentage completion of the cleanup process
            float percent = Mathf.Min(m_CurrentCleanUp / m_CleanUpTime, 1.0f);
            //inverse the percent value for the purposes of lerping out
            float lerpPosition = Mathf.Lerp(1.0f, 0.0f, percent);

            #region skip
            if (m_CurrentCleanUp >= m_CleanUpTime * 2.0f)
            {
                //reset the tracker variable
                m_CurrentCleanUp = 0.0f;
                //destroy all GameObjects created in the populate function
                MonoBehaviour.Destroy(m_SliderFillGO);
                MonoBehaviour.Destroy(m_SliderBaseGO);
                //return true to exit the loop
                return true;
            }
            #endregion

            //set the fill amount of the background element image as the lerp position
            m_SliderBaseImage.fillAmount = lerpPosition;
            //if the lerp position is passed the point of the current fill element fill amount
            if (lerpPosition <= m_SliderValue)
            {
                //set the fill element fill amount tot the lerp position
                m_SliderFillImage.fillAmount = lerpPosition;
            }

            //increment the current clean up time by the time since the last frame
            m_CurrentCleanUp += Time.deltaTime;

            //if the clean up function has finished
            if (percent >= 1.0f)
            {
                //reset the tracker variable
                m_CurrentCleanUp = 0.0f;
                //destroy all GameObjects created in the populate function
                MonoBehaviour.Destroy(m_SliderFillGO);
                MonoBehaviour.Destroy(m_SliderBaseGO);
                //return true to exit the loop
                return true;
            }
            //return false to continue the loop
            return false;
        }

        /// <summary>
        /// The Update function for transitioning into this layer
        /// </summary>
        public override bool ReadyUp()
        {
            //if the ready up function has just started
            if (m_CurrentReadyUp == 0.0f && !m_bReadyUpSetup)
            {
                //if cursor rotation hasn't been saved
                if (m_SavedCursorRotation == null)
                {
                    //save the cursor rotation
                    m_SavedCursorRotation = m_Cursor.transform.localRotation.eulerAngles.z;
                }

                //if the cursor fill amount hasn't been saved
                if (m_SavedCursorFill == null)
                {
                    //save the cursors fill amount
                    m_SavedCursorFill = m_Cursor.GetComponent<Image>().fillAmount;

                }

                //save the angle of the cursor based on the 2 saved values
                m_CursorAngle = m_SavedCursorRotation.GetValueOrDefault(0.0f) - ((m_SavedCursorFill.GetValueOrDefault(0.0f) * 360.0f) / 2);
            }

            //create the variable to hold the percent value
            float percent;

            #region skip
            if (m_CurrentReadyUp >= m_ReadyUpTime * 2.0f)
            {
                if (!m_bReadyUpSetup)
                {
                    //set the flag to say the setup has finished
                    m_bReadyUpSetup = true;
                    //resize the cursor to the size set in the layer
                    m_CursorImage.fillAmount = m_RefLayer.m_CursorSize / 360.0f;
                    //keep the cursor in the same center position as it resizes
                    m_Cursor.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, m_CursorAngle + (m_RefLayer.m_CursorSize / 2.0f));
                    //get the angle to the center of the cursor in radians
                    float theta = (360.0f - (m_Cursor.transform.localRotation.eulerAngles.z - (m_CursorImage.fillAmount * 180.0f))) * Mathf.Deg2Rad;
                    //set the cursor icon position to the same as the center of the cursor
                    m_CursorIconImageRect.localPosition = new Vector2(m_RadialBase.m_ScreenSize * Mathf.Sin(theta), m_RadialBase.m_ScreenSize * Mathf.Cos(theta));
                    //set the cursor icon holder to the opposite rotation to the cursor so that it is always at z rotation 0
                    m_CursorIconRect.localRotation = Quaternion.Euler(0.0f, 0.0f, -m_Cursor.transform.localRotation.eulerAngles.z);

                    //set the background to be the same rotation as the cursor
                    m_SliderBaseRect.localRotation = Quaternion.Euler(0.0f, 0.0f, m_Cursor.transform.localRotation.eulerAngles.z - m_RefLayer.m_CursorSize);
                }

                m_SliderBaseImage.fillAmount = 1.0f;
                m_SliderFillImage.fillAmount = m_SliderValue;

                //reset the tracker variable
                m_CurrentReadyUp = 0.0f;
                //reset the saved variables so that they will be set the next time this functio fires
                m_SavedCursorRotation = null;
                m_SavedCursorFill = null;
                //return true to exit the loop
                return true;
            }
            #endregion

            //if we are in the first stage of setup
            if (!m_bReadyUpSetup)
            {
                //percent is equal to the percentage through the setup x 2 capped at 1, effectively doubling the speed of time
                percent = Mathf.Min(m_CurrentReadyUp / m_ReadyUpTime, 0.5f) * 2.0f;
                //increment the current ready up time by the time since the last frame
                m_CurrentReadyUp += Time.deltaTime;

                //if the setup stage has finished
                if (percent >= 0.99f)
                {
                    //reset the tracker variable
                    m_CurrentReadyUp = 0.0f;
                    //set the percent variable to the max it can be
                    percent = 1.0f;
                    //set the flag to say the setup has finished
                    m_bReadyUpSetup = true;
                    //set the background to be the same rotation as the cursor
                    m_SliderBaseRect.localRotation = Quaternion.Euler(0.0f, 0.0f, m_Cursor.transform.localRotation.eulerAngles.z - m_RefLayer.m_CursorSize);
                }

                //resize the cursor to the size set in the layer
                m_CursorImage.fillAmount = Mathf.Lerp(m_SavedCursorFill.GetValueOrDefault(0.0f), m_RefLayer.m_CursorSize / 360.0f, percent);
                //keep the cursor in the same center position as it resizes
                m_Cursor.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(m_SavedCursorRotation.GetValueOrDefault(0.0f), m_CursorAngle + (m_RefLayer.m_CursorSize / 2.0f), percent));
                //get the angle to the center of the cursor in radians
                float tempTheta = (360.0f - (m_Cursor.transform.localRotation.eulerAngles.z - (m_CursorImage.fillAmount * 180.0f))) * Mathf.Deg2Rad;
                //set the cursor icon position to the same as the center of the cursor
                m_CursorIconImageRect.localPosition = new Vector2(m_RadialBase.m_ScreenSize * Mathf.Sin(tempTheta), m_RadialBase.m_ScreenSize * Mathf.Cos(tempTheta));
                //set the cursor icon holder to the opposite rotation to the cursor so that it is always at z rotation 0
                m_CursorIconRect.localRotation = Quaternion.Euler(0.0f, 0.0f, -m_Cursor.transform.localRotation.eulerAngles.z);
            }
            //not in the setup stage
            else
            {
                //set the percentage variable as the percent completion of the ready up function
                percent = Mathf.Min(m_CurrentReadyUp / m_ReadyUpTime, 1.0f);

                if (percent <= m_SliderMax)
                {
                    //set the background element image fill amount as the value of percent
                    m_SliderBaseImage.fillAmount = percent;
                    //if the value of percent is less than the value of the slider then we need to set the fill element image fill amount 
                    if (percent <= m_SliderValue)
                    {
                        m_SliderFillImage.fillAmount = percent;
                    }
                }

                //increment the ready up tracker by the time since the last frame
                m_CurrentReadyUp += Time.deltaTime;

                //if the ready up process has finished
                if (percent >= 1.0f)
                {
                    m_SliderBaseImage.fillAmount = 1.0f;
                    m_SliderFillImage.fillAmount = m_SliderValue;
                    //reset the tracker variable
                    m_CurrentReadyUp = 0.0f;
                    //reset the saved variables so that they will be set the next time this functio fires
                    m_SavedCursorRotation = null;
                    m_SavedCursorFill = null;
                    //return true to exit the loop
                    return true;
                }
            }
            //return false to stay in the loop
            return false;
        }

        /// <summary>
        /// Event call to process the layer
        /// </summary>
        public override void Confirm()
        {
            m_ValueSaved = true;
            m_RadialBase.TransitionToPreviousLayer();
        }

        /// <summary>
        /// Event call to cancel the layer and transition back up the breadcrumb
        /// </summary>
        public override void Cancel()
        {
            m_RadialBase.TransitionToPreviousLayer();
        }

		/// <summary>
        /// Skips the current layer transition
        /// </summary>
        public override void Skip()
        {
            if (m_CurrentReadyUp != 0.0f)
            {
                m_CurrentReadyUp = m_ReadyUpTime * 2.0f;
            }
            if (m_CurrentCleanUp != 0.0f)
            {
                m_CurrentCleanUp = m_CleanUpTime * 2.0f;
            }
        }

		/// <summary>
		/// Resizes the image on the cursor
		/// </summary>
		protected void ResizeCursorImage()
		{
			//get the position of the selected element
			Vector2 tempCPosition = new Vector2();
			float tempCAngle = (360.0f - (m_Cursor.transform.localRotation.eulerAngles.z - (m_CursorImage.fillAmount * 180.0f))) * Mathf.Deg2Rad;
			tempCPosition.x = m_RadialBase.m_ScreenSize * Mathf.Sin(tempCAngle);
			tempCPosition.y = m_RadialBase.m_ScreenSize * Mathf.Cos(tempCAngle);
			//set teh image on the cursor to that position
			m_CursorIconImageRect.localPosition = tempCPosition;
			//set the size of the image on the cursaor
			m_CursorIconImageRect.Resize(m_RadialBase.m_ElementMaxSize, m_RefLayer.m_CursorSize);
		}
	}
}