using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempQuestTest : MonoBehaviour
{
    public int function = 0;
    public string charKey = "bear";
    public string questKey = "fish";

    private void OnTriggerEnter(Collider other)
    {
        if (function == 0)
            GameManager.GM.AddCharQuests(charKey);
        if (function == 1)
            GameManager.GM.CompleteQuest(charKey, questKey);
        if (function == 2)
            GameManager.GM.UnlockQuest(charKey, questKey);
    }
}
