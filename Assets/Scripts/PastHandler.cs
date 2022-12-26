using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PastHandler : MonoBehaviour {
    private TilemapRenderer rend;

    private void Start() {
        rend = GetComponent<TilemapRenderer>();
        Material material = rend.material;
        Color color = material.color;
        //Color alpha [0, 1] Transparent <--> Opaque
        color.a = 0f;
        material.color = color;
        this.gameObject.SetActive(false);
    }

    private IEnumerator FadeIn() {
        for (float opacity = 0.05f; opacity <= 1f; opacity += 0.05f) {
            Material material = rend.material;
            Color color = material.color;
            color.a = opacity;
            material.color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator FadeOut() {
        for (float opacity = 1f; opacity >= 0f; opacity -= 0.05f) {
            Material material = rend.material;
            Color color = material.color;
            color.a = opacity;
            material.color = color;
            yield return new WaitForSeconds(0.05f);
        }
        this.gameObject.SetActive(false);
    }

    public void StartFading(bool toPresent) {
        StartCoroutine(toPresent ? nameof(FadeOut) : nameof(FadeIn));
    }
}
