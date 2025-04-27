using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    private float orizinSize;
    private float originZ;
    private float curShakeTime;
    private float curShakeAmount;

    [SerializeField, Range(0.01f, 1f)] private float ShakeAmount;
    [SerializeField, Range(0.01f, 1f)] private float ShakeLittleAmount;
    [SerializeField] private float shakeTime;
    [SerializeField][Range(0.01f, 0.5f)] public float minFollowSpeed;
    
    Vector3 targetPosition;

    private void Start()
    {
        orizinSize = GetComponent<Camera>().orthographicSize;
        originZ = transform.position.z;
        curShakeTime = 0f;
        curShakeAmount = 0f;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Time.timeScale == 0) return;
        targetPosition = new Vector3(Mathf.Clamp(player.position.x, -3f, 3f), 0, originZ);
        if (curShakeTime <= 0)
        {
            float followSpeed = Mathf.Max(Time.deltaTime * Vector2.Distance(transform.position, targetPosition), minFollowSpeed);
            transform.position = Vector2.Lerp(transform.position, targetPosition, followSpeed);
        }
        else
        {
            transform.position = Random.insideUnitSphere * curShakeAmount + targetPosition;
            curShakeTime -= Time.deltaTime;
        }
        transform.position += Vector3.back * 10;
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, orizinSize, Time.deltaTime*10);
        GetComponent<PixelPerfectCamera>().assetsPPU = (int)Mathf.Lerp(GetComponent<PixelPerfectCamera>().assetsPPU, 100, Time.deltaTime*5);
    }

    public void ShakeCamera(float time = 0)
    {
        if (time == 0) curShakeTime = shakeTime;
        else curShakeTime = time;
        curShakeAmount = ShakeAmount;
    }

    public void ShakeLittleCamera(float amount)
    {
        transform.position = Random.insideUnitSphere * amount + targetPosition;
    }

    public void EffectCamera()
    {
        GetComponent<Camera>().orthographicSize = 4.7f;
        GetComponent<PixelPerfectCamera>().assetsPPU = 110;
    }
}
