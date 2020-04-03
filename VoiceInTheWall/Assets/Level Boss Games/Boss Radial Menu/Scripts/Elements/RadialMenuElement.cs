using UnityEngine;
using UnityEngine.UI;

namespace LBG.UI.Radial
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(CanvasRenderer))]
	public class RadialMenuElement : MonoBehaviour
	{

		#region Public Variables

		#endregion

		#region Protected Variables

		#endregion

		#region Private Variables
		/// <summary>
		/// Original Scale of the element that is assigned on its construction
		/// </summary>
		Vector3 m_OriginalScale;

		/// <summary>
		/// Transform of the image that is a child of this gameobject
		/// </summary>
		RectTransform m_ImageTransform;

		/// <summary>
		/// The image that is displayed on the menu
		/// </summary>
		Image m_Image;
		#endregion
		
		/// <summary>
		/// Initalizes the element
		/// </summary>
		/// <param name="spirte">The sprite used for the element</param>
		public void Init(Sprite spirte, Color spriteColour)
		{
			//Creates a game object
			GameObject go = new GameObject("Image", typeof(RectTransform));
			//Sets the created gameobjects parent to this
			go.transform.SetParent(transform, false);
			//Add a rect transform
			m_ImageTransform = go.GetComponent<RectTransform>();
			//Position the ancors
			m_ImageTransform.AnchorsToCorners();
			//Add a canvas renderer
			go.AddComponent<CanvasRenderer>();
			//Add an Image component
			m_Image = go.AddComponent<Image>();
			//Preserve the aspect ration of the sprite on the image component
			m_Image.preserveAspect = true;
			//Set the sprite
			m_Image.sprite = spirte;
			//Set the sprites colour
			m_Image.color = spriteColour;
			//Set original Scale to the tansforms current scale
			m_OriginalScale = m_ImageTransform.localScale;
		}

		/// <summary>
		/// Changes the scale of the element
		/// </summary>
		/// <param name="scale">Value of the scale</param>
		public void SetScale(float scale)
		{
			m_ImageTransform.localScale = new Vector3(scale, scale, scale);
		}

		/// <summary>
		/// Resets the scale back to it's original value
		/// </summary>
		public void ResetScale()
		{
			m_ImageTransform.localScale = m_OriginalScale;
		}

		/// <summary>
		/// Update the image that is displayed on the radial menu
		/// </summary>
		/// <param name="sprite">Sprite that is displayed</param>
		/// <param name="colour">Colour of the sprite</param>
		public void UpdateSprite(Sprite sprite, Color colour)
		{
			m_Image.sprite = sprite;
			m_Image.color = colour;
		}
	}
}
