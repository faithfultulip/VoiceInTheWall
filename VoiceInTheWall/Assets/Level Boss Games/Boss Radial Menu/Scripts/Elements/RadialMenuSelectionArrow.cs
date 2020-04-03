using UnityEngine;
using System;

namespace LBG.UI.Radial
{
	[CreateAssetMenu(fileName = "New Radial Selection Arrow", menuName = "LBG/RadialMenu/Element/Selection Arrow", order = 6)]
	public class RadialMenuSelectionArrow : RadialMenuObject
	{ 
		/// <summary>
		/// Deals with all of the interaction - called from the layer
		/// </summary>
		public override Type Interact(RadialBase menu, string layerEvent)
		{
			return GetType();
		}

	}
}