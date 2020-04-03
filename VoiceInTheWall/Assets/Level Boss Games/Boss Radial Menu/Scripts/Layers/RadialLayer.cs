using UnityEngine;
using UnityEngine.UI;
using System;

namespace LBG.UI.Radial
{
	public abstract class RadialLayer : ScriptableObject
	{
		#region Public Variables

		/// <summary>
		/// The name of the layer that is shown on screen
		/// </summary>
		[Tooltip("Name that will be displayed on screen")]
		public string m_LayerName = "New Layer";
		
		/// <summary>
		/// The layer event name that is used for input
		/// </summary>
		[Tooltip("Event name that is used for the input manager")]
		public string m_LayerEvent = "Layer Event";
		
		/// <summary>
		/// If true, this layer will set it's own ready up and clean up timers
		/// </summary>
		[Tooltip("Should this layer overwite the base transition from the base menu")]
		public bool m_OverwriteTimers = false;

		/// <summary>
		/// If m_OverwriteTimers is true then this is how long the layer will take to load on to the screen
		/// </summary>
		[Tooltip("How long will this layer take to load on to the screen")]
		public float m_OverwriteReadyUpTime = 1.0f;

		/// <summary>
		/// If m_OverwriteTimers is true then this is how long the layer will take to be removed from the screen
		/// </summary>
		[Tooltip("How long will this layer take to remove itself from the screen")]
		public float m_OverwriteCleanUpTime = 1.0f;

		/// <summary>
		/// If true, this layer will decide what to show in the details section of the radial menu else it is left to the radial base
		/// </summary>
		[Tooltip("Should this layer overwite the details panel from the base menu")]
		public bool m_OverwriteDetailPanel = false;

		/// <summary>
		/// Should the layer name be shown in the details section
		/// </summary>
		[Tooltip("Will the layer name be shown on screen")]
		public bool m_ShowLayerName = false;

		/// <summary>
		/// Should the layers detail text be shown in the details section
		/// </summary>
		[Tooltip("Will the layers detail text be shown on screen")]
		public bool m_ShowDetailText = false;

		/// <summary>
		/// Should the layers element sprites be shown in the details section
		/// </summary>
		[Tooltip("Will the layers show the element sprite on screen")]
		public bool m_ShowSelectedElementSprite = false;

		/// <summary>
		/// If true this layers touch operation will be overwritten
		/// </summary>
		[Tooltip("Should this layers touch operation be overwritten")]
		public bool m_OverwriteTouchOpertation = false;

		/// <summary>
		/// States how the touch operation works. 
		/// If true the user has to drag to the selected element and then touch any where on the menu. 
		/// Else they can simply touch where they want to go.
		/// </summary>
		[Tooltip("Does the user have to drag to an element and then click or can they click simply click on an element to proceed")]
		public bool m_DragAndTouch = true;
		#endregion
	}
}
