using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PresentHandler : MonoBehaviour {
    private TilemapRenderer rend;
    private PolygonCollider2D boundingBox;
    private CinemachineConfiner cinemachineConfiner;
    private bool isFirstCreated = true;
    
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private void Awake() {
        rend = GetComponent<TilemapRenderer>();
        if (TryGetComponent(out PolygonCollider2D boundingBox)) {
            this.boundingBox = boundingBox;
            cinemachineConfiner = virtualCamera.GetComponent<CinemachineConfiner>();
        }
    }

    private void Start() {
        if (boundingBox)
            cinemachineConfiner.m_BoundingShape2D = boundingBox;
        isFirstCreated = false;
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

    public void OnEnable() {
        if (boundingBox)
            cinemachineConfiner.m_BoundingShape2D = boundingBox;
        if(!isFirstCreated)
            StartCoroutine(nameof(FadeIn));
    }
    
    public void StartFading() {
        StartCoroutine(nameof(FadeOut));
    }
}
