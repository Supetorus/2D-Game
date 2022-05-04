using UnityEngine;

public class Crouching : CharacterMoveState
{
	private bool wasMoving = false;
	private bool isMoving = false;

	public override void EnterState()
	{
		if (Mathf.Abs(c.pi.lateralMovement) > 0.01f)
		{
			animator.Play("Crouching");
			isMoving = true;
		}
		else
		{
			animator.Play("Crouched");
			isMoving = false;
		}

		c.standingCollider.SetActive(false);
		c.crouchingCollider.SetActive(true);
	}

	public override void ExitState()
	{
		c.standingCollider.SetActive(true);
		c.crouchingCollider.SetActive(false);
	}

	public override void FixedUpdateState()
	{
		OrientCharacter();
		ApplyLateralMovement(c.crouchSpeed);
	}

	public override void UpdateState()
	{
		// Falling
		if (!c.isGrounded)
		{
			c.ChangeState(c.falling);
			return;
		}

		// Only check for standing actions if standing is possible.
		if (!c.isCrouchBlocked)
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

			// Jumping
			if (c.pi.jumpPressed)
			{
				c.ChangeState(c.jumping);
				return;
			}

			// Running
			if (!c.pi.doCrouch && isMoving)
			{
				c.ChangeState(c.running);
				return;
			}

			// Idling
			if (!c.pi.doCrouch && !isMoving)
			{
				c.ChangeState(c.idling);
				return;
			}
		}

		wasMoving = isMoving;
		isMoving = Mathf.Abs(c.pi.lateralMovement) > 0.01f;
		// Crouching (moving)
		if (isMoving && !wasMoving)
		{
			animator.Play("Crouching");
		}
		// Crouched (not moving)
		else if (!isMoving && wasMoving)
		{
			animator.Play("Crouched");
		}
	}

	public override string ToString()
	{
		return "Crouching";
	}
}