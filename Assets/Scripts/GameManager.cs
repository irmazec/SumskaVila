using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public GameObject gameEndScreen;
    [HideInInspector] public bool gameEndScreenActive = false;

    [Header("Dialogue")]
    public GameObject dialogueCanvas;
    public GameObject dialogueTooltip;
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueTitle;
    public TextMeshProUGUI dialogueText;
    public Dictionary<string, DialogueInfo> dialogue;
    public event Action<string> OnDialogueUpdateNeeded;

    [Header("Quests")]
    public Transform questBox;
    public GameObject questPrefab;
    public GameObject subquestPrefab;
    public Dictionary<string, List<QuestInfo>> quests;
    public event Action<string> OnQuestCompleted;
    public event Action<string> OnQuestUnlocked;

    [Header("Inventory")]
    public Transform inventoryBox;
    public GameObject pickupTooltip;
    public List<ItemInfo> items = new();

    private int questCount = 0;
    private HashSet<string> completedQuests = new();
    private bool lairDiscovered = false;

    void Awake()
    {
        GM = this;
        dialogue = JsonConvert.DeserializeObject<Dictionary<string, DialogueInfo>>(Resources.Load<TextAsset>("dialogue").text);
        quests = JsonConvert.DeserializeObject<Dictionary<string, List<QuestInfo>>>(Resources.Load<TextAsset>("quests").text);

        foreach (List<QuestInfo> questList in quests.Values)
            questCount += questList.Count;
    }

    void Update()
    {
        if (completedQuests.Count == questCount && !dialogueCanvas.activeInHierarchy && !gameEndScreen.activeInHierarchy)
        {
            completedQuests.Add("gameEnded");
            gameEndScreen.SetActive(true);
            gameEndScreenActive = true;
        }
    }


    // Add all quests for a given character to the Quest UI; check if any are already finished
    public void AddCharQuests(string charKey)
    {
        Debug.Log(charKey);
        foreach (QuestInfo quest in quests[charKey])
        {
            GameObject q = Instantiate(quest.questLevel == 0 ? questPrefab : subquestPrefab, questBox);
            q.GetComponent<UIQuestItem>().InitQuest(charKey, quest);
        }

        if (charKey == "bunny" && lairDiscovered)
            CompleteQuest(charKey, "lairSearch");
        else if (charKey != "bunny")
            CheckInventory();
    }

    // Mark quest as complete, update UI and update character dialogue if needed
    public void CompleteQuest(string charKey, string questKey)
    {
        QuestInfo quest = quests[charKey].Find(q => q.questKey == questKey);
        quest.completed = true;
        OnQuestCompleted?.Invoke(questKey);
        completedQuests.Add(questKey);
        
        if (quest.updatesDialogue == DialogueUpdateOptions.onComplete)
            OnDialogueUpdateNeeded?.Invoke(charKey);
    }

    // Mark quest as unlocked, update UI and update character dialogue if needed; check if it's already completed
    public void UnlockQuest(string charKey, string questKey)
    {
        QuestInfo quest = quests[charKey].Find(q => q.questKey == questKey);
        quest.unlocked = true;
        OnQuestUnlocked?.Invoke(questKey);

        if (quest.updatesDialogue == DialogueUpdateOptions.onUnlock)
            OnDialogueUpdateNeeded?.Invoke(charKey);

        CheckInventory();
    }


    // Add item to inventory list and UI, and complete it's corresponding quest if there is one
    public void AddInventoryItem(ItemInfo item)
    {
        items.Add(item);
        inventoryBox.Find(item.itemName.FirstCharacterToUpper()).GetChild(0).gameObject.SetActive(true);

        if (item.questKey != "" && quests[item.questCharKey].Find(q => q.questKey == item.questKey).unlocked)
            CompleteQuest(item.questCharKey, item.questKey);
    }

    // Check all items in inventory and update quests accordingly
    public void CheckInventory()
    {
        foreach (ItemInfo item in items)
        {
            if (item.questKey != "" && quests[item.questCharKey].Find(q => q.questKey == item.questKey).unlocked)
                CompleteQuest(item.questCharKey, item.questKey);
        }
    }

    // Helper function to be called by bunny lair trigger
    public void SetLairDiscovered(bool value)
    {
        lairDiscovered = value;
        CompleteQuest("bunny", "lairSearch");
    }

    // Helper function to be called by game end screen
    public void ContinueFromEndScreen()
    {
        gameEndScreen.SetActive(false);
        gameEndScreenActive = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}


public class DialogueInfo
{
    public string charName;
    public string questToComplete;

    public List<string> questLines;
    public List<string> repeatLines;
    public List<string> endQuestLines;
    public List<string> endRepeatLines;

    public int dialogueProgress = 0;
}

public class QuestInfo
{
    public string questKey;
    public int questLevel;
    
    public string questTitle = "";
    public string questText;

    public bool unlocked = true;
    public bool completed = false;
    public DialogueUpdateOptions updatesDialogue = DialogueUpdateOptions.never;

    public List<string> questsToComplete = null;
    public List<string> questsToUnlock = null;
}

public enum DialogueUpdateOptions { never, onUnlock, onComplete }

public class ItemInfo
{
    public string itemName;
    public string questCharKey;
    public string questKey;

    public ItemInfo(Item item)
    {
        this.itemName = item.itemName;
        this.questCharKey = item.questCharKey;
        this.questKey = item.questKey;
    }
}