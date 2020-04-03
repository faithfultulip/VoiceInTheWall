using UnityEngine;
using System;
namespace LBG.UI.Radial
{
	public abstract class RadialMenuObject : ScriptableObject 
	{

		#region Public Variables

		/// <summary>
		/// The sprite that is associated with the object  
		/// </summary>
		[Header("Button Image")]
		[Tooltip("The sprite that is associated with the object")]
		public Sprite m_ElementSprite;

		/// <summary>
		/// Colour of the sprite that is associated with this object
		/// </summary>
		[Tooltip("Colour of the sprite that is associated with this object")]
		public Color m_SpriteColour = Color.white;

		#endregion

		/// <summary>
		/// Deals with all of the interaction - called from the layer
		/// </summary>
		public abstract Type Interact(RadialBase menu, string layerEvent);

		/// <summary>
		/// Returns the name associated with the object
		/// </summary>
		/// <returns>Returns the name associated with the object</returns>
		public virtual string GetName()
		{
			return "";
		}
	}
}