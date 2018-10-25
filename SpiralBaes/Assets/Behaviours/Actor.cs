using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour {
	
	//This actor's renderer/shadow/animator
	public SpriteRenderer m_renderer;
	public GameObject m_shadow;
	public Animator m_anim;
	
	// Healthbar GO
	public GameObject m_healthbar = null;

	// Collider GO?
	public GameObject m_colliderPosition;
	protected const float m_zWidth = 0.2f;

	public Vector3 m_fistOffset = new Vector3(0.3f, 0f, 0f);

	//Shadow offset when zDepth = 0
	public Vector3 m_shadowOffset = new Vector3(0f, -0.5f, 0f);
	//Current z-depth between 0-1
	public float m_zDepth = 0f;
	//Span of z-depth
	protected const float m_maxZDepth = 1.95f;
	//Minimum y point of z-depth
	protected const float m_minZDepth = -4.9f;
	//Smallest scale of actor
	protected const float m_minScale = 1.25f;
	//Largest scale of actor
	protected const float m_maxScale = 1.9f;

	protected float m_fudgeScale = 1f;
	public float m_maxHealth = 100f;
	public float m_currentHealth = 100f;

	//Is the human alive?
	public bool m_alive = false;
	public bool hit;
	//Is the player touching the ground
	public bool m_grounded = true;
	//Is the player facing right (true = right, false = left)
	protected bool m_facingRight = true;

	//Actor velocity
	public Vector3 m_velocity = Vector3.zero;

	//Max height in scene (used for flying)
	protected const float m_maxHeight = 2f;
	//The point at which the vertical drag starts getting stronger (used for flying)
	protected const float m_minHeightHeavyVerticalDrag = 1f;
	protected const float m_verticalDragModifierMax = 1f;

	//Maximum clamped horizontal velocity
	protected const float maxHorizontalVelocity = 4f;
	protected const float maxHorizontalFlyingVelocity = 6f;
	//Maximum clamped vertical velocity
	protected const float m_maxVerticalVelocity = 10f;
	//Horizontal drag per frame
	protected const float m_horizontalDrag = 30f;
	//Vertical drag per frame
	protected const float m_verticalDrag = 50f;
	//Horizontal speed per frame
	protected float m_horizontalSpeed = 1f;
	//Vertical speed per frame
	protected float m_verticalSpeed = 0.7f;

	public Actor m_collidedActor;

	bool CanMoveThisFrame = true;

	public bool m_currentlyInHitAnimation = false;
	public bool m_currentlyInDeathAnimation = false;

	public bool isDead;

	//Healthbar appear/disappear variables
	private float timeLastDamaged = 0f;
	private float timeToAutohide = 3f;

	public float TargetYPos
	{
		get {
			return m_minZDepth + (m_zDepth * m_maxZDepth);
		}
	}


	[System.Flags]
	public enum Direction {
		None = 1 << 0,
		Left = 1 << 1,
		Right = 1 << 2,
		Up = 1 << 3,
		Down = 1 << 4 

	}

	public Direction m_direction;

	void Awake() { 
		MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();
	}

	// Use this for initialization
	void Start () {
		timeLastDamaged = Time.time - 10f;
		if (m_healthbar != null)
		{
			m_healthbar.transform.parent.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	public virtual void Update() {
		if (m_currentlyInDeathAnimation) {
			return;
		}

		// Prevent us leaving the world
		ClampZDepth();

		// Perform physics calculation
		ApplyDragForce();
		ClampVelocity();

		// Set our sprite scale according to our z-axis positioning
		m_renderer.gameObject.transform.localScale = (Vector3.one * Mathf.Lerp(m_maxScale, m_minScale, m_zDepth)) * m_fudgeScale;

		// Sprite sorting
		SetSortingOrder();

		// Shadow sprite and player sprite position
		SetShadow();	
		UpdatePosition();
	}

	public virtual void LateUpdate() {

		// Update our shadow position again??? (we're doing this twice per tick!)
		m_shadow.transform.position = new Vector3(transform.position.x, TargetYPos, 0f) + m_shadowOffset + (new Vector3(0f, (m_zDepth * 0.15f), 0f));

		//Update healthbar
		if (m_healthbar != null) {
			if (m_healthbar.activeSelf) {
				if (Time.time - timeLastDamaged > timeToAutohide)
				{
					m_healthbar.transform.parent.gameObject.SetActive(false);
				}
			}
			m_healthbar.transform.localScale = new Vector3(Mathf.InverseLerp(0f, m_maxHealth, m_currentHealth), 1f, 1f);
		}
	}

	// TODO: Nesting in this func is driving me a little bit mad, rework this a tad -sam 19/01/2017
	public bool IsColliding() {
		Collider2D[] _other = Physics2D.OverlapCircleAll((Vector2)m_colliderPosition.transform.position, 0.35f, LayerMask.GetMask("ActorCollider"));
		for (int i = 0; i < _other.Length; i++) {
			if (_other[i] != null) { // Do we even need this check?
				Hero isHero = _other[i].transform.parent.GetComponent<Hero>();
				if (isHero != null) { 
					if (isHero.gameObject != gameObject) {
						if (isHero != null) {
							//If the hero hit is within punch width
							if (Mathf.Abs(m_zDepth - isHero.m_zDepth) <= m_zWidth) {
								m_collidedActor = (Actor)isHero;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	public virtual void UpdatePosition() {
		CanMoveThisFrame = true;
		if (m_grounded) {
			if (IsColliding()) {
				if (m_velocity.x < 0 && transform.position.x > m_collidedActor.transform.position.x) {
					CanMoveThisFrame = false;
				} else if (m_velocity.x > 0 && transform.position.x < m_collidedActor.transform.position.x) {
					CanMoveThisFrame = false;
				}
			}
			if (CanMoveThisFrame) {
				transform.position = new Vector3(transform.position.x + (m_velocity.x * Time.deltaTime), TargetYPos, 0f);
			}
		} else {
			transform.position = new Vector3(transform.position.x + (m_velocity.x * Time.deltaTime), transform.position.y + (m_velocity.y * Time.deltaTime), 0f);

			//Height Check
			if (transform.position.y > m_maxHeight) {
				m_velocity.y = 0f;
				transform.position = new Vector3(transform.position.x, m_maxHeight, transform.position.z);
			}
		}
	}

	// Move the specified actor
	public virtual void Move(Direction direction) {

	}
		
	// Move the specified actor
	// TODO: Find out why we need two of these
	public virtual void Move() {

	}

	/// Applies the drag force.
	protected virtual void ApplyDragForce() {
		if (m_velocity.x > 0.1f) {
			m_velocity += new Vector3(-m_horizontalDrag, 0f, 0f) * Time.deltaTime;
		} else if (m_velocity.x < -0.1f) {

			m_velocity += new Vector3(m_horizontalDrag, 0f, 0f) * Time.deltaTime;
		} else {
			m_velocity = new Vector3(0f, m_velocity.y, 0f);
		}
	}

	public void CheckDead() {
		if(m_currentHealth <= 0f) {
			StartCoroutine(IKilled());
		} else {
			StartCoroutine(IPunched());
		}
	}

	public virtual void Hit(Actor source, float damage = 30f) {
		if (!m_currentlyInDeathAnimation) {
			m_anim.SetBool("Hit", true);
			m_velocity.x = 0f;
			m_velocity.x += (source.transform.position.x - transform.position.x) > 0f ? -Random.Range(6f, 10f) : Random.Range(6f, 10f);
			m_currentlyInHitAnimation = true;
			m_currentHealth = Mathf.Max(m_currentHealth - damage, 0f);

			if(m_healthbar != null) {
				timeLastDamaged = Time.time;
				m_healthbar.transform.parent.gameObject.SetActive(true);
			}
			CheckDead();
		}
	}

	public IEnumerator IPunched() {
		m_renderer.material.SetFloat("_Brightness", 4f);
		yield return new WaitForSeconds(0.1f);
		m_renderer.material.SetFloat("_Brightness", 1f);
		yield return new WaitForSeconds(0.3f);

		m_currentlyInHitAnimation = false;
	}

	public virtual IEnumerator IKilled() {
		m_currentlyInDeathAnimation = true;
		if(m_healthbar != null)
		{
			m_healthbar.transform.parent.gameObject.SetActive(false);
		}
		m_anim.SetTrigger("Death");
		yield return new WaitForSeconds(0.6f);
		for (int i = 0; i < 4; i++)
		{
			m_renderer.material.SetFloat("_Brightness", 4f);
			yield return new WaitForSeconds(0.08f);
			m_renderer.material.SetFloat("_Brightness", 1f);
			yield return new WaitForSeconds(0.08f);
		}

		gameObject.SetActive(false);
	}

	public void SetSortingOrder(int add = 0)
	{
		m_renderer.sortingOrder = 200 -(int)(m_zDepth * 100f) + add;

	}

	protected void SetShadow()
	{
		m_shadow.GetComponent<SpriteRenderer>().sortingOrder = m_renderer.sortingOrder - 1;
		m_shadow.gameObject.transform.localScale = (Vector3.one * Mathf.Lerp(m_maxScale, m_minScale, m_zDepth)) * m_fudgeScale;
		m_shadow.gameObject.transform.localScale *= Mathf.Clamp((1 - Mathf.InverseLerp(TargetYPos, TargetYPos + 3f, m_renderer.gameObject.transform.position.y)), 0.6f, 1f);
	}

	protected void ClampZDepth()
	{
		//z-depth clamping
		if (m_zDepth < 0f)
		{
			m_zDepth = 0f;
		}
		else if (m_zDepth > 1f)
		{
			m_zDepth = 1f;
		}

	}

	protected void ClampVelocity() {
		Vector3 vel;
		//Clamp velocity

		vel = m_velocity;
		if (!m_currentlyInHitAnimation)
		{
			vel = new Vector3(Mathf.Clamp(m_velocity.x, -maxHorizontalVelocity, maxHorizontalVelocity), Mathf.Clamp(m_velocity.y, -m_maxVerticalVelocity, m_maxVerticalVelocity), 0f);
		}

		//Add velocity
		m_velocity = vel;


	}

	public virtual void DestroyObject() {



	}



	public void OnDrawGizmos() {
		//Draw fist position
		Gizmos.DrawWireSphere(transform.position + m_fistOffset, 0.35f);
	}
}
