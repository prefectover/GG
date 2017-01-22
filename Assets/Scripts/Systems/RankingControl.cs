﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingControl : MonoBehaviour {

	[SerializeField]
	private RankName _rankName;
	[SerializeField]
	private RankScore _rankScore;
	[SerializeField]
	private Button _closeButton;


	public void Awake()
	{
		_closeButton.onClick.AddListener(OnClicked);
	}
	public void ClearNameAndScore()
	{
		_rankName.ClearNameAndScore ();
		_rankScore.ClearNameAndScore ();
	}

	public void SetNameAndScore( object serverData )
	{
		_rankName.SetNameAndScore (serverData);
		_rankScore.SetNameAndScore (serverData);
	}

	public void OnClicked()
	{
		UIManager.Instance.OnRankingClosed ();
	}
}
