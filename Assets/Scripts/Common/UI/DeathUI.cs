using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
	[SerializeField] private GameObject respawnablePanel;
	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] IntData lives;

	private void OnEnable()
	{
		respawnablePanel.SetActive(lives.value > 0);
		gameOverPanel.SetActive(!(lives.value > 0));
	}
}
