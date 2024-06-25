using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Trevor.Tools.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Entity player;
    public AbilityTree abilityTree;
    public GameObject mainMenu, pauseMenu, gameoverScreen;
    public RectTransform healthBar, healthBarBack, healthBarMask, healthBarFrame, expBar;
    public TextMeshProUGUI score, highscore;
    public Ability firecrackerAbility;
    public Animator firecrackerCooldown;
    public int healthBarPixelsPerUnit, expBarSize;
    private int previousLevelUpExp, nextLevelUpExp;
    public ExpOrb expOrb;
    public AnimationCurve globalEnemyHealthScalingOverTime;
    public AudioDefinitionSO menuSong, gameSong;
    public Vector2 backgroundOffset;
    public Transform BG1, BG2, BG3, BG4;
    public static float enemyHealthMulti => Instance.globalEnemyHealthScalingOverTime.Evaluate(Instance.globalTick);
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

    private void Start()
    {
        CalculateNextExpRequirement();
        CalculateHighscore();
        menuSong.Play();
    }

    private void FixedUpdate()
    {
        globalTick++;
        EnemySpawns();
        UpdateHealthBar();
        FirecrackerCooldown();
    }

    private void Update()
    {
        Background();
        if (!mainMenu.activeSelf && !gameoverScreen.activeSelf && !abilityTree.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(Time.timeScale == 1 ? true : false);
        }
    }

    private void Background()
    {
        Vector2 currentCenter = new Vector2((player.transform.position.x + Mathf.Sign(player.transform.position.x) * backgroundOffset.x) - (player.transform.position.x + Mathf.Sign(player.transform.position.x) * backgroundOffset.x) % (backgroundOffset.x*2),
            (player.transform.position.y + Mathf.Sign(player.transform.position.y) * backgroundOffset.y) - (player.transform.position.y + Mathf.Sign(player.transform.position.y) * backgroundOffset.y) % (backgroundOffset.y*2));
        BG1.position = currentCenter + new Vector2(backgroundOffset.x, backgroundOffset.y);
        BG2.position = currentCenter + new Vector2(backgroundOffset.x, -backgroundOffset.y);
        BG3.position = currentCenter + new Vector2(-backgroundOffset.x, backgroundOffset.y);
        BG4.position = currentCenter + new Vector2(-backgroundOffset.x, -backgroundOffset.y);
    }

    private void UpdateHealthBar()
    {
        healthBar.sizeDelta = new Vector2(player.stats.currentHealth * healthBarPixelsPerUnit, healthBar.sizeDelta.y);
        healthBarBack.sizeDelta = new Vector2(player.stats.maxHealth * healthBarPixelsPerUnit, healthBarBack.sizeDelta.y);
        healthBarMask.sizeDelta = new Vector2(player.stats.maxHealth * healthBarPixelsPerUnit, healthBarMask.sizeDelta.y);
        healthBarFrame.sizeDelta = new Vector2(player.stats.maxHealth * healthBarPixelsPerUnit, healthBarFrame.sizeDelta.y);
        expBar.sizeDelta = new Vector2(1f * (player.stats.currentExp - previousLevelUpExp) / (nextLevelUpExp - previousLevelUpExp) * expBarSize, expBar.sizeDelta.y);
    }

    private void FirecrackerCooldown()
    {
        if(globalTick % (firecrackerAbility.cooldownTicks / 6f) < 1)
            firecrackerCooldown.SetTrigger("Progress");
    }

    public void OnFirecrackerUsed()
    {
        firecrackerCooldown.SetTrigger("Activated");
    }

    public void CalculateNextExpRequirement()
    {
        previousLevelUpExp = nextLevelUpExp;
        if(player.baseStats.stats.expLevelCurve.keys[player.baseStats.stats.expLevelCurve.keys.Length-1].time > previousLevelUpExp)
        {
            for (int i = player.stats.currentExp; i < player.baseStats.stats.expLevelCurve.keys[player.baseStats.stats.expLevelCurve.keys.Length-1].time; i++)
            {
                if((int)player.stats.expLevelCurve.Evaluate(i) > player.stats.level)
                {
                    nextLevelUpExp = i;
                    break;
                }
            }
        }
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
        if(PlayerPrefs.GetInt("highscore") < globalTick)
            PlayerPrefs.SetInt("highscore", globalTick);
        CalculateHighscore();
        globalTick = 0;
        gameoverScreen.SetActive(true);
    }

    private void CalculateHighscore()
    {
        highscore.text = new DateTime().AddSeconds(PlayerPrefs.GetInt("highscore") * 20 / 1000f).ToString("m:ss");
        score.text = new DateTime().AddSeconds(globalTick * 20 / 1000f).ToString("m:ss");
    }

    public void StartGame()
    {
        gameSong.Play();
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
