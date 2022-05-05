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

	public override string ToString()
	{
		return "Idling";
	}
}
