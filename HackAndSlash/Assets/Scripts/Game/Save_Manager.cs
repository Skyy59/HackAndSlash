using UnityEngine;
using UnityEngine.SceneManagement;

public static class Save_Manager
{
    private const string LEVEL_KEY = "ReachedLevelIndex";
    private const string MELEE_LEVEL_KEY = "WeaponMeleeLevel";
    private const string RANGED_LEVEL_KEY = "WeaponRangedLevel";
    private const string TIME_RECORD_PREFIX = "TimeRecord_Level_";

    public static void SaveProgress()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int savedIndex = PlayerPrefs.GetInt(LEVEL_KEY, 1);

        if (currentBuildIndex > savedIndex)
        {
            PlayerPrefs.SetInt(LEVEL_KEY, currentBuildIndex);
        }
        PlayerPrefs.Save();
    }

    public static int GetSavedLevel()
    {
        return PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }

    public static void SaveWeaponLevels(int meleeLevel, int rangedLevel)
    {
        PlayerPrefs.SetInt(MELEE_LEVEL_KEY, meleeLevel);
        PlayerPrefs.SetInt(RANGED_LEVEL_KEY, rangedLevel);
        PlayerPrefs.Save();
    }

    public static int GetMeleeLevel() => PlayerPrefs.GetInt(MELEE_LEVEL_KEY, 0);
    public static int GetRangedLevel() => PlayerPrefs.GetInt(MELEE_LEVEL_KEY, 0);

    public static void CheckAndSaveRecord(int sceneIndex, float timeSpent)
    {
        string key = TIME_RECORD_PREFIX + sceneIndex;

        float currentRecord = PlayerPrefs.GetFloat(key, 999999f);

        if(timeSpent < currentRecord)
        {
            PlayerPrefs.SetFloat(key, timeSpent);
            PlayerPrefs.Save();
        }
    }

    public static float GetRecord(int sceneIndex)
    {
        float record = PlayerPrefs.GetFloat(TIME_RECORD_PREFIX + sceneIndex, 9999999f);
        return record == 9999999f ? 0f : record;
    }

    public static void DeleteAllSaveData()
    {
        PlayerPrefs.DeleteKey(LEVEL_KEY);
        PlayerPrefs.DeleteKey(MELEE_LEVEL_KEY);
        PlayerPrefs.DeleteKey(RANGED_LEVEL_KEY);
        PlayerPrefs.Save();
    }
}
