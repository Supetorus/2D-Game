using UnityEngine;

public class WallSliding : CharacterMoveState
{
	[SerializeField] private float maxSlideVelocity;

	public override void EnterState()
	{
		//if (rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);
		OrientCharacter();
		animator.Play("WallSliding");
	}

	public override void ExitState()
	{

	}

	public override void FixedUpdateState()
	{
		// Slow sliding
		if (rb.velocity.y < 0 && Mathf.Abs(rb.velocity.y) > maxSlideVelocity)
		{
			rb.velocity = new Vector2(rb.velocity.x, -maxSlideVelocity);
		}
		ApplyLateralMovement(1f);
	}

	public override void UpdateState()
	{
		OrientCharacter();

		if (c.isGrounded)
		{
			// Crouching
			if (c.pi.down)
			{
				c.ChangeState(c.crouching);
				return;
			}

			// Jumping
			if (c.pi.jumpPressed)
			{
				c.ChangeState(c.jumping);
				return;
			}
		}

		// Attack
		if (c.pi.doMelee)
		{
			Flip();
			c.ChangeState(c.meleeing);
			return;
		}

		// Dash
		if (c.pi.doDash)
		{
			Flip();
			c.ChangeState(c.dashing);
			return;
		}

		// Wall Jump
		if (c.pi.jumpPressed)
		{
			Flip();
			c.ChangeState(c.wallJumping);
			return;
		}

		// Falling
		if (c.pi.down || !c.isWall)
		{
			c.ChangeState(c.falling);
			return;
		}

		// Idling
		if (c.isGrounded)
		{
			c.ChangeState(c.idling);
			return;
		}
	}

	public override string ToString()
	{
		return "WallSliding";
	}
}
