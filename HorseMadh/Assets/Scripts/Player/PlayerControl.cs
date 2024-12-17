using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeReference]
    private ControlInterfaceClass controlScheme;
    [SerializeField] private float stunTime = 2f;
    private float _currentStunTime = 0f;
    [SerializeField] private float immunityTimer = 3f;
    private float _currentImmunityTimer = 0f;

    public PlayerVariables playerVariables { get; private set; }
    public bool isStunned { get; private set; }
    public bool isImmune {  get; private set; }

    private void Update()
    {
        playerVariables = controlScheme.ControlVariables;
        if (isStunned)
        {
            isImmune = true;
            _currentStunTime -= Time.deltaTime;
            if (_currentStunTime < 0)
            {
                isStunned = false;
                _currentImmunityTimer = immunityTimer;
            }
        }
        if (_currentImmunityTimer > 0)
        {
            _currentImmunityTimer -= Time.deltaTime;
            if (_currentImmunityTimer <= 0) 
            {
                isImmune = false;
            }
        }
    }

    public void StunPlayer()
    {
        if (isImmune) return;
        isStunned = true;
        _currentStunTime = stunTime;
    }
}

public interface ControlInterface
{
    public abstract PlayerVariables ControlVariables { get; }
}

public abstract class ControlInterfaceClass : MonoBehaviour, ControlInterface
{
    public virtual PlayerVariables ControlVariables => throw new NotImplementedException();
}

public class PlayerVariables
{
    public PlayerVariables(Quaternion rotation, bool calibrateBTN, bool actionBTN)
    {
        this.rotation = rotation;
        this.calibrateThing = calibrateBTN;
        this.actionThing = actionBTN;
    }

    public Quaternion rotation { get; private set; }
    public bool calibrateThing { get; private set; }
    public bool actionThing { get; private set; }
}
