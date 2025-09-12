using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AnimationFramesClick : MonoBehaviour
{
    public Animator targetAnimator;
    public string animationName = "YourAnimation"; 
    private int currentFrame = 0;
    public int totalFrames = 10;
    public float autoReverseInterval = 0.2f;
    private bool isAutoReversing = false;
    private Coroutine reverseCoroutine;
    public GameObject targetPanel;
    private bool isEnabled = false;
    public Action collectRewardAction;
    public static AnimationFramesClick Instance { get; private set; }
    public bool isRewardFrames = false;
    private bool isOpened = false;
    public bool isWebFrames = false;

    void Start()
    {
        Instance = this;
        targetAnimator.speed = 0;
        SetFrame(currentFrame);
    }
    public void InitFrames(Action collectReward)
    {
        if (isOpened) return;
        isOpened = true;
        OpenTargetUI();
        if (collectReward != null)
            collectRewardAction = collectReward;
        isAutoReversing = false;
        currentFrame = 0;
        SetFrame(currentFrame);
        StartAutoReverse();
        PlayerMovement.playerMovementInstance.isMovementLocked = true;
    }
    public void OpenTargetUI()
    {
        targetPanel.SetActive(true);
    }
    public async Task CloseTargetUI()
    {
        await Task.Delay(500);
        targetPanel.SetActive(false);
        isOpened = false;
        if (isWebFrames)
        {
            
            // await PlayerMovement.playerMovementInstance.FreeTimeFromWeb();
            await PlayerMovement.playerMovementInstance.SlowPlayer();
        }

        PlayerMovement.playerMovementInstance.isMovementLocked = false;
    }
    public void NextFrame()
    {
        if(currentFrame >= totalFrames - 1) return;
        currentFrame++;
        if (currentFrame >= totalFrames)
            currentFrame = totalFrames - 1;

        SetFrame(currentFrame);
        if (currentFrame >= totalFrames - 1)
        {
            _ = CloseTargetUI();
            if (isRewardFrames)
                _ = GetReward();
        }
    }
    public async Task GetReward()
    {
        collectRewardAction?.Invoke();
        
        GameUIHandler.Instance.EnableOrDisableUI(GameUIHandler.Instance.obtainRewardPanel);
        PlayerMovement.playerMovementInstance.isMovementLocked = false;
        await Task.Delay(100);
    }
    public void StartAutoReverse()
    {
        if (!isAutoReversing)
        {
            isAutoReversing = true;
            reverseCoroutine = StartCoroutine(AutoReverse());
        }
    }

    public void StopAutoReverse()
    {
        if (isAutoReversing)
        {
            isAutoReversing = false;
            if (reverseCoroutine != null)
                StopCoroutine(reverseCoroutine);
        }
    }

    private IEnumerator AutoReverse()
    {
        if (currentFrame >= totalFrames-1) yield break;
        while (isAutoReversing)
        {
            if (currentFrame >= totalFrames-1) break;
            currentFrame--;
            if (currentFrame < 0)
                currentFrame = 0;

            SetFrame(currentFrame);

            yield return new WaitForSeconds(autoReverseInterval);
        }
    }

    private void SetFrame(int frame)
    {
        AnimationClip clip = GetAnimationClip(animationName);
        if (clip == null)
        {
            Debug.LogError("Animation clip not found: " + animationName);
            return;
        }

        float normalizedTime = (float)frame / (float)(totalFrames - 1);
        targetAnimator.Play(animationName, 0, normalizedTime);
        targetAnimator.Update(0f);
    }

    private AnimationClip GetAnimationClip(string name)
    {
        foreach (AnimationClip clip in targetAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
                return clip;
        }
        return null;
    }
}
