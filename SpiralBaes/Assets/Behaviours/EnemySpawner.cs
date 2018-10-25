using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public float m_cameraXPosTrigger = 999f;
    public float m_enemyZPos = 0.5f;
    public Actor m_enemyGameObject;
    public int m_waveCount = 3;
    public int m_waveSize = 5;
    public float m_waveDelay = 3f;
    public float m_waveInitialDelay = 0.1f;
    private bool m_started = false;
    private bool m_completed = false;
    private int m_currentWave = 0;
    private float m_timeLastSpawnedWave = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_started)
        {
            if(CameraController.Instance.m_averagePlayerPos.x > m_cameraXPosTrigger)
            {
                StartCoroutine(TriggerStart());
                m_started = true;
            }
        }
	}

    IEnumerator TriggerStart()
    {
        AttractorManager.Instance.YouMustStopThem();
        yield return new WaitForSeconds(m_waveInitialDelay);
        for (int wave = 0; wave < m_waveCount; wave++)
        {
            for (int i = 0; i < m_waveSize; i++)
            {
                Actor go = Instantiate(m_enemyGameObject);
                go.transform.parent = transform;
                go.transform.position = transform.position;
                go.m_zDepth = m_enemyZPos;
                go.transform.position = new Vector3(transform.position.x, go.TargetYPos, transform.position.z);
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(m_waveDelay);
            m_currentWave++;
        }
        m_completed = true;
    }
}
