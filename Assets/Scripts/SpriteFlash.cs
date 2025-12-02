using System.Collections;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    public Material flashMaterial;
    public float flashDuration = 0.1f;

    private SpriteRenderer sr;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        sr.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        sr.material = originalMaterial;
    }
}
