using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public float damage = 4f;
	public float knockback = 400f;
	public LayerMask enemyLayers;
	public Transform attackCheck;
	public float attackRadius = 1f;
	public GameObject throwableObject;
	public Animator animator;
	public bool canAttack = true;

	private Coroutine currentAttack;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.X) && canAttack)
		{
			if (currentAttack == null) currentAttack = StartCoroutine(FrontAttack());
		}

		if (Input.GetKeyDown(KeyCode.V) && throwableObject != null)
		{
			GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity) as GameObject;
			Vector2 direction = new Vector2(transform.localScale.x, 0);
			throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction;
			throwableWeapon.name = "ThrowableWeapon";
		}
	}

	IEnumerator FrontAttack()
	{
		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius, enemyLayers);
		foreach (var enemy in hitEnemies)
		{
			enemy.GetComponent<Health>()?.TakeDamage(damage, attackCheck.position, knockback);
		}
		canAttack = false;
		animator.SetBool("IsAttacking", true);
		yield return new WaitForSeconds(0.25f);
		canAttack = true;
		animator.SetBool("IsAttacking", false);
		currentAttack = null;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(attackCheck.position, attackRadius);
	}
}
