using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public GameObject dialogueCanvas;
    public GameObject tooltip;
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueTitle;
    public TextMeshProUGUI dialogueText;
    public Dictionary<string, CharInfo> charQuests;

    void Awake()
    {
        GM = this;
        charQuests = JsonConvert.DeserializeObject<Dictionary<string, CharInfo>>(Resources.Load<TextAsset>("char_quests").text);
    }
}
