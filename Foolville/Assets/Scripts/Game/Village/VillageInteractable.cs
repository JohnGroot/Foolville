using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageInteractable : MonoBehaviour
{

    protected bool _focusTarget = false;
    public bool FocusTarget { get { return _focusTarget; } }

    [SerializeField, ReadOnly] protected float _focusTime = 0.0f;
    public float FocusTime { get { return _focusTime; } set { _focusTime = value; } }

    public virtual void ReceivedGaze() { }
    public virtual void LostGaze() { }


}
