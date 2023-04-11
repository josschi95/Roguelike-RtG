using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioEvent GameMusic;

    private AudioSource _source;

    public void Play()
    {
        if (_source)
            GameMusic?.Play(_source);
    }

    public void Stop()
    {
        _source.Stop();
    }
}
