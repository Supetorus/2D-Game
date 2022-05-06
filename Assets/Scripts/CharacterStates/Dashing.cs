using UnityEngine;

public class Dashing : CharacterMoveState
{
	public float dashForce;
	public float dashDistance;

	private float normalGravity;
	private Vector2 startLocation;
	public override void EnterState()
	{
		//Debug.Break();
		if (c.isWall)
		{
			Flip();
			c.canAirDash = true;
		}
		else if (!c.isGrounded) c.canAirDash = false;
		normalGravity = rb.gravityScale;
		rb.gravityScale = 0;
		animator.Play("Dashing");
		startLocation = transform.position;
		rb.velocity = new Vector2(c.isFacingRight ? dashForce : -dashForce, 0);
		//rb.AddForce(, ForceMode2D.Impulse);
	}

	public override void ExitState()
	{
		rb.velocity = Vector2.zero;
		rb.gravityScale = normalGravity;
	}
	public override void UpdateState()
	{
		float sqrDistance = (new Vector2(transform.position.x, transform.position.y) - startLocation).sqrMagnitude;
		if (sqrDistance > Mathf.Pow(dashDistance, 2) || Mathf.Abs(rb.velocity.x) < Mathf.Abs(dashForce * 0.8f))
		{
			if (!c.isGrounded) c.ChangeState(c.falling);
			c.ChangeState(c.idling);
		}

		//if (c.isWall)
		//{
		//	c.ChangeState(c.wallSliding);
		//}
	}

	public override string ToString()
	{
		return "Dashing";
	}
}
