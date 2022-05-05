using UnityEngine;

public class WallSliding : CharacterMoveState
{
	public override void EnterState()
	{
		if (rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);
		OrientCharacter();
		animator.Play("WallSliding");
	}

	public override void ExitState()
	{
		
	}

	public override void FixedUpdateState()
	{
		
	}

	public override void UpdateState()
	{
		base.UpdateState();
	}

	public override string ToString()
	{
		return "WallSliding";
	}
}
