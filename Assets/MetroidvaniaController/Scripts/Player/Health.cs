using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	[SerializeField, Tooltip("The maximum and starting health")] public float maxHealth;
	[SerializeField, Tooltip("How much health is gained per second, 0 to turn off")] public float regenerationRate;
	[SerializeField, Tooltip("Whether or not the object is knocked back when damaged")] public bool knockback = true;
	[SerializeField, Tooltip("How long this object should be invincible when taking damage")] public float invincibilityTime = 0.2f;
	[SerializeField, Tooltip("The number of lives this object has, if it should respawn.")] IntData numberOfLives;
	[SerializeField, Tooltip("The object to be destroyed on death. Corpse will stick around if this is undefined.")] GameObject toDestroyOnDeath;
	[SerializeField, Tooltip("The amount of time to wait before removing the dead body.")] float destroyTime;
	//[SerializeField, Tooltip("A prefab to place when the object is destroyed.")] GameObject deathPrefab;

	[SerializeField, Tooltip("Events called when object takes damage")] private UnityEvent onTakeDamage;
	[SerializeField, Tooltip("Events called when object's health goes below 0")] private UnityEvent onDeath;
	[SerializeField, Tooltip("Events called when object's health increases")] private UnityEvent onHeal;

	//private LevelManager levelManager;
	private Animator animator;
	private Rigidbody2D rigidBody;
	private CheckpointManager checkpointManager;
	private bool deathAnimationComplete = false;
	private Coroutine dieCoroutine;
	private Coroutine hurtCoroutine;
	private bool isInvincible = false;

	/// <summary>
	/// The current amount of health this object has.
	/// </summary>
	public float CurrentH { get; private set; }
	private void Awake()
	{
		CurrentH = maxHealth;
		TryGetComponent(out animator);
		TryGetComponent(out checkpointManager);
		TryGetComponent(out rigidBody);
	}

	/// <summary>
	/// Apply damage and / or knockback to this object.
	/// </summary>
	/// <param name="damage">Value greater than or equal to 0.</param>
	/// <param name="origin">The enemy attack location, used to calculate direction of knockback.</param>
	/// <param name="knockbackPower">The amount of power in the knockback.</param>
	public void TakeDamage(float damage, Vector3 origin, float knockbackPower)
	{
		if (isInvincible) return;

		CurrentH = Mathf.Max(CurrentH - damage, 0);
		if (CurrentH == 0)
		{
			if (dieCoroutine == null) dieCoroutine = StartCoroutine(Die());
		}
		else
		{
			if (hurtCoroutine == null) hurtCoroutine = StartCoroutine(Hurt());
			//TODO turn off animator bool hit in coroutine.
		}

		if (knockback)
		{
			Vector2 damageDir = Vector3.Normalize(transform.position - origin);
			rigidBody.velocity = Vector2.zero;
			rigidBody.AddForce(damageDir * knockbackPower);
		}
	}

	/// <summary>
	/// Heals this game object's health. Maxes out at this object's max health.
	/// </summary>
	/// <param name="healthIncrease">Value greater than 0.</param>
	public void Heal(float healthIncrease)
	{
		if (healthIncrease <= 0) return;

		CurrentH = Mathf.Min(CurrentH + healthIncrease, maxHealth);
		onHeal?.Invoke();
		animator.SetBool("Healing", true);

		//TODO turn off animator bool healing in coroutine.
	}

	/// <summary>
	/// This should be called by the death animation's last frame.
	/// </summary>
	public void CompletedDeathAnimation() {
		deathAnimationComplete = true;
	}

	/// <summary>
	/// Waits until the death animation has finished. Death animation must have an event that calls CompletedDeathAnimation.
	/// </summary>
	private IEnumerator Die()
	{
		foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) c.enabled = false;
		//knockback = false;
		animator.SetBool("IsDead", true);
		yield return new WaitUntil(() => deathAnimationComplete);
		numberOfLives.value -= 1;
		onDeath?.Invoke();
		if (toDestroyOnDeath != null) Destroy(toDestroyOnDeath, destroyTime); // This is simple, but might not be the best way to do this.
	}

	private IEnumerator Hurt()
	{
		animator.SetBool("Hit", true);
		onTakeDamage?.Invoke();
		isInvincible = true;
		yield return new WaitForSeconds(invincibilityTime);
		isInvincible = false;
		animator.SetBool("Hit", false);
		hurtCoroutine = null;
	}

	private void Update()
	{
		CurrentH += regenerationRate * Time.deltaTime;
	}

}
