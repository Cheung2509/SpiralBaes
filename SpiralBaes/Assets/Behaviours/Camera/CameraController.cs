using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Created by David Dunnings

public class CameraController : MonoBehaviour {

    public static CameraController Instance;

    private Vector3 m_lastAveragePlayerPos;
    public Vector3 m_averagePlayerPos;
	public Actor m_p1;
    public Actor m_p2;
	public Actor m_p3;
	public Actor m_p4;

    private float minDistanceToZoom = 7f;
    private float maxDistanceToZoom = 11f;

    private float minSize = 4.7f;
    private float maxSize = 5.2f;

    private float minSizeCenter = -0.3f;
    private float maxSizeCenter = 0.2f;

	public static float pixelsToUnits = 1f;
	public static float scale = 1f;

	public Vector2 nativeResolution = new Vector2 (Screen.width, Screen.height);

    void Awake(){

		if (Camera.main.orthographic) {
			scale = Screen.height/nativeResolution.y;
			pixelsToUnits *= scale;
			Camera.main.orthographicSize = (Screen.height / 2.0f) / pixelsToUnits;
		}

	}

	void Start () {
        Instance = this;
	}
	
	void LateUpdate () {
        int count = 0;
        m_averagePlayerPos = Vector3.zero;
        float maxVelocity = 1f;
        if (m_p1 != null && m_p1.m_alive)
        {
            m_averagePlayerPos += m_p1.m_renderer.gameObject.transform.position;
            if(Mathf.Abs(m_p1.m_velocity.x) > maxVelocity) { maxVelocity = Mathf.Abs(m_p1.m_velocity.x); }
            count++;
        }
        if (m_p2 != null && m_p2.m_alive)
        {
            m_averagePlayerPos += m_p2.m_renderer.gameObject.transform.position;
            if (Mathf.Abs(m_p2.m_velocity.x) > maxVelocity) { maxVelocity = Mathf.Abs(m_p2.m_velocity.x); }
            count++;
        }
        if (m_p3 != null && m_p3.m_alive)
        {
            m_averagePlayerPos += m_p3.m_renderer.gameObject.transform.position;
            if (Mathf.Abs(m_p3.m_velocity.x) > maxVelocity) { maxVelocity = Mathf.Abs(m_p3.m_velocity.x); }
            count++;
        }
        if (m_p4 != null && m_p4.m_alive)
        {
            m_averagePlayerPos += m_p4.m_renderer.gameObject.transform.position;
            if (Mathf.Abs(m_p4.m_velocity.x) > maxVelocity) { maxVelocity = Mathf.Abs(m_p4.m_velocity.x); }
            count++;
        }
        m_averagePlayerPos /= count;
        float farLeft = m_averagePlayerPos.x;
        float farRight = m_averagePlayerPos.x;


        if (m_p1.m_alive)
        {
            if (m_p1.m_renderer.gameObject.transform.position.x < m_averagePlayerPos.x && m_p1.m_renderer.gameObject.transform.position.x < farLeft)
            {
                farLeft = m_p1.m_renderer.gameObject.transform.position.x;
            }
            else if (m_p1.m_renderer.gameObject.transform.position.x > m_averagePlayerPos.x && m_p1.m_renderer.gameObject.transform.position.x > farRight)
            {
                farRight = m_p1.m_renderer.gameObject.transform.position.x;
            }
        }
        if (m_p2.m_alive)
        {
            if (m_p2.m_renderer.gameObject.transform.position.x < m_averagePlayerPos.x && m_p2.m_renderer.gameObject.transform.position.x < farLeft)
            {
                farLeft = m_p2.m_renderer.gameObject.transform.position.x;
            }
            else if (m_p2.m_renderer.gameObject.transform.position.x > m_averagePlayerPos.x && m_p2.m_renderer.gameObject.transform.position.x > farRight)
            {
                farRight = m_p2.m_renderer.gameObject.transform.position.x;
            }
        }
        if (m_p3.m_alive)
        {
            if (m_p3.m_renderer.gameObject.transform.position.x < m_averagePlayerPos.x && m_p3.m_renderer.gameObject.transform.position.x < farLeft)
            {
                farLeft = m_p3.m_renderer.gameObject.transform.position.x;
            }
            else if (m_p3.m_renderer.gameObject.transform.position.x > m_averagePlayerPos.x && m_p3.m_renderer.gameObject.transform.position.x > farRight)
            {
                farRight = m_p3.m_renderer.gameObject.transform.position.x;
            }
        }
        if (m_p4.m_alive)
        {
            if (m_p4.m_renderer.gameObject.transform.position.x < m_averagePlayerPos.x && m_p4.m_renderer.gameObject.transform.position.x < farLeft)
            {
                farLeft = m_p4.m_renderer.gameObject.transform.position.x;
            }
            else if (m_p4.m_renderer.gameObject.transform.position.x > m_averagePlayerPos.x && m_p4.m_renderer.gameObject.transform.position.x > farRight)
            {
                farRight = m_p4.m_renderer.gameObject.transform.position.x;
            }
        }



        float maxDistance = Mathf.Abs(farRight - farLeft);

        if ((m_averagePlayerPos.x - transform.parent.position.x) > 2f)
        {
            transform.parent.position += new Vector3(maxVelocity * Time.deltaTime, 0f, 0f);
        }
        else if ((m_averagePlayerPos.x - transform.parent.position.x) < -2f)
        {
            transform.parent.position -= new Vector3(maxVelocity * Time.deltaTime, 0f, 0f);
        }
        if (maxDistance > minDistanceToZoom)
        {
            float lerpVal = (maxDistance - minDistanceToZoom) / (maxDistanceToZoom - minDistanceToZoom);
            Camera.main.orthographicSize = Mathf.Lerp(minSize, maxSize, lerpVal);
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(minSizeCenter, maxSizeCenter, lerpVal), transform.position.z);
        }
        else {
            Camera.main.orthographicSize = minSize;
            transform.position = new Vector3(transform.position.x, minSizeCenter, transform.position.z);
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Shake();
        //}
    }
	
	void OnDestroy(){
		
	}

    public void Shake()
    {
        //Punch the camera's position
        gameObject.transform.position = Vector3.zero;
        iTween.ShakePosition(gameObject, iTween.Hash("x", 0.3f, "y", 0.3f, "time", 1f, "isLocal", true));
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_averagePlayerPos, 0.5f);
    }
}
