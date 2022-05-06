using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{

	[SerializeField, Tooltip("Whether or not a player can steer while jumping")]
	public bool airControlAllowed = false;
	[SerializeField, Tooltip("How much to smooth out the movement"), Range(0, .3f)]
	public float movementSmoothing = .05f;
	[SerializeField, Tooltip("Positions to check if the player is grounded")]
	public List<Transform> groundChecks = new List<Transform>();
	[SerializeField, Tooltip("Positions to check if the player is against the wall")]
	public List<Transform> wallChecks = new List<Transform>();
	[SerializeField, Tooltip("Positions to check if the player can stand while crouching")]
	public List<Transform> ceilingChecks = new List<Transform>();

	[SerializeField, Tooltip("A mask determining what is ground to the character")]
	public LayerMask groundLayers;
	[SerializeField, Tooltip("A mask determining what is ceiling to the character while crouching")]
	public LayerMask ceilingLayers;
	[SerializeField, Tooltip("A mask determining what is wall to the character")]
	public LayerMask wallLayers;
	[SerializeField, Tooltip("The collider(s) used while standing")]
	public GameObject standingCollider;
	[SerializeField, Tooltip("The collider(s) used while crouching")]
	public GameObject crouchingCollider;
	[SerializeField, Tooltip("How fast the player moves while running")]
	public float runSpeed = 15f;
	[SerializeField, Tooltip("How fast the player moves while in the air")]
	public float airSpeed = 10f;
	[SerializeField, Tooltip("How fast the player moves while crouching")]
	public float crouchSpeed = 5f;
	[SerializeField, Tooltip("How fast to move while dashing")]
	public float dashspeed = 25f;
	[SerializeField, Tooltip("If the player is facing right (Check if your sprite's default direction is right")]
	public bool isFacingRight = true;
	[SerializeField, Tooltip("Particles used when the player jumps")]
	public ParticleSystem jumpParticles;
	[SerializeField, Tooltip("Particles used when the player lands")]
	public ParticleSystem landingParticles;

	// Radius of the circle used to check if character is crouch blocked.
	[HideInInspector] public float ceilingCheckRadius = .15f;
	// Radius of the circle used to check if character is against the wall.
	[HideInInspector] public float wallCheckRadius = .15f;


	public Text debugText;

	[HideInInspector] public bool wasCrouchBlocked; // If the ceiling check was true last fixed update frame;
	[HideInInspector] public bool wasGrounded; // If the ground check was true last fixed update frame;
	[HideInInspector] public bool wasWall; // If the wall check was true last fixed update frame;

	[HideInInspector] public bool wallPressing = false;
	[HideInInspector] public bool isGrounded;            // Whether or not the player is grounded.
	[HideInInspector] public bool isWall; //If there is a wall in front of the player
	[HideInInspector] public bool isCrouchBlocked; //If there is a ceiling in the player's headspace.

	[HideInInspector] public bool canDash = true;
	[HideInInspector] public bool isDashing = false; //If player is dashing
	[HideInInspector] public bool isWallSliding = false; //If player is sliding in a wall
	[HideInInspector] public bool prevWallSliding = false; //If player is sliding on a wall in the previous frame
	[HideInInspector] public bool canAirJump = true; //If the player can jump while in the air.
	[HideInInspector] public bool canAirDash = true; //If the player can dash while in the air.
	[SerializeField] public float secondJumpMultiplier = 0.8f; // The amount of impulse force an air jump will have compared to regular jump
															   //[HideInInspector] public float sjm = 0.9f;

	[HideInInspector] public bool m_wasCrouching = false; // If the player was crouching last frame.
	[HideInInspector] public bool timeLockedCrouching = false; // If the player is locked crouching because of cooldown.

	[HideInInspector] public bool wasMoving = false;
	[HideInInspector] public bool isMoving = false;

	[HideInInspector] public float jumpWallStartX = 0;
	[HideInInspector] public float jumpWallDistX = 0; //Distance between player and wall
	[HideInInspector] public bool limitVelOnWallJump = false; //For limit wall jump distance with low fps

	[HideInInspector] public Vector3 velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnFallEvent;
	public UnityEvent OnLandEvent;
	public BoolEvent OnCrouchEvent;

	//Player input status
	public struct PlayerInput
	{
		public float lateralMovement;
		public bool jumpPressed;
		public bool jumpHeld;
		public bool dashPressed;
		public bool downPressed;
		public bool downHeld;
		public bool doMelee;

		public bool ignoreLateralInput; // If lateral input should be ignored, usually temporarily


		public void Update()
		{
			if (!ignoreLateralInput) lateralMovement = Input.GetAxisRaw("Horizontal");
			else lateralMovement = 0;
			jumpPressed = Input.GetKeyDown(KeyCode.Z);
			jumpHeld = Input.GetKey(KeyCode.Z);
			dashPressed = Input.GetKeyDown(KeyCode.C);
			downPressed = Input.GetKeyDown(KeyCode.DownArrow);
			//if (downPressed) downHeld = true;
			downHeld = Input.GetKey(KeyCode.DownArrow);
			doMelee = Input.GetKeyDown(KeyCode.X);
		}
	}

	public PlayerInput pi;

	// States
	[HideInInspector] public Idling idling;
	[HideInInspector] public Crouching crouching;
	[HideInInspector] public Jumping jumping;
	[HideInInspector] public Falling falling;
	[HideInInspector] public Meleeing meleeing;
	[HideInInspector] public Dashing dashing;
	[HideInInspector] public TakingDamage takingDamage;
	[HideInInspector] public Dead dead;
	[HideInInspector] public WallSliding wallSliding;
	[HideInInspector] public Running running;
	[HideInInspector] public WallJumping wallJumping;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	public CharacterMoveState CurrentMState { get; private set; }

	public void ChangeState(CharacterMoveState newState)
	{
		if (newState != null && newState != CurrentMState)
		{
			CurrentMState?.ExitState();
			CurrentMState = newState;
			CurrentMState.EnterState();
		}
	}

	private void Awake()
	{
		idling = GetComponent<Idling>();
		crouching = GetComponent<Crouching>();
		jumping = GetComponent<Jumping>();
		falling = GetComponent<Falling>();
		meleeing = GetComponent<Meleeing>();
		dashing = GetComponent<Dashing>();
		takingDamage = GetComponent<TakingDamage>();
		dead = GetComponent<Dead>();
		wallSliding = GetComponent<WallSliding>();
		running = GetComponent<Running>();
		wallJumping = GetComponent<WallJumping>();

		standingCollider.SetActive(true);
		crouchingCollider.SetActive(false);
	}

	private void Start()
	{
		ChangeState(idling);
	}

	private void Update()
	{
		debugText.text = "State: " + CurrentMState.ToString() + 
			"\nCanAirDash: " + canAirDash;

		// Get player input
		pi.Update();

		// Context checks
		wasMoving = isMoving;
		isMoving = Mathf.Abs(pi.lateralMovement) > 0.01f;
		wallPressing = !isGrounded && Mathf.Abs(pi.lateralMovement) > 0.01f && isWall; // Not grounded and pressing toward the wall.

		// Update state
		CurrentMState
			.UpdateState();
	}

	private void FixedUpdate()
	{
		// Check if player is on the ground.
		wasGrounded = isGrounded;
		isGrounded = false;
		RaycastHit2D[] hits;
		foreach (Transform t in groundChecks)
		{
			hits = Physics2D.RaycastAll(transform.position, t.position - transform.position, (transform.position - t.position).magnitude, groundLayers);
			foreach (var col in hits)
			{
				if (col.transform.gameObject != gameObject) { isGrounded = true; break; }
			}
		}

		// Check if against the wall
		wasWall = isWall;
		isWall = false;
		foreach (Transform t in wallChecks)
		{
			hits = Physics2D.RaycastAll(transform.position, t.position - transform.position, (transform.position - t.position).magnitude, wallLayers);
			foreach (var col in hits)
			{
				if (col.transform.gameObject != gameObject) { isWall = true; break; }
			}
		}

		// Check if against the wall
		wasCrouchBlocked = isCrouchBlocked;
		isCrouchBlocked = false;
		foreach (Transform t in ceilingChecks)
		{
			hits = Physics2D.RaycastAll(transform.position, t.position - transform.position, (transform.position - t.position).magnitude, ceilingLayers);
			foreach (var col in hits)
			{
				if (col.transform.gameObject != gameObject) { isCrouchBlocked = true; break; }
			}
		}

		CurrentMState.FixedUpdateState();

		if (isGrounded) canAirDash = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		foreach (Transform t in groundChecks)
		{
			Gizmos.DrawLine(transform.position, t.position);
		}

		Gizmos.color = Color.blue;
		foreach (Transform t in wallChecks)
		{
			Gizmos.DrawLine(transform.position, t.position);
		}

		Gizmos.color = Color.green;
		foreach (Transform t in ceilingChecks)
		{
			Gizmos.DrawLine(transform.position, t.position);
		}
	}
}