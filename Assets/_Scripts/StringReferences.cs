using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringReferences : MonoBehaviour
{
    public static StringReferences Instance { get; private set; }
    private void Awake()
    {
        if(Instance == null)
           Instance = this;
    }

    //Animator Params
    string didDoubleJumpBool = "didDoubleJump";


    //Props
    public string DidDoubleJumpBool { get { return didDoubleJumpBool; } }
}
