using UnityEngine;

public class CaptureScene : MonoBehaviour
{
	public LayerMask m_RtLayers = -1;
	RenderTexture m_Rt;
	Camera m_RTCam;

	void Start ()
	{
		m_Rt = new RenderTexture (1024, 1024, 24, RenderTextureFormat.ARGB32);
		m_Rt.name = "Scene";
		m_Rt.useMipMap = true;
		m_Rt.autoGenerateMips = true;
	}
	void Update ()
	{
		Camera c = Camera.main;
		if (m_RTCam == null)
		{
			GameObject go = new GameObject ("RtCamera", typeof (Camera));
			m_RTCam = go.GetComponent<Camera> ();
			go.transform.parent = c.transform;
		}
		m_RTCam.CopyFrom (c);
		m_RTCam.targetTexture = m_Rt;
		m_RTCam.cullingMask &= ~m_RtLayers;
		m_RTCam.enabled = false;
		m_RTCam.Render ();
		Shader.SetGlobalTexture ("_Global_ScreenTex", m_Rt);
	}
}