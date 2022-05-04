using UnityEngine;

public class Running : CharacterMoveState
{
	public override void EnterState()
	{
		animator.Play("Running");
	}

	public override void ExitState()
	{

	}

	public override void FixedUpdateState()
	{
		ApplyLateralMovement(c.groundSpeed);
	}

	public override void UpdateState()
	{
		// Falling
		if (!c.isGrounded)
		{
			c.ChangeState(c.falling);
			return;
		}

		// Jumping
		if (c.pi.jumpPressed)
		{
			c.ChangeState(c.jumping);
			return;
		}

		// Crouching
		if (c.pi.doCrouch)
		{
			c.ChangeState(c.crouching);
			return;
		}

		// Dashing
		if (c.pi.doDash)
		{
			c.ChangeState(c.dashing);
			return;
		}

		// Meleeing
		if (c.pi.doMelee)
		{
			c.ChangeState(c.meleeing);
			return;
		}

		// Lateral movement
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
		else
		{
			c.ChangeState(c.idling);
			return;
		}
	}
	public override string ToString()
	{
		return "Running";
	}
}