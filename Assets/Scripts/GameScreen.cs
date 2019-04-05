﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private EndGamePopup endGamePopupPrefab;
    [SerializeField] Image resourcefulnessImage;
    [SerializeField] Image courageImage;
    [SerializeField] Image indifferenceImage;
    [SerializeField] Image happyImage;


    private StorySettings currentStorySettings;
    private bool isAnswerButtonsEnabled;


    private void OnEnable()
    {
        CardsAnimation.OnAnswer += CardsAnimationOnOnAnswer;
    }

    private void CardsAnimationOnOnAnswer(bool isPositive)
    {
        if (isPositive)
        {
            AgreeButton_OnClick();
        }
        else
        {
            DisagreeButton_OnClick();
        }
    }

    private void OnDisable()
    {
        CardsAnimation.OnAnswer -= CardsAnimationOnOnAnswer;

        DOTween.Kill(this);
    }

    private void Start()
    {
        SetNextText(true);
    }

    void AgreeButton_OnClick()
    {
        if (isAnswerButtonsEnabled)
        {
            SetNextText(true);
        }
    }

    void DisagreeButton_OnClick()
    {
        if (isAnswerButtonsEnabled)
        {
            SetNextText(false);
        }
    }

    void SetNextText(bool isPositiveAnswer)
    {
        bool isLose;
        currentStorySettings = GameManager.Instance.StoryManager.GetNextStory(currentStorySettings, isPositiveAnswer, out isLose);
        
        if (currentStorySettings != null)
        {
            isAnswerButtonsEnabled = false;
            card.HideText(() =>
            {
                card.ShowText(currentStorySettings, () => { isAnswerButtonsEnabled = true; });
            });
        }
        else
        {
            EndGamePopup endGamePopup = Instantiate(endGamePopupPrefab, transform);
            endGamePopup.transform.localPosition = Vector3.zero;
            endGamePopup.Show("End game");
        }

        StoryManager storyManager = GameManager.Instance.StoryManager;

        float res = resourcefulnessImage.fillAmount;
        float deltaResource = resourcefulnessImage.fillAmount -
                              storyManager.resourcefulness / StoryManager.MAX_SCORE_COUNT;

        float cor = courageImage.fillAmount;
        float deltaCour = courageImage.fillAmount - storyManager.courage / StoryManager.MAX_SCORE_COUNT;

        float happy = happyImage.fillAmount;
        float deltaHappy = happyImage.fillAmount - storyManager.happy / StoryManager.MAX_SCORE_COUNT;
        
        float indiff = indifferenceImage.fillAmount;
        float deltaIn = indifferenceImage.fillAmount - storyManager.indifference / StoryManager.MAX_SCORE_COUNT;


        float factor = 0f;
        DOTween.To(() => factor, (value) =>
        {
            factor = value;
            resourcefulnessImage.fillAmount = res - factor * deltaResource;
            happyImage.fillAmount = happy - factor * deltaHappy;
            courageImage.fillAmount = cor - factor * deltaCour;
            indifferenceImage.fillAmount = indiff - factor * deltaIn;
        }, 1, 0.1f).SetId(this);
    }
}
