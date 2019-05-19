using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DefinitiveScript;

public class PlayerLockTargetController : MonoBehaviour
{
    public float angleDiference = 5f;
    public float speedToMoveTargetMark = 50f;

    public float distanceToDeactivateTargeting;

    public GameObject targetMark;
    private Image targetMarkRenderer;
    private Transform targetMarkTransform;

    public CinemachineVirtualCamera thirdPersonLockedTargetCamera;

    private bool sableMode;
    private bool lockedTarget;

    private List<EnemyBehaviour> enemyList;

    private EnemyBehaviour currentTarget;

    void Start()
    {
        enemyList = new List<EnemyBehaviour>();
        
        targetMarkRenderer = targetMark.GetComponent<Image>();
        targetMarkTransform = targetMark.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(sableMode && !lockedTarget) EnemiesInScreen();
        EnableTargetMarkRenderer();
        if(sableMode && currentTarget)
        {
            MoveTargetMarkToTarget();
        }

        if(lockedTarget)
        {
            Vector3 vectorToTarget = currentTarget.transform.position - transform.position;
            vectorToTarget.y = 0f;
            float distanceFromTarget = vectorToTarget.magnitude;
            if(distanceFromTarget > distanceToDeactivateTargeting)
            {
                DeactivateTargeting();
            }
        }

        CheckNullsAndDeaths();

        /*Vector3 forwardFromCamera = Camera.main.transform.forward;
        forwardFromCamera.y = 0f;

        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + forwardFromCamera * 100000f, Color.red);*/
    }

    public void AddEnemy(EnemyBehaviour enemy)
    {
        enemyList.Add(enemy);
    }

    public void RemoveEnemy(EnemyBehaviour enemy)
    {
        enemyList.Remove(enemy);
    }

    private void EnemiesInScreen()
    {
        float minAngle = 360f, minDistance = Mathf.Infinity;

        EnemyBehaviour auxEnemy = null;

        for(int i = 0; i < enemyList.Count; i++)
        {
            Bounds enemyBounds = enemyList[i].GetComponentInChildren<Renderer>().bounds;
            Vector3 min = enemyBounds.center - enemyBounds.extents * 0.8f;
            Vector3 max = enemyBounds.center + enemyBounds.extents * 0.8f;

            //Debug.DrawLine(min, max, Color.green);

            Vector3 minInScreen = Camera.main.WorldToScreenPoint(min);
            Vector3 maxInScreen = Camera.main.WorldToScreenPoint(max);

            bool condition = minInScreen.x > 0f && minInScreen.y > 0f && minInScreen.x < Camera.main.pixelWidth && minInScreen.y < Camera.main.pixelHeight;
            condition = condition && maxInScreen.x > 0f && maxInScreen.y > 0f && maxInScreen.x < Camera.main.pixelWidth && maxInScreen.y < Camera.main.pixelHeight;
            if(condition)
            {
                if(CalculateAngleAndDistance(enemyList[i], ref minAngle, ref minDistance))
                {
                    auxEnemy = enemyList[i];
                }
            }
        }
        currentTarget = auxEnemy;
    }

    private void CheckNullsAndDeaths()
    {
        for(int i = 0; i < enemyList.Count; i++)
        {
            EnemyBehaviour enemy = enemyList[i];
            if((enemy != null && !enemy.GetAlive()) || enemy == null)
            {
                if(lockedTarget && (enemy != null && enemy == currentTarget))
                {
                    currentTarget = null;
                    DeactivateTargeting();
                }
                enemyList.RemoveAt(i);
            }
        }

    }

    private bool CalculateAngleAndDistance(EnemyBehaviour enemy, ref float minAngle, ref float minDistance)
    {
        Vector3 vectorBetweenCameraEnemy = enemy.transform.position - Camera.main.transform.position;
        vectorBetweenCameraEnemy.y = 0f;

        Vector3 forwardFromCamera = Camera.main.transform.forward;
        forwardFromCamera.y = 0f;

        float angle = Vector3.Angle(forwardFromCamera, vectorBetweenCameraEnemy);

        //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + vectorBetweenCameraEnemy * 100000f, Color.blue);

        if(angle < minAngle + angleDiference)
        {
            minAngle = angle;

            Vector3 vectorBetweenPlayerEnemy = transform.position - enemy.transform.position;
            vectorBetweenPlayerEnemy.y = 0f;
            minDistance = vectorBetweenPlayerEnemy.magnitude;

            return true;
        }
        else if(angle >= minAngle && angle <= minAngle + angleDiference)
        {
            Vector3 vectorBetweenPlayerEnemy = transform.position - enemy.transform.position;
            vectorBetweenPlayerEnemy.y = 0f;
            float distance = vectorBetweenPlayerEnemy.magnitude;

            if(minDistance < distance)
            {
                minDistance = distance;
                return true;
            }
        }
        return false;
    }   

    private void EnableTargetMarkRenderer()
    {
        targetMarkRenderer.enabled = sableMode && currentTarget != null;
        if(!targetMarkRenderer.enabled)
        {
            targetMarkTransform.position = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        }
    }
    
    private void MoveTargetMarkToTarget()
    {
        Vector3 destination = Camera.main.WorldToScreenPoint(currentTarget.characterCenter.position);

        float step = speedToMoveTargetMark * Time.deltaTime;
        targetMarkTransform.position = Vector3.MoveTowards(targetMarkTransform.position, destination, step);
    }

    public void LockUnlockTarget()
    {
        if(!lockedTarget && currentTarget)
        {
            lockedTarget = true;
            thirdPersonLockedTargetCamera.m_LookAt = currentTarget.transform;
        }
        else
        {
            lockedTarget = false;
            thirdPersonLockedTargetCamera.m_LookAt = null;
        }
    }

    private void DeactivateTargeting()
    {
        GetComponent<PlayerBehaviour>().LockUnlockTarget();
    }

    public Transform GetCurrentTargetTransform()
    {
        if(currentTarget == null) return null;
        return currentTarget.transform;
    }

    public void SetSableMode(bool value)
    {
        sableMode = value;
    }
}
