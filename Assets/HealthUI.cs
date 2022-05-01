using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
	//[SerializeField, Tooltip("The image that will be displayed as a single health.")] GameObject healthRep;
	[SerializeField, Tooltip("The object's health that will be represented.")] Health health;
	int shownHealth = 9;

	private void Update()
	{
		if (shownHealth < health.CurrentH)
		{
			for (int i = shownHealth; i < health.CurrentH; i++)
			{
				transform.GetChild(i-1).gameObject.SetActive(true);
				shownHealth++;
			}
		} else if (shownHealth > health.CurrentH)
		{
			for (int i = shownHealth; i > health.CurrentH; i--)
			{
				transform.GetChild(i-1).gameObject.SetActive(false);
				shownHealth--;
			}
		}
	}
}
