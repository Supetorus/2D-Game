using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
	[SerializeField, Tooltip("The object to be destroyed")] GameObject toDestroy;

	public void Run()
	{
		Destroy(toDestroy);
	}
}
