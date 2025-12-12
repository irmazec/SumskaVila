using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDialogue : MonoBehaviour
{
    public string charKey;

    private bool playerNear = false;
    private bool lookingAtNpc = false;
    private float raycastTime = 0;
    private bool playingDialogue = false;
    private bool enterPressed = false;
    private PlayerControl playerControl = null;

    // Game manager variables
    private GameObject dialogueCanvas;
    private GameObject tooltip;
    private GameObject dialogueBox;
    private TextMeshProUGUI title;
    private TextMeshProUGUI textbox;
    private DialogueInfo charInfo;

    void Start()
    {
        dialogueCanvas = GameManager.GM.dialogueCanvas;
        tooltip = GameManager.GM.tooltip;
        dialogueBox = GameManager.GM.dialogueBox;
        title = GameManager.GM.dialogueTitle;
        textbox = GameManager.GM.dialogueText;
        charInfo = GameManager.GM.dialogue[charKey];
    }

    void Update()
    {
        // Check if player is looking at NPC, but only so often and if the player is near (in the dialogue detection area)
        if (playerNear && Time.time - raycastTime > 0.05f)
        {
            raycastTime = Time.time;
            lookingAtNpc = Array.Exists(
                Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 10f),
                hit => hit.collider.transform.CompareTag("NPC")
            );
        }

        // (De)Activate dialogue canvas when it doesn't match expected state
        if (dialogueCanvas.activeInHierarchy != (playerNear && lookingAtNpc))
            dialogueCanvas.SetActive(playerNear && lookingAtNpc);

        // Update inputs; Enter to skip/advance dialogue (if playing), E to begin dialogue otherwise
        var keyboard = Keyboard.current;
        if (playingDialogue)
        {
            if (keyboard.enterKey.wasPressedThisFrame)
                enterPressed = true;
        }
        else if (dialogueCanvas.activeInHierarchy)
        {
            if (keyboard.eKey.wasPressedThisFrame)
                PlayDialogue();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerNear = true;
            if (playerControl == null)
                playerControl = collider.GetComponentInParent<PlayerControl>();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            playerNear = false;
    }

    public void PlayDialogue()
    {
        // Check current dialogue progress and play appropriate lines
        title.text = charInfo.charName;
        switch (charInfo.dialogueProgress)
        {
            case 0:
                StartCoroutine(PlayLines(charInfo.questLines));
                GameManager.GM.AddCharQuests(charKey);
                charInfo.dialogueProgress = 1;
                break;
            case 1:
                StartCoroutine(PlayLines(charInfo.repeatLines));
                break;
            case 2:
                StartCoroutine(PlayLines(charInfo.endLines));
                break;
        }
    }

    IEnumerator PlayLines(List<string> lines)
    {
        playerControl.ToggleDialogue();
        tooltip.SetActive(false);
        dialogueBox.SetActive(true);
        playingDialogue = true;

        // Play dialogue line by line
        int lineIndex = 0;
        while (lineIndex < lines.Count)
        {
            string line = lines[lineIndex];

            // Play line character by character, unless interrupted by Enter key
            textbox.text = "";
            foreach (char letter in line.ToCharArray())
            {
                textbox.text += letter;
                yield return new WaitForSeconds(0.015f);

                if (enterPressed)
                {
                    textbox.text = lines[lineIndex];
                    enterPressed = false;
                    break;
                }
            }
            lineIndex++;

            // Wait for Enter key to proceed to next line of dialogue
            while (!enterPressed)
                yield return null;
            enterPressed = false;
        }

        playingDialogue = false;
        dialogueBox.SetActive(false);
        tooltip.SetActive(true);
        playerControl.ToggleDialogue();
    }
}
