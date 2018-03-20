


using UnityEngine;
public class TimeContrl
{
    private static float timeSpeed = 1f;

    public static void setTimeSpeed(float t)
    {
        timeSpeed = t;
        Time.timeScale = t;
    }

    public static float getTimeSpeed()
    {
        return timeSpeed;
        ;
    }

}
