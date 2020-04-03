using UnityEngine;
using UnityEngine.UI;

namespace LBG.UI.Radial
{
    public class UIControls : MonoBehaviour
    {

        [SerializeField]
        InputField m_ReadyUpInputField;
        [SerializeField]
        InputField m_CleanUpInputField;

        [SerializeField]
        RadialBase m_ExampleMenu;
        
        void Start()
        {
            m_ReadyUpInputField.text = m_ExampleMenu.m_DefaultReadyUpTime.ToString();
            m_CleanUpInputField.text = m_ExampleMenu.m_DefaultCleanUpTime.ToString();
        }

        public void OnCleanUpChange()
        {
            m_ExampleMenu.m_DefaultCleanUpTime = float.Parse(m_CleanUpInputField.text);
        }

        public void OnReadyUpChange()
        {
            m_ExampleMenu.m_DefaultReadyUpTime = float.Parse(m_ReadyUpInputField.text);        }
    }
}