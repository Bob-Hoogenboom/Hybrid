using UnityEngine;

public class KeyboardInput : ControlInterfaceClass
{
    public override PlayerVariables ControlVariables => Control();

    [SerializeField]
    private KeyCode forwardKey;
    [SerializeField]
    private KeyCode backKey;
    [SerializeField]
    private KeyCode rightKey;
    [SerializeField]
    private KeyCode leftKey;

    public PlayerVariables Control()
    {
        Vector3 eulerRot = Vector3.zero;
        bool calibratePress = false;
        bool actionPress = false;

        if (Input.GetKey(leftKey))
        {
            eulerRot.z = 20;
        }
        else if (Input.GetKey(rightKey))
        {
            eulerRot.z = -20;
        }
        if (Input.GetKey(forwardKey))
        {
            eulerRot.x = 40;
        }
        else if (Input.GetKey(backKey))
        {
            eulerRot.x = -40;
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
