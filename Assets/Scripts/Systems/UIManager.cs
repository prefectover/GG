﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding.Serialization.JsonFx;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance
    {
        get { return _instance; }
    }
    private static UIManager _instance;

    [SerializeField]
    private BasicPattern _basicPattern;

    [Header("UI")]
    [SerializeField]
    private GameObject _startText;
    [SerializeField]
    private GameObject _lastTitle;
    [SerializeField]
    private DynamicTextGroup _lastDynamicScoreGroup;
    [SerializeField]
    private GameObject _highestTitle;
    [SerializeField]
    private DynamicTextGroup _highestDynamicScoreGroup;
    [SerializeField]
    private Button _fullScreenButton;
    [SerializeField]
	private Button _settingButton;
	[SerializeField]
	private Button _rankingButton;
    [SerializeField]
	private Button _introButton;
	[SerializeField]
	private Button _audioButton;
    [SerializeField]
    private Button _googlePlayButton;
    [SerializeField]
    private GameObject _circleParents;

    [Header("Panels")]
    [SerializeField]
    private GameObject _mainGamePanel;
    [SerializeField]
	private GameObject _settingPanel;
	[SerializeField]
	private GameObject _rankingPanel;
	[SerializeField]
	private GameObject _namePanel;
	[SerializeField]
	private GameObject _introPanel;

    [SerializeField]
    private GameOverEffect _gameOverEffect;
    private void Awake()
    {
        _instance = this;
        _fullScreenButton.onClick.AddListener(OnFullscreenButtonClicked);
		_settingButton.onClick.AddListener(OnSettingClicked);
		_rankingButton.onClick.AddListener(OnRankingClicked);
		_audioButton.onClick.AddListener(OnAudioClicked);
		_googlePlayButton.onClick.AddListener(OnGooglePlayClicked);
		_introButton.onClick.AddListener(OnIntroClicked);

        #if !(UNITY_ANDROID || UNITY_IPHONE) || UNITY_EDITOR
        _googlePlayButton.onClick.AddListener(OnGooglePlayClicked);
        _googlePlayButton.gameObject.SetActive(true);
        #endif

        _circleParents.AddComponent<RectTransformScaleShowHide>();
        _lastTitle.AddComponent<RectTransformScaleShowHide>();
        _highestTitle.AddComponent<RectTransformScaleShowHide>();
        _fullScreenButton.gameObject.AddComponent<RectTransformScaleShowHide>();
        _settingButton.gameObject.AddComponent<RectTransformScaleShowHide>();
		_audioButton.gameObject.AddComponent<RectTransformScaleShowHide>();
        _rankingButton.gameObject.AddComponent<RectTransformScaleShowHide>();
        _startText.gameObject.AddComponent<RectTransformScaleShowHide>();
        _introButton.gameObject.AddComponent<RectTransformScaleShowHide>();
        _googlePlayButton.gameObject.AddComponent<RectTransformScaleShowHide>();
        _gameOverEffect.onEffectCompleteCallback = OnGameOverEffectComplete;
    }

    private void Start()
    {
        _lastDynamicScoreGroup.SetText("0");
        _highestDynamicScoreGroup.SetText(AchieveManager.Instance.GetHightestScore().ToString());

        _lastTitle.gameObject.GetComponent<RectTransformScaleShowHide>().Show();
        _highestTitle.gameObject.GetComponent<AbsRectTransformShowHideAction>().Show();
        _fullScreenButton.gameObject.GetComponent<RectTransformScaleShowHide>().Show();
        _startText.gameObject.GetComponent<AbsRectTransformShowHideAction>().Show();
        _rankingButton.gameObject.GetComponent<AbsRectTransformShowHideAction>().Show();
        _introButton.gameObject.GetComponent<AbsRectTransformShowHideAction>().Show();

        #if !(UNITY_ANDROID || UNITY_IPHONE) || UNITY_EDITOR
        _googlePlayButton.gameObject.GetComponent<RectTransformScaleShowHide>().Show();
        #endif

        UpdateSettingAndAudioButtons(true);
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (_settingPanel.activeInHierarchy)
        {
            if (Input.anyKeyDown)
            {   
                foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keycode))
                    {
                        GameManager.Instance.TriggerKey = keycode;
                        OnSettingClosed();
                    }
                }
            }
        }
        else if(!_rankingPanel.activeInHierarchy)
        {
            if (Input.GetKeyDown(GameManager.Instance.TriggerKey) && _startText.activeSelf)
			{
                StartCoroutine(StartGame());
            }
        }
        #endif
    }

    private void OnFullscreenButtonClicked()
    {
        if (_rankingPanel.activeSelf)
        {
            _fullScreenButton.transform.SetSiblingIndex(5);
            _startText.GetComponent<RectTransformScaleShowHide>().Show();
            _circleParents.GetComponent<RectTransformScaleShowHide>().Show();
            _rankingPanel.SetActive(false);
            return;
        }

		if (_introPanel.activeSelf)
		{
            _fullScreenButton.transform.SetSiblingIndex(5);
            _startText.GetComponent<RectTransformScaleShowHide>().Show();
            _circleParents.GetComponent<RectTransformScaleShowHide>().Show();
			_introPanel.SetActive(false);
			return;
		}
        #if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        StartCoroutine(StartGame());
        #endif
    }

    private void OnSettingClicked()
    {
        _settingPanel.SetActive(true);
        _mainGamePanel.SetActive(false);
    }

    private void OnSettingClosed()
    {
        _settingPanel.SetActive(false);
        _mainGamePanel.SetActive(true);
    }

    private void UpdateSettingAndAudioButtons(bool show)
	{
        #if !UNITY_EDITOR
        _settingButton.gameObject.SetActive(false);
        _audioButton.gameObject.SetActive(!show);

        if(show)
        {
            _audioButton.gameObject.GetComponent<RectTransformScaleShowHide>().Show();
        }
        else
        {
            _audioButton.gameObject.GetComponent<RectTransformScaleShowHide>().Hide();
        }
        #else
        _audioButton.gameObject.SetActive(false);
        _settingButton.gameObject.SetActive(!show);

        if(show)
        {
            _settingButton.gameObject.GetComponent<RectTransformScaleShowHide>().Show();
        }
        else
        {
            _settingButton.gameObject.GetComponent<RectTransformScaleShowHide>().Hide();
        }
        #endif
	}

	private void OnRankingClicked()
	{
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }

        _fullScreenButton.transform.SetAsLastSibling();
        _startText.GetComponent<RectTransformScaleShowHide>().Hide();
        _circleParents.GetComponent<RectTransformScaleShowHide>().Hide();
		_rankingPanel.SetActive (true);
		_rankingPanel.SendMessage ("ClearNameAndScore");
		HttpRequestManager.Instance.Download ();
	}

	public void UpdateRankingPanel( string RankingStr )
	{
		Debug.logger.Log (RankingStr);
		Dictionary<string,object>[] RankInfo = JsonReader.Deserialize<Dictionary<string,object>[]> (RankingStr);
		int LowestScore = 0;
		if (RankInfo.Length >= 3) 
		{
			LowestScore = int.MaxValue;
			foreach (Dictionary<string,object> Info in RankInfo) {
				int Score = (int)Info ["score"];
				LowestScore = Math.Min (LowestScore, Score);
			}
		}
		AchieveManager.Instance.LowestRankScore = LowestScore;

		if (!_rankingPanel.activeInHierarchy) 
		{
			return;
		}
		_rankingPanel.SendMessage ( "SetNameAndScore", RankInfo );
	}

    private void OnRankingClosed()
	{
		_rankingPanel.SetActive (false);
	}

    private void OnAudioClicked()
	{
		_audioButton.SendMessage ("OnAudioClicked");
	}

    private void OnGooglePlayClicked()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ggj2017twh.dontdie");
    }

	public void OnIntroClicked()
	{
        _startText.GetComponent<RectTransformScaleShowHide>().Hide();
        _circleParents.GetComponent<RectTransformScaleShowHide>().Hide();
		_introPanel.SetActive (true);
        _fullScreenButton.transform.SetAsLastSibling();
	}
	public void OnIntroClose()
	{
		_introPanel.SetActive (false);
	}

	public void ShowNickNamePanel()
	{
		_namePanel.SetActive (true);
	}

	public void HideNickNamePanel()
	{
		_namePanel.SetActive (false);
	}

    private IEnumerator StartGame()
    {
        _gameOverEffect.ResetGameOverEffect();
        GameManager.Instance.GameReset();

        _fullScreenButton.gameObject.GetComponent<RectTransformScaleShowHide>().Hide();
        _startText.gameObject.GetComponent<AbsRectTransformShowHideAction>().Hide();
        _rankingButton.gameObject.GetComponent<AbsRectTransformShowHideAction>().Hide();
        _introButton.gameObject.GetComponent<AbsRectTransformShowHideAction>().Hide();

        #if !(UNITY_ANDROID || UNITY_IPHONE) || UNITY_EDITOR
        _googlePlayButton.gameObject.GetComponent<RectTransformScaleShowHide>().Hide();
        #endif

        UpdateSettingAndAudioButtons(false);

        yield return new WaitForEndOfFrame();

        AudioManager.Instance.OnStartButtonClicked();
        _basicPattern.OnStartGame();
    }

    public void OnLossGame()
    {
        StartCoroutine(LossGame());
    }

    private IEnumerator LossGame()
    {
        _gameOverEffect.StartGameOverEffect();

        yield return new WaitForSeconds(1.75f);

        _fullScreenButton.gameObject.GetComponent<RectTransformScaleShowHide>().Show();
        _startText.gameObject.GetComponent<AbsRectTransformShowHideAction>().Show();
        _rankingButton.gameObject.GetComponent<AbsRectTransformShowHideAction>().Show();
        _introButton.gameObject.GetComponent<AbsRectTransformShowHideAction>().Show();

        #if !(UNITY_ANDROID || UNITY_IPHONE) || UNITY_EDITOR
        _googlePlayButton.gameObject.GetComponent<RectTransformScaleShowHide>().Show();
        #endif

        UpdateSettingAndAudioButtons(true);

        _highestDynamicScoreGroup.SetText(AchieveManager.Instance.GetHightestScore().ToString());
    }

    public void OnScoreChanged(int score)
    {
        _lastDynamicScoreGroup.SetText(score.ToString());
    }

    private void OnGameOverEffectComplete()
    {
        _gameOverEffect.ResetGameOverEffect();
    }
}
