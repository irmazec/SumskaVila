using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.GM.OnItemCollected += ShowItem;
    }

    public void ShowItem(string itemName)
    {
        if (itemName == gameObject.name.ToLower())
            transform.GetChild(0).gameObject.SetActive(true);
    }
}
