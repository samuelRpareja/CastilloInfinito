using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    public GameObject room;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            room.GetComponent<RoomBehaveor>().isPlayerOnRoom = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            room.GetComponent<RoomBehaveor>().isPlayerOnRoom = false;
        }
    }
}
