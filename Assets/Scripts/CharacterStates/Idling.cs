using UnityEngine;

public class Idling : CharacterMoveState
{
	public override void EnterState()
	{
		animator.Play("Idling");
	}

	public override void ExitState()
	{

	}

	public override void FixedUpdateState()
	{

	}

	public override void UpdateState()
	{
		// Fall
		if (!c.isGrounded)
		{
			c.ChangeState(c.falling);
			return;
		}

		// Jump
		if (c.pi.jumpPressed)
		{
			c.ChangeState(c.jumping);
			return;
		}

		// Run
		if (Mathf.Abs(c.pi.lateralMovement) > 0.01f)
		{
			c.ChangeState(c.runningOnGround);
			return;
		}

		// Dash
		if (c.pi.doDash)
		{
			c.ChangeState(c.crouching);
			return;
		}

		// Crouch
		if (c.pi.doCrouch)
		{
			c.ChangeState(c.crouching);
			return;
		}

		// Meleeing
		if (c.pi.doMelee)
		{
			c.ChangeState(c.meleeing);
			return;
		}
	}
	public override string ToString()
	{
		return "Idling";
	}
}
