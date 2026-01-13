using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : MonoBehaviour
{
    public string questCharKey;
    public string questKey;
    public TextMeshProUGUI title;
    public TextMeshProUGUI text;

    private int defaultPadding;
    private VerticalLayoutGroup layoutGroup;
    private List<string> questsToComplete;
    private List<string> questsToUnlock;

    public void InitQuest(string charKey, QuestInfo info)
    {
        // Init properties
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        defaultPadding = layoutGroup.padding.top;
        // Subscribe to quest events
        GameManager.GM.OnQuestCompleted += CompleteQuest;
        GameManager.GM.OnQuestUnlocked += UnlockQuest;

        questCharKey = charKey;
        questKey = info.questKey;
        if (title  != null)
            title.text = info.questTitle;
        text.text = info.questText;
        questsToComplete = info.questsToComplete;
        questsToUnlock = info.questsToUnlock;

        // Hide if not unlocked
        if (!info.unlocked)
        {
            title?.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            layoutGroup.padding.top = 0;
            layoutGroup.padding.bottom = 0;
        }
    }

    // If quest corresponds to completed quest, update it's presentation
    // Else, if completed quest is subquest of this quest, mark it off the list and complete this quest if all subquests are completed
    // Else, if completed quest is required to unlock this quest, mark it off the list and unlock this quest if all required quests are completed
    public void CompleteQuest(string key)
    {
        if (key == questKey)
        {
            if (title != null)
                title.fontStyle = FontStyles.Bold | FontStyles.Underline | FontStyles.Strikethrough | FontStyles.Italic;
            text.fontStyle = FontStyles.Strikethrough | FontStyles.Italic;
        }
        else if (questsToComplete?.Contains(key) == true)
        {
            questsToComplete.Remove(key);
            if (questsToComplete.Count == 0)
                GameManager.GM.CompleteQuest(questCharKey, questKey);
        }
        else if (questsToUnlock?.Contains(key) == true)
        {
            questsToUnlock.Remove(key);
            if (questsToUnlock.Count == 0)
                GameManager.GM.UnlockQuest(questCharKey, questKey);
        }
    }

    // If quest corresponds to unlocked quest, show (unhide) it
    public void UnlockQuest(string key)
    {
        if (key != questKey)
            return;

        title?.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        layoutGroup.padding.top = defaultPadding;
        layoutGroup.padding.bottom = defaultPadding;
    }
}
