using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	public enum ControlMethod
	{
		Touch,
		Joystick,
		Mouse
	};

	[RequireComponent(typeof(RectTransform))]
	public class RadialBase : MonoBehaviour
	{
		#region public variables

		/// <summary>
		/// linked input manager
		/// </summary>
		[Tooltip("Input manager component that is associated with this menu")]
		public RadialMenuInputManager m_InputManager;

		/// <summary>
		/// Name of the axis used for horizontal input
		/// </summary>
		[Tooltip("Horizontal axis of a joystick used for the radial menu")]
		public string m_InputHorizontal;
		/// <summary>
		/// Name of the axis used for vertical input
		/// </summary>
		[Tooltip("Virtical axis of a joystick used for the radial menu")]
		public string m_InputVertical;
		/// <summary>
		/// Name of the axis used for confirming
		/// </summary>
		[Tooltip("Confirm button of a joystick used for the radial menu")]
		public string m_InputJoystickConfirm;
		/// <summary>
		/// Name of the axis used for cancelling
		/// </summary>
		[Tooltip("Cancel button of a joystick used for the radial menu")]
		public string m_InputJoystickCancel;

		/// <summary>
		/// True if all input methods are allowed
		/// </summary>
		[Tooltip("Should this radial menu react to touch, mouse and joystick inputs")]
		public bool m_AllInputEnabled = true;
		/// <summary>
		/// True if touch input is allowed
		/// </summary>
		[Tooltip("Should this radial menu react to touch input")]
		public bool m_TouchInputEnabled;
		/// <summary>
		/// True if joystick input is enabled
		/// </summary>
		[Tooltip("Should this radial menu react to joystick input")]
		public bool m_JoystickInputEnabled;
		/// <summary>
		/// Ture if mouse input is enabled
		/// </summary>
		[Tooltip("Should this radial menu react to mouse input")]
		public bool m_MouseInputEnabled;

		/// <summary>
		/// True if the cancel button should be shown when touch input is being received
		/// </summary>
		[Tooltip("Should a cancel button be shown for touch input (if not ensure that there is a back button on the element layers)")]
		public bool m_ShowCancelButton = false;

		/// <summary>
		/// the default time for the ready up transition if the layer doesn't overwrite
		/// </summary>
		[Tooltip("How long do all the layers take to transition on to the screen by default")]
		public float m_DefaultReadyUpTime = 1.0f;
		/// <summary>
		/// the default time for the clean up transition if the layer doesn't overwrite
		/// </summary>
		[Tooltip("How long do all the layers take to transition off the screen by default")]
		public float m_DefaultCleanUpTime = 1.0f;

		/// <summary>
		/// The index of the layer the menu should start with
		/// </summary>
		[Tooltip("Which layer should the radial menu load on start")]
		public int m_StartLayer;
		/// <summary>
		/// List of all the layers in the menu
		/// </summary>
		[Tooltip("List off all layers")]
		public List<RadialLayer> m_Layers = new List<RadialLayer>();

		/// <summary>
		/// Maximum size that the elements will grow to when being pointed at
		/// </summary>
		[Tooltip("How big will elements scale when selected")]
		public float m_ElementGrowthFactor = 1.5f;

		/// <summary>
		/// Maximum size the element can be sized to as a percentage of the menu size
		/// </summary>
		[Range(0.0f, 1.0f)]
		[Tooltip("Maximum size the element can be sized to as a percentage of the menu size")]
		public float m_ElementMaxSize = 0.1f;

		/// <summary>
		/// Radius of the circle that the elements are on as a percentage of the panel size
		/// </summary>
		[Range(0.0f, 1.0f)]
		[Tooltip("Radius of the circle that the elements are on as a percentage of the panel size")]
		public float m_PanelOffset = 0.85f;

		/// <summary>
		/// How far outside of the radial ring will input be selected
		/// </summary>
		[Tooltip("How far outside of the radial ring will input be recognised (0.1f is a good value)")]
		public float m_InputLeeway = 0.0f;

		/// <summary>
		/// Threshold where input will be ignored if below
		/// </summary>
		[Range(0.0f, 1.0f)]
		[Tooltip("Joystick axis deadzone")]
		public float m_InputDeadzone = 0.2f;

		/// <summary>
		/// Does all of the layers have element snapping
		/// </summary>
		[Tooltip("Does all of the layers have element snapping")]
		public bool m_ElementSnapping = false;

		/// <summary>
		/// The smallest of the height and width of the panel the menu is on
		/// </summary>
		public float m_ScreenSize
		{
			get
			{
				return Mathf.Min(PanelTransform.rect.width, PanelTransform.rect.height) * m_PanelOffset / 2.0f;
			}
			private set { m_ScreenSize = value; }
		}

		/// <summary>
		/// True if the layer name should be displayed in teh middle of the menu
		/// </summary>
		[Tooltip("Should the layer name be displayed on the details panel")]
		public bool m_ShowLayerName = true;

		/// <summary>
		/// True if the Detail text about the selected element should be displayed in teh middle of the menu
		/// </summary>
		[Tooltip("Should the element information be displayed on the details panel")]
		public bool m_ShowDetailText = true;

		/// <summary>
		/// True if the selected element sprite should be displayed in the middle of the menu
		/// </summary>
		[Tooltip("Should the element sprite be displayed on the details panel")]
		public bool m_ShowSelectedElementSprite = true;

		/// <summary>
		/// States how the touch operation works. 
		/// If true the user has to drag to the selected element and then touch any where on the menu. 
		/// Else they can simply touch where they want to go.
		/// </summary>
		[Tooltip("Does the user have to drag to an element and then click or can they click simply click on an element to proceed")]
		public bool m_DragAndTouch = true;

		/// <summary>
		/// Delay before a touch is recognised as a drag
		/// </summary>
		[Tooltip("Delay before a touch is recognised as a drag")]
		public float m_TouchTime = 0.2f;

		#endregion

		#region private variables

		/// <summary>
		/// Dictionary of all the layers and their indexes
		/// </summary>
		private Dictionary<string, int> m_LayerDictionary = new Dictionary<string, int>();
		/// <summary>
		/// The transfrm of the menu itself
		/// </summary>
		private RectTransform m_Transform;
		/// <summary>
		/// The Object that will hold all the elements
		/// </summary>
		private Transform m_ElementsParent;
		/// <summary>
		/// Angle that is being pointed at by the input
		/// </summary>
		private float m_CursorTargetAngle;
		/// <summary>
		/// True if either the transition in or out are in progress
		/// </summary>
		private bool m_TransitioningOut, m_TransitioningIn;
		/// <summary>
		/// True if the menuis on a world space canvas
		/// </summary>
		private bool m_WorldSpace;

		/// <summary>
		/// Index of the current layer
		/// </summary>
		private int m_CurrentLayer;
		/// <summary>
		/// Index of the next layer
		/// </summary>
		private int? m_NextLayer;

		/// <summary>
		/// The breadrumb of all the transitions to get to the current point
		/// </summary>
		private string m_BreadCrumb = "";
		/// <summary>
		/// Text component to display the layer name in the middle of the menu
		/// </summary>
		private Text m_LayerNameText;
		/// <summary>
		/// Image component to display the image of selected element in the middle of the menu
		/// </summary>
		private Image m_ElementSprite;
		/// <summary>
		/// Text component to display the name of the selected element in the middle of the menu
		/// </summary>
		private Text m_DetailText;
		/// <summary>
		/// Button that is used for touch controls to transition back to the last layer
		/// </summary>
		private Button m_CancelButton;
		/// <summary>
		/// the control method that is in use
		/// </summary>
		private ControlMethod? m_CurrentControl;
		/// <summary>
		/// Observer variable that keeps a record of the mouse position in the last frame
		/// </summary>
		private Vector2 m_OldMousePosition;
		/// <summary>
		/// Length of the current touch
		/// </summary>
		private float m_TouchTimer = 0.0f;

		/// <summary>
		/// The resolution of the screen
		/// </summary>
		private Vector2 m_Resolution;

		/// <summary>
		/// Has the init function finished
		/// </summary>
		private bool m_InitFinished = false;

		/// <summary>
		/// Name given to a created input manager
		/// </summary>
		[SerializeField]
		private string m_InputManagerName;

		/// <summary>
		/// The button handler for all button layers
		/// </summary>
		private RadialLayerButtonsHandler m_ButtonHandler;
		/// <summary>
		/// The slider handler for all slider layers
		/// </summary>
		private RadialLayerSliderHandler m_SliderHandler;
		/// <summary>
		/// The selection handler for all selection layers
		/// </summary>
		private RadialLayerSelectionHandler m_SelectionHandler;
		/// <summary>
		/// The current active layer
		/// </summary>
		private RadialLayerHandler m_ActiveLayer;
		/// <summary>
		/// The canvas that this radial menu belongs to
		/// </summary>
		private Canvas m_MainCanvas;
        //List of Custom Elements used for the start layer
        List<RadialMenuObject> m_CustomElements;

        #endregion

        #region Public Accessors

        /// <summary>
        /// Get the reference to the elements parent object
        /// </summary>
        public Transform ElementsParent
		{
			get
			{
				if (m_ElementsParent == null)
				{
					m_ElementsParent = PanelTransform.transform.Find("ElementsPanel");
				}
				return m_ElementsParent;
			}
			set { m_ElementsParent = value; }
		}

		/// <summary>
		/// Get the reference to the panel the menu is on
		/// </summary>
		public RectTransform PanelTransform
		{
			get
			{
				if (m_Transform == null)
				{
					m_Transform = transform.Find("RadialPanel").GetComponent<RectTransform>();
				}
				return m_Transform;
			}
			set { m_Transform = value; }
		}

		/// <summary>
		/// Get the component to display the layer name
		/// </summary>
		public Text LayerNameText
		{
			get
			{
				if (m_LayerNameText == null)
				{
					m_LayerNameText = PanelTransform.Find("ElementTextPanel").Find("ElementTextLayout").Find("LayerNameText").GetComponent<Text>();
				}
				return m_LayerNameText;
			}
			set { m_LayerNameText = value; }
		}

		/// <summary>
		/// Get the component to display the selected element sprite
		/// </summary>
		public Image ElementSprite
		{
			get
			{
				if (m_ElementSprite == null)
				{
					m_ElementSprite = PanelTransform.Find("ElementTextPanel").Find("ElementTextLayout").Find("ElementSpritePanel").Find("ElementSpriteImage").GetComponent<Image>();
				}
				return m_ElementSprite;
			}
			set { m_ElementSprite = value; }
		}

		/// <summary>
		/// Get the component to display the selecte element name
		/// </summary>
		public Text DetailText
		{
			get
			{
				if (m_DetailText == null)
				{
					m_DetailText = PanelTransform.Find("ElementTextPanel").Find("ElementTextLayout").Find("DetailInfoText").GetComponent<Text>();
				}
				return m_DetailText;
			}
			set { m_DetailText = value; }
		}

		/// <summary>
		/// Get a reference to the cancel button
		/// </summary>
		public Button CancelButton
		{
			get
			{
				if (m_CancelButton == null)
				{
					m_CancelButton = transform.Find("CancelButton").GetComponent<Button>();
				}
				return m_CancelButton;
			}
			set { m_CancelButton = value; }
		}

		/// <summary>
		/// Allows the layer scripts to assign themselves to the active layer slot
		/// </summary>
		public RadialLayerHandler ActiveLayer
		{
			set { m_ActiveLayer = value; }
		}

		public Vector2 ScreenScale
		{
			get
			{
				CanvasScaler canvasScaler = Scaler;
				if ((byte)canvasScaler.uiScaleMode == 1)
				{
					return new Vector2(canvasScaler.referenceResolution.x / Screen.width, canvasScaler.referenceResolution.y / Screen.height);
				}
				else
				{
					return Vector2.one;
				}
			}
		}

		public CanvasScaler Scaler
		{
			get
			{
				if (m_MainCanvas == null)
				{
					GetCanvas(transform.parent);
				}
				return m_MainCanvas.GetComponent<CanvasScaler>();
			}
		}

		#endregion

		/// <summary>
		/// Used for initialisation
		/// </summary>
		void Awake()
		{
			//Get the canvas
			GetCanvas(transform.parent);

			//initialise all the relevant values
			if (m_MainCanvas != null)
			{
				m_WorldSpace = m_MainCanvas.renderMode == RenderMode.WorldSpace;
			}
			else
			{
				Debug.LogError("No canvas has been found. Can not check if this menu is world space");
			}

			//Create the layer dictionary
			m_LayerDictionary = new Dictionary<string, int>();

			int layerCount = m_Layers.Count;

			//Add each of the layers to the dictionary
			for (int i = 0; i < layerCount; i++)
			{
				if (m_Layers[i] != null)
				{
					if (!m_LayerDictionary.ContainsKey(m_Layers[i].m_LayerEvent))
					{
						m_LayerDictionary.Add(m_Layers[i].m_LayerEvent, i);
					}
					else
					{
						Debug.LogError("Radial menu layer name " + m_Layers[i].m_LayerEvent + " appears more than once. Second appearence is at index " + i);
					}
				}
				else
				{
					Debug.LogError("Layer - " + i + " is an empty layer and can't be added");
				}
			}

			m_ButtonHandler = new RadialLayerButtonsHandler();
			m_SliderHandler = new RadialLayerSliderHandler();
			m_SelectionHandler = new RadialLayerSelectionHandler();
		}

		/// <summary>
		/// When the radial menu has been enabled run the init function
		/// </summary>
		void OnEnable()
		{
			m_InitFinished = false;
			StartCoroutine(Init());
		}

		/// <summary>
		/// If the radial menu has been disabled clean up the elements
		/// </summary>
		void OnDisable()
		{
			DestroyAllElements();
		}

		/// <summary>
		/// Ensures that the element canvas has turned on and then transitions to the first layer
		/// </summary>
		/// <returns></returns>
		IEnumerator Init()
		{
			yield return new WaitForEndOfFrame();

			bool layerError = false;

			m_BreadCrumb = "";

			//show or hide the parts of the details panel depending on the corresponding booleans
			LayerNameText.gameObject.SetActive(m_ShowLayerName);
			DetailText.gameObject.SetActive(m_ShowDetailText);
			ElementSprite.gameObject.transform.parent.gameObject.SetActive(m_ShowSelectedElementSprite);

			m_CursorTargetAngle = 0.0f;

			//Transition into the start layer
			m_CurrentLayer = m_StartLayer;

			//Init the input manager
			if (m_InputManager == null)
			{
				Debug.LogError("There is no input manager");
				yield return null;
			}
			else
			{
				m_InputManager.Init(this);

				m_TransitioningOut = m_TransitioningIn = false;
				m_ElementsParent = transform.Find("RadialPanel").Find("ElementsPanel");

				//Checks to see if a shuttle exists 
				GameObject shuttleGO = GameObject.Find("RadialShuttle");

				//If one has been found
				if (shuttleGO != null)
				{
					RadialShuttle shuttle = shuttleGO.GetComponent<RadialShuttle>();
					//Create the breadcrumb path to the new layer
					GenerateBreadcrumb(shuttle.m_LayerPath);
					//Get the layer index
					if (m_LayerDictionary.TryGetValue(shuttle.m_LayerEvent, out m_CurrentLayer))
					{
						//Try to get a slider value and if one exists load a slider layer
						if (shuttle.m_SliderValue != null)
						{
							m_SliderHandler.Init(this, m_Layers[m_CurrentLayer], shuttle.m_SliderValue.GetValueOrDefault());
						}

						//Try to get a selection value and if one exists load a selection layer
						else if (shuttle.m_SectionCurrentIndex != null)
						{
							m_SelectionHandler.Init(this, m_Layers[m_CurrentLayer], shuttle.m_SectionCurrentIndex.GetValueOrDefault(), shuttle.m_SelectionValues);
						}
						else
						{
							m_ButtonHandler.Init(this, m_Layers[m_CurrentLayer], shuttle.m_CustomElements);
						}
					}
					else
					{
						Debug.LogError("Layer from the shuttle does not exist - layer event: " + shuttle.m_LayerEvent);
					}

					Destroy(shuttleGO.gameObject);
				}
				else
				{
					if (m_Layers[m_CurrentLayer] != null)
					{
						if (m_Layers[m_CurrentLayer].GetType() == typeof(RadialLayerButtons))
						{
							m_ButtonHandler.Init(this, m_Layers[m_CurrentLayer], m_CustomElements);
						}
						else
						{
							Debug.LogError("Start layer needs to be a button layer");
							layerError = true;
						}
					}
					else
					{
						Debug.LogError("Trying to load an empty layer in layer slot " + m_CurrentLayer);
						layerError = true;
					}
				}

				if (!layerError)
				{
					StartCoroutine(C_TransitionIn());

					//Make sure the breadcrumb knows where we are
					UpdateBreadCrumb(m_Layers[m_CurrentLayer].m_LayerEvent);

					//Set up the cancel button
					CancelButton.onClick.AddListener(ButtonCancel);

					//if touch is enabed, supported and the cancel button is allowed
					if ((m_TouchInputEnabled || m_AllInputEnabled) && Input.touchSupported && m_ShowCancelButton)
					{
						//Set the cancel button as visible
						CancelButton.gameObject.SetActive(true);
					}
					else
					{
						//Hide the cancel button
						CancelButton.gameObject.SetActive(false);
					}

					m_Resolution = PanelTransform.rect.size;

					m_InitFinished = true;
				}
			}
            m_CustomElements = null;
		}

		/// <summary>
		/// Main update for the menu
		/// </summary>
		void Update()
		{
			if (m_InitFinished)
			{
				if (PanelTransform.rect.size == m_Resolution)
				{
					m_CurrentControl = null;

					#region Touch input
					//if touch is enabled, supported and no control method has been set
					if (((m_TouchInputEnabled || m_AllInputEnabled) && Input.touchSupported) && m_CurrentControl == null)
					{
						//if there is a touch
						if (Input.touchCount > 0)
						{
							//set the control method as touch
							m_CurrentControl = ControlMethod.Touch;

							//if the cancel button is hidden then show it
							if (!CancelButton.gameObject.activeSelf && !Input.touchSupported && m_ShowCancelButton)
							{
								CancelButton.gameObject.SetActive(true);
							}

							//Create a pointer and raycast it into the scene from the touch point
							PointerEventData pointer = new PointerEventData(EventSystem.current);
							pointer.position = Input.GetTouch(0).position;
							var results = new List<RaycastResult>();
							EventSystem.current.RaycastAll(pointer, results);

							//if it hit something
							if (results.Count > 0)
							{
								//cast the hit results to the radial base
								RadialBase[] tempRadials = results[0].gameObject.transform.root.GetComponentsInChildren<RadialBase>();
								//if one was able to cast
								if (tempRadials.Length > 0)
								{
									//Search through all menus in the canvas and ensure that the correct menu is picking up the information
									foreach (RadialBase radial in tempRadials)
									{
										if (radial == this)
										{
											//Set up the coordinate of the touch
											Vector2 UICoord = Vector2.zero;
											//if in world space
											if (m_WorldSpace)
											{
												//get the direction of the raycast and the world position of the touch
												//use those inputs to get the local position on the world space canvas that was hit
												UICoord = PanelTransform.ViewportToWorldSpaceUI(Camera.main.ScreenPointToRay(Input.GetTouch(0).position).origin, Camera.main.ScreenPointToRay(Input.GetTouch(0).position).direction.normalized);
											}
											else
											{
												//translate the hit position by half the screen height and wdth so that the center is the origin
												UICoord = (Vector2)Input.GetTouch(0).position - (Vector2)PanelTransform.position;
											}

											//if the touch is closer to the center of the menu than its radius
											//the radius of the menu is adjusted to compensate for different screen scalings
											float check = 1;
											if ((byte)Scaler.uiScaleMode == 1)
											{
												check = (Mathf.Min(PanelTransform.rect.width, PanelTransform.rect.height) / Mathf.Lerp(ScreenScale.x, ScreenScale.y, Scaler.matchWidthOrHeight)) * (((m_PanelOffset + m_ElementMaxSize) / 2.0f) + m_InputLeeway);
											}
											else if ((byte)Scaler.uiScaleMode == 2)
											{
												check = Mathf.Min(PanelTransform.rect.width, PanelTransform.rect.height) * (((m_PanelOffset + m_ElementMaxSize) / 2.0f) + (m_InputLeeway * 2.0f));
											}
											else
											{
												check = Mathf.Min(PanelTransform.rect.width, PanelTransform.rect.height) * (((m_PanelOffset + m_ElementMaxSize) / 2.0f) + m_InputLeeway);
											}
											if (new Vector2(UICoord.x, UICoord.y).magnitude < check)
											{
												if (!m_TransitioningOut && !m_TransitioningIn)
												{
													//Save the mouse position to the observer variable
													m_OldMousePosition = Input.mousePosition;

													//get the normalised form of the touch coordinates
													Vector2 UIDir = UICoord.normalized;
													//Work out the clockwise angle from up to the UIDir
													float tempTargetAngle = Mathf.Atan2(UIDir.x, UIDir.y) * Mathf.Rad2Deg;
													//if the Direction is toward the left of the screen the angleneeds flipping
													if (UICoord.x < 0.0f)
													{
														tempTargetAngle = 360.0f - Mathf.Abs(tempTargetAngle);
													}

													bool angleSet = false;
													//Check how to see how the touch should operate

													if (m_Layers[m_CurrentLayer].m_OverwriteTouchOpertation)
													{
														if (!m_Layers[m_CurrentLayer].m_DragAndTouch)
														{
															//apply the angle to the targetAngle
															m_CursorTargetAngle = tempTargetAngle;
															angleSet = true;
														}
													}
													else
													{
														if (!m_DragAndTouch)
														{
															m_CursorTargetAngle = tempTargetAngle;
															angleSet = true;
														}
													}


													//increment the touch timer by the time since the last frame
													m_TouchTimer += Time.deltaTime;

													//if the touch timer is above the threshold
													if (m_TouchTimer >= m_TouchTime)
													{
														if (!angleSet)
														{
															m_CursorTargetAngle = tempTargetAngle;
														}

														//update the layer
														m_ActiveLayer.UpdateLayer(m_CursorTargetAngle, ControlMethod.Touch);
													}
													else
													{
														//if the touch let go this frame
														if (Input.GetMouseButtonUp(0))
														{
															//reset the touch timer
															m_TouchTimer = 0;
															//Update the layer
															m_ActiveLayer.UpdateLayer(m_CursorTargetAngle, ControlMethod.Touch);
															//Confirm the layer
															m_ActiveLayer.Confirm();
														}
													}
												}
												else if ((Input.GetMouseButtonUp(0)) || (Input.GetMouseButtonUp(1)))
												{
													m_ActiveLayer.Skip();
												}
											}
											else
											{
												//reset the touch timer
												m_TouchTimer = 0;
											}
										}
									}
								}
							}
						}
						else
						{
							//reset the touch timer
							m_TouchTimer = 0;
						}
					}
					#endregion

					#region Joystick input
					//if joysticks are enabled, connected and no control method has been set
					if (((m_JoystickInputEnabled || m_AllInputEnabled) && Input.GetJoystickNames().Length > 0) && m_CurrentControl == null)
					{
						//if we the menu is not in a transition
						if (!m_TransitioningOut && !m_TransitioningIn)
						{
							//if the input is above the deadzone threshold in either direction
							if (Mathf.Abs(Input.GetAxis(m_InputHorizontal)) > m_InputDeadzone || Mathf.Abs(Input.GetAxis(m_InputVertical)) > m_InputDeadzone)
							{
								//set the control method to joystick
								m_CurrentControl = ControlMethod.Joystick;

								//get the normalised input from the joystick axis
								Vector2 tempInput = new Vector2(Input.GetAxis(m_InputHorizontal), Input.GetAxis(m_InputVertical)).normalized;
								//work out the clockwise angle from up to the target direction
								float tempTargetAngle = Mathf.Atan2(tempInput.x, tempInput.y) * Mathf.Rad2Deg;
								//if the target direction is to the left
								if (tempInput.x < 0.0f)
								{
									//the angle needs reversing
									tempTargetAngle = 360.0f - Mathf.Abs(tempTargetAngle);
								}
								//apply the angle to target angle
								m_CursorTargetAngle = tempTargetAngle;
								//update the layer
								m_ActiveLayer.UpdateLayer(m_CursorTargetAngle, ControlMethod.Joystick);
							}

							//if confirm is pressed
							if (Input.GetButtonDown(m_InputJoystickConfirm))
							{
								//set the control method to joystick
								m_CurrentControl = ControlMethod.Joystick;
								//confirm the layer
								m_ActiveLayer.Confirm();
							}

							//if cancel is pressed
							if (Input.GetButtonDown(m_InputJoystickCancel))
							{
								//set the control method to joystick
								m_CurrentControl = ControlMethod.Joystick;
								//cancel the layer
								m_ActiveLayer.Cancel();
							}
						}
						else if ((Input.GetButtonDown(m_InputJoystickConfirm)) || (Input.GetButtonDown(m_InputJoystickCancel)))
						{
							m_ActiveLayer.Skip();
						}

						//if the touch cancel button is visible and the current control method is not touch 
						if (CancelButton.gameObject.activeSelf && (!Input.touchSupported || m_CurrentControl.GetValueOrDefault() != ControlMethod.Touch))
						{
							//hide it
							CancelButton.gameObject.SetActive(false);
						}
					}
					#endregion

					#region Mouse input
					//if mouse is enabled and no control method has been set
					if ((m_MouseInputEnabled || m_AllInputEnabled) && m_CurrentControl == null)
					{
						//set the control method to mouse
						m_CurrentControl = ControlMethod.Mouse;

						//if the cancel button is visible
						if (CancelButton.gameObject.activeSelf && !Input.touchSupported)
						{
							//hide it
							CancelButton.gameObject.SetActive(false);
						}

						//create a raycast from the mouse position into the scene and store the results in a list
						PointerEventData pointer = new PointerEventData(EventSystem.current);
						pointer.position = Input.mousePosition;
						var results = new List<RaycastResult>();
						EventSystem.current.RaycastAll(pointer, results);

						//if something was hit
						if (results.Count > 0)
						{
							//cast the list to RadialBase
							RadialBase[] tempRadials = results[0].gameObject.transform.root.GetComponentsInChildren<RadialBase>();
							//if there is a radial base in the list
							if (tempRadials.Length > 0)
							{
								//Search through all menus in the canvas and ensure that the correct menu is picking up the information
								foreach (RadialBase radial in tempRadials)
								{
									if (radial == this)
									{
										//Set up the UICoord variable
										Vector2 UICoord = Vector2.zero;
										//if the canvas the menu is on is in world space
										if (m_WorldSpace)
										{
											//Get the mouse position in world space and the direction it is pointing into the scene
											//Use this to get the local position of the mouse on the canvas
											UICoord = PanelTransform.ViewportToWorldSpaceUI(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized);
										}
										else
										{
											//translate the mouse position by half the width and height of the sceen to put the origin in the center of the screen
											UICoord = (Vector2)Input.mousePosition - (Vector2)(PanelTransform.position);
										}

										//if the magnitude of the UICoord is less than the radius of the menu
										//the radius of the menu is adjusted to compensate for different screen scalings
										float check = 1;
										if ((byte)Scaler.uiScaleMode == 1)
										{
											check = (Mathf.Min(PanelTransform.rect.width, PanelTransform.rect.height) / Mathf.Lerp(ScreenScale.x, ScreenScale.y, Scaler.matchWidthOrHeight)) * (((m_PanelOffset + m_ElementMaxSize) / 2.0f) + m_InputLeeway);
										}
										else if ((byte)Scaler.uiScaleMode == 2)
										{
											check = Mathf.Min(PanelTransform.rect.width, PanelTransform.rect.height) * (((m_PanelOffset + m_ElementMaxSize) / 2.0f) + (m_InputLeeway * 2.0f));
										}
										else
										{
											check = Mathf.Min(PanelTransform.rect.width, PanelTransform.rect.height) * (((m_PanelOffset + m_ElementMaxSize) / 2.0f) + m_InputLeeway);
										}
										if (new Vector2(UICoord.x, UICoord.y).magnitude < check)
										{
											//if we the menu is not in a transition
											if (!m_TransitioningOut && !m_TransitioningIn)
											{
												if (m_OldMousePosition != (Vector2)Input.mousePosition)
												{
													//update the observer variable with the mouse position
													m_OldMousePosition = Input.mousePosition;
													//normalise the UICoord vector
													Vector2 UIDir = UICoord.normalized;
													//Work out the angle between up and the target vector
													float tempTargetAngle = Mathf.Atan2(UIDir.x, UIDir.y) * Mathf.Rad2Deg;
													//if the UICoord was pointing to the left then the angle needs to be flipped
													if (UICoord.x < 0.0f)
													{
														tempTargetAngle = 360.0f - Mathf.Abs(tempTargetAngle);
													}
													//apply the angle to target angle
													m_CursorTargetAngle = tempTargetAngle;
													//update the layer
													m_ActiveLayer.UpdateLayer(m_CursorTargetAngle, ControlMethod.Mouse);
												}

												//if the left mouse button is pressed
												if (Input.GetMouseButtonUp(0))
												{
													//confirm the layer
													m_ActiveLayer.Confirm();
												}

												//if the right mouse button is pressed
												if (Input.GetMouseButtonUp(1))
												{
													//cancel the layer
													m_ActiveLayer.Cancel();
												}
											}
											else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
											{
												m_ActiveLayer.Skip();
											}
										}
									}
								}
							}
						}
					}
					#endregion
				}
				else
				{
                    //Remove all the elements thats are currently on the layer
                    DestroyAllElements();
                    //Redraws all the elements at the correct position and size
                    m_ActiveLayer.Redraw(m_CursorTargetAngle, ControlMethod.Mouse);
                    //update the observer variable
                    m_Resolution = PanelTransform.rect.size;
                }
			}
		}

		/// <summary>
		/// Destroys all the current elements for the layer
		/// </summary>
		void DestroyAllElements()
		{
			int childCount = ElementsParent.childCount;

			for (int i = 0; i < childCount; i++)
			{
				Destroy(ElementsParent.GetChild(i).gameObject);
			}
		}

		#region Transition To Layer
		/// <summary>
		/// Transition to a new layer
		/// </summary>
		/// <param name="layer">Layer name </param>
		public void TransitionToLayer(string layer, List<RadialMenuObject> customElements = null)
		{
			if (layer != "")
			{
				int tempLayer = -1;
				if (m_LayerDictionary.TryGetValue(layer, out tempLayer))
				{
					m_NextLayer = tempLayer;
					StartCoroutine(C_TransitionOut(customElements));
					UpdateBreadCrumb(layer);
				}
				else
				{

					Debug.LogError("No radial menu layer by the name of " + layer);
				}
			}
			else
			{
				Debug.LogError("No layer name by the name of " + layer);
			}
		}

		/// <summary>
		/// Transition to a slider layer
		/// </summary>
		/// <param name="layer">Layer name</param>
		/// <param name="sliderValue">Slider value</param>
		public void TransitionToLayer(string layer, float sliderValue)
		{
			if (layer != "")
			{
				int tempLayer = -1;
				if (m_LayerDictionary.TryGetValue(layer, out tempLayer))
				{
					m_NextLayer = tempLayer;
					StartCoroutine(C_TransitionOut(sliderValue));
					UpdateBreadCrumb(layer);
				}
				else
				{

					Debug.LogError("No radial menu layer by the name of " + layer);
				}
			}
			else
			{
				Debug.LogError("No layer name by the name of " + layer);
			}
		}

		/// <summary>
		/// Tranisiton to a selection layer
		/// </summary>
		/// <param name="layer">Layer name</param>
		/// <param name="currentIndex">Current index of the array values</param>
		/// <param name="selectionValues">The values that will be used in the menu</param>
		public void TransitionToLayer(string layer, int currentIndex, string[] selectionValues)
		{
			if (layer != "")
			{
				int tempLayer = -1;
				if (m_LayerDictionary.TryGetValue(layer, out tempLayer))
				{
					m_NextLayer = tempLayer;
					StartCoroutine(C_TransitionOut(currentIndex, selectionValues));
					UpdateBreadCrumb(layer);
				}
				else
				{

					Debug.LogError("No radial menu layer by the name of " + layer);
				}
			}
			else
			{
				Debug.LogError("No layer name by the name of " + layer);
			}
		}

		#endregion

		#region Skip To Layer

		/// <summary>
		/// Skips to a given element layer and sets a breadcrumb path to return
		/// </summary>
		/// <param name="layer">The layer event that correspondence to the target layer</param>
		/// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
		public void SkipToLayer(string layer, List<RadialMenuObject> customElements = null, params string[] layerPath)
		{
			GenerateBreadcrumb(layerPath);
			TransitionToLayer(layer, customElements);
		}

		/// <summary>
		/// Skips to a given slider layer and sets a breadcrumb path to return
		/// </summary>
		/// <param name="layer">The layer event that correspondence to the target layer</param>
		/// <param name="sliderValue">Value of the slider that is will be loaded</param>
		/// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
		public void SkipToLayer(string layer, float sliderValue, params string[] layerPath)
		{
			GenerateBreadcrumb(layerPath);
			TransitionToLayer(layer, sliderValue);
		}

		/// <summary>
		/// Skips to a given selection layer and sets a breadcrumb path to return
		/// </summary>
		/// <param name="layer">The layer event that correspondence to the target layer</param>
		/// <param name="currentIndex">Current index of the selection layer that will be loaded</param>
		/// <param name="selectionValues">Values of the selection layer that will be loaded</param>
		/// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
		public void SkipToLayer(string layer, int currentIndex, string[] selectionValues, params string[] layerPath)
		{
			GenerateBreadcrumb(layerPath);
			TransitionToLayer(layer, currentIndex, selectionValues);
		}

		#endregion

		private IEnumerator C_TransitionIn()
		{
			//set the flag for transitioning in
			m_TransitioningIn = true;
			//run the ready up function every frame until it completes
			while (!m_ActiveLayer.ReadyUp())
			{
				yield return new WaitForEndOfFrame();
			}
			//reset the flag
			m_TransitioningIn = false;

		}

		#region Transition out functions

		private IEnumerator C_TransitionOut(List<RadialMenuObject> customElements = null)
		{
			//if there is no next layer
			if (m_NextLayer == null)
			{
				//error and stop
				Debug.LogError("Trying to transition to nothing!");
				StopCoroutine(C_TransitionOut());
			}
			//if the next layer is the current layer
			else if (m_NextLayer.GetValueOrDefault(0) == m_CurrentLayer)
			{
				//error and stop
				Debug.LogWarning("Trying to transition to the same menu");
				StopCoroutine(C_TransitionOut());
			}

			//set the flag for transitioning out
			m_TransitioningOut = true;

			//call the clean up function every fram until it returns true
			while (!m_ActiveLayer.CleanUp())
			{
				yield return new WaitForEndOfFrame();
			}

			//update the current layer to be the same as the next layer
			m_CurrentLayer = m_NextLayer.GetValueOrDefault(0);
			//initialise the new current layer
			m_ButtonHandler.Init(this, m_Layers[m_CurrentLayer], customElements);

			//reset the flag
			m_TransitioningOut = false;
			//start the corutine for transitioning in
			StartCoroutine(C_TransitionIn());
		}

		//same as above but for slider layers
		private IEnumerator C_TransitionOut(float sliderValue)
		{
			if (m_NextLayer == null)
			{
				Debug.LogError("Trying to transition to nothing!");
				StopCoroutine(C_TransitionOut(sliderValue));
			}
			else if (m_NextLayer.GetValueOrDefault(0) == m_CurrentLayer)
			{
				Debug.LogWarning("Trying to transition to the same menu");
				StopCoroutine(C_TransitionOut(sliderValue));
			}

			m_TransitioningOut = true;

			while (!m_ActiveLayer.CleanUp())
			{
				yield return new WaitForEndOfFrame();
			}

			m_CurrentLayer = m_NextLayer.GetValueOrDefault(0);
			m_SliderHandler.Init(this, m_Layers[m_CurrentLayer], sliderValue);

			m_TransitioningOut = false;
			StartCoroutine(C_TransitionIn());
		}

		//same as above but for selection layers
		private IEnumerator C_TransitionOut(int currentIndex, string[] selectionValues)
		{
			if (m_NextLayer == null)
			{
				Debug.LogError("Trying to transition to nothing!");
				StopCoroutine(C_TransitionOut(currentIndex, selectionValues));
			}
			else if (m_NextLayer.GetValueOrDefault(0) == m_CurrentLayer)
			{
				Debug.LogWarning("Trying to transition to the same menu");
				StopCoroutine(C_TransitionOut(currentIndex, selectionValues));
			}

			m_TransitioningOut = true;

			while (!m_ActiveLayer.CleanUp())
			{
				yield return new WaitForEndOfFrame();
			}

			m_CurrentLayer = m_NextLayer.GetValueOrDefault(0);
			m_SelectionHandler.Init(this, m_Layers[m_CurrentLayer], currentIndex, selectionValues);

			m_TransitioningOut = false;
			StartCoroutine(C_TransitionIn());
		}

        #endregion

        #region Radial Shuttle

        /// <summary>
        /// Creates a shuttle that will be used to load to a new layer when a new radial menu get initalised
        /// </summary>
        /// <param name="layer">Layer event of the target layer</param>
        /// <param name="sliderValue">Value of the slider if a slider layer</param>
        /// <param name="selectionCurrentIndex">Current index of the selection layer if target layer is a selection</param>
        /// <param name="customElements">list of custom elements</param>
        /// <param name="selectionValues">Values of the selection layer if target layer is a selection</param>
        /// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
        public void CreateShuttle(string layer, float? sliderValue = null, int? selectionCurrentIndex = null, List<RadialMenuObject> customElements = null, string[] selectionValues = null, params string[] layerPath)
		{
			GameObject shuttleGO = new GameObject();
			shuttleGO.name = "RadialShuttle";
			RadialShuttle shuttle = shuttleGO.AddComponent<RadialShuttle>();
			shuttle.Init(layer, sliderValue.GetValueOrDefault(), selectionCurrentIndex.GetValueOrDefault(), customElements, selectionValues, layerPath);
		}

		#endregion

		private IEnumerator C_TransitionToScene(string sceneName)
		{
			//set the flag for transitioning out
			m_TransitioningOut = true;

			//run the clean up function every fram until it returns true
			while (!m_ActiveLayer.CleanUp())
			{
				yield return new WaitForEndOfFrame();
			}

			//reset the flag
			m_TransitioningOut = false;
			//load the scene
			SceneManager.LoadScene(sceneName);
		}

		private IEnumerator C_TransitionToQuit()
		{
			m_TransitioningOut = true;

			while (!m_ActiveLayer.CleanUp())
			{
				yield return new WaitForEndOfFrame();
			}

			m_TransitioningOut = false;
			Application.Quit();
		}

		/// <summary>
		/// Call to Transition to a different scene from a layer
		/// </summary>
		/// <param name="sceneName">the name of the scene</param>
		public void TransitionToScene(string sceneName)
		{
			StartCoroutine(C_TransitionToScene(sceneName));
		}

		/// <summary>
		/// Call to transition out of the current layer and quit the application
		/// </summary>
		public void TransitionToQuit()
		{
			StartCoroutine(C_TransitionToQuit());
		}

		/// <summary>
		/// Call to transition back up through the breadcrumb one layer
		/// </summary>
		public void TransitionToPreviousLayer()
		{
			string breadcrumb = GetPreviousBreadCrumb();

			if (breadcrumb == "")
			{
				//If you are on the top layer and you press back
				m_InputManager.LastCancel();
			}
			else
			{
				TransitionToLayer(GetPreviousBreadCrumb());
			}
		}

		/// <summary>
		/// Update the breadcrumbs via layer names
		/// </summary>
		void UpdateBreadCrumb(string layerEvent)
		{
			//init the breadcrumb
			string resultBreadCrumb = "";
			//init the index
			int stringIndex = -1;
			//init the list
			List<string> tempBreadCrumbs = new List<string>();

			//makes sure there is a / in the breadcrumb and then splits the string based on the /
			if (m_BreadCrumb.Contains("/"))
			{
				tempBreadCrumbs.AddRange(m_BreadCrumb.Split('/'));
			}

			//Loop through the breadcrumbs and check to make sure that the new layer doesn't already exist
			int breadCrumbCount = tempBreadCrumbs.Count;
			for (int i = 0; i < breadCrumbCount; i++)
			{
				//last index
				if (i == breadCrumbCount - 1)
				{
					//checks to see if the breadcrumb value is the same as the layer name 
					//if it is it sets the string index to i else it adds the layername to the result breadcrumb
					if (tempBreadCrumbs[i] == layerEvent)
					{
						stringIndex = i;
					}
					else
					{
						resultBreadCrumb += layerEvent + "/";
					}
				}
				//others
				else
				{
					if (tempBreadCrumbs[i] == layerEvent)
					{
						stringIndex = i;
					}
					else
					{
						resultBreadCrumb += tempBreadCrumbs[i] + "/";
					}
				}

			}

			//if the string index has been changed
			if (stringIndex != -1)
			{
				//reset the breadcrumb
				resultBreadCrumb = "";

				//Make the breadcrumb = all of the values in the breadcrumbs list before the string index value
				for (int i = 0; i <= stringIndex; i++)
				{
					resultBreadCrumb += tempBreadCrumbs[i] + "/";
				}
			}

			//If the breadcrumb is blank add the layer name
			if (resultBreadCrumb == "")
			{
				resultBreadCrumb = layerEvent + "/";
			}

			//Set the breadcrumb value
			m_BreadCrumb = resultBreadCrumb;

		}

		/// <summary>
		/// Return the previous layers name
		/// </summary>
		string GetPreviousBreadCrumb()
		{
			//init the tembreadcrumb
			string tempBreadCrumb = "";
			//Init the list of breadcrumbs 
			List<string> tempBreadCrumbs = new List<string>();

			//Makes sure the breadcrumb contains a /
			if (m_BreadCrumb.Contains("/"))
			{
				//split the string up and then get the previous breadcrumb
				tempBreadCrumbs.AddRange(m_BreadCrumb.Split('/'));
			}
			if (tempBreadCrumbs.Count >= 3)
			{
				tempBreadCrumb = tempBreadCrumbs[tempBreadCrumbs.Count - 3];
			}

			//return the breadcrumb
			return tempBreadCrumb;
		}

		/// <summary>
		/// Generates a breadcrumb from an array of breadcrumbs
		/// </summary>
		/// <param name="layers">Path to target layer by the layer events in the radial base hierarchy</param>
		void GenerateBreadcrumb(params string[] layers)
		{
			m_BreadCrumb = "";
			int tempLayer;

			int nameLength = layers.Length;
			for (int i = 0; i < nameLength; i++)
			{
				tempLayer = -1;
				if (m_LayerDictionary.TryGetValue(layers[i], out tempLayer))
				{
					UpdateBreadCrumb(m_Layers[tempLayer].m_LayerEvent);
				}
				else
				{
					Debug.LogError("Trying to generate a breadcrumb from a non-existing layer called - " + layers);
				}
			}
		}

		/// <summary>
		/// Returns true if a layer is in the layer dictionary
		/// </summary>
		/// <param name="layerEvent"></param>
		/// <returns>Returns true if a layer is in the layer dictionary</returns>
		public bool CheckForLayer(string layerEvent)
		{
			return m_LayerDictionary.ContainsKey(layerEvent);
		}

		/// <summary>
		/// Used for the GUI cancel button
		/// </summary>
		private void ButtonCancel()
		{
			m_ActiveLayer.Cancel();
		}

		/// <summary>
		/// Finds the canvas that is a parent of this menu
		/// </summary>
		/// <param name="parent">the first parents transform</param>
		void GetCanvas(Transform parent)
		{
			if (parent != null)
			{
				if (parent.GetComponent<Canvas>() != null)
				{
					m_MainCanvas = parent.GetComponent<Canvas>();
				}
				else
				{
					if (parent.parent != null)
					{
						GetCanvas(parent.parent);
					}
					else
					{
						Debug.LogError("No canvas can be found");
					}
				}
			}
			else
			{
				Debug.LogError("Transform does not exist so canvas could not be found");
			}
		}

        public void TurnMenuOn(List<RadialMenuObject> customElements = null)
        {
            m_CustomElements = customElements;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// To be used on an button layer if different elements need to be loaded in
        /// </summary>
        /// <param name="customElements"></param>
        public void ReloadLayer(List<RadialMenuObject> customElements = null)
        {
            Type t = m_ActiveLayer.GetType();
            if (t == (typeof(RadialLayerButtonsHandler)))
            {
                DestroyAllElements();
                (m_ActiveLayer as RadialLayerElementsHandler).Reload(m_CursorTargetAngle, ControlMethod.Mouse, customElements);
            }
            else
            {
                Debug.LogError("The reload method only works on a button layer. trying to use it on - " + t);
            }
        }
	}
}