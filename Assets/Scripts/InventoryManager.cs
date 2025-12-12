using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject inventoryPanel;

    public Image[] slots;

    private List<Item> items = new List<Item>();

    void Awake()
    {
        Instance = this;
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryPanel != null)
            {
                bool newState = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(newState);
            }
        }
    }

    public void AddItem(Item item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            UpdateUI();
        }
        else
        {
            Debug.Log("Item already in inventory: " + item.itemID);
        }
    }


    void UpdateUI()
    {
        Debug.Log("DODANO" + items.Count);

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].sprite = items[i].icon;
                slots[i].color = Color.white;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0);
            }
        }
    }
}
