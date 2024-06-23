using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Entity player;
    public AbilityTree abilityTree;
    public GameObject mainMenu, pauseMenu, gameoverScreen;
    public ExpOrb expOrb;
    public AnimationCurve globalEnemyHealthScalingOverTime;
    public static float enemyHealthMulti => 1;// Instance.globalEnemyHealthScalingOverTime.Evaluate(Instance.globalTick);
    public List<Entity> entities = new List<Entity>();
    public float spawnRadius;
    public List<Wave> waves = new List<Wave>();
    private int globalTick;

    [Serializable]
    public class Wave
    {
        public int startTick;
        public int endTick;
        public List<Spawn> spawns = new List<Spawn>();
        [HideInInspector]
        public bool active;

        [Serializable]
        public class Spawn
        {
            public Entity enemy;
            public int spawnEveryXFrames;
            [HideInInspector]
            public int count;
        }
    }

    private void Awake()
    {
        Time.timeScale = 0;
        Instance = this;
    }

    private void FixedUpdate()
    {
        globalTick++;
        EnemySpawns();
    }

    private void EnemySpawns()
    {
        foreach (Wave wave in waves)
        {
            if (!wave.active && globalTick >= wave.startTick && globalTick < wave.endTick) wave.active = true;
            if (wave.active)
            {
                if (globalTick > wave.endTick) wave.active = false;
                foreach (Wave.Spawn spawn in wave.spawns)
                {
                    spawn.count--;
                    if (spawn.count <= 0)
                    {
                        Vector3 dir = ((Vector2)(UnityEngine.Random.rotationUniform * Vector3.up)).normalized;
                        spawn.count = spawn.spawnEveryXFrames;
                        spawn.enemy.RequestObject().transform.position = player.transform.position + dir * spawnRadius;
                    }
                }
            }
        }
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        foreach(Entity entity in FindObjectsByType(typeof(Entity), FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            entity.currentTick = 0;
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        mainMenu.SetActive(false);
    }

    public void EndGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
