using UnityEngine;
using System.Collections;

public class DogOrbInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject orb2;
    public BasementBonePickup bonePickup;

    [Header("Camera")]
    public Vector3 basementCamPos   = new Vector3(-53.1f, 28.8f, -10f);
    public float   basementOrtho    = 4.3f;
    public float   zoomOrtho        = 3.0f;
    public float   zoomDuration     = 2.2f;

    private bool phase1Done = false;
    private bool solved     = false;
    private bool playerNear = false;
    // Prevents the Space press that dismissed the last whisper line from
    // immediately firing Phase 2 on the same frame.
    private bool _dialogueWasPlaying = false;


void Awake()
    {
        // Solid collider — reuse existing BoxCollider2D rather than adding duplicates
        var solid       = GetComponent<BoxCollider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        solid.size      = new Vector2(0.65f, 0.75f);
        solid.isTrigger = false;

        // Proximity trigger — only add one CircleCollider2D if none exists yet
        CircleCollider2D trig = null;
        foreach (var c in GetComponents<CircleCollider2D>())
            if (c.isTrigger) { trig = c; break; }
        if (trig == null) trig = gameObject.AddComponent<CircleCollider2D>();
        trig.radius    = 1.1f;
        trig.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) playerNear = true; }
    void OnTriggerExit2D(Collider2D other)  { if (other.CompareTag("Player")) playerNear = false; }

void Update()
    {
        if (!playerNear || solved) return;

        bool isPlaying      = BasementDialogueSequence.Instance != null
                              && BasementDialogueSequence.Instance.IsPlaying;
        bool wasPlaying     = _dialogueWasPlaying;
        _dialogueWasPlaying = isPlaying;

        // Block while dialogue is active
        if (isPlaying) return;
        // Skip the frame immediately after dialogue ends so the Space that
        // dismissed the last line cannot simultaneously trigger Phase 2.
        if (wasPlaying) return;

        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (!phase1Done)
        {
            phase1Done = true;
            BasementDialogueSequence.Instance.Play(new[] {
                "[Faint whispers of needing a bone...]",
                "The path is blocked... how do I get over there?"
            }); // no callback — movement stays unlocked
        }
        else if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem("bone"))
        {
            solved = true;
            InventoryManager.Instance.RemoveItem("bone");
            if (bonePickup != null) bonePickup.gameObject.SetActive(false);
            StartCoroutine(SolveSequence());
        }
    }

    IEnumerator LerpCamera(Camera cam, Vector3 from, float fromO, Vector3 to, float toO, float dur)
    {
        float t = 0f;
        while (t < 1f) {
            t = Mathf.MoveTowards(t, 1f, Time.deltaTime / dur);
            float s = Mathf.SmoothStep(0f, 1f, t);
            cam.transform.position = Vector3.Lerp(from, to, s);
            cam.orthographicSize   = Mathf.Lerp(fromO, toO, s);
            yield return null;
        }
    }

IEnumerator SolveSequence()
    {
        PlayerMovement.movementLocked = true;

        // Enable sprite glow rings (CrystalGlowPulse)
        var cgp        = GetComponent<CrystalGlowPulse>();
        if (cgp != null) {
            cgp.glowColor  = new Color(0f, 0.616f, 1f, 1f); // #009DFF
            cgp.pulseSpeed = 1.2f;
            cgp.enabled    = true;
        }

        // Enable Rim Light 2D + its pulse
        var rimTransform = transform.Find("RimLight2D");
        if (rimTransform != null) {
            var rimLight = rimTransform.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
            var rimPulse = rimTransform.GetComponent<OrbRimLightPulse>();
            if (rimLight != null) rimLight.enabled = true;
            if (rimPulse  != null) rimPulse.enabled  = true;
        }

        var cam        = Camera.main;
        var zoomTarget = new Vector3(transform.position.x, transform.position.y, -10f);
        yield return StartCoroutine(LerpCamera(cam,
            cam.transform.position, cam.orthographicSize,
            zoomTarget, zoomOrtho, zoomDuration));

        bool d1 = false;
        BasementDialogueSequence.Instance.Play(
            new[] { "I remember... I was a dog." }, () => d1 = true);
        yield return new WaitUntil(() => d1);

        yield return StartCoroutine(LerpCamera(cam,
            cam.transform.position, cam.orthographicSize,
            basementCamPos, basementOrtho, zoomDuration * 1.3f));

        bool d2 = false;
        BasementDialogueSequence.Instance.Play(
            new[] { "The orbs show the past." }, () => d2 = true);
        yield return new WaitUntil(() => d2);

        PlayerMovement.movementLocked = false;
    }
}
