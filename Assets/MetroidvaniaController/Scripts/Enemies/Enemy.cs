using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {


	//public float life = 10;
	public float damage = 100;
	private bool isPlat;
	private bool isObstacle;
	private bool isGrounded;
	private Transform fallCheck;
	private Transform wallCheck;
	private Transform groundcheck;
	private float groundCheckRadius = 0.1f;
	private Health health;
	public LayerMask turnLayerMask;
	private Rigidbody2D rb;

	private bool facingRight = true;
	
	public float speed = 5f;

	public bool isInvincible = false;
	private bool isHitted = false;

	void Awake () {
		fallCheck = transform.Find("FallCheck");
		wallCheck = transform.Find("WallCheck");
		groundcheck = transform.Find("GroundCheck");
		health = GetComponent<Health>();
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// Freeze player movement if dead.
		if (health.CurrentH <= 0)
		{
			rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
			return;
		}

		isPlat = Physics2D.OverlapCircle(fallCheck.position, .1f, 1 << LayerMask.NameToLayer("Default"));
		isObstacle = Physics2D.OverlapCircle(wallCheck.position, .1f, turnLayerMask);
		isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundCheckRadius, turnLayerMask);

		if (!isHitted && isGrounded)
		{
			if (isPlat && !isObstacle && !isHitted)
			{
				if (facingRight)
				{
					rb.velocity = new Vector2(speed, rb.velocity.y);
				}
				else
				{
					rb.velocity = new Vector2(-speed, rb.velocity.y);
				}
			}
			else
			{
				Flip();
			}
		}
	}

	void Flip (){
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	//public void ApplyDamage(float damage) {
	//	if (!isInvincible) 
	//	{
	//		float direction = damage / Mathf.Abs(damage);
	//		damage = Mathf.Abs(damage);
	//		transform.GetComponent<Animator>().SetBool("Hit", true);
	//		life -= damage;
	//		rb.velocity = Vector2.zero;
	//		rb.AddForce(new Vector2(direction * 500f, 100f));
	//		StartCoroutine(HitTime());
	//	}
	//}

	void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			collision.gameObject.GetComponent<Health>().TakeDamage(damage, transform.position, 400f);
		}
	}

	//IEnumerator HitTime()
	//{
	//	isHitted = true;
	//	isInvincible = true;
	//	yield return new WaitForSeconds(0.1f);
	//	isHitted = false;
	//	isInvincible = false;
	//}

	//IEnumerator DestroyEnemy()
	//{
	//	CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
	//	capsule.size = new Vector2(1f, 0.25f);
	//	capsule.offset = new Vector2(0f, -0.8f);
	//	capsule.direction = CapsuleDirection2D.Horizontal;
	//	yield return new WaitForSeconds(0.25f);
	//	rb.velocity = new Vector2(0, rb.velocity.y);
	//	yield return new WaitForSeconds(3f);
	//	Destroy(gameObject);
	//}

	//private void OnDrawGizmosSelected()
	//{
	//	Gizmos.DrawWireSphere(groundcheck.position, groundCheckRadius);
	//}
}
