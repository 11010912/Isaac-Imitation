﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RoomType { Gold, Boss }
public class RoomTemplates : MonoBehaviour
{
      public GameObject[] allRooms;

      public GameObject[] bottomRooms;
      public GameObject[] topRooms;
      public GameObject[] leftRooms;
      public GameObject[] rightRooms;

      public List<GameObject> rooms;
      public GameObject closedRoom;

      public int maxRoomCount = 10;
      public int minRoomCount = 5;

      public float waitTime = 2f;
      public bool createdRooms;
      public bool refreshedRooms = false;

      [Header("Doors")]
      public GameObject bossDoor;
      public GameObject goldDoor;
      public GameObject exitDoor;

      [Header("Rooms")]
      public GameObject GoldRoomSet;
      public GameObject BossRoomSet;

      [Header("Props")]
      public GameObject prop;

      private void Update()
      {
            // Test code
            if (Input.GetKeyDown(KeyCode.Escape)) {
                  SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (waitTime <= 0 && createdRooms == false) {
                  createdRooms = true;
                  RefreshRooms();
            }
            else if (rooms.Count >= minRoomCount && waitTime > 0) {
                  waitTime -= Time.deltaTime;
            }
      }

      private void RefreshRooms()
      {
            for (int i = 1; i < rooms.Count; i++) {
                  if (rooms[i].GetComponentInChildren<Modifyer>()) {
                        rooms[i].GetComponentInChildren<Modifyer>().RefreshRoom();
                  }
            }

            foreach (Door door in rooms[^1].GetComponentsInChildren<Door>()) {
                  if (door.doorDirection == 0) continue;
                  else {
                        StartCoroutine(door.ChangeToSelectedDoorCoroutine(bossDoor));
                        door.transform.parent.parent.GetComponentInChildren<Modifyer>()
                              .SetSpecialRoom(RoomType.Boss);
                        break;
                  }
            }

            for (int i = 1; i < (rooms.Count < 5 ? rooms.Count : 5); i++) {
                  if (rooms[i].GetComponentsInChildren<Door>().Length == 2) {
                        foreach (Door door in rooms[i].GetComponentsInChildren<Door>()) {
                              if (door.doorDirection != 0) {
                                    StartCoroutine(door.ChangeToSelectedDoorCoroutine(goldDoor));
                                    door.transform.parent.parent.GetComponentInChildren<Modifyer>()
                                          .SetSpecialRoom(RoomType.Gold);
                              }
                        }
                        break;
                  }
            }

            refreshedRooms = true;
      }


      private void OnDisable()
      {
            refreshedRooms = false;
      }
}
