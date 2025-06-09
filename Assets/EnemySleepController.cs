using System.Collections.Generic;
using UnityEngine;

public class EnemySleepController : MonoBehaviour
{
    public static EnemySleepController Instance;

    private static List<enemy> allEnemies = new List<enemy>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(enemy e)
    {
        if (!allEnemies.Contains(e))
            allEnemies.Add(e);
    }

    public static void SetAllSleeping(bool sleep)
    {
        foreach (var e in allEnemies)
        {
            e.SetSleeping(sleep);
        }
    }
}
