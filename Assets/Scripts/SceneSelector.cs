using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
	[SerializeField, Tooltip("The scene name to load. If none is given will attempt to load next scene in build index order.")] private string sceneName;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.gameObject.CompareTag("Player")) return;
		if (!string.IsNullOrEmpty(sceneName))
		{
			SceneManager.LoadScene(sceneName);
		}
		else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void LoadScene()
	{
		if (!string.IsNullOrEmpty(sceneName))
		{
			SceneManager.LoadScene(sceneName);
		}
		else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
