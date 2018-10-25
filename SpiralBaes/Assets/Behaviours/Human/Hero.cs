using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;

// Created by David Dunnings
// Edited by Jake Baugh 11/01/2016

/// <summary>
/// Hero.
/// </summary>
public class Hero : Human {

    public HeroPower m_heroPower;

    public Color m_heroColor;

    public ParticleSystem[] m_hitParticleSystems;

    private Player m_player;

    public Player Player
    {
        get { return m_player; }
    }

    public Transform m_heldItemPos;

    //Which controller is this hero using
    public int m_controllerId = 1;

    public GameObject m_grab;

    //Collided player position
    public Transform m_collidedPlayer;
    //Is the player holding an item
    private bool m_holdingItem = false;
    //The held pickup (null if not holding a trashbag)
    private PickUp m_heldTrashItem;

    private float slowPlayerBoost = 1f;

    private float flyingSpeedPerPress = 300f;
    private float flyingSpeedHold = 100f;

    #region Jumping/flying
    private int m_jumpCount = 0;
    private int m_maxJumpCount = 2;

    private bool m_inPickupAnimation = false;
    private bool m_inThrowAnimation = false;

    #endregion

    void Start()
    {
        //Get Hero power component
        m_heroPower = gameObject.GetComponent<HeroPower>();
        //Initialise input
        m_player = ReInput.players.GetPlayer(m_controllerId);
    }

    public override void Update()
    {
        horizontalMovement = false;
        verticalMovement = false;

        //Power check
        if (m_flying)
        {
            HitObject(0.3f);
            m_heroPower.m_heroPower -= m_heroPower.m_flyingPowerDrainPerSec * Time.deltaTime;
        }

        if (m_heroPower.m_heroPower <= 0f && m_flying)
        {
            m_flying = false;
            m_anim.SetBool("CanFly", false);
            m_heroPower.m_heroPower = 0f;
        }

        #region z-depth Calculations
        //Calculate the y position from the z-depth
        float yPos = Mathf.Lerp(m_minZDepth, m_maxZDepth + m_minZDepth, m_zDepth);
        //Set the y position of the hero
        //Create a modifier value to flip scale based on direction
        m_renderer.flipX = !m_facingRight;
        if (m_facingRight)
        {
            m_fistOffset = new Vector2(Mathf.Abs(m_fistOffset.x), m_fistOffset.y);
            m_heldItemPos.localPosition = new Vector2(-Mathf.Abs(m_heldItemPos.transform.localPosition.x), m_heldItemPos.transform.localPosition.y);
        }
        else
        {
            m_fistOffset = new Vector2(-Mathf.Abs(m_fistOffset.x), m_fistOffset.y);
            m_heldItemPos.localPosition = new Vector2(Mathf.Abs(m_heldItemPos.transform.localPosition.x), m_heldItemPos.transform.localPosition.y);
        }
        //Scale shadow based on z-depth and flip if facing left
        //Scale shadow based on z-depth
        #endregion

        #region Input

        Input();

        #endregion


        #region Slow player boost
        if (m_velocity.x > 0f && CameraController.Instance.m_averagePlayerPos.x > gameObject.transform.position.x + 1f || m_velocity.x < 0f && CameraController.Instance.m_averagePlayerPos.x < gameObject.transform.position.x - 1f)
        {
            slowPlayerBoost = 1.25f;
        }
        #endregion

        #region Animation speed
        //Animation
        float animSpeed = Mathf.Abs(m_velocity.x) * 10f;
        if (verticalMovement)
        {
            animSpeed += 10f;
        }
        m_anim.SetFloat("Speed", animSpeed);
        m_anim.SetFloat("VerticalSpeed", m_velocity.y);
        #endregion


        //HACKY WORKAROUND
        if (m_heldTrashItem != null)
        {
            if (m_anim.GetFloat("Speed") > 1f)
            {
                m_heldTrashItem.m_anim.SetBool("Carried", true);
            }
            else
            {
                m_heldTrashItem.m_anim.SetBool("Carried", false);
            }
        }

        base.Update();
    }

    public override void LateUpdate()
    {
        //m_fistOffset = m_fistOffset - (new Vector3((m_zDepth * 0.2f), 0f, 0f));
        base.LateUpdate();
    }

    private void Update_Input()
    {

    }

    /// <summary>
    /// Joystick/Keyboard Input.
    /// </summary>
    public void Input()
    {
        if (EnableMovement())
        {
            if (!m_currentlyInHitAnimation)
            {
                if (m_player.GetAxis("MoveHorizontal") > 0.1f || m_player.GetButton("Left"))
                {
                    m_direction = Direction.Left;
                    Move(m_direction);
                }
                if (m_player.GetAxis("MoveHorizontal") < -0.1f || m_player.GetButton("Right"))
                {
                    m_direction = Direction.Right;
                    Move(m_direction);
                }
                if (m_player.GetAxis("MoveVertical") > 0.1f || m_player.GetButton("Up"))
                {
                    m_direction = Direction.Up;
                    Move(m_direction);
                }
                if (m_player.GetAxis("MoveVertical") < -0.1f || m_player.GetButton("Down"))
                {
                    m_direction = Direction.Down;
                    Move(m_direction);
                }
            }

            if (transform.position.y <= TargetYPos)
            {
                Land();
            }

            if (m_grounded)
            {
                if (m_player.GetButtonDown("Punch") && !m_holdingItem)
                {
                    Punch();
                }

                if (m_player.GetButtonDown("Grab"))
                {
                    if (!m_holdingItem)
                    {
                        Grab();
                    }
                    else
                    {
                        if (m_heldTrashItem.GetComponent<TrashBag>() != null)
                        {
                            Throw();
                        }
                    }
                }
            }
            if (m_player.GetButtonDown("Jump"))
            {
                if (!m_flying && m_heroPower.m_canFly && m_jumpCount == 1 && m_heroPower.m_heroPower > m_heroPower.m_flyingMinPower)
                {
                    Fly();
                }
                else if (!m_flying && m_jumpCount < m_maxJumpCount)
                {
                    Jump();
                }
                else if (m_flying)
                {
                    m_velocity.y += flyingSpeedPerPress * Time.deltaTime;
                }
            }
            else if (m_player.GetButton("Jump"))
            {
                if (m_flying)
                {
                    m_velocity.y += flyingSpeedHold * Time.deltaTime;
                }
            }

        }

    }

    /// <summary>
    /// Enables the movement.
    /// </summary>
    /// <returns><c>true</c>, if movement was enabled, <c>false</c> otherwise.</returns>
    public bool EnableMovement()
    {
        if (!m_punching && !m_inPickupAnimation && !m_inThrowAnimation)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Move the Hero.
    /// </summary>
    /// <param name="movement">Movement.</param>
    public override void Move(Direction direction)
    {
        switch (direction) {

            case Direction.Left:
                {
                    //Move left
                    Vector3 addForce = Vector3.right * m_horizontalSpeed;

                    if (m_flying && m_velocity.x < 0f) {
                        m_velocity.x = 0f;
                    }
                    m_velocity += addForce;
                    m_facingRight = true;
                    horizontalMovement = true;

                }
                break;

            case Direction.Right:
                {
                    //Move right
                    Vector3 addForce = Vector3.right * m_horizontalSpeed;
                    if (m_flying && m_velocity.x > 0f) {
                        m_velocity.x = 0f;
                    }
                    m_velocity -= addForce;
                    m_facingRight = false;
                    horizontalMovement = true;

                }
                break;

            case Direction.Up:
                {
                    //Move up
                    m_zDepth += m_verticalSpeed * Time.deltaTime;
                    verticalMovement = true;
                }
                break;

            case Direction.Down:
                {
                    //Move down
                    m_zDepth -= m_verticalSpeed * Time.deltaTime;
                    verticalMovement = true;
                }
                break;

            case Direction.None:
                {
                    //m_velocity = Vector3.zero;

                }
                break;
        }

    }

    /// <summary>
    /// Hero fly .
    /// </summary>
    public void Fly()
    {
        m_flying = true;
        m_velocity.y = 3f;
        m_grounded = false;
        m_anim.SetBool("Grounded", false);
        m_anim.SetBool("CanFly", true);

    }

    public override void Hit(Actor source, float damage = 30)
    {
        StartCoroutine(Vibrate(0.3f));
        base.Hit(source, damage);
    }

    ///Vibration

    public IEnumerator Vibrate(float power = 1f, float duration = 0.1f)
    {
        foreach (Joystick j in Player.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            j.SetVibration(power, power);
        }
        yield return new WaitForSeconds(duration);

        // Stop vibration
        foreach (Joystick j in Player.controllers.Joysticks)
        {
            j.StopVibration();
        }
    }


    /// <summary>
    /// Hero Jump.
    /// </summary>
    public void Jump()
    {
        m_jumpCount++;
        m_velocity.y = 9f;
        m_grounded = false;
        m_anim.SetBool("Grounded", false);

    }

    /// <summary>
    /// Hero Ground Land.
    /// </summary>
    public void Land()
    {
        m_anim.SetBool("Grounded", true);
        m_anim.SetBool("CanFly", false);
        m_grounded = true;
        m_flying = false;
        m_velocity.y = 0f;
        m_jumpCount = 0;
    }

    /// <summary>
    /// Hero Punch.
    /// </summary>
    public void Punch()
    {
        m_anim.SetBool("Punch", true);
        m_velocity.x = 0f;
        m_velocity.y = 0f;
        m_punching = true;

        HitObject(0.1f);
        StartCoroutine(DelayedStopPunch());
    }

    /// <summary>
    /// Grab objects with the "pickups" tag.
    /// </summary>
    public void Grab()
    {
        m_inPickupAnimation = true;
        StartCoroutine(NotInPickupAnimation());
        m_anim.SetBool("Grab", true);
        Collider2D _other = Physics2D.OverlapCircle((Vector2)m_grab.transform.position, 0.4f, LayerMask.GetMask(new string[1] { "Pickups" }));
        if (_other != null)
        {

            PickUp isPickUp = _other.gameObject.GetComponent<PickUp>();
            if (isPickUp != null)
            {
                if (isPickUp.tag != "Fruit") {
                    m_anim.SetBool("Carrying", true);
                    m_holdingItem = true;
                    m_velocity = Vector3.zero;
                }

                isPickUp.Pickup(this);
                m_heldTrashItem = isPickUp;

            }
        }
    }

    /// <summary>
    /// Throw currently picked up object.
    /// </summary>
    public void Throw()
    {
        if (m_heldTrashItem != null)
        {
            m_inThrowAnimation = true;
            StartCoroutine(NotInThrowAnimation());
            m_holdingItem = false;
            m_anim.SetBool("Carrying", false);
            m_anim.SetTrigger("Throw");
            StartCoroutine(m_heldTrashItem.GetComponent<TrashBag>().ThrowAfterDelay(m_facingRight));
            m_heldTrashItem = null;
        }

    }

    public override void HitObject(float fistWidth)
    {
        Collider2D[] _others = Physics2D.OverlapCircleAll(transform.position + new Vector3(m_fistOffset.x, m_fistOffset.y, 0f), 0.4f, LayerMask.GetMask(new string[1] { "HitBox" }));
        float hitMultiplier = 0f;
        for (int i = 0; i < _others.Length; i++)
        {
            Collider2D _other = _others[i];

            if (_other != null)
            {
                Actor isActor = _other.transform.parent.GetComponent<Actor>();

                if (isActor != null && isActor != this)
                {
                    if (isActor.GetComponent<TrashBag>() != null)
                    {

                        if (isActor.GetComponent<TrashBag>().count < 1)
                        {
                            if (Mathf.Abs(m_zDepth - isActor.m_zDepth) <= fistWidth)
                            {
                                isActor.Hit(this);
                                hitMultiplier += 0.2f;
                            }
                        }
                    }
                    else
                    {
                        //If the hero hit is within punch width
                        if (Mathf.Abs(m_zDepth - isActor.m_zDepth) <= fistWidth)
                        {
                            isActor.Hit(this);
                            hitMultiplier += 0.1f;
                        }
                    }
                }
            }
        }

        if (hitMultiplier != 0f)
        {
            StartCoroutine(Vibrate(hitMultiplier, 0.1f));
            PlayHitParticle();
        }
    }

    public void PlayHitParticle()
    {
        ParticleSystem ps = RandomHit;
        ps.transform.position = transform.position + m_fistOffset;
        ps.startColor = m_heroColor;
        ps.enableEmission = true;
        ps.Emit(1);
    }

    /// <summary>
    /// Delayes the punch.
    /// </summary>
    /// <returns>The stop punch.</returns>
    IEnumerator DelayedStopPunch()
    {
        yield return new WaitForSeconds(0.30f);
        //m_anim.SetBool("Punch", false);
        m_punching = false;
    }

    /// <summary>
    /// Set Pickup animation end.
    /// </summary>
    /// <returns>The in pickup animation.</returns>
    IEnumerator NotInPickupAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        m_inPickupAnimation = false;
    }

    /// <summary>
    /// Set Throw animation end.
    /// </summary>
    /// <returns>The in throw animation.</returns>
    IEnumerator NotInThrowAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        m_inThrowAnimation = false;
    }

    /// <summary>
    /// Updates the position.
    /// </summary>
    public override void UpdatePosition()
    {
        if (m_grounded && !m_punching)
        {
            bool canMove = true;
            if (IsColliding())
            {
                if (m_velocity.x < 0 && transform.position.x > m_collidedActor.transform.position.x)
                {
                    canMove = false;
                }
                else if (m_velocity.x > 0 && transform.position.x < m_collidedActor.transform.position.x)
                {
                    canMove = false;
                }
            }
            else
            {
                canMove = true;
            }
            if (canMove)
            {
                transform.position = new Vector3(transform.position.x + (m_velocity.x * slowPlayerBoost * Time.deltaTime), TargetYPos, 0f);
            }
        }
        else if (!m_punching)
        {
            transform.position = new Vector3(transform.position.x + (m_velocity.x * slowPlayerBoost * Time.deltaTime), transform.position.y + (m_velocity.y * Time.deltaTime), 0f);

            //Height Check
            if (transform.position.y > m_maxHeight)
            {
                m_velocity.y = 0f;
                transform.position = new Vector3(transform.position.x, m_maxHeight, transform.position.z);
            }

        }
    }

    /// <summary>
    /// Applies the drag force.
    /// </summary>
    protected override void ApplyDragForce()
    {
        if (!horizontalMovement)
        {
            if (m_velocity.x > 0.1f)
            {
                if (m_flying)
                {
                    m_velocity += new Vector3(-m_horizontalDrag * m_flyingHorizontalDragModifier, 0f, 0f) * Time.deltaTime;
                }
                else
                {
                    m_velocity += new Vector3(-m_horizontalDrag, 0f, 0f) * Time.deltaTime;
                }
            }
            else if (m_velocity.x < -0.1f)
            {
                if (m_flying)
                {
                    m_velocity += new Vector3(m_horizontalDrag * m_flyingHorizontalDragModifier, 0f, 0f) * Time.deltaTime;
                }
                else
                {
                    m_velocity += new Vector3(m_horizontalDrag, 0f, 0f) * Time.deltaTime;
                }
            }
            else
            {
                m_velocity = new Vector3(0f, m_velocity.y, 0f);
            }
        }
        //If not moving vertically apply drag force
        if (m_velocity.y > -m_maxVerticalVelocity && !m_grounded)
        {
            if (m_flying)
            {
                if (transform.position.y > m_minHeightHeavyVerticalDrag)
                {
                    m_velocity += new Vector3(0f, -m_verticalDrag * Mathf.Lerp(m_flyingVerticalDragModifier, m_verticalDragModifierMax, (transform.position.y - m_minHeightHeavyVerticalDrag) / (m_maxHeight - m_minHeightHeavyVerticalDrag)), 0f) * Time.deltaTime;
                }
                else
                {
                    m_velocity += new Vector3(0f, -m_verticalDrag * m_flyingVerticalDragModifier, 0f) * Time.deltaTime;
                }
            }
            else
            {
                m_velocity += new Vector3(0f, -m_verticalDrag, 0f) * Time.deltaTime;
            }
        }
    }

    public ParticleSystem RandomHit
    {
        get{
            return m_hitParticleSystems[Random.Range(0, m_hitParticleSystems.Length)];
        }
    }
}
