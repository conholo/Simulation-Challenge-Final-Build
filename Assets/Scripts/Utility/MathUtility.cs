public static class MathUtility
{
    public static float Remap(float v, float a0, float a1, float b0, float b1)
    {
        return b0 + (v - a0) * (b1 - b0) / (a1 - a0);
    }
}
