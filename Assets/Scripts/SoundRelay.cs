using UnityEngine;

public class SoundRelay : MonoBehaviour
{
    [SerializeField] float vol;
    public void SendSound(string name)
    {
        AudioManager.Instance.PlaySoundAmbient(name,vol);
    }

}
