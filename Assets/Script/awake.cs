using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public GameObject[] jamObjects;

    void Awake()
    {
        foreach (var jam in jamObjects)
        {
            if (jam != null)
                jam.SetActive(false);
        }
    }
}
