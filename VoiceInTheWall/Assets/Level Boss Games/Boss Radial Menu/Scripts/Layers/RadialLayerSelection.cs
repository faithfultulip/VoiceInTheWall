using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	[CreateAssetMenu(fileName = "New Radial Layer Selection", menuName = "LBG/RadialMenu/Layer/Selection", order = 2)]
	public class RadialLayerSelection : RadialLayerElements
	{
		#region Public Variables

		[Header("Selection Elements")]

		/// <summary>
		/// Left object that is used to shift the current index down
		/// </summary>
		[Tooltip("Selection Arrow that is displayed on the left side of the radial menu")]
		public RadialMenuSelectionArrow m_Left;

		/// <summary>
		/// Right object that is used to shift the current index up
		/// </summary>
		[Tooltip("Selection Arrow that is displayed on the right side of the radial menu")]
		public RadialMenuSelectionArrow m_Right;

		[Header("Bottom Buttons (Optional)")]

		/// <summary>
		/// Bottom left button on the layer
		/// </summary>
		[Tooltip("Button that is displayed on the bottom left of the radial menu")]
		public RadialMenuButton m_BottomLeft;

		/// <summary>
		/// Bottom button on the layer
		/// </summary>
		[Tooltip("Button that is displayed at the bottom of the radial menu")]
		public RadialMenuButton m_BottomMiddle;

		/// <summary>
		/// Bottom right button on the layer
		/// </summary>
		[Tooltip("Button that is displayed on the bottom right of the radial menu")]
		public RadialMenuButton m_BottomRight;

        /// <summary>
        /// Set whether the update is called every time the left or right arrow is pressed
        /// </summary>
        [Header("Update Type")]
        [Tooltip("Set whether the value is changed everytime the left or right arrow is pressed")]
        public bool m_UpdateOnSwitch = true;
		#endregion
	}
}