using System.Collections;
using UnityEngine;

public class Jumping : CharacterMoveState
{
	[SerializeField, Tooltip("Maximum amount of force added while the player is in the air")]
	public float maxJumpForce = 60f;
	[SerializeField, Tooltip("Amount of force added when the player jumps.")]
	public float impulseJumpForceY = 10f;
	[SerializeField, Tooltip("The amount of time the jump can continue to gain force (in seconds).")]
	public float airTime = 0.4f;

	private float floatForce; // The amount of force applied based on how long player has been in air.
	private float jumpTime; // The amount of time the player has been in the air.
							//private bool canDoubleJump = true; //If player can double jump
	private bool jumpCanceled = false; // If the player has pressed then released jump.

	public override void EnterState()
	{
		//print(c.secondJumpMultiplier);
		if (c.isGrounded && canCheckGround)
		{ // Ground jump
			c.canDoubleJump = true;
			Jump(1f);
		}
		else
		{ // Air jump
			AirJump();
		}
	}

	IEnumerator GroundCheckTimer()
	{
		canCheckGround = false;
		//Won't check ground again until either left the ground entirely, or has stopped ascending (such as if the player hits their head on the ceiling while still on the ground
		yield return new WaitWhile(() => c.isGrounded || rb.velocity.y > 0);
		canCheckGround = true;
	}

	public override void ExitState()
	{

	}

	public override void FixedUpdateState()
	{
		//Debug.Break();
		if (!jumpCanceled)
		{
			// Calculate float force
			jumpTime += Time.fixedDeltaTime;
			floatForce = Mathf.Max(maxJumpForce * (1 - (jumpTime / airTime)), 0);
			rb.AddForce(new Vector2(0f, floatForce));// * Time.fixedDeltaTime));
		}
		ApplyLateralMovement(c.airSpeed);
	}

	public override void UpdateState()
	{
		if (!c.pi.jumpHeld) jumpCanceled = true;
		base.UpdateState();

		//// Melee
		//if (c.pi.doMelee)
		//{
		//	c.ChangeState(c.meleeing);
		//	return;
		//}

		//// Dashing
		//if (c.pi.doDash)
		//{
		//	c.ChangeState(c.dashing);
		//	return;
		//}

		//// Jumping (double jump)
		//if (c.canDoubleJump && c.pi.jumpPressed)
		//{
		//	AirJump();
		//}

		//if (canCheckGround && c.isGrounded)
		//{
		//	// Running
		//	if (Mathf.Abs(c.pi.lateralMovement) > 0.01f)
		//	{
		//		c.ChangeState(c.running);
		//		return;
		//	}
		//	// Idling
		//	else
		//	{
		//		c.ChangeState(c.idling);
		//		return;
		//	}
		//}

		//// WallSliding
		//if (c.wallPressing)
		//{
		//	c.ChangeState(c.wallSliding);
		//	return;
		//}

		//// Falling
		//if (rb.velocity.y < 0)
		//{
		//	c.ChangeState(c.falling);
		//	return;
		//}

		//OrientCharacter();
	}

	private void Jump(float forceMultiplier)
	{
		jumpTime = 0;
		jumpCanceled = false;
		c.isGrounded = false;
		animator.Play("Jump");
		c.jumpParticles.Play();
		rb.AddForce(new Vector2(0f, impulseJumpForceY * forceMultiplier), ForceMode2D.Impulse);

		StartCoroutine(GroundCheckTimer());
	}

	private void AirJump()
	{
		c.canDoubleJump = false;
		rb.velocity = new Vector2(rb.velocity.x, 0);
		Jump(c.secondJumpMultiplier);
	}

	public override string ToString()
	{
		return "Jumping";
	}
}