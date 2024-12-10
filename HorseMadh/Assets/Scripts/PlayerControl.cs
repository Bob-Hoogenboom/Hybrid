using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeReference]
    private ControlInterfaceClass controlScheme;

    public PlayerVariables playerVariables { get; private set; }

    private void Update()
    {
        playerVariables = controlScheme.ControlVariables;
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
    public PlayerVariables(Quaternion rotation, bool button)
    {
        this.rotation = rotation;
        this.buttonThing = button;
    }

    public Quaternion rotation;
    public bool buttonThing;
}
