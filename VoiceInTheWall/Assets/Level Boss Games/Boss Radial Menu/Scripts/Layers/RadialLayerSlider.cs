using UnityEngine;
using UnityEngine.UI;

namespace LBG.UI.Radial
{
	[CreateAssetMenu(fileName = "New Radial Layer Slider", menuName = "LBG/RadialMenu/Layer/Slider", order = 3)]
	public class RadialLayerSlider : RadialLayer
    {
		#region Public Variables

		[Tooltip("The sprite used for the cursor if one hasn't been taken from another layer")]
		/// <summary>
		/// The sprite used for the cursor if one hasn't been taken from another layer
		/// </summary>
		public Sprite m_CursorSprite;

        /// <summary>
        /// Full arc size of the cursor in degrees
        /// </summary>
        [Tooltip("The size of the cursor element in degrees (full arc)")]
        [Range(0.0f, 360.0f)]
        public float m_CursorSize = 35.0f;

        /// <summary>
        /// Sprite used as the fill image for the slider
        /// </summary>
        [Tooltip("The fill sprite of the slider")]
        public Sprite m_FillImage;

        /// <summary>
        /// Tint colour of the fill image
        /// </summary>
        [Tooltip("The fill colour of the slider")]
        public Color m_FillColour = Color.white;

        /// <summary>
        /// Sprite used as the background image of the slider
        /// </summary>
        [Tooltip("The background sprite of the slider")]
        public Sprite m_BackgroundImage;

        /// <summary>
        /// Tint colour of the background image
        /// </summary>
        [Tooltip("The background colour of the slider")]
        public Color m_BackgroundColour = Color.white;

        /// <summary>
        /// Minimum value that can be output from the layer
        /// </summary>
        [Tooltip("The min value the layer will produce")]
        public float m_LayerMin = 0;

        /// <summary>
        /// Maximum value that can be output from the layer
        /// </summary>
        [Tooltip("The max value the layer will produce")]
        public float m_LayerMax = 100;

        /// <summary>
        /// Amount of decimal points the displayed value will be rounded to
        /// </summary>
        [Tooltip("What decimal point should the slider value display as on the detail text")]
        public int m_DisplayedTextDecimalPoint = 0;

        #endregion
    }
}