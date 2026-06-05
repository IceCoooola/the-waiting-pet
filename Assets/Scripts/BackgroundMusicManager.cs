using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    public AudioSource audioSource;
    public AudioClip startTheme;
    public AudioClip harmoniousTheme;
    public AudioClip basementTheme;
    public AudioClip witchTheme;

    [Header("Transition Settings")]
    public float fadeDuration = 1.0f;
    [Range(0f, 1f)]
    public float maxVolume = 0.5f;

    private GameObject player;
    private GameObject outsideObject;
    private GameObject room1Object;
    private GameObject basementObject;
    
    private Collider2D outsideCollider;
    private Collider2D room1Collider;
    private Collider2D basementCollider;

    private bool isWitchScene = false;
    private AudioClip currentTargetClip;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.playOnAwake = false;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        FindRoomObjects();

        // Start with the starting music
        if (startTheme != null)
        {
            PlayMusic(startTheme);
        }
    }

    private void FindRoomObjects()
    {
        outsideObject = GameObject.Find("Outside");
        if (outsideObject != null)
        {
            outsideCollider = outsideObject.GetComponent<Collider2D>();
            if (outsideCollider == null) outsideCollider = outsideObject.GetComponentInChildren<Collider2D>();
        }

        room1Object = GameObject.Find("2nd Floor Room1");
        if (room1Object == null) room1Object = GameObject.Find("2nd Floor Room 1");
        if (room1Object != null)
        {
            room1Collider = room1Object.GetComponent<Collider2D>();
            if (room1Collider == null) room1Collider = room1Object.GetComponentInChildren<Collider2D>();
        }

        basementObject = GameObject.Find("Basement");
        if (basementObject != null)
        {
            basementCollider = basementObject.GetComponent<Collider2D>();
            if (basementCollider == null) basementCollider = basementObject.GetComponentInChildren<Collider2D>();
        }
    }

    void Update()
    {
        if (isWitchScene) return;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null) return;
        }

        // Check for witch transformation dialogue
        var witchDialogue = FindFirstObjectByType<WitchTransformationDialogue>();
        if (witchDialogue != null)
        {
            isWitchScene = true;
            if (witchTheme != null) PlayMusic(witchTheme);
            return;
        }

        // Room detection logic
        AudioClip nextClip = startTheme;

        // Check basement first (priority)
        if (IsPlayerInRoom(basementObject, basementCollider))
        {
            nextClip = basementTheme;
        }
        // Check outside or 2nd floor room 1
        else if (IsPlayerInRoom(outsideObject, outsideCollider) || IsPlayerInRoom(room1Object, room1Collider))
        {
            nextClip = harmoniousTheme;
        }

        if (nextClip != null && nextClip != currentTargetClip)
        {
            PlayMusic(nextClip);
        }
    }

    private bool IsPlayerInRoom(GameObject roomObj, Collider2D roomCol)
    {
        if (roomCol != null)
        {
            return roomCol.OverlapPoint(player.transform.position);
        }
        if (roomObj != null)
        {
            return roomObj.activeInHierarchy;
        }
        return false;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            currentTargetClip = clip;
            return;
        }

        currentTargetClip = clip;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToClip(clip));
    }

    private IEnumerator FadeToClip(AudioClip clip)
    {
        if (audioSource.isPlaying)
        {
            float startVol = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVol, 0, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = 0;
            audioSource.Stop();
        }

        audioSource.clip = clip;
        if (clip != null)
        {
            audioSource.Play();
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0, maxVolume, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = maxVolume;
        }
    }
}


