using UnityEngine;
using System.Collections;

public enum EnemyState
{
    idle,
    seeking,
    fighting,
    dead
}

public class HumanEnemy : Human {
    
    public EnemyState m_currentState = EnemyState.idle;
    public Hero m_targetPlayer;
    private float m_reaquireTime = 3f;
    private float m_timeLastAquired = 0f;

    private float timeLastPunched = 0f;
    private float timeBetweenPunches = 1.5f;
       
	// Use this for initialization
	void Start ()
    {
        m_verticalSpeed *= 0.7f;
        m_horizontalSpeed *= 0.7f;
    }

    // Update is called once per frame
    public override void Update ()
    {
        bool movingOnZ = false;
        if(m_currentState == EnemyState.dead)
        {
            return;
        }
        if (m_targetPlayer == null)
        {
            AquireHero();
            m_timeLastAquired = Time.time;
        }
        else
        {
            bool nextToInX = false;
            if (m_targetPlayer.transform.position.x < transform.position.x - 0.9f && !m_anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                Vector3 addForce = Vector3.left * m_horizontalSpeed;

                m_velocity += addForce;
                m_currentState = EnemyState.seeking;
            }
            else if (m_targetPlayer.transform.position.x > transform.position.x + 1f && !m_anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                Vector3 addForce = Vector3.right * m_horizontalSpeed;
                m_velocity += addForce;
                m_currentState = EnemyState.seeking;
            }
            else if (!m_currentlyInHitAnimation)
            {
                nextToInX = true;
                m_velocity.x = 0f;
            }

            if(m_velocity.x > 0f && !m_currentlyInHitAnimation)
            {
                m_fistOffset = new Vector2(Mathf.Abs(m_fistOffset.x), m_fistOffset.y);
                m_renderer.flipX = true;
            }
            else if(m_velocity.x < 0f && !m_currentlyInHitAnimation)
            {
                m_fistOffset = new Vector2(-Mathf.Abs(m_fistOffset.x), m_fistOffset.y);
                m_renderer.flipX = false;
            }


            if (m_targetPlayer.m_zDepth < m_zDepth - 0.02f)
            {
                m_zDepth -= m_verticalSpeed * Time.deltaTime;
                movingOnZ = true;
                m_currentState = EnemyState.seeking;
            }
            else if (m_targetPlayer.m_zDepth > m_zDepth + 0.02f)
            {
                m_zDepth += m_verticalSpeed * Time.deltaTime;
                movingOnZ = true;
                m_currentState = EnemyState.seeking;
            }
            else if (nextToInX)
            {
                m_currentState = EnemyState.fighting;
            }

            if(m_currentState == EnemyState.fighting && !m_anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                if(Time.time - timeLastPunched > timeBetweenPunches)
                {
                    timeLastPunched = Time.time;
                    timeBetweenPunches = Random.Range(0.9f, 1.2f);
                    m_anim.SetBool("Punching", true);
                    //HitObject();
                }
            }

        }

        //Aqurie a hero target
        if(Time.time - m_timeLastAquired > m_reaquireTime)
        {
            AquireHero();
            m_timeLastAquired = Time.time;
        }
        m_anim.SetBool("Walking", m_velocity.x > 0.1f || m_velocity.x < -0.1f || movingOnZ);

        base.Update();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void UpdatePosition()
    {
        base.UpdatePosition();
    }

    public override IEnumerator IKilled()
    {
        m_currentState = EnemyState.dead;
        return base.IKilled();
    }

    public void AquireHero()
    {
        Hero[] allHeroes = FindObjectsOfType<Hero>();
        float distance = float.MaxValue;
        for (int i = 0; i < allHeroes.Length; i++)
        {
            float thisXDistance = Mathf.Abs(allHeroes[i].transform.position.x - transform.position.x);
            float thisZDistance = (Mathf.Abs(allHeroes[i].m_zDepth - m_zDepth));
            float thisDistance = thisXDistance + thisZDistance * 10f;
            if(thisDistance < distance)
            {
                m_targetPlayer = allHeroes[i];
                distance = thisDistance;
            }
        }
    }
}
