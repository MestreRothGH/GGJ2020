﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LUT.Events.Primitives;

[RequireComponent(typeof(Animation), typeof(AudioSource))]
public class FeedbackScreen : MonoBehaviour
{
	[SerializeField]
	public EventBool onGameEnd;

	[Header("UI Setup")]
	public TextMeshProUGUI mainText;
	/// <summary>
	/// Used for score and sub texts
	/// </summary>
	public TextMeshProUGUI feedbackText;
	public Image background;

	public List<Graphic> interactables;

	public GameObject continueButton = null;
	public GameObject restartButton = null;

	[Header("Transition")]
	public AnimationClip inAnimation;
	public AnimationClip outAnimation;

	[Header("Win")]
	public Color winBackgroundColor;
	public Color winTextColor;
	public Color winInteractableColor;
	public string winMainText = "Great!";
	public AudioClip winAudioClip = null;
	[Header("Fail")]
	public Color loseBackgroundColor;
	public Color loseTextColor;
	public Color loseInteractableColor;
	public string loseMainText = "Ops...";
	public string loseSubText = "You've lost you breath";
	public AudioClip loseAudioClip = null;
	[Header("Game Over")]
	public Color gameOverBackgroundColor;
	public Color gameOverTextColor;
	public string gameOverMainText = "RIP Air";

	private new Animation animation;
	private GameFlowController cachedGameFlow;
	private AudioSource audioSource;

	protected void Start()
	{
		audioSource = GetComponent<AudioSource>();
		animation = GetComponent<Animation>();
		onGameEnd.Register(OnGameEnd);
		cachedGameFlow = GameFlowController.Instance;
		restartButton.SetActive(false);
		continueButton.SetActive(true);
	}

	private void OnDestroy()
	{
		onGameEnd.Unregister(OnGameEnd);
	}

	private void SetInteractablesColor(Color color)
	{
		interactables.ForEach((graphic) => graphic.color = color);
	}

	public void OnGameEnd(bool won)
	{
		if (won)
		{
			mainText.text = winMainText;
			feedbackText.color = mainText.color = winTextColor;
			background.color = winBackgroundColor;
			UpdateScore();
			SetInteractablesColor(winInteractableColor);
			audioSource.clip = winAudioClip;
			audioSource.Play();
		}
		else if (cachedGameFlow.IsAlive())
		{
			mainText.text = loseMainText;
			feedbackText.text = loseSubText;
			feedbackText.color = mainText.color = loseTextColor;
			background.color = loseBackgroundColor;
			SetInteractablesColor(loseInteractableColor);
			audioSource.clip = loseAudioClip;
			audioSource.Play();
		}
		else
		{
			mainText.text = gameOverMainText;
			feedbackText.color = mainText.color = gameOverTextColor;
			restartButton.SetActive(true);
			continueButton.SetActive(false);
			background.color = gameOverBackgroundColor;
			UpdateScore();
			SetInteractablesColor(loseInteractableColor);
			audioSource.clip = loseAudioClip;
			audioSource.Play();
		}

		animation.Play(inAnimation.name);
	}

	private void UpdateScore() => feedbackText.text = $"Score: {cachedGameFlow.currentScore}";

	public void FinishScene()
	{
		cachedGameFlow.ContinueGame();
		animation.Play(outAnimation.name);
	}
}
