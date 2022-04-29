using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_MaxJumpForce = 400f;                       // Amount of force added while the player is in the air
	[SerializeField] private float m_ImpulseJumpForce = 400f;                   // Amount of force added when the player jumps.
	[SerializeField] private float m_CrouchSpeed = 0.5f; // How fast the player moves while crouching. 1 = 100%.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_WallCheck;                             //Posicion que controla si el personaje toca una pared
	[SerializeField] private Transform m_CeilingCheck;                          //Position to check for ceiling.
	[SerializeField] private CapsuleCollider2D m_CrouchDisableCollider;			// The collider disabled by crouching.
	[SerializeField] private CapsuleCollider2D m_CrouchEnableCollider;			// The collider enabled by crouching.

	const float k_CeilingRadius = .15f; // Radius of the overlap circle to determine if under ceiling.
	const float k_GroundedRadius = .15f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;
	private float limitFallSpeed = 25f; // Limit fall speed
	private float m_FloatForce; // The amount of force applied based on how long player has been in air.
	private float m_JumpTime; // The amount of time the player has been in the air.
	public float AirTime = 1; // The amount of time the jump can continue to gain force (in seconds).

	public bool canDoubleJump = true; //If player can double jump
	[SerializeField] private float m_DashForce = 25f;
	private bool canDash = true;
	private bool timeLockedDashing = false; //If player is dashing
	private bool m_IsWall = false; //If there is a wall in front of the player
	private bool isWallSliding = false; //If player is sliding in a wall
	private bool prevWallSliding = false; //If player is sliding in a wall in the previous frame
	private float prevVelocityX = 0f;
	private bool canCheck = false; //For check if player is wallsliding
	private bool jumpCanceled = false; // If the player has pressed then released jump.
	private bool m_wasCrouching = false; // If the player was crouching last frame.
	private bool timeLockedCrouching = false; // If the player is locked crouching because of cooldown.

	public float life = 10f; //Life of the player
	public bool invincible = false; //If player can die
	private bool canMove = true; //If player can move

	private Animator animator;
	public ParticleSystem particleJumpUp; //Trail particles
	public ParticleSystem particleJumpDown; //Explosion particles

	private float jumpWallStartX = 0;
	private float jumpWallDistX = 0; //Distance between player and wall
	private bool limitVelOnWallJump = false; //For limit wall jump distance with low fps

	[Header("Events")]
	[Space]

	public UnityEvent OnFallEvent;
	public UnityEvent OnLandEvent;
	public BoolEvent OnCrouchEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		if (OnFallEvent == null)
			OnFallEvent = new UnityEvent();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}


	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				m_Grounded = true;
			if (!wasGrounded)
			{
				OnLandEvent.Invoke();
				if (!m_IsWall && !timeLockedDashing)
					particleJumpDown.Play();
				canDoubleJump = true;
				if (m_Rigidbody2D.velocity.y < 0f)
					limitVelOnWallJump = false;
			}
		}

		m_IsWall = false;

		if (!m_Grounded)
		{
			OnFallEvent.Invoke();
			Collider2D[] collidersWall = Physics2D.OverlapCircleAll(m_WallCheck.position, k_GroundedRadius, m_WhatIsGround);
			for (int i = 0; i < collidersWall.Length; i++)
			{
				if (collidersWall[i].gameObject != null)
				{
					timeLockedDashing = false;
					m_IsWall = true;
				}
			}
			prevVelocityX = m_Rigidbody2D.velocity.x;
		}

		if (limitVelOnWallJump)
		{
			if (m_Rigidbody2D.velocity.y < -0.5f)
				limitVelOnWallJump = false;
			jumpWallDistX = (jumpWallStartX - transform.position.x) * transform.localScale.x;
			if (jumpWallDistX < -0.5f && jumpWallDistX > -1f)
			{
				canMove = true;
			}
			else if (jumpWallDistX < -1f && jumpWallDistX >= -2f)
			{
				canMove = true;
				m_Rigidbody2D.velocity = new Vector2(10f * transform.localScale.x, m_Rigidbody2D.velocity.y);
			}
			else if (jumpWallDistX < -2f)
			{
				limitVelOnWallJump = false;
				m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
			}
			else if (jumpWallDistX > 0)
			{
				limitVelOnWallJump = false;
				m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
			}
		}
	}


	public void Move(float move, bool jump, bool jumping, bool dash, bool crouch)
	{
		if (!canMove) return;

		// Set jump force based on how long they have been in the air.
		if (m_Grounded || isWallSliding)
		{
			m_FloatForce = m_MaxJumpForce;
			jumpCanceled = false;
		}
		else
		{
			if (!jumping) jumpCanceled = true;
			if (!jumpCanceled) m_JumpTime += Time.deltaTime;
			else m_JumpTime = AirTime;
			m_FloatForce = m_MaxJumpForce * (1 - (m_JumpTime / AirTime));
		}

		// Forces player to remain crouching if there is an obstruction overhead.
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		// If crouching
		if (crouch && !isWallSliding)
		{
			if (!m_wasCrouching)
			{
				m_wasCrouching = true;
				OnCrouchEvent.Invoke(true);
				animator.SetBool("IsCrouching", true);
				StartCoroutine(CrouchCooldown());
			}

			// Reduce the speed by the crouchSpeed multiplier
			move *= m_CrouchSpeed;

			// Disable one of the colliders when crouching
			if (m_CrouchDisableCollider != null)
				m_CrouchDisableCollider.enabled = false;
			// Enables one of the colliders when crouching
			if (m_CrouchEnableCollider != null)
				m_CrouchEnableCollider.enabled = true;
		}
		else if (!timeLockedCrouching)
		{
			// Enable the collider when not crouching
			if (m_CrouchDisableCollider != null)
				m_CrouchDisableCollider.enabled = true;
			// Disable one of the colliders when crouching
			if (m_CrouchEnableCollider != null)
				m_CrouchEnableCollider.enabled = false;

			if (m_wasCrouching)
			{
				m_wasCrouching = false;
				OnCrouchEvent.Invoke(false);
				animator.SetBool("IsCrouching", false);
			}
		}

		if (dash && canDash && !isWallSliding)
		{
			//m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_DashForce, 0f));
			StartCoroutine(DashCooldown());
		}
		// If crouching, check to see if the character can stand up
		if (timeLockedDashing)
		{
			m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * m_DashForce, 0);
		}
		//only control the player if grounded or airControl is turned on
		else if (m_Grounded || m_AirControl)
		{
			if (m_Rigidbody2D.velocity.y < -limitFallSpeed)
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -limitFallSpeed);
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight && !isWallSliding)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight && !isWallSliding)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			animator.SetBool("IsJumping", true);
			animator.SetBool("JumpUp", true);
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_ImpulseJumpForce), ForceMode2D.Impulse);
			m_JumpTime = 0;
			canDoubleJump = true;
			particleJumpDown.Play();
			particleJumpUp.Play();
		}
		else if (!m_Grounded && jump && canDoubleJump && !isWallSliding)
		{
			canDoubleJump = false;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
			m_Rigidbody2D.AddForce(new Vector2(0f, m_ImpulseJumpForce / 1.2f), ForceMode2D.Impulse);
			m_JumpTime = 0;
			jumpCanceled = false;
			animator.SetBool("IsDoubleJumping", true);
		}
		else if (jumping && !jumpCanceled && !isWallSliding)
		{
			m_Rigidbody2D.AddForce(new Vector2(0f, m_FloatForce));
		}

		else if (m_IsWall && !m_Grounded)
		{
			if (!prevWallSliding && m_Rigidbody2D.velocity.y < 0 || timeLockedDashing)
			{ // start wall sliding
				isWallSliding = true;
				m_WallCheck.localPosition = new Vector3(-m_WallCheck.localPosition.x, m_WallCheck.localPosition.y, 0);
				Flip();
				StartCoroutine(WaitToCheck(0.1f));
				canDoubleJump = true;
				animator.SetBool("IsWallSliding", true);
			}
			timeLockedDashing = false;

			if (isWallSliding)
			{
				if (move * transform.localScale.x > 0.1f)
				{
					StartCoroutine(WaitToEndSliding());
				}
				else
				{
					prevWallSliding = true;
					m_Rigidbody2D.velocity = new Vector2(-transform.localScale.x * 2, -5);
				}
			}

			// Jumping from wallslide
			if (jump && isWallSliding)
			{
				animator.SetBool("IsJumping", true);
				animator.SetBool("JumpUp", true);
				m_Rigidbody2D.velocity = new Vector2(0f, 0f);
				m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_ImpulseJumpForce * 1.2f, m_ImpulseJumpForce), ForceMode2D.Impulse);
				m_JumpTime = 0;
				jumpWallStartX = transform.position.x;
				limitVelOnWallJump = true;
				canDoubleJump = true;
				isWallSliding = false;
				animator.SetBool("IsWallSliding", false);
				prevWallSliding = false;
				m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
				canMove = false;
			}
			else if (dash && canDash)
			{
				isWallSliding = false;
				animator.SetBool("IsWallSliding", false);
				prevWallSliding = false;
				m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
				canDoubleJump = true;
				StartCoroutine(DashCooldown());
			}
		}
		else if (isWallSliding && !m_IsWall && canCheck)
		{
			isWallSliding = false;
			animator.SetBool("IsWallSliding", false);
			prevWallSliding = false;
			m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
			canDoubleJump = true;
		}

	}

	IEnumerator CrouchCooldown()
	{
		timeLockedCrouching = true;
		yield return new WaitForSeconds(0.3f);
		timeLockedCrouching = false;
	}

	/// <summary>
	/// Flip the player sprite horizontally
	/// </summary>
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void ApplyDamage(float damage, Vector3 position)
	{
		if (!invincible)
		{
			animator.SetBool("Hit", true);
			life -= damage;
			Vector2 damageDir = Vector3.Normalize(transform.position - position) * 40f;
			m_Rigidbody2D.velocity = Vector2.zero;
			m_Rigidbody2D.AddForce(damageDir * 10);
			if (life <= 0)
			{
				StartCoroutine(WaitToDead());
			}
			else
			{
				StartCoroutine(Stun(0.25f));
				StartCoroutine(MakeInvincible(1f));
			}
		}
	}

	IEnumerator DashCooldown()
	{
		animator.SetBool("IsDashing", true);
		timeLockedDashing = true;
		canDash = false;
		yield return new WaitForSeconds(0.1f);
		timeLockedDashing = false;
		yield return new WaitForSeconds(0.5f);
		canDash = true;
	}

	IEnumerator Stun(float time)
	{
		canMove = false;
		yield return new WaitForSeconds(time);
		canMove = true;
	}
	IEnumerator MakeInvincible(float time)
	{
		invincible = true;
		yield return new WaitForSeconds(time);
		invincible = false;
	}
	IEnumerator WaitToMove(float time)
	{
		canMove = false;
		yield return new WaitForSeconds(time);
		canMove = true;
	}

	IEnumerator WaitToCheck(float time)
	{
		canCheck = false;
		yield return new WaitForSeconds(time);
		canCheck = true;
	}

	IEnumerator WaitToEndSliding()
	{
		yield return new WaitForSeconds(0.1f);
		canDoubleJump = true;
		isWallSliding = false;
		animator.SetBool("IsWallSliding", false);
		prevWallSliding = false;
		m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
	}

	IEnumerator WaitToDead()
	{
		animator.SetBool("IsDead", true);
		canMove = false;
		invincible = true;
		GetComponent<Attack>().enabled = false;
		yield return new WaitForSeconds(0.4f);
		m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
		yield return new WaitForSeconds(1.1f);
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}
}
