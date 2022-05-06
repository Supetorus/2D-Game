using UnityEngine;

public class Falling : CharacterMoveState
{
	public override void EnterState()
	{
		animator.Play("Falling");
	}

	public override void FixedUpdateState()
	{
		ApplyLateralMovement(c.airSpeed);
	}

	public override string ToString()
	{
		return "Falling";
	}
}
