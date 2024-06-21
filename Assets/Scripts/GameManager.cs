using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Entity player;
    public ExpOrb expOrb;
    public List<Entity> entities = new List<Entity>();

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        EnemySpawns();
    }

    private void EnemySpawns()
    {

    }
}
