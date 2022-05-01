using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
	[SerializeField, Tooltip("The number of lives.")] IntData lives;
	int shownLives = 9;

	private void Update()
	{
		if (shownLives < lives.value)
		{
			for (int i = shownLives; i < lives.value; i++)
			{
				transform.GetChild(i - 1).gameObject.SetActive(true);
				shownLives++;
			}
		}
		else if (shownLives > lives.value)
		{
			for (int i = shownLives; i > lives.value; i--)
			{
				transform.GetChild(i - 1).gameObject.SetActive(false);
				shownLives--;
			}
		}
	}
}
