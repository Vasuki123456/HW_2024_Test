using UnityEngine;

public class ConfigurationManager : MonoBehaviour
{
    public TextAsset JSONConfig;

    private static Config config;

    private void Awake()
    {
        if (config == null)
        {
            config = JsonUtility.FromJson<Config>(JSONConfig.text);
        }
    }

    public static Config GetConfig()
    {
        return config;
    }
}


[System.Serializable]
public class Config
{
    public PlayerData player_data;
    public PulpitData pulpit_data;
}

[System.Serializable]
public class PlayerData
{
    public float speed;
}

[System.Serializable]
public class PulpitData
{
    public float min_pulpit_destroy_time;
    public float max_pulpit_destroy_time;
    public float pulpit_spawn_time;
    public float pulpit_size;
}