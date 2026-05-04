using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject doorModel;

    public void CloseDoor()
    {
        doorModel.SetActive(true);
    }

    public void OpenDoor()
    {
        doorModel.SetActive(false);
    }
}
