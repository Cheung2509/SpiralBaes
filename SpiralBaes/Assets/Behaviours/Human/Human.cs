using UnityEngine;
using System.Collections;

/// <summary>
/// Humanoid.
/// </summary>
public class Human : Actor 
{
	//Is the player flying
	public bool m_flying = false;
	//Is the player punching
	public bool m_punching = false;
	//Is the human alive?
	//public bool m_alive = false;

	protected const int m_jumpCount = 0;
	protected const int m_maxJumpCount = 2;

	//Modifier applied to vertical drag force if flying
	protected const float m_flyingVerticalDragModifier = 1f;

	//Modifier applied to horizontal drag force if flying
	protected const float m_flyingHorizontalDragModifier = 1f;

	//Is the hero currently moving horizontally
	protected bool horizontalMovement = false;
	//Is the hero currently moving vertically
	protected bool verticalMovement = false;

	/// <summary>
	/// Move the specified Human.
	/// </summary>
	/// <param name="movement">Movement.</param>
	public override void Move(Direction direction)
	{

    }
    public virtual void HitObject(float fistWidth = 0.3f)
    {
        Collider2D[] _others = Physics2D.OverlapCircleAll(transform.position + new Vector3(m_fistOffset.x, m_fistOffset.y, 0f), 0.4f, LayerMask.GetMask(new string[1] { "HitBox" }));

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
                            }
                        }
                    }
                    else
                    {
                        //If the hero hit is within punch width
                        if (Mathf.Abs(m_zDepth - isActor.m_zDepth) <= fistWidth)
                        {
                            isActor.Hit(this);
                        }
                    }
                }
            }
        }

    }

}
