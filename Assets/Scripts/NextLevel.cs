using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.gameObject.CompareTag("Player")) return;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
