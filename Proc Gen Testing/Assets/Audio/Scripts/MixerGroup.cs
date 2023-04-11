using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Mixer Group")]
public class MixerGroup : ScriptableObject
{
    public AudioMixerGroup _MixerGroup;

    public void SetVolume(float targetVolume)
    {
        float targetFloat = (targetVolume > 0) ? Mathf.Log10(targetVolume) * 20 : -80;
        _MixerGroup.audioMixer.SetFloat(_MixerGroup.name, targetFloat);
    }
}