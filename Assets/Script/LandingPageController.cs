using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LandingPageController : MonoBehaviour
{
    [Header("Clock 1 - Clockwise")]
    public GameObject clock1;
    public RectTransform clock1LongHand;
    public RectTransform clock1ShortHand;
    public float clock1LongHandSpeed = 360f; // derajat per detik
    public float clock1ShortHandSpeed = 30f; // derajat per detik

    [Header("Clock 2 - Counter-Clockwise")]
    public GameObject clock2;
    public RectTransform clock2LongHand;
    public RectTransform clock2ShortHand;
    public float clock2LongHandSpeed = 360f;
    public float clock2ShortHandSpeed = 30f;

    [Header("Clock 3 - Pop-up")]
    public GameObject clock3;

    [Header("Clock 4")]
    public GameObject clock4;

    [Header("Hourglass")]
    public GameObject hourglass1;
    public GameObject hourglass2;

    [Header("Logos")]
    public GameObject eyeLogo; // Logo mata
    public GameObject mainLogo; // Logo utama
    public Image logoShineOverlay; // Image putih transparan untuk efek shine
    public float shineDuration = 2f;
    public float shineWidth = 0.5f; // Lebar cahaya relatif (0-1)
    public float shineAlpha = 0.3f; // Transparansi cahaya (0-1)

    [Header("Transition Settings")]
    public float shakeIntensity = 10f;
    public float shakeDuration = 0.5f;
    public float fastShakeDuration = 0.25f; // Transisi getar cepat untuk jam 3 ke jam 4
    public float zoomOutDuration = 0.8f;
    public float popupScale = 1.5f;
    public float popupDuration = 0.5f;
    public float fadeDuration = 1f;

    private float clock1ShortHandRotation = 0f;
    private float clock2ShortHandRotation = 0f;

    void Start()
    {
        // Setup awal
        clock1.SetActive(true);
        clock2.SetActive(false);
        clock3.SetActive(false);
        clock4.SetActive(false);
        hourglass1.SetActive(false);
        hourglass2.SetActive(false);
        eyeLogo.SetActive(false);
        mainLogo.SetActive(false);

        // Pastikan semua GameObject punya CanvasGroup untuk fade effect
        EnsureCanvasGroup(clock1);
        EnsureCanvasGroup(clock2);
        EnsureCanvasGroup(clock3);
        EnsureCanvasGroup(clock4);
        EnsureCanvasGroup(hourglass1);
        EnsureCanvasGroup(hourglass2);
        EnsureCanvasGroup(eyeLogo);
        EnsureCanvasGroup(mainLogo);

        // Setup shine overlay
        if (logoShineOverlay != null)
        {
            logoShineOverlay.gameObject.SetActive(false);
        }

        StartCoroutine(AnimationSequence());
    }

    void EnsureCanvasGroup(GameObject obj)
    {
        if (obj.GetComponent<CanvasGroup>() == null)
        {
            obj.AddComponent<CanvasGroup>();
        }
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
        // Transisi bergetar dari jam 2 ke jam 3 (tanpa fade)
        yield return StartCoroutine(ShakeTransition(clock2));

        clock2.SetActive(false);
        clock3.SetActive(true);
        // Langsung muncul tanpa fade in

        // Jam 3 mendapat transisi getar cepat (tanpa fade out)
        yield return new WaitForSeconds(1f); // Tampil sebentar
        yield return StartCoroutine(FastShakeTransition(clock3));

        // Phase 4: Jam 4 muncul dengan fade in
        clock3.SetActive(false);
        clock4.SetActive(true);
        yield return StartCoroutine(FadeIn(clock4));
        yield return new WaitForSeconds(1.5f); // Tampil sebentar

        // Fade out jam 4
        yield return StartCoroutine(FadeOut(clock4));

        // Phase 5: Jam Pasir 1 muncul dengan pop-up
        clock4.SetActive(false);
        hourglass1.SetActive(true);
        yield return StartCoroutine(PopUpEffect(hourglass1));
        yield return new WaitForSeconds(1f); // Tampil sebentar

        // Transisi dari jam pasir 1 ke jam pasir 2 dengan getaran saja (tanpa fade)
        yield return StartCoroutine(ShakeTransition(hourglass1));

        hourglass1.SetActive(false);
        hourglass2.SetActive(true);
        // Langsung muncul tanpa fade in
        yield return new WaitForSeconds(1f); // Tampil sebentar

        // Fade out jam pasir 2
        yield return StartCoroutine(FadeOut(hourglass2));

        // Phase 6: Logo Mata muncul dengan pop-up effect
        hourglass2.SetActive(false);
        eyeLogo.SetActive(true);
        yield return StartCoroutine(PopUpEffect(eyeLogo));

        // Fade out logo mata
        yield return StartCoroutine(FadeOut(eyeLogo));

        // Phase 7: Main Logo muncul dengan efek pencahayaan mewah
        eyeLogo.SetActive(false);
        mainLogo.SetActive(true);
        yield return StartCoroutine(FadeIn(mainLogo));
        yield return StartCoroutine(ShineEffect());
    }

    IEnumerator RotateClock1()
    {
        clock1ShortHandRotation = 0f;
        float longHandRotation = 0f;

        while (clock1ShortHandRotation < 360f)
        {
            // Putar jarum panjang (lebih cepat) - searah jarum jam
            longHandRotation += clock1LongHandSpeed * Time.deltaTime;
            clock1LongHand.localRotation = Quaternion.Euler(0, 0, -longHandRotation);

            // Putar jarum pendek (lebih lambat) - searah jarum jam
            clock1ShortHandRotation += clock1ShortHandSpeed * Time.deltaTime;
            clock1ShortHand.localRotation = Quaternion.Euler(0, 0, -clock1ShortHandRotation);

            yield return null;
        }
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
    }

    IEnumerator ShakeTransition(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector2 originalPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPos.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = originalPos.y + Random.Range(-shakeIntensity, shakeIntensity);

            rectTransform.anchoredPosition = new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
    }

    IEnumerator FastShakeTransition(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector2 originalPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        // Getaran lebih cepat dengan durasi lebih pendek
        while (elapsed < fastShakeDuration)
        {
            float x = originalPos.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = originalPos.y + Random.Range(-shakeIntensity, shakeIntensity);

            rectTransform.anchoredPosition = new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
    }

    IEnumerator ZoomOutTransition(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector3 originalScale = rectTransform.localScale;
        float elapsed = 0f;

        while (elapsed < zoomOutDuration)
        {
            float t = elapsed / zoomOutDuration;
            rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * 0.3f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PopUpEffect(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector3 originalScale = rectTransform.localScale;
        rectTransform.localScale = Vector3.zero;

        float elapsed = 0f;

        // Scale up dengan overshoot effect
        while (elapsed < popupDuration)
        {
            float t = elapsed / popupDuration;
            // Elastic ease-out effect
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            rectTransform.localScale = originalScale * scale * popupScale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Scale back to normal
        elapsed = 0f;
        while (elapsed < popupDuration * 0.5f)
        {
            float t = elapsed / (popupDuration * 0.5f);
            rectTransform.localScale = Vector3.Lerp(originalScale * popupScale, originalScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = originalScale;

        // Tunggu sebentar sebelum transisi berikutnya
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FadeOut(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
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

    IEnumerator ShineEffect()
    {
        if (logoShineOverlay == null)
        {
            yield break;
        }

        logoShineOverlay.gameObject.SetActive(true);
        RectTransform shineRect = logoShineOverlay.GetComponent<RectTransform>();
        RectTransform logoRect = mainLogo.GetComponent<RectTransform>();

        // Setup awal shine overlay
        float logoWidth = logoRect.rect.width;
        float logoHeight = logoRect.rect.height;
        float actualShineWidth = logoWidth * shineWidth;

        // Set ukuran shine (strip vertikal lebih tinggi dari logo)
        shineRect.sizeDelta = new Vector2(actualShineWidth, logoHeight * 1.2f);

        // Gunakan gradient untuk efek cahaya alami
        // Note: Untuk gradient yang lebih halus, sebaiknya buat texture gradient di Photoshop
        // Tapi kita bisa simulasi dengan alpha yang smooth

        float elapsed = 0f;
        float startX = -logoWidth / 2 - actualShineWidth;
        float endX = logoWidth / 2 + actualShineWidth;

        // Animasi bergerak dari kiri ke kanan dengan smooth fade
        while (elapsed < shineDuration)
        {
            float t = elapsed / shineDuration;
            elapsed += 0.5f;

            // Posisi shine bergerak dari kiri ke kanan
            float currentX = Mathf.Lerp(startX, endX, t);
            shineRect.anchoredPosition = new Vector2(currentX, 0);

            // Alpha fade in dan fade out untuk efek lebih natural
            float alpha;
            if (t < 0.3f)
            {
                // Fade in di awal (0-30%)
                alpha = Mathf.Lerp(0f, shineAlpha, t / 0.3f);
            }

        }
    }
}