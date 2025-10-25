using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class KeyboardManager : MonoBehaviour
{
    public TMP_InputField[] Fields;

    public TMP_InputField FoucsField;

    private Animator _animator;

    public bool isClicked = false;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetInputFieldFoucs(int ID){
        FoucsField = Fields[ID];
        _animator.SetBool("isOpen", true);
    }

    public void EndFoucs()
    {
        FoucsField = null;
        _animator.SetBool("isOpen", false);
    }

    public void AddKey(string s)
    {

        isClicked = true;
        
        if(FoucsField == null){
            return;
        }

        if(s == "<" && FoucsField.text.Length > 0){
            FoucsField.text = FoucsField.text.Substring(0, FoucsField.text.Length - 1);
            return;
        }

        if(s == "Space"){
            FoucsField.text += " ";
            return;
        }
        FoucsField.text += s;
    }
    
    IEnumerator DoCheck(bool True = true) 
    {
        yield return new WaitForSeconds(0.2f);
        if(isClicked == false && True){
       
        }
        isClicked = false;
    }    
}

    

