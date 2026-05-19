using UnityEngine;

public class UserGuide : MonoBehaviour
{
    public GameObject manualPanel;
    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            isOpen = !isOpen;
            manualPanel.SetActive(isOpen);
        }
    }
}