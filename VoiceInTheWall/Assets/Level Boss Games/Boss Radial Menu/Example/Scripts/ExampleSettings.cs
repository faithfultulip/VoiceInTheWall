using UnityEngine;
using System.Collections;

namespace LBG.UI.Radial
{
    public static class ExampleSettings
    {
        private static float m_Volume = 50.0f;
        public static float Volume { get { return m_Volume; } }
 
        private static bool m_Mute = false;
        public static bool Mute { get { return m_Mute; } }

        private static string[] m_BackgroundColours = new string[] {"Black", "White", "Red", "Green", "Blue" };
        public static string[] BackgroundColours
        {
            get
            {
                return m_BackgroundColours;
            }
        }

        private static int m_CurrentBackgroundIndex = 0;
        public static int CurrentBackgroundIndex { get { return m_CurrentBackgroundIndex; } }

        public static void ChangeVolume(float volume)
        {
            Camera.main.gameObject.GetComponent<AudioSource>().volume = Mathf.InverseLerp(0, 100, volume);
            m_Volume = volume;
        }

        public static void ToggleMute()
        {
            m_Mute = !m_Mute;
            Camera.main.gameObject.GetComponent<AudioSource>().mute = m_Mute;

        }

        public static void ChangeBackgroundColour(int backgroundIndex)
        {
            switch(backgroundIndex)
            {
                case 0:
                    Camera.main.backgroundColor = Color.black;
                    break;
                case 1:
                    Camera.main.backgroundColor = Color.white;
                    break;
                case 2:
                    Camera.main.backgroundColor = Color.red;
                    break;
                case 3:
                    Camera.main.backgroundColor = Color.green;
                    break;
                case 4:
                    Camera.main.backgroundColor = Color.blue;
                    break;
            }

            m_CurrentBackgroundIndex = backgroundIndex;
        }



    }
}
