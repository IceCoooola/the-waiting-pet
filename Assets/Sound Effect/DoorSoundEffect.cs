using UnityEngine;

public class DoorSoundEffect : MonoBehaviour
{
    [Header("Door Sounds")]
    public AudioClip openClip;
    public AudioClip lockedClip;

    [Header("Volumes")]
    [Range(0f, 1f)]
    public float openVolume = 1f;

    [Range(0f, 1f)]
    public float lockedVolume = 1f;

    [Header("Pitch")]
    public float openPitch = 1f;

    public float lockedPitch = 1f;

    [Header("Random Pitch Variation")]
    public bool randomPitch = true;

    [Range(0f, 0.5f)]
    public float pitchVariation = 0.05f;

    [Header("Audio Type")]
    public bool use3DSound = false;

    private void PlayClip(
        AudioClip clip,
        float volume,
        float basePitch
    )
    {
        if (clip == null) return;

        GameObject tempSound =
            new GameObject("TempDoorSound");

        tempSound.transform.position = transform.position;

        AudioSource source =
            tempSound.AddComponent<AudioSource>();

        source.clip = clip;

        source.volume = volume;

        float finalPitch = basePitch;

        if (randomPitch)
        {
            finalPitch += Random.Range(
                -pitchVariation,
                pitchVariation
            );
        }

        source.pitch = finalPitch;

        source.spatialBlend =
            use3DSound ? 1f : 0f;

        source.Play();

        Destroy(tempSound, clip.length + 0.2f);
    }

    public void PlayOpen()
    {
        PlayClip(
            openClip,
            openVolume,
            openPitch
        );
    }

    public void PlayLocked()
    {
        PlayClip(
            lockedClip,
            lockedVolume,
            lockedPitch
        );
    }
}