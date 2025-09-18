using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using TMPro;

public class Resurrection : MonoBehaviour
{
    public Animator uiSkullAnimator;
    public string animationName = "YourAnimation"; 
    private int currentFrame = 0;
    private int totalFrames = 11;
    public float autoReverseInterval = 0.2f;
    private bool isAutoReversing = false;
    private Coroutine reverseCoroutine;
    public GameObject resurrectionPanel;
    private bool isResurrecting = false;
    public TextMeshProUGUI resurrectionCountText;
    public int resurrectionCount = 0;
    public bool resurrectionInProgress = false;
    public void UndeadPower()
    {
        resurrectionCount += 2;
    }
    void Start()
    {
        if (GameManager.Instance.playerPowers.undead)
            UndeadPower();
        resurrectionCountText.text = resurrectionCount.ToString();
        uiSkullAnimator.speed = 0;
        SetFrame(currentFrame);
    }
    public void InitResurrection()
    {
        isAutoReversing = false;
        currentFrame = 0;
        SetFrame(currentFrame);
        StartAutoReverse();
        PlayerMovement.playerMovementInstance.isMovementLocked = true;
        PlayerMovement.playerMovementInstance.GetComponent<CharacterController>().enabled = false;
    }
    public void OpenResurrectionUI()
    {
        resurrectionInProgress = true;
        resurrectionPanel.SetActive(true);
    }
    public async Task CloseResurrectionUI()
    {
        await Task.Delay(500);
        resurrectionPanel.SetActive(false);

        PlayerMovement.playerMovementInstance.GetComponent<CharacterController>().enabled = true;
        
        resurrectionInProgress = false;
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
            _ = CloseResurrectionUI();
            _= ResurrectPlayer();
        }
    }
    public async Task ResurrectPlayer()
    {
        if (isResurrecting) return;
        isResurrecting = true;
        resurrectionCount -= 1;
        resurrectionCountText.text = resurrectionCount.ToString();
        await Task.Delay(200);
        PlayerMovement.playerMovementInstance.HealPlayer(GameManager.Instance.playerHealth);
        PlayerMovement.playerMovementInstance.isMovementLocked = false;
        isResurrecting = false;
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
        uiSkullAnimator.Play(animationName, 0, normalizedTime);
        uiSkullAnimator.Update(0f);
    }

    private AnimationClip GetAnimationClip(string name)
    {
        foreach (AnimationClip clip in uiSkullAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
                return clip;
        }
        return null;
    }
}
