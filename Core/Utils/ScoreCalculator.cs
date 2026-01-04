namespace PaceFriendsBackend.Core.Utils;

public static class ScoreCalculator
{

    private static float TIME_FACTOR = 0.75f;
    private static float CALORIES_FACTOR = 0.15f;
    private static float AREA_FACTOR = 0.005f;
    private static float STEPS_FACTOR = 0.005f;
    private static float WALKING_ACTIVITY_FACTOR = 1.0f;
    private static float JOGGING_ACTIVITY_FACTOR = 1.5f;
    private static float RUNNING_ACTIVITY_FACTOR = 2.0f;

    public static long CalculateScore(long totalSteps, long totalCalories, long timeSpent, double areaCovered, 
        Activity activity)
    {
        float activityFactor = WALKING_ACTIVITY_FACTOR;
        if(activity == Activity.Jogging) activityFactor = JOGGING_ACTIVITY_FACTOR;
        else if (activity == Activity.Running) activityFactor = RUNNING_ACTIVITY_FACTOR;

        double calculatedScore = ((timeSpent * TIME_FACTOR) + (totalCalories * CALORIES_FACTOR) +
                                   (areaCovered * AREA_FACTOR) + (totalSteps * STEPS_FACTOR)) * activityFactor;
        return (long)Math.Floor(calculatedScore);
    }
}