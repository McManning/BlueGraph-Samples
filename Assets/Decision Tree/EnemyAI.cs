using System.Collections;
using UnityEngine;

namespace BlueGraphSamples
{
    /// <summary>
    /// A basic AI that uses a Decision Tree graph to determine
    /// when it can attack the player. 
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        [Tooltip("Graph to execute when making AI decisions")]
        public DecisionTree decisionTree;

        [Tooltip("Reference to the player GO in the scene")]
        public GameObject player;

        [Tooltip("How fast can we try to attack the player")]
        public float attackSpeed = 1f;

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(attackSpeed);
                TryAttackPlayer();
            }
        }

        void TryAttackPlayer()
        {
            if (player == null)
            {
                Debug.LogWarning("You need to specify a player", this);
                return;
            }

            if (decisionTree == null)
            {
                Debug.LogWarning("You need to add a Decision Tree asset", this);
                return;
            }

            // Maybe it attacks ALL agents?
            // And just turn this into AgentAI? 

            // Use the graph to decide whether we can attack
            if (decisionTree.CanAttackTarget(gameObject, player))
            {
                Attack();
            }
        }

        private void Attack()
        {
            // Do your VFX, sound, damage, etc.

            Debug.Log("Attacked the player!", this);
        }
    }
}
