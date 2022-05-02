using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
	int shownHealth = 9;

	public void Change(float newValue)
	{
		if (shownHealth < newValue)
		{
			for (; shownHealth < newValue; shownHealth++)
			{
				transform.GetChild(shownHealth - 1).gameObject.SetActive(true);
			}
		}
		else if (shownHealth > newValue)
		{
			for (; shownHealth > newValue; shownHealth--)
			{
				transform.GetChild(shownHealth - 1).gameObject.SetActive(false);
			}
		}
	}
}
