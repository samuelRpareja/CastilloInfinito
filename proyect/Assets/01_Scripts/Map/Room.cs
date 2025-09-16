using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] walls;
    public GameObject[] doors;
    public bool[] status = new bool[4];
    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            this.status[i] = status[i];
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
