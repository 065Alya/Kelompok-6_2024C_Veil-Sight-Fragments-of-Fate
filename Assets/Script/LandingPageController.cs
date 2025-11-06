using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LandingPageController : MonoBehaviour
{
    [Header("Clock 1 - Clockwise")]
    public GameObject clock1;
    public Transform clock1LongHand;
    public Transform clock1ShortHand;
    public float clock1LongHandSpeed = 360f; // derajat per detik
    public float clock1ShortHandSpeed = 30f; // derajat per detik

    [Header("Clock 2 - Counter-Clockwise")]
    public GameObject clock2;
    public Transform clock2LongHand;
    public Transform clock2ShortHand;
    public float clock2LongHandSpeed = 360f;
    public float clock2ShortHandSpeed = 30f;

    [Header("Clock 3 - Pop-up")]
    public GameObject clock3;

    [Header("Hourglass")]
    public GameObject hourglass;

    [Header("Transition Settings")]
    public float shakeIntensity = 10f;
    public float shakeDuration = 0.5f;
    public float zoomOutDuration = 0.8f;
    public float popupScale = 1.5f;
    public float popupDuration = 0.5f;
    public float fadeDuration = 1f;

    private float clock1ShortHandRotation = 0f;
    private float clock2ShortHandRotation = 0f;
    private bool clock1Complete = false;
    private bool clock2Complete = false;

    void Start()
    {
        // Setup awal
        clock1.SetActive(true);
        clock2.SetActive(false);
        clock3.SetActive(false);
        hourglass.SetActive(false);

        StartCoroutine(AnimationSequence());
    }

    IEnumerator AnimationSequence()
    {
        // Phase 1: Jam 1 berputar searah jarum jam
        yield return StartCoroutine(RotateClock1());

        // Transisi bergetar
        yield return StartCoroutine(ShakeTransition(clock1));

        // Phase 2: Jam 2 berputar berlawanan arah jarum jam
        clock1.SetActive(false);
        clock2.SetActive(true);
        yield return StartCoroutine(RotateClock2());

        // Transisi zoom out
        yield return StartCoroutine(ZoomOutTransition(clock2));

        // Phase 3: Jam 3 muncul dengan efek pop-up
        clock2.SetActive(false);
        clock3.SetActive(true);
        yield return StartCoroutine(PopUpEffect(clock3));

        // Fade out jam 3
        yield return StartCoroutine(FadeOut(clock3));

        // Tampilkan jam pasir
        clock3.SetActive(false);
        hourglass.SetActive(true);
        StartCoroutine(FadeIn(hourglass));
    }

    IEnumerator RotateClock1()
    {
        clock1ShortHandRotation = 0f;
        float longHandRotation = 0f;

        while (clock1ShortHandRotation < 360f)
        {
            // Putar jarum panjang (lebih cepat)
            longHandRotation += clock1LongHandSpeed * Time.deltaTime;
            clock1LongHand.localRotation = Quaternion.Euler(0, 0, -longHandRotation);

            // Putar jarum pendek (lebih lambat)
            clock1ShortHandRotation += clock1ShortHandSpeed * Time.deltaTime;
            clock1ShortHand.localRotation = Quaternion.Euler(0, 0, -clock1ShortHandRotation);

            yield return null;
        }

        clock1Complete = true;
    }

    IEnumerator RotateClock2()
    {
        clock2ShortHandRotation = 0f;
        float longHandRotation = 0f;

        while (clock2ShortHandRotation < 360f)
        {
            // Putar jarum panjang berlawanan arah (lebih cepat)
            longHandRotation += clock2LongHandSpeed * Time.deltaTime;
            clock2LongHand.localRotation = Quaternion.Euler(0, 0, longHandRotation);

            // Putar jarum pendek berlawanan arah (lebih lambat)
            clock2ShortHandRotation += clock2ShortHandSpeed * Time.deltaTime;
            clock2ShortHand.localRotation = Quaternion.Euler(0, 0, clock2ShortHandRotation);

            yield return null;
        }

        clock2Complete = true;
    }

    IEnumerator ShakeTransition(GameObject obj)
    {
        Vector3 originalPos = obj.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPos.x + Random.Range(-shakeIntensity, shakeIntensity) * 0.01f;
            float y = originalPos.y + Random.Range(-shakeIntensity, shakeIntensity) * 0.01f;

            obj.transform.position = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = originalPos;
    }

    IEnumerator ZoomOutTransition(GameObject obj)
    {
        Vector3 originalScale = obj.transform.localScale;
        float elapsed = 0f;

        while (elapsed < zoomOutDuration)
        {
            float t = elapsed / zoomOutDuration;
            obj.transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.3f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PopUpEffect(GameObject obj)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.localScale = Vector3.zero;

        float elapsed = 0f;

        // Scale up dengan overshoot effect
        while (elapsed < popupDuration)
        {
            float t = elapsed / popupDuration;
            // Elastic ease-out effect
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            obj.transform.localScale = originalScale * scale * popupScale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Scale back to normal
        elapsed = 0f;
        while (elapsed < popupDuration * 0.5f)
        {
            float t = elapsed / (popupDuration * 0.5f);
            obj.transform.localScale = Vector3.Lerp(originalScale * popupScale, originalScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = originalScale;

        // Tunggu sebentar sebelum fade
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FadeOut(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    IEnumerator FadeIn(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}