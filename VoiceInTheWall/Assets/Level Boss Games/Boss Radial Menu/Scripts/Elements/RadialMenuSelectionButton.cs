using UnityEngine;
using System;

namespace LBG.UI.Radial
{
	[CreateAssetMenu(fileName = "New Radial Selection Button", menuName = "LBG/RadialMenu/Element/Selection Button", order = 5)]
	public class RadialMenuSelectionButton : RadialMenuButton
	{
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