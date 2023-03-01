using UnityEngine;

namespace DefaultNamespace
{
    public class ViverMechanics : EnemyMechanics
    {
        public bool canDie;
        level3 level3Remote;
        [SerializeField] private GameObject tilemapGrid;


        protected override void Start()
        {
            canDie = false;
            currentPosition = this.transform.position;
            level3Remote = tilemapGrid.GetComponent<level3>();

        }
        public override void TakeDamage(Vector2 kick, int damage = 1)
        {
            if (canDie == true)
            {
                base.currentHealth -= damage;

                if (hurtSFX)
                    audioSource.PlayOneShot(hurtSFX);
                if (enemyAnimator.HasState(0, Hurt))
                    enemyAnimator.SetTrigger(Hurt);

                if (currentHealth <= 0)
                    Die();
                base.Knockback(kick);

            }
        }
        
        protected override void Die()
        {
            level3Remote.player.GetComponent<Rigidbody2D>().gravityScale = 1f;
            level3Remote.presentTilemap.GetComponent<AudioSource>().clip = level3Remote.part1AudioClip;
            level3Remote.presentTilemap.GetComponent<AudioSource>().Play();
            enemyAnimator.SetTrigger(Death);
        }

    }
}