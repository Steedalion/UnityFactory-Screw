using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScrewDown : MonoBehaviour, Screwable
{
    private Vector3 originalLocalPosition;
    private UnityEvent onScrewIn;

    [Tooltip("In testing the position is  reset after screwing.")]
    public bool testing = false;
    [Range(-0.5f,0.5f)]
    public float screwDistance = -0.1f;
    public LocalDirection localDirection;

    public void ScrewIn()
    {
        originalLocalPosition = transform.localPosition;
        transform.localPosition = originalLocalPosition + GetScrewInTranslation();
        // StartCoroutine(AnimateScrewIn());
        onScrewIn?.Invoke();
        if (testing)
        {
                 Invoke(nameof (ResetPosition), 2);
        }
        else
        {
            Destroy(this);
        }
    }

    private Vector3 GetScrewInTranslation()
    {
        Vector3 translation = default;
        switch (localDirection)
        {
            case LocalDirection.UP:
                translation = new Vector3(0, screwDistance, 0);
                break;
            case LocalDirection.FORWARD:
                translation = new Vector3(0, 0, screwDistance);
                break;
            case LocalDirection.RIGHT:
                translation = new Vector3(screwDistance, 0, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return translation;
    }

    private IEnumerator AnimateScrewIn()
    {
        float animationSpeed = 10f;
        Vector3 translation = GetScrewInTranslation();
        Vector3 goalPosition = originalLocalPosition + translation;
        while (Vector3.Distance(goalPosition, originalLocalPosition) > 0.001f)
        {
            transform.localPosition = originalLocalPosition + goalPosition*Time.deltaTime*animationSpeed;
        yield return null;
        }
        
    }

    private void ResetPosition()
    {
        transform.localPosition = originalLocalPosition;
    }
    
public enum LocalDirection
{
    FORWARD,
    UP,
    RIGHT
}
}
