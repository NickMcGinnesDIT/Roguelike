﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour
{

	public bool IsDebug = false;
	
	public float Radius = 5.0f;

	public LayerMask Mask;

	private GameObject _targetEnemy;

	private float _attackTime = 0.0f;
	
	
	// Update is called once per frame
	void Update () {
		if (gameObject.CompareTag("Player"))
		{
			PlayerMovement();
		}
		else
		{
			EnemyMovement();
		}
	}

	void PlayerMovement()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				StopHere();
				
				//this lookat function kept breaking the player, need to 
				//gameObject.transform.LookAt(hit.point);
			}
			if (Input.GetMouseButton(0))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					//Attack();
					
					PlayerAttack();
				}
				else
				{
					if (GetComponent<NavMeshAgent>().isStopped)
						GetComponent<NavMeshAgent>().isStopped = false;
					MoveTo(hit.point);
				}
			}

			if (Input.GetMouseButtonDown(1))
			{
				//SpecialAttack();
			}
		}
	}

	private void PlayerAttack()
	{
		//set up our ray from screen to scene
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		//if we hit
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			if (hit.collider.gameObject.CompareTag("Enemies"))
			{
				if (Time.time > _attackTime + GetComponent<CharacterInfo>().AttackSpeed)
				{
					float myDamage = GetComponent<CharacterInfo>().Damage;
		
					hit.collider.gameObject.GetComponent<CharacterInfo>().Hit(myDamage);
					_attackTime = Time.time;
				}
			}
			
		}
	}

	void EnemyMovement()
	{
		Collider[] things = Physics.OverlapSphere(transform.position, Radius, Mask);

		if (things.Length > 0)
		{
			if (GetComponent<NavMeshAgent>().isStopped)
				GetComponent<NavMeshAgent>().isStopped = false;
			MoveTo(things[0].gameObject.transform.position);

			float distance = Vector3.Distance(gameObject.transform.position, things[0].gameObject.transform.position);
			
			if (!(distance < 1.2f)) return;
			StopHere();
			transform.LookAt(things[0].gameObject.transform);
			EnemyAttack(things[0].gameObject);
		}
		else
		{
			StopHere();
		}
	}

	private void EnemyAttack(GameObject myTarget)
	{
		if (Time.time > _attackTime + GetComponent<CharacterInfo>().AttackSpeed)
		{
			float myDamage = GetComponent<CharacterInfo>().Damage;
		
			myTarget.GetComponent<CharacterInfo>().Hit(myDamage);
			_attackTime = Time.time;
		}
		
		
	}

	private void MoveTo(Vector3 myTarget)
	{
		GetComponent<NavMeshAgent>().SetDestination(myTarget);
	}

	private void StopHere()
	{
		GetComponent<NavMeshAgent>().isStopped = true;
	}
}
