using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PresentHandler : MonoBehaviour {
    private TilemapRenderer rend;
    private CinemachineConfiner cinemachineConfiner;
    private AudioSource audioSource;
    private bool isFirstCreated = true;
    
    [Header("Children")] 
    [SerializeField] GameObject backgroundTilemap;
    private PolygonCollider2D boundingBox;
    private TilemapRenderer backgroundRend;
    [SerializeField] GameObject objectTilemap;
    private TilemapRenderer objectRend;

    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private void Awake() {
        rend = GetComponent<TilemapRenderer>();
        boundingBox = backgroundTilemap.GetComponent<PolygonCollider2D>();
        backgroundRend = backgroundTilemap.GetComponent<TilemapRenderer>();
        objectRend = objectTilemap.GetComponent<TilemapRenderer>();
        cinemachineConfiner = virtualCamera.GetComponent<CinemachineConfiner>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        isFirstCreated = false;
    }
    
    private IEnumerator FadeIn(Renderer render) {
        for (float opacity = 0.05f; opacity <= 1f; opacity += 0.05f) {
            Material material = render.material;
            Color color = material.color;
            color.a = opacity;
            material.color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator FadeOut(Renderer render) {
        for (float opacity = 1f; opacity >= 0f; opacity -= 0.05f) {
            Material material = render.material;
            Color color = material.color;
            color.a = opacity;
            material.color = color;
            yield return new WaitForSeconds(0.05f);
        }
        this.gameObject.SetActive(false);
    }

    public void OnEnable() {
        if (!isFirstCreated) {
            StartCoroutine(FadeIn(this.rend));
            
            backgroundTilemap.SetActive(true);
            cinemachineConfiner.m_BoundingShape2D = this.boundingBox;
            StartCoroutine(FadeIn(backgroundRend));
            
            objectTilemap.SetActive(true);
            StartCoroutine(FadeIn(objectRend));
            
            audioSource.Play();
        }
    }
    
    public void StartFading() {
        StartCoroutine(FadeOut(objectRend));
        StartCoroutine(FadeOut(backgroundRend));
        StartCoroutine(FadeOut(this.rend));
        
        audioSource.Stop();
    }
}