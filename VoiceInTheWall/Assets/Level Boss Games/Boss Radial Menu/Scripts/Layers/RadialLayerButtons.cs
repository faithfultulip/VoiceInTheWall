using UnityEngine;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	[CreateAssetMenu(fileName = "New Radial Layer Button", menuName = "LBG/RadialMenu/Layer/Button", order = 1)]
	public class RadialLayerButtons : RadialLayerElements 
	{
		#region Public Variables

		[Header("Layer Element")]

		/// <summary>
		/// List of elements that are used to create the buttons around the layer - does not include the header
		/// </summary>
		[Tooltip("The elements that will be displayed on the radial menu")]
		public List<RadialMenuObject> m_MenuElements = new List<RadialMenuObject>();


        /// <summary>
		/// array of elements used if a custom list is to be loaded in
		/// </summary>
		[Tooltip("The elements that will be displayed on the radial menu")]
        [HideInInspector]
        public List<RadialMenuObject> m_CustomMenuElements = new List<RadialMenuObject>();

        #endregion
    }
}