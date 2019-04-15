using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEnemyBehaviour : MonoBehaviour
{
    public Material InitialMaterial;
    public Material KnockbackMaterial;
    public float KnockbackTime = 0.5f;
    public float knockbackForce = 10f;
    private bool knockback;

    public float InitialHealth = 100f;
    private float health;

    private MeshRenderer meshRenderer;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        InitialMaterial = meshRenderer.material;

        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        health = InitialHealth;
        knockback = false;
    }

    public void Attacked(float damage, Vector3 direction)
    {
        if(!knockback)
        {
            health -= damage;
            if(health <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(Knockback(KnockbackTime, direction));
            }
        }
        
    }

    IEnumerator Knockback(float time, Vector3 direction)
    {
        meshRenderer.material = KnockbackMaterial;
        knockback = true;

        rigidbody.AddForce(direction * knockbackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(time);

        meshRenderer.material = InitialMaterial;
        knockback = false;
    }
}
