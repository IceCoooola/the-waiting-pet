using UnityEngine;

public class FishtankTeleport : MonoBehaviour
{
    [Header("Destination")]
    public Transform destinationPoint;

    [Header("Room Toggling (Optional)")]
    public GameObject roomToDisable;
    public GameObject roomToEnable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.EnterFishtankTeleport(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.ExitFishtankTeleport(this);
        }
    }

    public void Teleport(PlayerMovement player)
    {
        if (destinationPoint == null)
        {
            Debug.LogWarning("[FishtankTeleport] Destination point is not set!");
            return;
        }

        player.transform.position = destinationPoint.position;

        // Change appearance to fish
        PlayerAppearanceSwitcher switcher = player.GetComponent<PlayerAppearanceSwitcher>();
        if (switcher != null)
        {
            switcher.SwitchToFish();
        }

        if (roomToDisable != null) roomToDisable.SetActive(false);
        if (roomToEnable != null) roomToEnable.SetActive(true);

        Debug.Log($"[FishtankTeleport] Player teleported to {destinationPoint.name} and turned into a fish!");
    }
}