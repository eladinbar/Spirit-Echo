using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PastHandler : MonoBehaviour {
    private TilemapRenderer rend;
    private CinemachineConfiner cinemachineConfiner;
    private AudioSource audioSource;

    [Header("Children")] 
    [SerializeField] GameObject backgroundTilemap;
    private PolygonCollider2D boundingBox;
    private TilemapRenderer backgroundRend;
    private Dictionary<Transform, Renderer> children;

    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private void Awake() {
        rend = GetComponent<TilemapRenderer>();
        boundingBox = backgroundTilemap.GetComponent<PolygonCollider2D>();
        backgroundRend = backgroundTilemap.GetComponent<TilemapRenderer>();
        AddAllChildren();
        cinemachineConfiner = virtualCamera.GetComponent<CinemachineConfiner>();
        audioSource = GetComponent<AudioSource>();
        
        Material material = rend.material;
        Color color = material.color;
        //Color alpha [0, 1] Transparent <--> Opaque
        color.a = 0f;
        material.color = color;
        this.gameObject.SetActive(false);
    }

    private void AddAllChildren() {
        children = new Dictionary<Transform, Renderer>();

        foreach (Transform child in this.gameObject.transform) {
            if (child.gameObject.CompareTag("Rendered") || child.gameObject.CompareTag("Background")) {
                if (!child.gameObject.CompareTag("Background"))
                    children.Add(child, child.GetComponent<Renderer>());
            }
        }
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
        StartCoroutine(FadeIn(this.rend));
        
        backgroundTilemap.SetActive(true);
        cinemachineConfiner.m_BoundingShape2D = this.boundingBox;
        StartCoroutine(FadeIn(backgroundRend));

        foreach (KeyValuePair<Transform, Renderer> child in children) {
            child.Key.gameObject.SetActive(true);
            StartCoroutine(FadeIn(child.Value));
        }

        audioSource.Play();
    }

    public void StartFading() {
        foreach (KeyValuePair<Transform, Renderer> child in children)
            StartCoroutine(FadeOut(child.Value));
        StartCoroutine(FadeOut(backgroundRend));
        StartCoroutine(FadeOut(this.rend));
        
        audioSource.Stop();
    }
}
