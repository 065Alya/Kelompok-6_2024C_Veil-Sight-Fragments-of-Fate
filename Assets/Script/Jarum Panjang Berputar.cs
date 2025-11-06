using UnityEngine;

public class JarumPanjang1Controller : MonoBehaviour
{
    public float speed = 180f; // lebih cepat dari jarum pendek

    void Update()
    {
        transform.Rotate(0, 0, -speed * Time.deltaTime); // searah jarum jam
    }
}