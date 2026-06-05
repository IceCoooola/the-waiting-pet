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
    
    private struct RoomConfig {
        public string name;
        public Bounds bounds;
        public AudioClip clip;
        public float area;
    }

    private List<RoomConfig> roomConfigs = new List<RoomConfig>();
    private bool isWitchScene = false;
    private AudioClip currentTargetClip;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        if (!audioSource.isPlaying) audioSource.volume = 0;
    }

    void Start()
    {
        InitializeRoomConfigs();

        if (startTheme != null)
        {
            PlayMusic(startTheme);
        }
    }

    private void InitializeRoomConfigs()
    {
        roomConfigs.Clear();
        
        // Define rooms and their themes
        AddRoomConfig("Hallway", startTheme);
        AddRoomConfig("Bedroom", startTheme);
        AddRoomConfig("Living Room", startTheme);
        AddRoomConfig("Magic Room", startTheme);
        AddRoomConfig("2nd Floor Room 1", harmoniousTheme);
        AddRoomConfig("2nd Floor Room1", harmoniousTheme);
        AddRoomConfig("Basement", basementTheme);
        AddRoomConfig("Outside", harmoniousTheme);

        // Sort by area (smallest first) to prioritize more specific rooms
        roomConfigs.Sort((a, b) => a.area.CompareTo(b.area));
    }

    private void AddRoomConfig(string name, AudioClip clip)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;

        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            b.Encapsulate(renderers[i].bounds);
        }

        roomConfigs.Add(new RoomConfig {
            name = name,
            bounds = b,
            clip = clip,
            area = b.size.x * b.size.y
        });
    }

    void Update()
    {
        if (isWitchScene) return;

        if (player == null)
        {
            player = GameObject.Find("WitchPlayer");
            if (player == null) player = GameObject.Find("GoldenRetrieverPlayer");
            if (player == null) player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                var movement = FindFirstObjectByType<PlayerMovement>();
                if (movement != null) player = movement.gameObject;
            }
            if (player == null) return;
        }

        // Check for witch transformation dialogue
        var witchDialogue = FindFirstObjectByType<WitchTransformationDialogue>();
        if (witchDialogue != null)
        {
            isWitchScene = true;
            if (witchTheme != null) 
            {
                PlayMusic(witchTheme);
            }
            else
            {
                Debug.LogWarning("[MusicManager] Witch scene triggered but Witch Theme clip is missing (check file format).");
            }
            return;
        }

        // Determine target clip based on player position
        AudioClip nextClip = startTheme;
        Vector3 playerPos = player.transform.position;

        foreach (var config in roomConfigs)
        {
            if (config.bounds.Contains(playerPos))
            {
                if (config.clip != null)
                {
                    nextClip = config.clip;
                }
                break;
            }
        }

        if (nextClip != null && nextClip != currentTargetClip)
        {
            PlayMusic(nextClip);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource == null) EnsureAudioSource();
        
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