using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [Space]
    [Space]

    [SerializeField] private InputActionProperty _north;
    [SerializeField] private InputActionProperty _east;
    [SerializeField] private InputActionProperty _south;
    [SerializeField] private InputActionProperty _west;
    [SerializeField] private InputActionProperty _northEast;
    [SerializeField] private InputActionProperty _northWest;
    [SerializeField] private InputActionProperty _southEast;
    [SerializeField] private InputActionProperty _southWest;
    [SerializeField] private InputActionProperty _up;
    [SerializeField] private InputActionProperty _down;
    [SerializeField] private InputActionProperty _wait;

    private string horizontal = "horizontal";
    private string vertical = "vertical";

    private void OnEnable()
    {
        _north.action.performed += i => anim.SetFloat(vertical, 1);
        _south.action.performed += i => anim.SetFloat(vertical, -1);
        _east.action.performed += i => anim.SetFloat(horizontal, 1);
        _west.action.performed += i => anim.SetFloat(horizontal, -1);

        _northEast.action.performed += i =>
        {
            anim.SetFloat(vertical, 1);
            anim.SetFloat(horizontal, 1);
        };
        _northWest.action.performed += i =>
        {
            anim.SetFloat(vertical, 1);
            anim.SetFloat(horizontal, -1);
        };
        _southEast.action.performed += i =>
        {
            anim.SetFloat(vertical, -1);
            anim.SetFloat(horizontal, 1);
        };
        _southWest.action.performed += i =>
        {
            anim.SetFloat(vertical, -1);
            anim.SetFloat(horizontal, -1);
        };
    }

    private void OnDisable()
    {
        _north.action.performed -= i => anim.SetFloat(vertical, 1);
        _south.action.performed -= i => anim.SetFloat(vertical, -1);
        _east.action.performed -= i => anim.SetFloat(horizontal, 1);
        _west.action.performed -= i => anim.SetFloat(horizontal, -1);

        _northEast.action.performed -= i =>
        {
            anim.SetFloat(vertical, 1);
            anim.SetFloat(horizontal, 1);
        };
        _northWest.action.performed -= i =>
        {
            anim.SetFloat(vertical, 1);
            anim.SetFloat(horizontal, -1);
        };
        _southEast.action.performed -= i =>
        {
            anim.SetFloat(vertical, -1);
            anim.SetFloat(horizontal, 1);
        };
        _southWest.action.performed -= i =>
        {
            anim.SetFloat(vertical, -1);
            anim.SetFloat(horizontal, -1);
        };
    }
}
