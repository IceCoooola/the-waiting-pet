using UnityEngine;

public class GenericSoundEffect : MonoBehaviour
{
    [Header("Sound")]
    public AudioClip clip;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;

    public bool randomPitch = true;

    [Range(0.5f, 1.5f)]
    public float minPitch = 0.95f;

    [Range(0.5f, 1.5f)]
    public float maxPitch = 1.05f;

    [Header("Audio Type")]
    public bool use3DSound = false;

    public bool usePlayClipAtPoint = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    public void PlaySound()
    {
        if (clip == null) return;

        float pitch = randomPitch
            ? Random.Range(minPitch, maxPitch)
            : 1f;

        if (usePlayClipAtPoint)
        {
            GameObject tempSound = new GameObject("TempSound");

            tempSound.transform.position = transform.position;

            AudioSource source =
                tempSound.AddComponent<AudioSource>();

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.spatialBlend = use3DSound ? 1f : 0f;

            source.Play();

            Destroy(tempSound, clip.length + 0.2f);
        }
        else
        {
            audioSource.pitch = pitch;
            audioSource.spatialBlend = use3DSound ? 1f : 0f;

            audioSource.PlayOneShot(clip, volume);
        }
    }
}