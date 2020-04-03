using UnityEngine;
using System.Collections.Generic;

namespace LBG.UI.Radial
{
	public class RadialShuttle : MonoBehaviour
	{

		public string m_LayerEvent;
		public string[] m_LayerPath;
		public float? m_SliderValue;
		public int? m_SectionCurrentIndex;
		public string[] m_SelectionValues;
        public List<RadialMenuObject> m_CustomElements;


        void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		public void Init(string layerEvent, float? sliderValue = null , int? selectionCurrentIndex = null, List<RadialMenuObject> customElements = null, string[] selectionValues = null, params string[] layerPath)
		{
			m_LayerEvent = layerEvent;
			m_LayerPath = layerPath;
			m_SliderValue = sliderValue;
            m_CustomElements = customElements;
			m_SectionCurrentIndex = selectionCurrentIndex;
			m_SelectionValues = selectionValues;
		}
	}
}