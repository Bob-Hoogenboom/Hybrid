using UnityEngine;

public class AiInput : ControlInterfaceClass
{
    public override PlayerVariables ControlVariables => Control();

    bool movingForward = false;
    private float waitTimer = 0.5f;

    public PlayerVariables Control()
    {
        Vector3 eulerRot = Vector3.zero;
        bool calibratePress = false;
        bool actionPress = false;

        if (waitTimer <= 0)
        {
            waitTimer = 0.5f;
            movingForward = !movingForward;
        }

        if (!movingForward)
        {
            eulerRot.x = 20;
        }
        else 
        {
            eulerRot.x = -20;
        }

        return new PlayerVariables(Quaternion.Euler(eulerRot), calibratePress, actionPress);
    }

    private void Update()
    {
        waitTimer -= Time.deltaTime;
    }
}
