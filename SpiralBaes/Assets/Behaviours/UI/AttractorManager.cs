using UnityEngine;
using System.Collections;
using EaseyEase;

public class AttractorManager : MonoBehaviour {
    public static AttractorManager Instance;
    public RectTransform m_YouMustStopThem;

	// Use this for initialization
	void Awake () {
        Instance = this;
	}

    public void YouMustStopThem()
    {
        m_YouMustStopThem.gameObject.SetActive(true);
        StartCoroutine(IYouMustStopThem());
    }

    private IEnumerator IYouMustStopThem()
    {

        float duration = 0.75f;
        EaseyTimer timer = new EaseyTimer(duration);
        float t = 0f;
        while (t < duration)
        {
            m_YouMustStopThem.transform.localPosition = Easey.Ease(Easey.EaseType.BackOut, new Vector3(-Screen.width, 0f, 0f), new Vector3(0f, 0f, 0f), timer);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        m_YouMustStopThem.transform.localPosition = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(2f);
        duration = 0.5f;
        timer = new EaseyTimer(duration);
        t = 0f;
        while (t < duration)
        {
            m_YouMustStopThem.transform.localPosition = Easey.Ease(Easey.EaseType.BackIn, new Vector3(0f, 0f, 0f), new Vector3(Screen.width, 0f, 0f), timer);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        m_YouMustStopThem.transform.localPosition = new Vector3(Screen.width, 0f, 0f);
        m_YouMustStopThem.gameObject.SetActive(false);
    }
}
