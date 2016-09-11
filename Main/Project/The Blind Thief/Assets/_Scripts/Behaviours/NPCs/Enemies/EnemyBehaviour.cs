using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyAnimationController enemyAnimation;
    [SerializeField] private AudioSource audioSource;

    public EnemyAttackType enemyAttackType;

    private EnemyState enemyState;
    public EnemyState currentEnemyState
    {
        get { return enemyState; }
    }             

    [Header("Projectiles")]
    [SerializeField] private GameObject enemyProjectile;
    [SerializeField] private int numberOfProjectilesToSpawn;
    [SerializeField] private Transform projectileSpawnPoint;
    private List<GameObject> projectiles;
    [SerializeField] private float projectileMovementSpeed;
    [SerializeField] private float projectileMaxDistance; // Determines how far a projectile can travel

    [Header("Sound Wave")]
    [SerializeField]
    private FieldOfView fov;

    public delegate void attacking();
    public attacking Attacking;  
    
      

    //Enums
    public enum EnemyState
    {
        stalking,
        aggro,
    }

    public enum EnemyAttackType
    {
        Projectile,
        AreaOfEffect,
    }

    void Start()
    {
        SubscribeToEvents();
        //SpawnInDetectionWaves();

        switch(enemyAttackType)
        {            
            case (EnemyAttackType.Projectile):
                SpawnInProjectiles();
                break;
        }
    }    

    void SubscribeToEvents()
    {
        fov.FinishedFOV += StopAttacking;
    }

    void SpawnInProjectiles()
    {
        projectiles = new List<GameObject>();

        for (int i = 0; i < numberOfProjectilesToSpawn; i++)
        {
            GameObject projectile = (GameObject)Instantiate(enemyProjectile);

            //Set Projectile Values
            EnemyProjectileBehaviour epbScript = projectile.GetComponent<EnemyProjectileBehaviour>();
            epbScript.ProjectileSpeed = projectileMovementSpeed;
            epbScript.MaxProjectileDistance = projectileMaxDistance;
            epbScript.OriginalParent = projectileSpawnPoint;

            projectile.transform.parent = projectileSpawnPoint;
            projectile.transform.localPosition = Vector3.zero;

            projectiles.Add(projectile);
            projectile.SetActive(false);
        }
    }
   
    /// <summary>
    /// Trigger by Send Message from Sound Wave Behaviour
    /// </summary>
	void HitByMainCharacter(Transform playerPosition)
    {
        if(enemyState == EnemyState.stalking)
        {
            switch (enemyAttackType)
            {
                case (EnemyAttackType.AreaOfEffect):
                    StartCoroutine(ActivateSoundWave());                 
                    break;
                case (EnemyAttackType.Projectile):
                    SendOutProjectile(playerPosition);
                    break;
            }
        }
    }

    IEnumerator ActivateSoundWave()
    {
        Attacking();
        enemyAnimation.TurnOnAnimation("isAttacking");
        yield return new WaitForSeconds(1.5f);
        audioSource.Play();
        fov.enabled = true;
        CameraEffectsController.Instance.SetEnemySoundWaveToCameraShake();
    }

    void StopAttacking()
    {
        CameraEffectsController.Instance.StopCameraShake(); ;
        audioSource.Stop();
    }

    void SendOutProjectile(Transform playerPos)
    {
        for (int i = 0; i < projectiles.Count; i++)
        {
            if(!projectiles[i].activeInHierarchy)
            {
                EnemyProjectileBehaviour projectileScript = projectiles[i].GetComponent<EnemyProjectileBehaviour>();
                projectileScript.PlayerPosition = playerPos.position;
                projectiles[i].SetActive(true);
                break;
            }
        }
    }   
   
}
