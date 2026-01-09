using UnityEngine;

public class ChangeMusicOnStart : MonoBehaviour
{
    public AudioClip newMusic;

    void Start()
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("MusicManager not found! Cannot change music.");   // pojavljuje se kad krenemo s pokrrtanjem od 3. scene pri testiranju
            return;
        }

        AudioSource source = MusicManager.Instance.GetComponent<AudioSource>();

        if (source.clip != newMusic)
        {
            source.clip = newMusic;
            source.Play();
        }
    }

}
