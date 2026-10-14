using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class tcHandControler : MonoBehaviour
{

    public event Action ChoiceA;
    public event Action ChoiceB;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("A choosen wisely");
            ChoiceA?.Invoke();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("B choosen wisely");
            ChoiceB?.Invoke();
        }
    }
}
