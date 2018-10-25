using UnityEngine;
using System.Collections;

public class HeroPower : MonoBehaviour {

    //The power bar
    public RectTransform m_heroPowerBar;

    //Current hero power
    public float m_heroPower = 50f;
    //Max hero power
    public float m_maxHeroPower = 100f;
    //How much does flying drain per second
    public float m_flyingPowerDrainPerSec = 1f;
    //How much does power regenerate every second
    private float m_heroPowerPerSec = 1f;
    //Minimum power to start flying
    public float m_flyingMinPower = 5f;
    //Can this hero fly
    public bool m_canFly = true;

	// Use this for initialization
	void Start ()
    {
        m_heroPowerBar.localScale = new Vector3(m_heroPower / m_maxHeroPower, 1f, 1f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Debug toggle flying
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            m_canFly = !m_canFly;
        }
        if (UIManager.Instance.m_animFinished)
        {
            m_heroPowerBar.localScale = new Vector3(m_heroPower / m_maxHeroPower, 1f, 1f);
        }
        m_heroPower += m_heroPowerPerSec * Time.deltaTime;

        if(m_heroPower > m_maxHeroPower)
        {
            m_heroPower = m_maxHeroPower;
        }
	}
}
