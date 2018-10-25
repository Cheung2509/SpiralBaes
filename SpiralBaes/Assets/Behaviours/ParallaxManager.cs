using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Created by David Dunnings

public class ParallaxManager : MonoBehaviour {

    public float m_parallaxFarModifier, m_parallaxMidModifier, m_parallaxNearModifier;

    #region Parallax transform parents and lists
    public Transform m_parallaxBackground;
    private List<Transform> m_parallaxBackgroundTransforms = new List<Transform>();
    public Transform m_parallaxFar;
    private List<Transform> m_parallaxFarTransforms = new List<Transform>();
    public Transform m_parallaxMid;
    private List<Transform> m_parallaxMidTransforms = new List<Transform>();
    public Transform m_parallaxNear;
    private List<Transform> m_parallaxNearTransforms = new List<Transform>();
    public Transform m_noParallax;
    private List<Transform> m_noParallaxTransforms = new List<Transform>();
    #endregion

    private Vector3 m_cameraStartPosition;
    private Vector3 m_cameraPreviousPosition;
    
	void Start ()
    {
        #region Variables
        float offset = 100f;
        float time = 1f;
        iTween.EaseType ease = iTween.EaseType.easeOutQuint;
        #endregion
        #region Adding child transforms to lists
        foreach (Transform child in m_parallaxBackground) {
            m_parallaxBackgroundTransforms.Add(child);
        }
        foreach (Transform child in m_parallaxFar) { 
            m_parallaxFarTransforms.Add(child);
            iTween.MoveFrom(child.gameObject, iTween.Hash("x", child.position.x + offset, "time", time, "easetype", ease, "delay", time * 0.3f));
        }
        foreach (Transform child in m_parallaxMid)
        {
            m_parallaxMidTransforms.Add(child);
            iTween.MoveFrom(child.gameObject, iTween.Hash("x", child.position.x - offset, "time", time, "easetype", ease, "delay", time * 0.3f));
        }
        foreach (Transform child in m_parallaxNear)
        {
            m_parallaxNearTransforms.Add(child);
            iTween.MoveFrom(child.gameObject, iTween.Hash("x", child.position.x + offset, "time", time, "easetype", ease, "delay", time * 0.3f));
        }
        foreach (Transform child in m_noParallax)
        {
            m_noParallaxTransforms.Add(child);
        }
        #endregion
        #region CameraPositions
        m_cameraStartPosition = Camera.main.transform.position;
        m_cameraPreviousPosition = m_cameraStartPosition;
        iTween.MoveFrom(Camera.main.gameObject, iTween.Hash("x", m_cameraPreviousPosition.x - offset, "time", time, "easetype", ease));
        #endregion
    }
	
	void LateUpdate ()
    {
        #region Update transforms based on camera movement * parallax modifier
        foreach (Transform child in m_parallaxBackgroundTransforms)
        {
            Vector3 mod = ((Camera.main.transform.position - m_cameraPreviousPosition) * 1f);
            mod.y = 0f;
            child.transform.position += mod;
        }
        foreach (Transform child in m_parallaxFarTransforms)
        {
            Vector3 mod = (Camera.main.transform.position - m_cameraPreviousPosition) * m_parallaxFarModifier;
            mod.y = 0f;
            child.transform.position += mod;
        }
        foreach (Transform child in m_parallaxMidTransforms)
        {
            Vector3 mod = (Camera.main.transform.position - m_cameraPreviousPosition) * m_parallaxMidModifier;
            mod.y = 0f;
            child.transform.position += mod;
        }
        foreach (Transform child in m_parallaxNearTransforms)
        {
            Vector3 mod = (Camera.main.transform.position - m_cameraPreviousPosition) * m_parallaxNearModifier;
            mod.y = 0f;
            child.transform.position += mod;
        }
        #endregion
        m_cameraPreviousPosition = Camera.main.transform.position;
	}
}
