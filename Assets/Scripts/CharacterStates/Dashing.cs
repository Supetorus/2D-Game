public class Dashing : CharacterMoveState
{
	public override void EnterState()
	{
		throw new System.NotImplementedException();
	}

	public override void ExitState()
	{
		throw new System.NotImplementedException();
	}

	public override void FixedUpdateState()
	{
		throw new System.NotImplementedException();
	}

	public override void UpdateState()
	{

		OrientCharacter();
	}
	public override string ToString()
	{
		return "Dashing";
	}
}
