using UnityEngine;

public class KeyboardInput : ControlInterfaceClass
{
    public override PlayerVariables ControlVariables => Control();

    public PlayerVariables Control()
    {
        Vector3 eulerRot = Vector3.zero;
        bool calibratePress = false;
        bool actionPress = false;

        if (Input.GetKey(KeyCode.A))
        {
            eulerRot.z = 45;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            eulerRot.z = -45;
        }
        if (Input.GetKey(KeyCode.W))
        {
            eulerRot.x = 70;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            eulerRot.x = -70;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            calibratePress = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            actionPress = true;
        }

        return new PlayerVariables(Quaternion.Euler(eulerRot), calibratePress, actionPress);
    }
}
