using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterMoveState : MonoBehaviour
{
	//Components
	[HideInInspector] protected Rigidbody2D rb;
	[HideInInspector] protected Animator animator;
	[HideInInspector] protected Health health;
	[HideInInspector] protected CharacterController2D c;
	[HideInInspector] protected oldController controller;

	protected void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		health = GetComponent<Health>();
		c = GetComponent<CharacterController2D>();
		controller = GetComponent<oldController>();
	}

	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void FixedUpdateState();
	public abstract void UpdateState();

	/// <summary>
	/// Flip the player sprite horizontally
	/// </summary>
	protected void Flip()
	{
		// Switch the way the player is labelled as facing.
		c.isFacingRight = !c.isFacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	/// <summary>
	/// Changes the character's velocity according to the current controls and the speedMultiplier.
	/// </summary>
	/// <param name="speedMultiplier"></param>
	protected void ApplyLateralMovement(float speedMultiplier)
	{
		if ((!c.isGrounded && !c.airControlAllowed) || Mathf.Abs(c.pi.lateralMovement) < 0.01f) return;
		// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(c.pi.lateralMovement * speedMultiplier, rb.velocity.y);
		//And then smoothing it out and applying it to the character
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref c.velocity, c.movementSmoothing);
	}

	/// <summary>
	/// Checks if the player is facing the way they are moving.
	/// If input is opposite of movement then character is flipped.
	/// </summary>
	protected void OrientCharacter()
	{
		if (Mathf.Abs(c.pi.lateralMovement) > 0.01f)
		{
			// If the input is moving the player right and the player is facing left...
			if (c.pi.lateralMovement > 0.1f && !c.isFacingRight)
			{
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (c.pi.lateralMovement < 0.1f && c.isFacingRight)
			{
				Flip();
			}
		}
	}
}
