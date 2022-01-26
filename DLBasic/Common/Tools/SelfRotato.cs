using UnityEngine;

public class SelfRotato : MonoBehaviour
{
    public Vector2 speedX;
    public Vector2 speedY;
    public Vector2 speedZ;

    private Transform tr;
    private float x;
    private float y;
    private float z;

    private void Awake()
    {
        tr = GetComponent<Transform>();
    }
    private void OnEnable()
    {
        x = Random.Range(speedX.x, speedX.y);
        y = Random.Range(speedY.x, speedY.y);
        z = Random.Range(speedZ.x, speedZ.y);
    }
    private void Update()
    {
        tr.Rotate(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
    }
}