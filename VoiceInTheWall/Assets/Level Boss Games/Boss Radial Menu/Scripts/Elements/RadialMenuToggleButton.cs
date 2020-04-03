using UnityEngine;
using System;

namespace LBG.UI.Radial
{
	[CreateAssetMenu(fileName = "New Radial Toggle Button", menuName = "LBG/RadialMenu/Element/Toggle Button", order = 3)]
	public class RadialMenuToggleButton : RadialMenuButton 
	{
		#region Public Variables


		/// <summary>
		/// The sprite that is associated with the object when the value is false  
		/// </summary>
		[Header("False Image Settings")]

		[Tooltip("The sprite that is associated with the object when the value is false")]
		public Sprite m_FalseElementSprite;

		/// <summary>
		/// Colour of the sprite that is associated with this object when the value is false
		/// </summary>
		[Tooltip("Colour of the sprite that is associated with this object when the value is false")]
		public Color m_FalseSpriteColour = Color.white;

		[Header("Toggle Values")]

		/// <summary>
		/// Value that is displayed on screen if this button is associated with a true value
		/// </summary>
		[Tooltip("If the toggle value is true, this value will be displayed on screen")]
		public string m_TrueValue = "Yes";

		/// <summary>
		/// Value that is displayed on screen if this button is associated with a false value
		/// </summary>
		[Tooltip("If the toggle value is false, this value will be displayed on screen")]
		public string m_FalseValue = "No";
		
		#endregion

		/// <summary>
		/// Deals with all of the interaction - called from the layer
		/// </summary>
		public override Type Interact(RadialBase menu, string layerEvent)
		{
			menu.m_InputManager.ProcessButton(layerEvent, m_ElementEvent);

			return GetType();
		}
	}
}