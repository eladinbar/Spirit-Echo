using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PresentHandler : MonoBehaviour {
    private TilemapRenderer rend;

    private void Start() {
        rend = GetComponent<TilemapRenderer>();
        Material material = rend.material;
        Color color = material.color;
        //Color alpha [0, 1] Transparent <--> Opaque
        color.a = 1f;
        material.color = color;
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
        StartCoroutine(toPresent ? nameof(FadeIn) : nameof(FadeOut));
    }
}
