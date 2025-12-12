using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    [Header("Dialogue")]
    public GameObject dialogueCanvas;
    public GameObject tooltip;
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueTitle;
    public TextMeshProUGUI dialogueText;
    public Dictionary<string, DialogueInfo> dialogue;

    [Header("Quests")]
    public Transform questBox;
    public GameObject questPrefab;
    public GameObject subquestPrefab;
    public Dictionary<string, List<QuestInfo>> quests;
    public event Action<string> OnQuestCompleted;
    public event Action<string> OnQuestUnlocked;

    void Awake()
    {
        GM = this;
        dialogue = JsonConvert.DeserializeObject<Dictionary<string, DialogueInfo>>(Resources.Load<TextAsset>("dialogue").text);
        quests = JsonConvert.DeserializeObject<Dictionary<string, List<QuestInfo>>>(Resources.Load<TextAsset>("quests").text);
    }

    // Add all quests for a given character to the Quest UI; check if any are already finished
    public void AddCharQuests(string charKey)
    {
        foreach (QuestInfo quest in quests[charKey])
        {
            GameObject q = Instantiate(quest.questLevel == 0 ? questPrefab : subquestPrefab, questBox);
            q.GetComponent<UIQuestItem>().InitQuest(charKey, quest);
        }

        CheckInventory();
    }

    // Mark quest as complete and update UI
    public void CompleteQuest(string charKey, string questKey)
    {
        quests[charKey].Find(q => q.questKey == questKey).completed = true;
        OnQuestCompleted?.Invoke(questKey);
    }

    // Mark quest as unlocked and update UI; check if it's already finished
    public void UnlockQuest(string charKey, string questKey)
    {
        quests[charKey].Find(q => q.questKey == questKey).unlocked = true;
        OnQuestUnlocked?.Invoke(questKey);

        CheckInventory();
    }

    // Check all items in inventory and update quests accordingly
    public void CheckInventory()
    {
        // iterira kroz sve iteme u inventoryju
        // ako item ima pripadajuci quest, provjerava jel taj quest unlocked
        // ako je, completea ga
    }
}


public class DialogueInfo
{
    public string charName;

    public List<string> questLines;
    public List<string> repeatLines;
    public List<string> endLines;

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

    public List<string> questsToComplete = null;
    public List<string> questsToUnlock = null;
}