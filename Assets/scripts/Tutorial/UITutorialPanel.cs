using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITutorialPanel : UIPanelBase
{

    private float m_diameter; // 直径
    private Material m_material;
    private float m_current = 0f;
    private Vector3[] m_corners = new Vector3[4];
    private float m_yVelocity = 0f;

    [SerializeField]
    private TutorialClick m_simulateButton;

    [SerializeField]
    private Image m_maskImage;

    [SerializeField]
    private Image m_clickMask;

    private Canvas m_canvas;
    public override void Init()
    {
        m_canvas = GameObject.Find("UIManager/Canvas").GetComponent<Canvas>();
        m_maskImage.enabled = false;
        m_maskImage.raycastTarget = false;
        m_simulateButton.gameObject.SetActive(false);
        CloseClickMask();
    }

    public void SetGuideTarget(TutorialObj target,bool IsMask)
    {
        m_maskImage.enabled = true;
        m_maskImage.raycastTarget = IsMask;
       
        if (m_canvas == null) m_canvas = GameObject.Find("UIManager/Canvas").GetComponent<Canvas>();
  
        target.targetImage.rectTransform.GetWorldCorners(m_corners);

        m_simulateButton.gameObject.SetActive(true);
        m_simulateButton.SetSimulateButton(target);

        m_diameter = Vector2.Distance(WorldToCanvasPos(m_canvas, m_corners[0]), WorldToCanvasPos(m_canvas, m_corners[2])) / 2f;
        Debug.Log("半径：" + m_diameter);
        float x = m_corners[0].x + ((m_corners[3].x - m_corners[0].x) / 2f);
        float y = m_corners[0].y + ((m_corners[1].y - m_corners[0].y) / 2f);


        Vector3 center = new Vector3(x, y, 0f);
        Camera camera = UIManager.Instance.UICamera;
        center = camera.WorldToScreenPoint(center);

        //Debug.Log("屏幕坐标中心点" + center);

        Vector2 position = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvas.transform as RectTransform, center, camera, out position);

        //Debug.Log("转换完之后的中心点：" + position);

        center = new Vector4(position.x, position.y, 0f, 0f);
        if (m_material == null)
        {
            m_material = new Material(GetComponent<Image>().material);
        }
        GetComponent<Image>().material = m_material;
        m_material.SetVector("_Center", center);



        (m_canvas.transform as RectTransform).GetWorldCorners(m_corners);
        for (int i = 0; i < m_corners.Length; i++)
        {
            m_current = Mathf.Max(Vector3.Distance(WorldToCanvasPos(m_canvas, m_corners[i]), center), m_current);
        }

        m_material.SetFloat("_Silder", m_current);

    }

    public void CloseGuide()
    {
        m_maskImage.enabled = false;
        m_maskImage.raycastTarget = false;
        m_simulateButton.Close();
        CloseClickMask();
    }

    public void OpenClickMask()
    {
        m_clickMask.raycastTarget = true;
    }

    public void CloseClickMask()
    {
        m_clickMask.raycastTarget = false;
    }

    private Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position = Vector2.zero;
        Camera camera = UIManager.Instance.UICamera;
        Vector3 screenPoint = camera.WorldToScreenPoint(world);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, camera, out position);
        return position;
    }

    void Update()
    {
        float value = Mathf.SmoothDamp(m_current, m_diameter, ref m_yVelocity, 0.2f);
        if (!Mathf.Approximately(value, m_current))
        {
            m_current = value;
            if (m_material == null) return;
            m_material.SetFloat("_Silder", m_current);
        }
    }
}
