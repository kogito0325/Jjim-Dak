using UnityEngine;

public class LayerArrange : MonoBehaviour
{
    [SerializeField] private Transform[] layers;
    [SerializeField] private Transform far;
    [SerializeField] private Transform near;


    private void Update()
    {
        Arrange();
    }

    private void Arrange()
    {
        float xUnit = (near.position.x - far.position.x) / (layers.Length-1);
        float yUnit = (near.position.y - far.position.y) / (layers.Length-1);

        for (int i = 0; i < layers.Length; i++)
        {
            if (!layers[i]) continue;
            layers[i].position = new Vector3(far.position.x + xUnit*i, far.position.y + yUnit*i, layers[i].position.z);
        }
    }
}
