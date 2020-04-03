using UnityEngine;
using System;

namespace LBG.UI.Radial
{
	[CreateAssetMenu(fileName = "New Radial Header", menuName = "LBG/RadialMenu/Element/Header", order = 1)]
	public class RadialMenuHeader : RadialMenuObject
	{
		/// <summary>
		/// Deals with all of the interaction - called from the layer
		/// </summary>
		public override Type Interact(RadialBase menum, string layerEvent)
		{
			return GetType();
		}
	}
}