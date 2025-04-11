using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(Button))]
public class UIButtonPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Общие параметры")]
    [SerializeField] private RectTransform target;
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private bool enableHoverEffect = true;
    [SerializeField] private bool runAwayFromCursor = false;

    [Header("Анимация")]
    [SerializeField] private float idlePulseScale = 1.02f;
    [SerializeField] private float hoverScale = 1.07f;
    [SerializeField] private float clickScale = 0.93f;
    [SerializeField] private float pulseDuration = 1f;
    [SerializeField] private float transitionDuration = 0.15f;

    [Header("RunAway")]
    [SerializeField] private float maxOffset = 100f;
    [SerializeField] private float minDistanceToCenter = 50f;
    [SerializeField] private float runAwayDelay = 0.1f;

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Sequence pulseSequence;
    private Coroutine runAwayCoroutine;

    void Awake()
    {
        if (target == null) target = GetComponent<RectTransform>();
        originalScale = target.localScale;
        originalPosition = target.anchoredPosition;

        if (enablePulse)
            StartPulse();
    }

    private void StartPulse()
    {
        if (!enablePulse) return;

        pulseSequence = DOTween.Sequence()
            .Append(target.DOScale(originalScale * idlePulseScale, pulseDuration).SetEase(Ease.InOutSine))
            .Append(target.DOScale(originalScale, pulseDuration).SetEase(Ease.InOutSine))
            .SetLoops(-1);
    }

    private void StopPulse()
    {
        if (pulseSequence != null && pulseSequence.IsActive())
            pulseSequence.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (runAwayFromCursor)
        {
            if (runAwayCoroutine != null)
                StopCoroutine(runAwayCoroutine);

            runAwayCoroutine = StartCoroutine(RunAwayAfterDelay(runAwayDelay));
            return;
        }

        if (!enableHoverEffect) return;

        StopPulse();
        target.DOScale(originalScale * hoverScale, transitionDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (runAwayFromCursor)
        {
            if (runAwayCoroutine != null)
                StopCoroutine(runAwayCoroutine);

            // Вернуть в центр
            target.DOAnchorPos(originalPosition, 0.4f).SetEase(Ease.InOutSine);
            return;
        }

        if (!enableHoverEffect) return;

        target.DOScale(originalScale, transitionDuration).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (enablePulse)
                    StartPulse();
            });
    }

    public void OnClick()
    {
        if (!enablePulse && !enableHoverEffect && !runAwayFromCursor) return;

        StopPulse();
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(target.DOScale(originalScale * clickScale, transitionDuration / 1.5f).SetEase(Ease.InOutSine));
        clickSequence.Append(target.DOScale(originalScale, transitionDuration / 1.5f).SetEase(Ease.InOutSine));
        clickSequence.OnComplete(() =>
        {
            if (enablePulse)
                StartPulse();
        });
    }

    void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClick);
        StopPulse();
    }

    private IEnumerator RunAwayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector2 newPos;
        int attempts = 0;

        do
        {
            newPos = originalPosition + new Vector2(
                Random.Range(-maxOffset, maxOffset),
                Random.Range(-maxOffset, maxOffset)
            );
            attempts++;
        }
        while (Vector2.Distance(newPos, originalPosition) < minDistanceToCenter && attempts < 10);

        target.DOAnchorPos(newPos, 0.2f).SetEase(Ease.OutSine);
    }
}
