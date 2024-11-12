using UnityEngine;
using System;
using System.Collections;

public class AOEDealer : MonoBehaviour
{
    [SerializeField] GameObject aoePrefab;
    [SerializeField] float lifetime = 0.75f;

    // note: Assume this gameObject has a DamageDealer script that will do the actual damage
    // All this does is create another AOE (if asked, by a Health script) and destroy itself
    // Enemy ships' Health scripts call CreateAOE() and DestroyLater() if an AOEDealer (such
    // as an AOE-dealing projectile or another AOE object) collides with them.

    private CircleCollider2D circleCollider2D;
    private bool isProjectile = false;
    Coroutine destroyLater = null;

    void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        isProjectile = TryGetComponent<Projectile>(out var projectile);
    }

    void OnEnable()
    {
        if (!isProjectile)
        {
            DestroyLater();
        }
    }

    public void DestroyLater()
    {
        if (destroyLater == null)
        {
            destroyLater = StartCoroutine(DestroySelf());
        }
    }

    IEnumerator DestroySelf()
    {
        Debug.Log("AOEDealer: Begin coroutine to destroy " + gameObject.name + " in " + lifetime + " seconds");
        yield return new WaitForSeconds(lifetime);
        Debug.Log("AOEDealer: Time elapsed -- Attempt to destroy " + gameObject.name);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        Debug.Log("AOEDealer: Destroyed " + gameObject.name);
    }

    public AOEDealer CreateAOE(Vector3 position, string source = "(N/A)")
    {
        GameObject aoe = Instantiate(aoePrefab, position, Quaternion.identity);
        string timestamp = DateTime.Now.ToString("HHmmssfff"); // HH: hour, mm: minute, ss: second, fff: millisecond
        aoe.name = "AOE_" + timestamp;
        Debug.Log("AOE created: " + aoe.name + " -- by: " + source);
        return aoe.GetComponent<AOEDealer>();
    }

    void OnDrawGizmos()
    {
        if (circleCollider2D != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, circleCollider2D.radius);
        }
    }
}
