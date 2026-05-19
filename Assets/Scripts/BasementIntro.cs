using UnityEngine;
using System.Collections;

/// <summary>
/// Locks player on entry, plays intro dialogue, then unlocks.
/// </summary>
public class BasementIntro : MonoBehaviour
{
    [Header("Camera")]
    public Vector3 basementCamPos   = new Vector3(-53.1f, 28.8f, -10f);
    public float   basementCamOrtho = 4.3f;

    void Start()
    {
        var cam = Camera.main;
        if (cam != null) {
            cam.transform.position = basementCamPos;
            cam.orthographicSize   = basementCamOrtho;
            foreach (var mb in cam.GetComponents<MonoBehaviour>())
                if (mb.GetType().Name == "CameraFollow") mb.enabled = false;
        }
        PlayerMovement.movementLocked = true;
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.6f);
        bool done = false;
        BasementDialogueSequence.Instance?.Play(new[] {
            "I've turned into a fish!",
            "An orb... let me check it out."
        }, () => done = true);
        yield return new WaitUntil(() => done);
        PlayerMovement.movementLocked = false; // UNLOCK after intro finishes
    }
}
