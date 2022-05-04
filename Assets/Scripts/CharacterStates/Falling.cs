using UnityEngine;

public class Falling : CharacterMoveState
{
	public override void EnterState()
	{
		animator.Play("Falling");
	}

	public override void ExitState()
	{

	}

	public override void FixedUpdateState()
	{
		ApplyLateralMovement(c.airSpeed);
	}

	public override void UpdateState()
	{
		// Meleeing
		if (c.pi.doMelee)
		{
			c.ChangeState(c.meleeing);
			return;
		}

		// Dashing
		if (c.pi.doDash)
		{
			c.ChangeState(c.dashing);
			return;
		}

		if (c.isGrounded)
		{
			// Crouching
			if (c.pi.doCrouch)
			{
				c.ChangeState(c.crouching);
				return;
			}

			// Running
			if (Mathf.Abs(c.pi.lateralMovement) > 0.01f)
			{
				c.ChangeState(c.runningOnGround);
				return;
			}

			// Jumping
			if (c.pi.jumpPressed)
			{
				c.ChangeState(c.jumping);
				return;
			}

			// Idling
			c.ChangeState(c.idling);
			return;
		}
	}
	public override string ToString()
	{
		return "Falling";
	}
}
