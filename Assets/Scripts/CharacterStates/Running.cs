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
		ApplyLateralMovement(c.runSpeed);
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

		if (Mathf.Abs(c.pi.lateralMovement) < 0.01f)
		{
			c.ChangeState(c.idling);
			return;
		}

		OrientCharacter();
	}

	public override string ToString()
	{
		return "Running";
	}
}