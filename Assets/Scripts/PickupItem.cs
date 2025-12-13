using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour
{
    public string itemName;
    public string questCharKey;
    public string questKey;

    public bool playerNear = false;
    public bool lookingAtItem = false;
    private float raycastTime = 0;

    private GameObject tooltip;

    void Start()
    {
        tooltip = GameManager.GM.pickupTooltip;
    }

    void Update()
    {
        // Check if player is looking at item, but only so often and if the player is near (in the pickup detection area)
        if (playerNear && Time.time - raycastTime > 0.05f)
        {
            raycastTime = Time.time;
            lookingAtItem = Array.Exists(
                Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 10f),
                hit => hit.collider.transform.CompareTag("Item")
            );
        }

        // (De)Activate tooltip when it doesn't match expected state
        if (playerNear && tooltip.activeInHierarchy != lookingAtItem)
            tooltip.SetActive(lookingAtItem);

        // Get input; E to pick up item
        var keyboard = Keyboard.current;
        if (keyboard.eKey.wasPressedThisFrame && tooltip.activeInHierarchy)
        {
            GameManager.GM.AddInventoryItem(new ItemInfo(this));
            tooltip.SetActive(false);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
            playerNear = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerNear = false;
            tooltip.SetActive(false);
        }
    }
}
