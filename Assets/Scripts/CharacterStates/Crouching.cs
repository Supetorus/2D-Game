using UnityEngine;

public class Crouching : CharacterMoveState
{

	public override void EnterState()
	{
		if (Mathf.Abs(c.pi.lateralMovement) > 0.01f)
		{
			animator.Play("Crouching");
		}
		else
		{
			animator.Play("Crouched");
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
		// Crouching (moving)
		if (c.isMoving && !c.wasMoving)
		{
			animator.Play("Crouching");
		}
		// Crouched (not moving)
		else if (!c.isMoving && c.wasMoving)
		{
			animator.Play("Crouched");
		}
		base.UpdateState();
	}

	public override string ToString()
	{
		return "Crouching";
	}
}