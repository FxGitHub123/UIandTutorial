using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TitleScreen : UIPanelBase 
{
	[SerializeField]
    private GameObject m_UIGroupTitleScreen;

    [SerializeField]
    private GameObject m_UIGroupSceneLoader;

	[SerializeField]
	private Image		m_WhiteFade;

    [SerializeField]
    private RectTransform m_RectHangLineLogo;

	[SerializeField]
	private float		m_FadeInTimeWhenLoadingMenu = 0.5f;

	[SerializeField]
	private float		m_FadeOutTimeWhenLoadingMenu = 1.0f;

	[SerializeField]
	private float		m_FadeInTimeWhenLoadingGameLevel = 0.5f;

	[SerializeField]
	private float		m_FadeOutTimeWhenLoadingGameLevel = 0.5f;

	[SerializeField]
	private ParticleSystem	m_ParticlesFadingSnow;

    [SerializeField]
    private Image m_ImageLoading;

    [SerializeField]
    private Animation  m_Animation;

    [SerializeField]
    private List<GameObject> m_DefaultObjects;

    [SerializeField]
    private List<GameObject> m_CnObjects;

    // ------------------------------------------------------------------------------------------------------
    #region STANDARD

    protected override void AddListener()
    {
       
    }

    protected override void RemoveListener()
    {
       
    }

    public override void Init()
    {
        m_UIGroupTitleScreen.SetActive(true);
        m_UIGroupSceneLoader.SetActive(false);

        var rectTrans = m_RectHangLineLogo;
        var corners = new Vector3[4];
        rectTrans.GetWorldCorners(corners);
        var shape = m_ParticlesFadingSnow.shape;
        var box = shape.scale;
        box.x = corners[2].x - corners[0].x;
        box.y = corners[1].y - corners[0].y;
        shape.scale = box;

        //GlobalEvents.OnLanguageChange += OnLanguageChange;

        OnLanguageChange();
        StartSplash();
    }

	#endregion
	// ------------------------------------------------------------------------------------------------------
    #region CUSTOM

    public void StartSplash()
    {
        SetActiveState(true);
        StartCoroutine(CR_FadeInLogosOneByOneOnInitialShow());
    }


    IEnumerator CR_LoadingImage()
    {
        while (m_ImageLoading!=null)
        {
            yield return new WaitForSeconds(0.1f);
            m_ImageLoading.transform.Rotate(new Vector3(0, 0, -30.0f));
        }
    }

	private IEnumerator CR_FadeInLogosOneByOneOnInitialShow ()
    {
        //m_readyToLoad = false;
        m_UIGroupSceneLoader.SetActive(false);
        m_UIGroupTitleScreen.SetActive(true);
        m_Animation.Play();
        m_ParticlesFadingSnow.Play();

        StartCoroutine(CR_LoadingImage());//Loading Icon

        yield return new WaitForSecondsRealtime(5f); //TODO  加载需要的东西

        Debug.Log("开屏Splash加载完成");

        m_ImageLoading.gameObject.SetActive(false);

        StopAllCoroutines();

        m_ParticlesFadingSnow.Stop();

        StartCoroutine(ScreenOut(false,true));

        UIManager.LoadMainUI(false);

        //SoundManager.Instance.PlayEvent("event:/Music/EffectLowMystery", transform);
    }


    public void LoadingScreenIn(bool loadingGameLevel, bool fade = true)
    {
        StartCoroutine(ScreenIn(loadingGameLevel,fade));
    }

    public void LoadingScreenOut(bool loadingGameLevel, bool fade = true)
    {
        StartCoroutine(ScreenOut(loadingGameLevel, fade));
    }

    private IEnumerator ScreenIn(bool loadingGameLevel, bool fade = true)
    {
        m_UIGroupTitleScreen.SetActive(false);
        if (loadingGameLevel)
        {
            m_UIGroupSceneLoader.SetActive(false);
        }
        else
        {
            m_UIGroupSceneLoader.SetActive(true);
        }
            
        if (fade)
        {
            float fadeTime = loadingGameLevel ? m_FadeInTimeWhenLoadingGameLevel : m_FadeInTimeWhenLoadingMenu;

            SetActiveState(true);

            yield return new WaitForSeconds(fadeTime);
        }
        else
        {
            SetActiveState(true);
            yield return null;
        }

	}

    private IEnumerator ScreenOut(bool loadingGameLevel, bool fade = true)
    {
        if (fade)
        {
            float fadeTime = loadingGameLevel ? m_FadeOutTimeWhenLoadingGameLevel : m_FadeOutTimeWhenLoadingMenu;
            SetActiveState(false);
            yield return new WaitForSeconds(fadeTime);
        }
        else
        {
            SetActiveState(false);
        }
	}

    void OnLanguageChange(string languageCode=null)
    {
        if (languageCode == null)
        {
            //languageCode = LocalizationManager.CurrentLanguageCode;
        }
        bool isChina = languageCode == "zh-CN";

        foreach (var go in m_DefaultObjects)
        {
            go.SetActive(!isChina);
        }

        foreach (var go in m_CnObjects)
        {
            go.SetActive(isChina);
        }
    }

	#endregion
	// ------------------------------------------------------------------------------------------------------
	#region EVENTS


	#endregion
}
