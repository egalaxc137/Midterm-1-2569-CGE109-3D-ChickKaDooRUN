using UnityEngine;

public class ItemAnimate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotateSpeed = 90f;

    [Header("Float Settings")]
    public float floatAmplitude = 0.15f;
    public float floatFrequency = 2f;

    private float startLocalY; 

    void Start()
    {

        startLocalY = transform.localPosition.y;
    }

    void Update()
    {
 
        transform.Rotate(Vector3.up * rotateSpeed * Time.unscaledDeltaTime, Space.World);

       
        Vector3 localPos = transform.localPosition;
        localPos.y = startLocalY + (Mathf.Sin(Time.unscaledTime * floatFrequency) * floatAmplitude);

      
        transform.localPosition = localPos;
    }
}
