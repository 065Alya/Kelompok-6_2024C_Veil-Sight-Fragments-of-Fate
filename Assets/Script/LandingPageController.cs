using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public float fastShakeDuration = 0.25f;
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

        // Pastikan semua GameObject punya CanvasGroup
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
            logoShineOverlay.gameObject.SetActive(false);

        // Jalankan animasi sequence
        StartCoroutine(AnimationSequence());
    }

    void EnsureCanvasGroup(GameObject obj)
    {
        if (obj != null && obj.GetComponent<CanvasGroup>() == null)
            obj.AddComponent<CanvasGroup>();
    }

    IEnumerator AnimationSequence()
    {
        // Clock 1
        yield return StartCoroutine(RotateClock1());
        yield return StartCoroutine(ShakeTransition(clock1));

        // Clock 2
        clock1.SetActive(false);
        clock2.SetActive(true);
        yield return StartCoroutine(RotateClock2());
        yield return StartCoroutine(ZoomOutTransition(clock2));

        // Jam 3 muncul dengan efek pop-up
        // Transisi bergetar dari jam 2 ke jam 3 
        // Phase 3: Clock 3 pop-up
        yield return StartCoroutine(ShakeTransition(clock2));
        clock2.SetActive(false);
        clock3.SetActive(true);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FastShakeTransition(clock3));

        // Clock 4 fade in
        clock3.SetActive(false);
        clock4.SetActive(true);
        yield return StartCoroutine(FadeIn(clock4));
        yield return new WaitForSeconds(1.5f);
        yield return new WaitForSeconds(1.5f); 

        // Fade out jam 4
        yield return StartCoroutine(FadeOut(clock4));

        // Hourglass 1 pop-up
        clock4.SetActive(false);
        hourglass1.SetActive(true);
        yield return StartCoroutine(PopUpEffect(hourglass1));
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(1f); 

        // Hourglass 2
        // Transisi dari jam pasir 1 ke jam pasir 2 dengan getaran saja 
        yield return StartCoroutine(ShakeTransition(hourglass1));
        hourglass1.SetActive(false);
        hourglass2.SetActive(true);
        yield return new WaitForSeconds(1f);
        // Langsung muncul tanpa fade in
        yield return new WaitForSeconds(1f); 

        // Fade out jam pasir 2
        yield return StartCoroutine(FadeOut(hourglass2));

        // Eye Logo
        hourglass2.SetActive(false);
        eyeLogo.SetActive(true);
        yield return StartCoroutine(PopUpEffect(eyeLogo));
        yield return StartCoroutine(FadeOut(eyeLogo));

        // Main Logo
        eyeLogo.SetActive(false);
        mainLogo.SetActive(true);
        yield return StartCoroutine(FadeIn(mainLogo));
        yield return StartCoroutine(ShineEffect());

        // Semua animasi selesai, load PrologFP
        SceneManager.LoadScene("PrologScene", LoadSceneMode.Single);
    }

    #region Clock Animations
    IEnumerator RotateClock1()
    {
        clock1ShortHandRotation = 0f;
        float longHandRotation = 0f;

        while (clock1ShortHandRotation < 360f)
        {
            longHandRotation += clock1LongHandSpeed * Time.deltaTime;
            clock1LongHand.localRotation = Quaternion.Euler(0, 0, -longHandRotation);

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
            // Putar jarum panjang berlawanan arah 
            longHandRotation += clock2LongHandSpeed * Time.deltaTime;
            clock2LongHand.localRotation = Quaternion.Euler(0, 0, longHandRotation);

            // Putar jarum pendek berlawanan arah 
            clock2ShortHandRotation += clock2ShortHandSpeed * Time.deltaTime;
            clock2ShortHand.localRotation = Quaternion.Euler(0, 0, clock2ShortHandRotation);

            yield return null;
        }
    }
    #endregion

    #region Transitions
    IEnumerator ShakeTransition(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector2 original = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = original.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = original.y + Random.Range(-shakeIntensity, shakeIntensity);
            rect.anchoredPosition = new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = original;
    }

    IEnumerator FastShakeTransition(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector2 original = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < fastShakeDuration)
        {
            float x = original.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = original.y + Random.Range(-shakeIntensity, shakeIntensity);
            rect.anchoredPosition = new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = original;
    }

    IEnumerator ZoomOutTransition(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        float elapsed = 0f;

        while (elapsed < zoomOutDuration)
        {
            float t = elapsed / zoomOutDuration;
            rect.localScale = Vector3.Lerp(originalScale, originalScale * 0.3f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PopUpEffect(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        rect.localScale = Vector3.zero;
        float elapsed = 0f;

        while (elapsed < popupDuration)
        {
            float t = elapsed / popupDuration;
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            rect.localScale = originalScale * scale * popupScale;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Scale back
        elapsed = 0f;
        while (elapsed < popupDuration * 0.5f)
        {
            float t = elapsed / (popupDuration * 0.5f);
            rect.localScale = Vector3.Lerp(originalScale * popupScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.localScale = originalScale;
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FadeOut(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0f;
    }

    IEnumerator FadeIn(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            cg.alpha = Mathf.Lerp(0f, 1f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 1f;
    }

    IEnumerator ShineEffect()
    {
        if (logoShineOverlay == null) yield break;

        logoShineOverlay.gameObject.SetActive(true);
        RectTransform shineRect = logoShineOverlay.GetComponent<RectTransform>();
        RectTransform logoRect = mainLogo.GetComponent<RectTransform>();

        float logoWidth = logoRect.rect.width;
        float logoHeight = logoRect.rect.height;
        float actualShineWidth = logoWidth * shineWidth;

        // Set ukuran shine 
        shineRect.sizeDelta = new Vector2(actualShineWidth, logoHeight * 1.2f);



        float elapsed = 0f;
        float startX = -logoWidth / 2 - actualShineWidth;
        float endX = logoWidth / 2 + actualShineWidth;

        while (elapsed < shineDuration)
        {
            float t = elapsed / shineDuration;
            float currentX = Mathf.Lerp(startX, endX, t);
            shineRect.anchoredPosition = new Vector2(currentX, 0);

            // Alpha fade in/out
            float alpha = (t < 0.3f) ? Mathf.Lerp(0f, shineAlpha, t / 0.3f) :
                          (t > 0.7f) ? Mathf.Lerp(shineAlpha, 0f, (t - 0.7f) / 0.3f) :
                          shineAlpha;
            logoShineOverlay.color = new Color(1f, 1f, 1f, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        logoShineOverlay.gameObject.SetActive(false);
    }
    #endregion
}