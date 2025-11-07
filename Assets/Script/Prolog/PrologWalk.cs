using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrologCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.2f;
    public float durationBeforeChoke = 5f;

    [Header("Camera Settings")]
    public float lookAroundAmount = 10f; // derajat menoleh kanan-kiri
    public float lookAroundSpeed = 0.5f;

    [Header("Head Bob Settings")]
    public float bobAmplitude = 0.003f; // lebih rendah
    public float bobFrequency = 1.2f;

    [Header("Choke / Fall Settings")]
    public float fallDuration = 1.5f;
    public float throwUpHeight = 0.3f; // kamera terlempar sedikit ke atas
    public float cameraShakeAmount = 0.05f;

    [Header("Blackout Settings")]
    public Image blackoutImage; // UI Image fullscreen hitam
    public float blackoutFadeDuration = 2f;
    public float blackoutDelay = 0.5f; // tunggu sedikit sebelum mulai fade

    [Header("Optional Audio")]
    public AudioSource breathingAudio;
    public AudioSource chokeAudio;

    private bool isChoking = false;
    private float lookTimer = 0f;
    private float bobTimer = 0f;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (breathingAudio != null)
            breathingAudio.Play();

        if (blackoutImage != null)
            blackoutImage.color = new Color(0, 0, 0, 0); // transparan awal

        Invoke(nameof(StartChokeSequence), durationBeforeChoke);
    }

    void Update()
    {
        if (!isChoking)
        {
            MoveForward();
            LookAroundSlightly();
            ApplyHeadBob();
        }
    }

    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        startPosition = transform.position; // update referensi headbob
    }

    void LookAroundSlightly()
    {
        lookTimer += Time.deltaTime;
        float angleY = Mathf.Sin(lookTimer * lookAroundSpeed) * lookAroundAmount;
        transform.rotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + angleY, startRotation.eulerAngles.z);
    }

    void ApplyHeadBob()
    {
        bobTimer += Time.deltaTime * bobFrequency;
        float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
        transform.position = startPosition + Vector3.up * bobOffset;
    }

    void StartChokeSequence()
    {
        isChoking = true;
        StartCoroutine(ChokeEffect());
    }

    IEnumerator ChokeEffect()
    {
        if (breathingAudio != null)
            breathingAudio.pitch = 0.6f;

        if (chokeAudio != null)
            chokeAudio.Play();

        float t = 0f;
        Vector3 basePos = transform.position;
        Quaternion baseRot = transform.rotation;

        while (t < fallDuration)
        {
            float fallProgress = t / fallDuration;

            // Shake kecil
            Vector3 shakeOffset = Random.insideUnitSphere * cameraShakeAmount;

            // Efek terlempar sedikit ke atas di awal → turun perlahan
            Vector3 verticalOffset = Vector3.up * throwUpHeight * (1f - fallProgress) - Vector3.up * throwUpHeight * fallProgress;

            // Posisi akhir = base + shake + vertical
            transform.position = basePos + shakeOffset + verticalOffset;

            // Rotasi menghadap ke atas (+X)
            Quaternion targetRot = Quaternion.Euler(-90f, baseRot.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(baseRot, targetRot, fallProgress);

            t += Time.deltaTime;
            yield return null;
        }

        // Setelah jatuh, posisi final & rotasi menghadap atas
        transform.position = basePos + Vector3.up * 0f; // tetap di basePos
        transform.rotation = Quaternion.Euler(-90f, baseRot.eulerAngles.y, 0);

        // Tunggu sebentar sebelum mulai blackout
        if (blackoutDelay > 0)
            yield return new WaitForSeconds(blackoutDelay);

        if (blackoutImage != null)
            yield return StartCoroutine(BlackoutSequence());
    }

    IEnumerator BlackoutSequence()
    {
        // Durasi tiap tahap bisa diatur agar terasa lebih lama
        float stageDuration = blackoutFadeDuration; // misal total 2 detik tiap tahap
        float t;

        // Warna tahap-tahap
        Color startColor = new Color(0, 0, 0, 0f);       // transparan
        Color lightBlack = new Color(0, 0, 0, 0.2f);    // pudar pertama
        Color midBlack = new Color(0, 0, 0, 0.5f);      // mulai terasa dicekik
        Color almostBlack = new Color(0, 0, 0, 0.75f);  // nyaris pingsan
        Color fullBlack = new Color(0, 0, 0, 1f);       // blackout

        // Tahap 1: transparan → pudar pertama
        t = 0f;
        while (t < stageDuration)
        {
            blackoutImage.color = Color.Lerp(startColor, lightBlack, t / stageDuration);
            t += Time.deltaTime;
            yield return null;
        }

        // Tahap 2: pudar pertama → mulai terasa dicekik
        t = 0f;
        while (t < stageDuration)
        {
            blackoutImage.color = Color.Lerp(lightBlack, midBlack, t / stageDuration);
            t += Time.deltaTime;
            yield return null;
        }

        // Tahap 3: mulai terasa dicekik → nyaris pingsan
        t = 0f;
        while (t < stageDuration)
        {
            blackoutImage.color = Color.Lerp(midBlack, almostBlack, t / stageDuration);
            t += Time.deltaTime;
            yield return null;
        }

        // Tahap 4: nyaris pingsan → full blackout
        t = 0f;
        while (t < stageDuration)
        {
            blackoutImage.color = Color.Lerp(almostBlack, fullBlack, t / stageDuration);
            t += Time.deltaTime;
            yield return null;
        }

        // Pastikan final warna benar-benar full black
        blackoutImage.color = fullBlack;
    }

}
