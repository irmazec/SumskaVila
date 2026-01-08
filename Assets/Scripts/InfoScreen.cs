using UnityEngine;

public class InfoScreen : MonoBehaviour
{
    public GameObject[] panels;   // map=0, quests=1, inventory=2
    public int startIndex = 0;

    private int currentIndex;

    void OnEnable()
    {
        currentIndex = Mathf.Clamp(startIndex, 0, panels.Length - 1);
        ShowCurrent();
    }

    public void Next()
    {
        currentIndex = (currentIndex + 1) % panels.Length;
        ShowCurrent();
    }

    public void Previous()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = panels.Length - 1;

        ShowCurrent();
    }

    void ShowCurrent()
    {
        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(i == currentIndex);
    }
}
