﻿using UnityEngine;
using UnityEngine.UI;

public class ClickEffect : MonoBehaviour
{
    [SerializeField] private float _effectTime = 1.0f;
    [SerializeField] private float _scaleSpeed = 1.0f;
    [SerializeField] private GameObject _successParticle;
    [SerializeField] private GameObject _failureParticle;

    private Transform _transform;
    private Image _image;
    private Color _color;
    private Color _clearColor = Color.clear;

    private void Awake()
    {
        _transform = transform;
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _effectTime -= Time.deltaTime;

        if (_effectTime <= 0)
        {
            Destroy(gameObject);
        }

        _transform.localScale += Vector3.one * _scaleSpeed * Time.deltaTime;
        _image.color = Color.Lerp(_clearColor, _color, _effectTime);
    }

    public void Initialize(bool success)
    {
        _color = success ? Color.white : Color.red;
        _image.color = _color;
        _transform.localScale = Vector3.one;

        _successParticle.SetActive(success);
        _failureParticle.SetActive(!success);
    }
}
