﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] float health = 100;
    [SerializeField] int points = 100;

    [Header("SFX")]
    [SerializeField] AudioClip shotSFX;
    [SerializeField] [Range(0f, 1f)] float shotVolume = 1f;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] [Range(0f, 1f)] float hitVolume = 1f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0f, 1f)] float deathVolume = 1f;

    [Header("VFX")]
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float timeDestroyVFX = 1.5f;
    [SerializeField] float timeToDestroyObject = 0.15f;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 10f;

    [Header("Shooting")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShot();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        if (hitSFX) { AudioSource.PlayClipAtPoint(hitSFX, Camera.main.transform.position, hitVolume); }
        ProcessHit(damageDealer);
    }

    private void CountDownAndShot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject spawnedProjectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity ) as GameObject;

        if (shotSFX) { AudioSource.PlayClipAtPoint(shotSFX, Camera.main.transform.position, shotVolume); }

        spawnedProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        HandleVFX();
        HandleDestroying();
        if (deathSFX) { AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathVolume); }
        FindObjectOfType<GameSession>().AddScore(points);
    }

    private void HandleDestroying()
    {
        GetComponent<EnemyPathing>().enabled = false;
        GetComponent<PolygonCollider2D>().enabled = false;
        Destroy(gameObject, timeToDestroyObject);
    }

    private void HandleVFX()
    {
        GameObject spawnedExplosionVFX = Instantiate(
                                explosionVFX,
                                new Vector3(transform.position.x, transform.position.y, -1f),
                                Quaternion.identity) as GameObject;
        Destroy(spawnedExplosionVFX, timeDestroyVFX);
    }
}
