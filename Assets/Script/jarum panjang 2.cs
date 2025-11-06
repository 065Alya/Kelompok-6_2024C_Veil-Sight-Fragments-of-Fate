using UnityEngine;

public class JarumPanjang2Controller : MonoBehaviour
{
    public float speed = 180f;

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime); // berlawanan arah jarum jam
    }
}