using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Created by David Dunnings

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    public Canvas m_canvas;
    public GameObject m_P1;
    public GameObject m_P2;
    public GameObject m_P3;
    public GameObject m_P4;

    public bool m_animFinished = false;

    void Awake(){
        Instance = this;
	}

	void Start ()
    {
        Vector3 startPos = gameObject.transform.position;
        gameObject.transform.position -= new Vector3(0f, -250f * (Screen.height / m_canvas.GetComponent<CanvasScaler>().referenceResolution.y), 0f);
        iTween.MoveTo(gameObject, iTween.Hash("y", startPos.y, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuint, "delay", 1.25f));
	
        StartCoroutine(FinishedAnimations());
    }
	
	void Update () {
        //Debug reload scene button
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(0);
        }
	}
	
	void OnDestroy(){
		
	}

    IEnumerator FinishedAnimations()
    {
        yield return new WaitForSeconds(1.95f);
        m_animFinished = true;
    }

	public void GenerateHealth(int id)
	{
		switch (id)
		{
			case 0:
			{
				iTween.ScaleFrom(m_P1, iTween.Hash("x", 0f, "time", 0.2f, "easetype", iTween.EaseType.easeOutQuint, "delay", 1.75f));
			}
		
			break;

			case 1:
			{
				iTween.ScaleFrom(m_P2, iTween.Hash("x", 0f, "time", 0.2f, "easetype", iTween.EaseType.easeOutQuint, "delay", 1.75f));
			}

			break;

			case 2:
			{
				iTween.ScaleFrom(m_P3, iTween.Hash("x", 0f, "time", 0.2f, "easetype", iTween.EaseType.easeOutQuint, "delay", 1.75f));
			}

			break;

			case 3:
			{
				iTween.ScaleFrom(m_P4, iTween.Hash("x", 0f, "time", 0.2f, "easetype", iTween.EaseType.easeOutQuint, "delay", 1.75f));
			}

			break;
		}

	}
}
