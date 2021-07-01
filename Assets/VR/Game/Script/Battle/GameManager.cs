
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Basic.Math;
using UnityEngine;

namespace Battle
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] 
        private Transform[] spawnPoints;

        [SerializeField] 
        private GameObject spawnEnemyPrefab;

        private List<Enemy> enemies;
        
        private static GameManager _instance;
        public static GameManager Instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }
        
        private void Awake()
        {
            Instance = this;
            enemies = new List<Enemy>();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            
            // 播放开场;
            Messenger.Broadcast("KingAction");
            
            yield return new WaitForSeconds(2);

            // 循环生成敌人;
            while (true)
            {
                bool allDead = true;
                foreach (var enemy in enemies)
                {
                    if (!enemy.dead)
                    {
                        allDead = false;
                        break;
                    }
                }
                if (allDead)
                {
                    enemies.Clear();
                    SpawnEnemy();
                }
                yield return null;
            }
        }

        public void SpawnEnemy()
        {
            print("spawn!");
            var index = Mathf.RoundToInt(Random.Range(0, spawnPoints.Length - 1));
            var point = spawnPoints[index];
            var p = point.position;
            var go = Instantiate(spawnEnemyPrefab, p, point.rotation);
            go.SetActive(true);
            var enemy = go.GetComponent<Enemy>();
            enemies.Add(enemy);
        }
    }
}