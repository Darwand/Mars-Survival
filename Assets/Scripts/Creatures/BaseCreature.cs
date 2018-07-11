using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Base class for all creatures, contains base logic for movement, needs and life cycle

public enum Needs
{
    Food, Water, Oxygen
}

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class BaseCreature : MonoBehaviour
{
    protected Collider col;
    protected Rigidbody rb;

    Dictionary<Needs, int> needs;
    Dictionary<Needs, int> needDamage;

    int health;

    [SerializeField]
    int maxHealth = 200;

    [SerializeField]
    protected int needsMaxValue = 100;

    [SerializeField]
    protected float foodLossTime = 5f;

    [SerializeField]
    protected float waterLossTime = 5f;

    [SerializeField]
    protected float moveSpeed = 3f;

    void Start()
    {
        Init();
    }

    protected void Init()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        health = maxHealth;

        needs = CreateNeedDict(needsMaxValue);


        //start water and food loop
        StartCoroutine(FoodLoop());
        StartCoroutine(WaterLoop());
    }

    //stat functions

    Dictionary<Needs, int> CreateNeedDict(int value)
    {
        Dictionary<Needs, int> ret = new Dictionary<Needs, int>();

        ret.Add(Needs.Food, value);
        ret.Add(Needs.Water, value);
        ret.Add(Needs.Oxygen, value);

        return ret;
    }

    public void ModifyNeed( Needs need, int amount )
    {
        needs[need] += amount;
    }

    IEnumerator FoodLoop()
    {
        while(health > 0)
        {
            yield return new WaitForSeconds(foodLossTime);

            ModifyNeed(Needs.Food, -1);
        }
    }

    IEnumerator WaterLoop()
    {
        while (health > 0)
        {
            yield return new WaitForSeconds(waterLossTime);

            ModifyNeed(Needs.Water, -1);
        }
    }

    //movement functions

    public abstract void Move( Vector3 direction );

    public abstract void Rotate( Vector3 rot );

    protected virtual Vector3 GroundCheckStart()
    {
        return transform.position;
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        return Physics.Raycast(ray, 25f);
    }
}
