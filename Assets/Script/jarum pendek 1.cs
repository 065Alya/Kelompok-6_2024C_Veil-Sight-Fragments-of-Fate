using UnityEngine;
using System.Collections;

public class JarumPendek1Controller : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private GameObject jam1;
    [SerializeField] private GameObject jam2;

    private float rotationAmount = 0f;
    private bool sudahSelesai = false;

    void Update()
    {
        if (sudahSelesai) return;

        float delta = speed * Time.deltaTime;
        transform.Rotate(0, 0, -delta); // searah jarum jam
        rotationAmount += delta;

        if (rotationAmount >= 360f)
        {
            rotationAmount = 0f;
            StartCoroutine(GantiKeJamBerikut());
        }
    }

    IEnumerator GantiKeJamBerikut()
    {
        sudahSelesai = true;

        // Efek getar kecil di jam1
        RectTransform rt = jam1.GetComponent<RectTransform>();
        Vector3 originalPos = rt.localPosition;

        float shakeDuration = 0.3f;
        float shakeStrength = 6f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeStrength, shakeStrength);
            float offsetY = Random.Range(-shakeStrength, shakeStrength);
            rt.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.localPosition = originalPos;

        // Fade-out jam1
        CanvasGroup cg1 = jam1.GetComponent<CanvasGroup>();
        if (cg1 == null) cg1 = jam1.AddComponent<CanvasGroup>();
        for (float t = 0; t < 1; t += Time.deltaTime / 0.5f)
        {
            cg1.alpha = 1 - t;
            yield return null;
        }
        cg1.alpha = 0f;
        jam1.SetActive(false);

        // Fade-in jam2
        jam2.SetActive(true);
        CanvasGroup cg2 = jam2.GetComponent<CanvasGroup>();
        if (cg2 == null) cg2 = jam2.AddComponent<CanvasGroup>();
        cg2.alpha = 0f;

        for (float t = 0; t < 1; t += Time.deltaTime / 0.5f)
        {
            cg2.alpha = t;
            yield return null;
        }
        cg2.alpha = 1f;
    }
}