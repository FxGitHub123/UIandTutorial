using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialClick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Button m_coveredButton;
    private Vector2 m_pressPosition;

    private RectTransform rectTransform;
    private GameObject m_currentButton;
    public void OnPointerClick(PointerEventData eventData)
    {
        //if (IsPointerOverUIObject() != false)
        //{
        //    return;
        //}
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_pressPosition = eventData.pressPosition;
        m_coveredButton = null;
        PassEvent(eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerEnterHandler);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerUpHandler);
        if (m_coveredButton != null)
        {
            eventData.pressPosition = m_pressPosition;
            m_coveredButton.OnPointerUp(eventData);

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_coveredButton != null)
        {
            Debug.Log(">>> FInd Button : " + m_coveredButton.name);
            eventData.pressPosition = m_pressPosition;
            m_coveredButton.OnPointerExit(eventData);
        }
    }

    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
    {
        if (m_currentButton == null) return;
        ExecuteEvents.Execute(m_currentButton, data, function);
    }

    public void SetSimulateButton(Button bt)
    {
        transform.position = bt.transform.position;
        RectTransform rectTransform = bt.GetComponent<RectTransform>();
        if (rectTransform == null) rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        m_currentButton = bt.gameObject;
    }

    public void Close()
    {
        m_currentButton = null;
        gameObject.SetActive(false);
    }
}
