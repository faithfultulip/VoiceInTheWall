using UnityEngine;
using System;

namespace LBG.UI.Radial
{
	[CreateAssetMenu(fileName = "New Radial Slider Button", menuName = "LBG/RadialMenu/Element/Slider Button", order = 4)]
	public class RadialMenuSliderButton : RadialMenuButton
	{

        /// <summary>
        /// Amount of decimal points the displayed value will be rounded to
        /// </summary>
        [Header("Formatting")]
        [Tooltip("What decimal point should the slider value display as on the detail text")]
        public int m_DisplayedTextDecimalPoint = 0;


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