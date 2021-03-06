﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class LevelGen : MonoBehaviour
{
	
	public Vector3 WorldSize = new Vector3(4,0,4);
	
	public Room[,] Rooms;
	
	private List<Vector3> _takenPositions = new List<Vector3>();
	
	private int _gridSizeX, _gridSizeZ, _numberofRooms = 35;
	
	public GameObject RoomWhiteObj;
	
	private void Start()
	{

		if (_numberofRooms >= (WorldSize.x * 2) * (WorldSize.z * 2))
		{
			_numberofRooms = Mathf.RoundToInt((WorldSize.x * 2) * (WorldSize.z * 2));
		}

		_gridSizeX = Mathf.RoundToInt(WorldSize.x);
		_gridSizeZ = Mathf.RoundToInt(WorldSize.z);

		CreateRooms();
		SetRoomDoors();
		DrawMap();

	}
	
	void CreateRooms()
	{
		//setup
		Rooms = new Room[_gridSizeX * 2, _gridSizeZ * 2];
		Rooms[_gridSizeX,_gridSizeZ] = new Room(Vector3.zero, 1);
		_takenPositions.Insert(0,Vector3.zero);
		Vector3 checkPosition;
		
		//magic numbers
		float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;
		
		//add rooms
		for (int i = 0; i < _numberofRooms; i++)
		{
			//Debug.Log("inside forloop");
			float randomPerc = i / ((float) _numberofRooms - 1);
			randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
			
			//grab new position
			checkPosition = NewPostion();

			float myRandomNumber = Random.value;
			Debug.Log("number of neighbors " + NumberOfNeighbors(checkPosition, _takenPositions));
			//Debug.Log(myRandomNumber);
			//Debug.Log(randomCompare);
			
			//test new position   
			if (NumberOfNeighbors(checkPosition, _takenPositions) >= 1 && myRandomNumber > randomCompare)
			{
				int iterations = 0;
				do
				{
					//Debug.Log("inside do loop");

					checkPosition = SelectiveNewPostion();
					iterations++;
				} while (NumberOfNeighbors(checkPosition, _takenPositions) > 1 && iterations < 70);

				if (iterations >= 50)
				{
					print("error; could not create with fewer neighbors than: " + NumberOfNeighbors(checkPosition,_takenPositions));
				}
				
				//finalize position
				//Debug.Log(checkPosition);
				Rooms[(int) checkPosition.x + _gridSizeX,(int) checkPosition.z+_gridSizeZ] = new Room(checkPosition, 0);
				_takenPositions.Insert(0,checkPosition);
			}
			
		}
	}
	
	Vector3 NewPostion()
	{
		int x = 0, y = 0, z = 0;
		Vector3 checkingPos;

		do
		{
			int index = Mathf.RoundToInt(Random.value * (_takenPositions.Count - 1));
			x = (int) _takenPositions[index].x;
			z = (int) _takenPositions[index].z;
			bool upDown = (Random.value < 0.5f);
			bool positive = (Random.value < 0.5f);
			if (upDown)
			{
				if (positive)
				{
					z += 1;
				}
				else
				{
					z -= 1;
				}
			}
			else
			{
				if (positive)
				{
					x += 1;
				}
				else
				{
					x -= 1;
				}
			}

			checkingPos = new Vector3(x, y, z);
		} while (_takenPositions.Contains(checkingPos) || x >= _gridSizeX || x < -_gridSizeX || z >= _gridSizeZ || z < -_gridSizeZ);
		
		return checkingPos;
	}
	
	Vector3 SelectiveNewPostion()
	{
		int index = 0, inc = 0;
		int x = 0, y = 0, z = 0;
		Vector3 checkingPos = Vector3.zero;

		do
		{
			inc = 0;
			do
			{
				index = Mathf.RoundToInt(Random.value * (_takenPositions.Count - 1));
				inc++;
			} while (NumberOfNeighbors(_takenPositions[index],_takenPositions) > 1 && inc < 100);
			
			x = (int) _takenPositions[index].x;
			z = (int) _takenPositions[index].z;
			
			bool upDown = (Random.value < 0.5f);
			bool positive = (Random.value < 0.5f);
			if (upDown)
			{
				if (positive)
				{
					z += 1;
				}
				else
				{
					z -= 1;
				}
			}
			else
			{
				if (positive)
				{
					x += 1;
				}
				else
				{
					x -= 1;
				}
			}

			checkingPos = new Vector3(x, y, z);
		} while (_takenPositions.Contains(checkingPos) || x >= _gridSizeX || x < -_gridSizeX || z >= _gridSizeZ || z < -_gridSizeZ);

		if (inc >= 100)
		{
			print("Error: could not find a position with only 1 neighbor");
		}
		return checkingPos;
	}
	
	int NumberOfNeighbors(Vector3 checkingPos, List<Vector3> usedPositions)
	{
		int ret = 0;

		if (usedPositions.Contains(checkingPos + Vector3.right))
		{
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector3.left))
		{
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector3.forward))
		{
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector3.back))
		{
			ret++;
		}
		
		return ret;
	}
	
	void SetRoomDoors()
	{
		for (int x = 0; x < (_gridSizeX * 2); x++)
		{
			for (int z = 0; z < (_gridSizeZ * 2); z++)
			{
				if (Rooms[x, z] == null)
					continue;
				Vector3 gridPosition = new Vector3(x,0,z);
				
				if (z - 1 < 0)
				{
					Rooms[x, z].DoorS = false;
				}
				else
				{
					Rooms[x, z].DoorS = (Rooms[x,z-1] != null);
				}
				//****
				if (z + 1 >= _gridSizeZ * 2)
				{
					Rooms[x, z].DoorN = false;
				}
				else
				{
					Rooms[x, z].DoorN = (Rooms[x,z+1] != null);
				}
				//****
				if (x - 1 < 0)
				{
					Rooms[x, z].DoorW = false;
				}
				else
				{
					Rooms[x, z].DoorW = (Rooms[x-1,z] != null);
				}
				//****
				if (x + 1 >= _gridSizeX * 2)
				{
					Rooms[x, z].DoorE = false;
				}
				else
				{
					Rooms[x, z].DoorE = (Rooms[x+1,z] != null);
				}
				
			}
		}
	}
	
	void DrawMap()
	{
		foreach (Room room in Rooms)
		{
			if (room == null)
				continue;

			Vector3 drawPosition = room.GridPosition;
			drawPosition.x *= 10;
			drawPosition.z *= 10;
			
			
			 
			RoomObjectControl mapper = GameObject.Instantiate(RoomWhiteObj, drawPosition, Quaternion.identity).GetComponent<RoomObjectControl>();
			mapper.type = room.Type;
			mapper.north = room.DoorN;
			mapper.south = room.DoorS;
			mapper.east = room.DoorE;
			mapper.west = room.DoorW;
			
			
		}
	}
}
