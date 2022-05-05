using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumping : CharacterMoveState
{
	[SerializeField, Tooltip("How much force is added when the player jumps off a wall")]
	public Vector2 jumpForce;
	[SerializeField, Tooltip("Maximum amount of force added while the player is in the air")]
	public Vector2 maxFloatForce;
	[SerializeField, Tooltip("The amount of time the jump can continue to gain force (in seconds).")]
	public float airTime = 0.4f;

	private Vector2 floatForce; // The amount of force applied based on how long player has been in air.
	private float jumpTime; // The amount of time the player has been in the air.
							//private bool canDoubleJump = true; //If player can double jump
	private bool jumpCanceled = false; // If the player has pressed then released jump.

	private Vector2 flipLeft = new Vector2(-1, 1);

	public override void EnterState()
	{
		// Should already be flipped.
		jumpTime = 0;
		jumpCanceled = false;
		c.isGrounded = false;
		c.isWall = false;
		animator.Play("Jump");
		c.canDoubleJump = true;

		rb.AddForce(c.isFacingRight ? jumpForce : jumpForce * flipLeft, ForceMode2D.Impulse);

		StartCoroutine(IgnoreLateralInput());
	}

	public override void ExitState()
	{

	}

	public override void FixedUpdateState()
	{
		if (!jumpCanceled)
		{
			// Calculate float force
			jumpTime += Time.fixedDeltaTime;
			floatForce = jumpForce * (1 - (jumpTime / airTime));
			rb.AddForce(c.isFacingRight ? floatForce : floatForce * flipLeft);
		}
		ApplyLateralMovement(c.airSpeed);
	}

	public override void UpdateState()
	{
		if (!c.pi.jumpHeld) jumpCanceled = true;
		if (c.pi.jumpPressed)
		{
			c.ChangeState(c.jumping);
			return;
		}
		base.UpdateState();
	}

	public override string ToString()
	{
		return "Wall Jumping";
	}

	private IEnumerator IgnoreLateralInput()
	{
		c.pi.ignoreLateralInput = true;
		yield return new WaitForSeconds(0.1f);
		c.pi.ignoreLateralInput = false;
	}
}
