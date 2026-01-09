using UnityEngine;

public class ChangeMusicOnStart : MonoBehaviour
{
    public AudioClip newMusic;

    void Start()
    {
        AudioSource source = MusicManager.Instance.GetComponent<AudioSource>();

        if (source.clip != newMusic)
        {
            source.clip = newMusic;
            source.Play();
        }
    }
}
