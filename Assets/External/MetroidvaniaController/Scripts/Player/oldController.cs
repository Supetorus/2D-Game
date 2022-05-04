using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(CharacterController2D))]
public class oldController : MonoBehaviour
{
	private CharacterController2D c;

	

	private void Start()
	{
		c = GetComponent<CharacterController2D>();
	}

	private void FixedUpdate()
	{


		//if (c.limitVelOnWallJump)
		//{
		//	if (c.m_Rigidbody2D.velocity.y < -0.5f)
		//		c.limitVelOnWallJump = false;
		//	c.jumpWallDistX = (c.jumpWallStartX - transform.position.x) * transform.localScale.x;
		//	if (c.jumpWallDistX < -0.5f && c.jumpWallDistX > -1f)
		//	{
		//		//canMove = true;
		//	}
		//	else if (c.jumpWallDistX < -1f && c.jumpWallDistX >= -2f)
		//	{
		//		//canMove = true;
		//		c.m_Rigidbody2D.velocity = new Vector2(10f * transform.localScale.x, c.m_Rigidbody2D.velocity.y);
		//	}
		//	else if (c.jumpWallDistX < -2f)
		//	{
		//		c.limitVelOnWallJump = false;
		//		c.m_Rigidbody2D.velocity = new Vector2(0, c.m_Rigidbody2D.velocity.y);
		//	}
		//	else if (c.jumpWallDistX > 0)
		//	{
		//		c.limitVelOnWallJump = false;
		//		c.m_Rigidbody2D.velocity = new Vector2(0, c.m_Rigidbody2D.velocity.y);
		//	}
		//}
	}

	public void Move(float move, bool jumpPressed, bool jumpHeld, bool dash, bool crouch)
	{


		//// Freeze player movement if dead.
		//if (c.health.CurrentH <= 0)
		//{
		//	c.m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
		//	return;
		//}

		//// Turn off jumping animation if on the ground and not jumping.
		//if (c.isGrounded && !jumpPressed && !jumping) c.animator.SetBool("IsJumping", false);

		//// Forces player to remain crouching if there is an obstruction overhead.
		//if (!crouch)
		//{
		//	// If the character has a ceiling preventing them from standing up, keep them crouching
		//	if (Physics2D.OverlapCircle(c.ceilingCheckPosition.position, c.ceilingCheckRadius, c.groundLayers) && c.m_wasCrouching)
		//	{
		//		crouch = true;
		//	}
		//}

		//// If crouching
		//if (crouch && !c.isWallSliding)
		//{
		//	if (!c.m_wasCrouching)
		//	{
		//		c.m_wasCrouching = true;
		//		c.OnCrouchEvent.Invoke(true);
		//		c.animator.SetBool("IsCrouching", true);
		//		StartCoroutine(CrouchCooldown());
		//	}

		//	// Reduce the speed by the crouchSpeed multiplier
		//	move *= c.m_CrouchSpeed;

		//	// Disable one of the colliders when crouching
		//	if (c.m_CrouchDisableCollider != null)
		//		c.m_CrouchDisableCollider.enabled = false;
		//	// Enables one of the colliders when crouching
		//	if (c.m_CrouchEnableCollider != null)
		//		c.m_CrouchEnableCollider.enabled = true;
		//}
		//else if (!c.timeLockedCrouching)
		//{
		//	// Enable the collider when not crouching
		//	if (c.m_CrouchDisableCollider != null)
		//		c.m_CrouchDisableCollider.enabled = true;
		//	// Disable one of the colliders when crouching
		//	if (c.m_CrouchEnableCollider != null)
		//		c.m_CrouchEnableCollider.enabled = false;

		//	if (c.m_wasCrouching)
		//	{
		//		c.m_wasCrouching = false;
		//		c.OnCrouchEvent.Invoke(false);
		//		c.animator.SetBool("IsCrouching", false);
		//	}
		//}

		//if (dash && c.canDash && !c.isWallSliding)
		//{
		//	//m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_DashForce, 0f));
		//	StartCoroutine(DashCooldown());
		//}

		////if currently dashing.
		//if (c.isDashing)
		//{
		//	c.m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * c.m_DashForce, 0);
		//}

		////only control the player if grounded or airControl is turned on
		//else if (c.isGrounded || c.m_AirControl)
		//{
		//	if (c.m_Rigidbody2D.velocity.y < -c.limitFallSpeed)
		//		c.m_Rigidbody2D.velocity = new Vector2(c.m_Rigidbody2D.velocity.x, -c.limitFallSpeed);





		//// If the player should jump...
		//if (c.isGrounded && jumpPressed)
		//{
		//	// Add a vertical force to the player.
		//	c.animator.SetBool("IsJumping", true);
		//	c.animator.SetBool("JumpUp", true);
		//	c.isGrounded = false;
		//	c.m_JumpTime = 0;
		//	c.canDoubleJump = true;
		//	c.particleJumpDown.Play();
		//	c.particleJumpUp.Play();
		//}
		//else if (!c.isGrounded && jumpPressed && c.canDoubleJump && !c.isWallSliding)
		//{
		//	c.canDoubleJump = false;
		//	c.m_Rigidbody2D.velocity = new Vector2(c.m_Rigidbody2D.velocity.x, 0);
		//	c.m_Rigidbody2D.AddForce(new Vector2(0f, c.m_ImpulseJumpForceY / 1.2f), ForceMode2D.Impulse);
		//	c.m_JumpTime = 0;
		//	c.jumpCanceled = false;
		//	c.animator.SetBool("IsDoubleJumping", true);
		//}
		//else if (jumping && !c.jumpCanceled && !c.isWallSliding)
		//{
		//	c.m_Rigidbody2D.AddForce(new Vector2(0f, c.m_FloatForce));
		//}

		//else if (c.isWall && !c.isGrounded)
		//{
		//	// TODO Change this part to make player start wall sliding if pressed against the wall
		//	if (!c.prevWallSliding && c.m_Rigidbody2D.velocity.y < 0 || c.isDashing)
		//	{ // start wall sliding
		//		c.isWallSliding = true;
		//		c.wallCheckPosition.localPosition = new Vector3(-c.wallCheckPosition.localPosition.x, c.wallCheckPosition.localPosition.y, 0);
		//		Flip();
		//		StartCoroutine(WaitToCheck(0.1f));
		//		c.canDoubleJump = true;
		//		c.animator.SetBool("IsWallSliding", true);
		//	}
		//	c.isDashing = false;

		//	if (c.isWallSliding)
		//	{
		//		if (move * transform.localScale.x > 0.1f)
		//		{
		//			StartCoroutine(WaitToEndSliding());
		//		}
		//		else
		//		{
		//			c.prevWallSliding = true;
		//			c.m_Rigidbody2D.velocity = new Vector2(-transform.localScale.x * 2, -5);
		//		}
		//	}

		//	// Jumping from wallslide
		//	if (jumpPressed && c.isWallSliding)
		//	{
		//		c.animator.SetBool("IsJumping", true);
		//		c.animator.SetBool("JumpUp", true);
		//		c.m_Rigidbody2D.velocity = new Vector2(0f, 0f);
		//		//old x jump force
		//		//m_XJumpForce = transform.localScale.x * m_ImpulseJumpForce * 1.2f
		//		//TODO fix wall jump force.
		//		c.m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * c.m_WallJumpForce, c.m_WallJumpForce / 2), ForceMode2D.Impulse);
		//		c.m_JumpTime = 0;
		//		c.jumpWallStartX = transform.position.x;
		//		c.limitVelOnWallJump = true;
		//		c.canDoubleJump = true;
		//		c.isWallSliding = false;
		//		c.animator.SetBool("IsWallSliding", false);
		//		c.prevWallSliding = false;
		//		c.wallCheckPosition.localPosition = new Vector3(Mathf.Abs(c.wallCheckPosition.localPosition.x), c.wallCheckPosition.localPosition.y, 0);
		//		//canMove = false;
		//	}
		//	else if (dash && c.canDash)
		//	{
		//		c.isWallSliding = false;
		//		c.animator.SetBool("IsWallSliding", false);
		//		c.prevWallSliding = false;
		//		c.wallCheckPosition.localPosition = new Vector3(Mathf.Abs(c.wallCheckPosition.localPosition.x), c.wallCheckPosition.localPosition.y, 0);
		//		c.canDoubleJump = true;
		//		StartCoroutine(DashCooldown());
		//	}
		//}
		//else if (c.isWallSliding && !c.isWall && c.canCheck)
		//{
		//	c.isWallSliding = false;
		//	c.animator.SetBool("IsWallSliding", false);
		//	c.prevWallSliding = false;
		//	c.wallCheckPosition.localPosition = new Vector3(Mathf.Abs(c.wallCheckPosition.localPosition.x), c.wallCheckPosition.localPosition.y, 0);
		//	c.canDoubleJump = true;
		//}
	}

	IEnumerator CrouchCooldown()
	{
		c.timeLockedCrouching = true;
		yield return new WaitForSeconds(0.3f);
		c.timeLockedCrouching = false;
	}

	//IEnumerator DashCooldown()
	//{
	//	c.prevGravityScale = c.m_Rigidbody2D.gravityScale;
	//	c.m_Rigidbody2D.gravityScale = 0;
	//	c.animator.SetBool("IsDashing", true);
	//	c.isDashing = true;
	//	c.canDash = false;
	//	yield return new WaitForSeconds(0.1f);
	//	c.isDashing = false;
	//	c.m_Rigidbody2D.gravityScale = c.prevGravityScale;
	//	yield return new WaitForSeconds(0.5f);
	//	c.canDash = true;
	//}

	IEnumerator WaitToCheck(float time)
	{
		c.canCheck = false;
		yield return new WaitForSeconds(time);
		c.canCheck = true;
	}

	//IEnumerator WaitToEndSliding()
	//{
	//	yield return new WaitForSeconds(0.1f);
	//	c.canDoubleJump = true;
	//	c.isWallSliding = false;
	//	c.animator.SetBool("IsWallSliding", false);
	//	c.prevWallSliding = false;
	//	c.wallCheckPosition.localPosition = new Vector3(Mathf.Abs(c.wallCheckPosition.localPosition.x), c.wallCheckPosition.localPosition.y, 0);
	//}
}
