using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
	public AudioClip clip;
	public string matchTag;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (string.IsNullOrEmpty(matchTag) || collision.gameObject.CompareTag(matchTag))
		{
			AudioSource.PlayClipAtPoint(clip, transform.position);
		}
	}
}
