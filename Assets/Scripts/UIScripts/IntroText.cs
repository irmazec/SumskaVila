using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntroText : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public Button PlayButton;
    public Button BackButton;

    [Header("Settings")]
    public float lettersPerSecond = 30f; // brzina

    [TextArea]
    public string[] sentences;

    private int currentSentence = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        PlayButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        ShowNextSentence();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))    //skip s enter ili lijevi klik
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = sentences[currentSentence];
                isTyping = false;
                currentSentence++;
                if (currentSentence >= sentences.Length)
                {
                    PlayButton.gameObject.SetActive(true);
                    BackButton.gameObject.SetActive(true);
                }
            }
            else
            {
                ShowNextSentence();
            }
        }
    }

    void ShowNextSentence()
    {
        if (currentSentence >= sentences.Length)    // sve recenice ispisane - prikazi botune
        {
            PlayButton.gameObject.SetActive(true);
            BackButton.gameObject.SetActive(true);
            return;
        }

        typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentence]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
        currentSentence++;

        if (currentSentence >= sentences.Length)
        {
            PlayButton.gameObject.SetActive(true);
            BackButton.gameObject.SetActive(true);
        }
    }
}
