using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    private float originZ;

    private void Start()
    {
        originZ = transform.position.z;
    }

    private void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, originZ);
    }
}
