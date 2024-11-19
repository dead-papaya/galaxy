using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DoTransparent : MonoBehaviour
{
    private Tween tween;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeTransparency(0.5f, 0.5f);   
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeTransparency(1f, 0.5f);  
        }
    }

    private async UniTask ChangeTransparency(float value, float time)
    {
        SpriteRenderer _spriteRenderer = GetComponent<SpriteRenderer>();
        
        Color currentColor = _spriteRenderer.color;
        // Используем DOTween для анимации изменения альфа-канала
        await _spriteRenderer.DOColor(new Color(currentColor.r, currentColor.g, currentColor.b, value), time)
            .SetEase(Ease.InOutQuad) // Задаём плавность
            .AsyncWaitForCompletion();
    }
}
