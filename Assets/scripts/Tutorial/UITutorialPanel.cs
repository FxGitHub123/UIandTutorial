using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITutorialPanel : UIPanelBase
{

    private float diameter; // 直径
    private Material material;
    private float current = 0f;
    private Vector3[] corners = new Vector3[4];
    private float yVelocity = 0f;

    [SerializeField]
    private TutorialClick simulateButton;

    [SerializeField]
    private Image mask;

    private Canvas canvas;
    public override void Init()
    {
        canvas = GameObject.Find("UIManager/Canvas").GetComponent<Canvas>();
        mask.enabled = false;
        mask.raycastTarget = false;
        simulateButton.gameObject.SetActive(false);
    }

    public void SetGuideTarget(TutorialObj target)
    {
        mask.enabled = true;
        mask.raycastTarget = true;
       
        if (canvas == null) canvas = GameObject.Find("UIManager/Canvas").GetComponent<Canvas>();
  
        target.image.rectTransform.GetWorldCorners(corners);

        simulateButton.gameObject.SetActive(true);
        simulateButton.SetSimulateButton(target.button);

        diameter = Vector2.Distance(WorldToCanvasPos(canvas, corners[0]), WorldToCanvasPos(canvas, corners[2])) / 2f;
        Debug.Log("半径：" + diameter);
        float x = corners[0].x + ((corners[3].x - corners[0].x) / 2f);
        float y = corners[0].y + ((corners[1].y - corners[0].y) / 2f);


        Vector3 center = new Vector3(x, y, 0f);
        Camera camera = Camera.main;
        center = camera.WorldToScreenPoint(center);

        Debug.Log("屏幕坐标中心点" + center);

        Vector2 position = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, center, camera, out position);

        Debug.Log("转换完之后的中心点：" + position);

        //if(mask == null) mask = GetComponent<Image>(); 
        //mask.rectTransform.sizeDelta = new Vector2(target.rectTransform.rect.width, target.rectTransform.rect.height);

        //mask.transform.localPosition = position;
        //if (Tap)
        //{
        //    tap.Play("Click");
        //    float offset = target.rectTransform.rect.width / 4;
        //    tap.transform.localPosition = new Vector3(position.x + offset, position.y);
        //    tap.gameObject.SetActive(true);
        //}
        //else
        //{
        //    tap.gameObject.SetActive(false);
        //}


        center = new Vector4(position.x, position.y, 0f, 0f);
        if (material == null)
        {
            material = new Material(GetComponent<Image>().material);
        }
        GetComponent<Image>().material = material;
        material.SetVector("_Center", center);



        (canvas.transform as RectTransform).GetWorldCorners(corners);
        for (int i = 0; i < corners.Length; i++)
        {
            current = Mathf.Max(Vector3.Distance(WorldToCanvasPos(canvas, corners[i]), center), current);
        }

        material.SetFloat("_Silder", current);

    }

    public void CloseGuide()
    {
        mask.enabled = false;
        mask.raycastTarget = false;
        simulateButton.Close();
    }

    private Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position = Vector2.zero;
        Camera camera = Camera.main;
        Vector3 screenPoint = camera.WorldToScreenPoint(world);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, camera, out position);
        return position;
    }

    void Update()
    {
        float value = Mathf.SmoothDamp(current, diameter, ref yVelocity, 0.2f);
        if (!Mathf.Approximately(value, current))
        {
            current = value;
            if (material == null) return;
            material.SetFloat("_Silder", current);
        }
    }
}
