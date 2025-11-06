using UnityEngine;
using System.Collections;

public class JarumPendek2Controller : MonoBehaviour
{
    public float speed = 20f;
    public GameObject jam1;
    public GameObject jam2;
    public GameObject jam3;

    private bool sudahBerputar = false;
    private bool bolehMulai = false;

    void Start()
    {
        // Nonaktifkan jam3 di awal
        if (jam3 != null)
        {
            jam3.SetActive(false);
            CanvasGroup cg = jam3.GetComponent<CanvasGroup>();
            if (cg == null) cg = jam3.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
        }

        StartCoroutine(TungguJam1Selesai());
    }

    IEnumerator TungguJam1Selesai()
    {
        while (jam1 != null && jam1.activeSelf)
            yield return null;

        bolehMulai = true;
        Debug.Log("✅ Jarum 2 mulai berputar!");
    }

    void Update()
    {
        if (!bolehMulai || sudahBerputar) return;

        transform.Rotate(Vector3.forward * speed * Time.deltaTime); // lawan arah jarum jam

        float rotasiZ = transform.localEulerAngles.z;
        if (rotasiZ >= 359f || rotasiZ <= 1f)
        {
            sudahBerputar = true;
            StartCoroutine(GantiKeJamBerikut());
        }
    }

    private IEnumerator GantiKeJamBerikut()
    {
        // Getar ekstrem
        Vector3 posisiAwal = transform.parent.localPosition;
        for (int i = 0; i < 20; i++)
        {
            transform.parent.localPosition = posisiAwal + Random.insideUnitSphere * 6f;
            yield return new WaitForSeconds(0.015f);
        }
        transform.parent.localPosition = posisiAwal;

        // Splash transisi ke jam3
        if (jam3 != null)
        {
            jam3.SetActive(true);

            CanvasGroup cg2 = jam2.GetComponent<CanvasGroup>();
            CanvasGroup cg3 = jam3.GetComponent<CanvasGroup>();
            if (cg2 == null) cg2 = jam2.AddComponent<CanvasGroup>();
            if (cg3 == null) cg3 = jam3.AddComponent<CanvasGroup>();

            cg3.alpha = 0f;
            jam3.transform.localScale = Vector3.zero;

            float durasi = 1.2f;
            float timer = 0f;

            while (timer < durasi)
            {
                timer += Time.deltaTime;
                float t = timer / durasi;
                cg2.alpha = 1f - t;
                cg3.alpha = t;

                float scale = Mathf.Sin(t * Mathf.PI) * 1.3f;
                jam3.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            cg2.alpha = 0f;
            jam2.SetActive(false);
            cg3.alpha = 1f;
            jam3.transform.localScale = Vector3.one;
        }
    }
}