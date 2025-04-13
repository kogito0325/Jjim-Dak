using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    private float originZ;
    private float curShakeTime;

    [SerializeField, Range(0.01f, 1f)] private float ShakeAmount;
    [SerializeField] private float shakeTime;
    
    Vector3 initialPosition;

    private void Start()
    {
        originZ = transform.position.z;
        curShakeTime = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Time.timeScale == 0) return;
        initialPosition = new Vector3(Mathf.Clamp(player.position.x, -3.6f, 3.6f), 0, originZ);
        if (curShakeTime <= 0) transform.position = initialPosition;
        else
        {
            transform.position = Random.insideUnitSphere * ShakeAmount + initialPosition;
            curShakeTime -= Time.deltaTime;
        }
    }

    public void ShakeCamera(float time = 0)
    {
        if (time == 0) curShakeTime = shakeTime;
        else curShakeTime = time;
    }

}
