using UnityEngine;

public class Running : CharacterMoveState
{
	public override void EnterState()
	{
		animator.Play("Running");
	}

	public override void FixedUpdateState()
	{
		ApplyLateralMovement(c.runSpeed);
	}

	public override string ToString()
	{
		return "Running";
	}
}