using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, .01f);

        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Wall")
            {
                Destroy(collider.gameObject);
                return;
            }
        }

        GetComponent<Collider>().enabled = false;
    }
}
