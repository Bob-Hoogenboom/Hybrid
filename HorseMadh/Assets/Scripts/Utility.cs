public class Utility
{
    public static float Remap(float value, float minOld, float maxOld, float newOld, float newMax)
    {
        return (value - minOld) / (maxOld - minOld) * (newMax - newOld) + newOld;
    }
}
