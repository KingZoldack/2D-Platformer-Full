using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public static AnimatorManager Instance { get; private set; }

    Animator _anim;

    AnimationStates _animation;

    enum AnimationStates
    {
        idle,
        run,
        jump

    }



    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(7);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Idel()
    {
        var _animation = AnimationStates.idle.ToString();
        _anim.Play(_animation);
    }

}
