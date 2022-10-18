/****************************************
*Copyright(C) 2021 by AfterEarth
*All rights reserved.
*ProductName:  AEGPrototype1
*Author:       Denev AEG
*Version:      1.0
*UnityVersion: 2020.3.9f1
*CreateTime:   2021/08/13 14:57:28
*File:         UITweener.cs
*Description:   
****************************************/
/****************************************
*Copyright(C) 2021 by AfterEarth
*All rights reserved.
*ProductName:  Mechanics_test
*Author:       kevin
*Version:      0.1
*UnityVersion: 2020.3.9f1
*CreateTime:   2021/06/23 13:54:43
*File:         UiTweener.cs
*Description:   
****************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UIAnimationTypes
{
    Move,
    Scale,
    ScaleX,
    ScaleY,
    Fade,

    SlideOutLeft,
    SlideOutRight,
    SlideOutTop,
    SlideOutBottom
}



public class UITweener : MonoBehaviour
{
    public GameObject ObjectToAnimate;

    private bool enabling, disabling;

    [Header("In Animation Settings")]   
    public UIAnimationTypes InAnimationType;
    public LeanTweenType EaseInType;
    public AnimationCurve InAnimCurve;
    public bool useInCurve;
    public float InDuration;
    public float InDelay;


    [Header("Out Animation Settings")]
    public bool UseOutAnimation; //if yes, the exit of the panel use this settings, if not, just swap the positions and use the same as de in 
    public UIAnimationTypes OutAnimationType;
    public LeanTweenType EaseOutType;    
    public AnimationCurve OutAnimCurve;
    public bool useOutCurve;
    public float OutDuration;
    public float OutDelay;


    [Header("General Settings")]
    public UIAnimationTypes currentAnimType;
    public LeanTweenType currentEaseType;
    public AnimationCurve currentAnimCurve;
    public float currentDuration;
    public float currentDelay;

    [Space(20)]
    public bool UseCurve;
    public bool IgnoreTimeScale;
    public bool Loop;
    public bool Pingpong;
    public bool ShowEnable;
    public bool WorkOndisable;
    public bool RefreshCanvasOnComplete;

    [Header("Positions Values Settings")]
    public bool StartPositionOffset;
    public Vector3 From;
    public Vector3 To;

    private LTDescr _tweenObject; //Reference to the object created to handle the animation from Lean tween   

    [Header("Callback")]
    public UnityEvent OnCompleteCallback;
    public UnityEvent OnCompleteDisableCallback;


    public void Awake()
    {
        currentAnimType = InAnimationType;
    }

    public void OnEnable()
    {
        enabling = true;
        disabling = false;

        UseCurve = useInCurve;
        currentAnimType = InAnimationType;
        currentEaseType = EaseInType;
        currentAnimCurve = InAnimCurve;

        currentDelay = InDelay;
        currentDuration = InDuration;

        if (ShowEnable)
        {
            Show();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    public void Show()
    {
        HandleTween();
    }

    public void HandleTween() 
    {
        if (ObjectToAnimate == null)
            ObjectToAnimate = gameObject;

        switch (currentAnimType)
        {
            case UIAnimationTypes.Move:
                MoveAbsolute();
                break;
            case UIAnimationTypes.Scale:
                Scale();
                break;
            case UIAnimationTypes.ScaleX:
                Scale();
                break;
            case UIAnimationTypes.ScaleY:
                Scale();
                break;
            case UIAnimationTypes.Fade:
                Fade();
                break;
            default:
                break;
        }


        //Set General settings to Tween Object
        _tweenObject.setDelay(currentDelay);

        if (UseCurve)
            _tweenObject.setEase(currentAnimCurve);
        else
            _tweenObject.setEase(currentEaseType);


        if (IgnoreTimeScale)        
            _tweenObject.setIgnoreTimeScale(IgnoreTimeScale);


        if (Pingpong)
            _tweenObject.setLoopPingPong();

    }




    //Animation Functions

    public void Fade()
    {
        if (gameObject.GetComponent<CanvasGroup>() == null)        
            gameObject.AddComponent<CanvasGroup>();

        if (StartPositionOffset)        
            ObjectToAnimate.GetComponent<CanvasGroup>().alpha = From.x;

        _tweenObject = LeanTween.alphaCanvas(ObjectToAnimate.GetComponent<CanvasGroup>(), To.x, currentDuration).setOnComplete(OnComplete);
    }

    public void MoveAbsolute()
    {
        ObjectToAnimate.GetComponent<RectTransform>().anchoredPosition = From;
        _tweenObject = LeanTween.move(ObjectToAnimate.GetComponent<RectTransform>(), To, currentDuration).setOnComplete(OnComplete);
    }

    public void Scale()
    {
        if (StartPositionOffset)
        {
            ObjectToAnimate.GetComponent<RectTransform>().localScale = From;
        }
        _tweenObject = LeanTween.scale(ObjectToAnimate, To, currentDuration).setOnComplete(OnComplete);
    }



    //Auxiliar Settings

    void SwapDirection()
    {
        var temp = From;
        From = To;
        To = temp;
    }

    public void Disable()
    {
        enabling = false;
        disabling = true;

        if (!UseOutAnimation)
        {
            SwapDirection();
            HandleTween();
            _tweenObject.setOnComplete(() =>
            {
                SwapDirection();
                gameObject.SetActive(false);
            });
        }
        else
        {
            UseCurve = useOutCurve;
            currentAnimType = OutAnimationType;
            currentEaseType = EaseOutType;
            currentAnimCurve = OutAnimCurve;
            currentDelay = OutDelay;
            currentDuration = OutDuration;

            SwapDirection();
            HandleTween();
            _tweenObject.setOnComplete(() =>
            {
                SwapDirection();
                gameObject.SetActive(false);
            });
            //Using out animation on disable       
        }
        
    }

    public void OnComplete()
    {
        if (enabling)
        {
            if (OnCompleteCallback != null)
            {
                OnCompleteCallback.Invoke();
            }
        }

        else if (disabling)
        {
            if (OnCompleteDisableCallback != null)
            {
                OnCompleteDisableCallback.Invoke();
            }
        }

      
    }

    public Vector3 CalculateDestiny(Vector3 origin, UIAnimationTypes direction, GameObject parent)
    {
        Vector3 destiny = Vector3.zero;
        return destiny;
    }

    //Getters Setters
    public LTDescr GetTweenObjct()
    {
        return _tweenObject;
    }
}
