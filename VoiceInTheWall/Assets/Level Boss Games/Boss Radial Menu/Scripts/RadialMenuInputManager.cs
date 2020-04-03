using UnityEngine;
using System;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	public abstract class RadialMenuInputManager : MonoBehaviour 
	{
		/// <summary>
		/// Base radial menu that this input manager is associated with
		/// </summary>
		RadialBase m_RadialBase;

		/// <summary>
		/// Initalize the input manager
		/// </summary>
		/// <param name="radialBase">The radial base that this is assoiciated with</param>
		public void Init(RadialBase radialBase)
		{
			m_RadialBase = radialBase;
		}

		/// <summary>
		/// Process a button based on that layers event name and the buttons event name
		/// </summary>
		/// <param name="layerEvent">Event name of the layer</param>
		/// <param name="buttonEvent">Event name of the button</param>
		public abstract void ProcessButton(string layerEvent, string buttonEvent);

		/// <summary>
		/// Process a slider based on it's layer event name
		/// </summary>
		/// <param name="layerEvent">Event name of the layer</param>
		/// <param name="value">Value that is returned from the slider</param>
		/// <param name="saveValue">Should the value be saved</param>
		public abstract void ProcessSlider(string layerEvent, float value, bool saveValue);

		/// <summary>
		/// Process a selection layer based on it's layer event name
		/// </summary>
		/// <param name="layerEvent">Event name of the layer</param>
		/// <param name="buttonEvent">Event name of the button</param>
		/// <param name="newIndex">The index that is returned from the selction layer</param>
		/// <param name="originalIndex">The original index that was set on the layers construct</param>
		public abstract void ProcessSelection(string layerEvent, string buttonEvent, int newIndex, int originalIndex);

		/// <summary>
		/// Process to return values for a slider, selection or toggle button
		/// </summary>
		/// <param name="layerEvent">Event name of the layer</param>
		/// <param name="buttonEvent">Event name of the button</param>
		/// <returns>Process to return values for a slider, selection or toggle button</returns>
		public abstract T ProcessReturnValues<T>(string layerEvent, string buttonEvent);

		/// <summary>
		/// What happens when you are on the top layer and cancel is pressed
		/// </summary>
		public abstract void LastCancel();

		#region Change Layer

		/// <summary>
		/// Change to a button layer
		/// </summary>
		/// <param name="layerName">Layers event name</param>
		protected void ChangeLayer(string layerName, List<RadialMenuObject> customElements = null)
		{
			m_RadialBase.TransitionToLayer(layerName, customElements);
		}

		/// <summary>
		/// Change to a slider layer
		/// </summary>
		/// <param name="layerName">Layers event name</param>
		/// <param name="sliderValue">Value passed in to the slider on its construct</param>
		protected void ChangeLayer(string layerName, float sliderValue)
		{
			m_RadialBase.TransitionToLayer(layerName, sliderValue);
		}

		/// <summary>
		/// Change to a selection layer
		/// </summary>
		/// <param name="layerName">Layers event name</param>
		/// <param name="currentIndex">Index that will be selected on the layers construct</param>
		/// <param name="selectionValue">Array of  values that will be displayed on screen</param>
		protected void ChangeLayer(string layerName, int currentIndex, string[] selectionValue)
		{
			m_RadialBase.TransitionToLayer(layerName, currentIndex, selectionValue);
		}


		/// <summary>
		/// Skips to a given element layer and sets a breadcrumb path to return
		/// </summary>
		/// <param name="layer">The layer event that correspondence to the target layer</param>
		/// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
		protected void SkipToLayer(string layer, List<RadialMenuObject> customElements = null, params string[] layerPath)
		{
			m_RadialBase.SkipToLayer(layer, customElements, layerPath);
		}

		/// <summary>
		/// Skips to a given slider layer and sets a breadcrumb path to return
		/// </summary>
		/// <param name="layer">The layer event that correspondence to the target layer</param>
		/// <param name="sliderValue">Value of the slider that is will be loaded</param>
		/// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
		protected void SkipToLayer(string layer, float sliderValue, params string[] layerPath)
		{
			m_RadialBase.SkipToLayer(layer, sliderValue, layerPath);
		}

		/// <summary>
		/// Skips to a given selection layer and sets a breadcrumb path to return
		/// </summary>
		/// <param name="layer">The layer event that correspondence to the target layer</param>
		/// <param name="currentIndex">Current index of the selection layer that will be loaded</param>
		/// <param name="selectionValues">Values of the selection layer that will be loaded</param>
		/// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
		protected void SkipToLayer(string layer, int currentIndex, string[] selectionValues, params string[] layerPath)
		{
			m_RadialBase.SkipToLayer(layer, currentIndex, selectionValues, layerPath);
		}

		#endregion

		#region Create Shuttle

		/// <summary>
		/// Creates a shuttle that will be used to load to an element layer when a new radial menu get initalised
		/// </summary>
		/// <param name="layer">The layer event that correspondence to the target layer</param>
		/// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
		protected void CreateShuttle(string layer, params string[] layerPath)
		{
			m_RadialBase.CreateShuttle(layer, null, null, null, layerPath);
		}

		/// <summary>
		/// Creates a shuttle that will be used to load to a slider layer when a new radial menu get initalised
		/// </summary>
		/// <param name="layer">The layer event that correspondence to the target layer</param>
		/// <param name="sliderValue">Slider value that the shuttle will save</param>
		/// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
		protected void CreateShuttle(string layer, float sliderValue, params string[] layerPath)
		{
			m_RadialBase.CreateShuttle(layer, sliderValue, null, null, layerPath);
		}

        /// <summary>
        /// Creates a shuttle that will be used to load to a selection layer when a new radial menu get initalised
        /// </summary>
        /// <param name="layer">The layer event that correspondence to the target layer</param>
        /// <param name="currentIndex">Selection current index that the shuttle will save</param>
        /// <param name="customElements">list of custom elements</param>
        /// <param name="SelectionValues">Selection values that the shuttle will save</param>
        /// <param name="layerPath">Path to target layer by the layer events in the radial base hierarchy</param>
        protected void CreateShuttle(string layer, int currentIndex, List<RadialMenuObject> customElements = null, string[] SelectionValues = null, params string[] layerPath)
		{
			m_RadialBase.CreateShuttle(layer, null, currentIndex, customElements, SelectionValues, layerPath);
		}

		#endregion

		/// <summary>
		/// Change the layer and go to the previous layer
		/// </summary>
		protected void GoToPreviousLayer()
		{
			m_RadialBase.TransitionToPreviousLayer();
		}

		/// <summary>
		/// Transition to a scene
		/// </summary>
		/// <param name="sceneName">The name of the scene</param>
		protected void GoToScene(string sceneName)
		{
			m_RadialBase.TransitionToScene(sceneName);
		}

		/// <summary>
		/// Transitions to quit the application
		/// </summary>
		protected void QuitApplication()
		{
			m_RadialBase.TransitionToQuit();
		}

		/// <summary>
		/// Used to return the value of a toggle button
		/// </summary>
		/// <param name="toggleValue">The value of the toggle that will be returned</param>
		/// <returns>Used to return the value of a toggle button</returns>
		protected T GetToggleValue<T>(bool toggleValue)
		{
			var value = toggleValue;
			return (T)Convert.ChangeType(value, typeof(T));
		}

		/// <summary>
		/// Used to return the value of a slider buton
		/// </summary>
		/// <param name="sliderValue">The value of the slider that will be returned</param>
		/// <returns>Used to return the value of a slider buton</returns>
		protected T GetSliderValue<T>(float sliderValue)
		{
			var value = sliderValue;
			return (T)Convert.ChangeType(value, typeof(T));
		}

		/// <summary>
		/// Used to return the value of a selection button
		/// </summary>
		/// <param name="selectionValue">The values of the selection that will be returned</param>
		/// <returns>Used to return the value of a selection button</returns>
		protected T GetSelectionValue<T>(string selectionValue)
		{
			var value = selectionValue;
			return (T)Convert.ChangeType(value, typeof(T));
		}

		/// <summary>
		/// Used at the end of the ProcessReturnValues function to return a blank value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected T GetNullValue<T>()
		{
			if(typeof(T) == typeof(bool))
			{
				return (T)Convert.ChangeType(false, typeof(T));
			}

			if (typeof(T) == typeof(string))
			{
				return (T)Convert.ChangeType("", typeof(T));
			}

			//if float
			return (T)Convert.ChangeType(0.0f, typeof(T));

		}

        /// <summary>
        /// Refreshes the current layer with the option to change the elements
        /// </summary>
        /// <param name="customElements"></param>
        protected void ReloadMenu(List<RadialMenuObject> customElements = null)
        {
            m_RadialBase.ReloadLayer(customElements);
        }
	}
}