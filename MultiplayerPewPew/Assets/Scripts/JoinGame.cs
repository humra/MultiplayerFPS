using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System;

public class JoinGame : MonoBehaviour {

    private NetworkManager networkManager;
    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;
    [SerializeField]
    private GameObject roomListItemPrefab;
    [SerializeField]
    private Transform roomListParent;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";
        if(!success || matchList == null)
        {
            status.text = "Couldn't get room list";
            return;
        }

        foreach(MatchInfoSnapshot match in matchList)
        {
            GameObject roomListItemGO = Instantiate(roomListItemPrefab);
            roomListItemGO.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = roomListItemGO.GetComponent<RoomListItem>();
            if(_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom);
            }

            //Have a component sit on the gameobject that will take care
            //of setting up the name and amount of users as well as setting
            //up a callback function that will join the game
            roomList.Add(roomListItemGO);
        }

        if(roomList.Count == 0)
        {
            status.text = "No rooms at the moment :(";
        }
    }

    private void ClearRoomList()
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot match)
    {
        networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        StartCoroutine(WaitForJoin());
    }

    private IEnumerator WaitForJoin()
    {
        ClearRoomList();

        int countdown = 20;
        while(countdown > 0)
        {
            status.text = "Joining... (" + countdown + ")";
            yield return new WaitForSeconds(1);
            countdown--;
        }

        //If we reach this we failed to connect
        status.text = "Failed to connect";
        yield return new WaitForSeconds(3);

        MatchInfo matchInfo = networkManager.matchInfo;
        if(matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }

        RefreshRoomList();
    }
}
