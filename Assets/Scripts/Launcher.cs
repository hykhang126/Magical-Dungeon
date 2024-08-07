using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEditor;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject MenuButtons;

    public GameObject CreateRoomScreen;
    public TMP_InputField roomNameInput;

    public GameObject RoomScreen;
    public TMP_Text roomNameText, playerNameLabel;
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public GameObject ErrorScreen;
    public TMP_Text errorText;

    public GameObject RoomBrowserScreen;
    public RoomButton theRoomButton;
    private List<RoomButton> allRoomButtons = new List<RoomButton>();

    // Start is called before the first frame update
    void Start()
    {
        CloseMenus();
        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to Network...";

        PhotonNetwork.ConnectUsingSettings();
    }
    //Function to make sure all menus are disabled
    void CloseMenus()
    {
        loadingScreen.SetActive(false);
        MenuButtons.SetActive(false);
        CreateRoomScreen.SetActive(false);
        RoomScreen.SetActive(false);
        ErrorScreen.SetActive(false);
        RoomBrowserScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        loadingText.text = "Joining Lobby";
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();

        MenuButtons.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0f, 1000f).ToString();
    }

    public void OpenRoomCreate()
    {
        CloseMenus();

        CreateRoomScreen.SetActive(true);
    }

    public void QuitGame()
    {
        CloseMenus();
        Application.Quit();
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;
            
            PhotonNetwork.CreateRoom(roomNameInput.text, options);

            CloseMenus();
            loadingText.text = "Creating Room...";
            loadingScreen.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        RoomScreen.SetActive(true);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        listAllPlayers();
    }

    private void listAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        for(int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel,playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);
            allPlayerNames.Add(newPlayerLabel);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);
        
        allPlayerNames.Add(newPlayerLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        listAllPlayers();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed to Create Room: " + message + "...";
        CloseMenus();
        ErrorScreen.SetActive(true);
    }

    public void CloseErrorScreen()
    {
        CloseMenus();
        MenuButtons.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room...";
        loadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        MenuButtons.SetActive(true);
    }
    public void openRoomBrowser()
    {
        CloseMenus();
        RoomBrowserScreen.SetActive(true);
    }
    public void closeRoomBrowser()
    {
        CloseMenus();
        MenuButtons.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
       foreach(RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
       allRoomButtons.Clear();
       theRoomButton.gameObject.SetActive(false);
       for(int i = 0; i < roomList.Count; i++)
       {
           if(roomList[i].PlayerCount!= roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
           {
               RoomButton newButton = Instantiate(theRoomButton,theRoomButton.transform.parent);
               newButton.setButtonDetails(roomList[i]);
               newButton.gameObject.SetActive(true);
                
               allRoomButtons.Add(newButton);
           }
       }
    }

    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);
        CloseMenus();
        loadingText.text = "Joining Room...";
        loadingScreen.SetActive(true);
    }
}
