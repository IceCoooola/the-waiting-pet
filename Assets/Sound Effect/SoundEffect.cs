using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = 1f;

    public bool randomPitch = true;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    public bool usePlayClipAtPoint = true;

    public void PlaySound()
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (usePlayClipAtPoint)
        {
            GameObject temp = new GameObject("Temp Sound");
            temp.transform.position = transform.position;

            AudioSource source = temp.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : 1f;
            source.spatialBlend = 0f;

            source.Play();
            Destroy(temp, clip.length + 0.5f);
        }
        else
        {
            AudioSource source = GetComponent<AudioSource>();
            if (source == null) source = gameObject.AddComponent<AudioSource>();

            source.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : 1f;
            source.PlayOneShot(clip, volume);
        }
    }
}