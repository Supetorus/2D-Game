using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	[SerializeField] private GameObject imagePrefab;

	private Dictionary<int, GameObject> sprites = new Dictionary<int, GameObject>();

	public void Add(GameObject go, int currentKey)
	{
		var image = Instantiate(imagePrefab, transform);
		image.GetComponent<Image>().sprite = go.GetComponent<SpriteRenderer>().sprite;
		sprites.Add(currentKey, image);
	}
}
