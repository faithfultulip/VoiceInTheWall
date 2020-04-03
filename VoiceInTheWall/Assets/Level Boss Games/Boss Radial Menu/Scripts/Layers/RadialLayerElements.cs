using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	public abstract class RadialLayerElements: RadialLayer
	{
		#region Public Variables

		/// <summary>
		/// List of all the menu elements that belong to this layer
		/// </summary>
		public RadialMenuHeader m_MenuHeader;

		/// <summary>
		/// Should the cursor snap to the closest elemtent
		/// </summary>
		[Tooltip("Does this layer overwrite the snapping setting from the base")]
		public bool				m_ElementSnapOverwrite = false;

		/// <summary>
		/// Should the cursor snap to the closest elemtent
		/// </summary>
		[Tooltip("Should the cursor snap to the closest elemtent")]
		public bool				m_ElementSnap = false;

		#endregion
	}
}