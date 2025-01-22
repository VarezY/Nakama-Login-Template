using System;
using System.Collections;
using System.Text;
using Hige.Network;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Hige
{
    public class MainMenuManager : MonoBehaviour
    {
        private enum CaraLogin{Device = 1, Custom = 2}
    
        [Title("UI Login")]
        [SerializeField] private GameObject usernameInputPanel;
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private Button firstUser;

        [Title("UI Profile")]
        [SerializeField] private GameObject playerProfilePanel;
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private Button quickMatchButton;
        
        private NetworkManager _networkManager;
        private string _deviceId;
        private CaraLogin _login;
    
        private void Start()
        {
            _networkManager = NetworkManager.Instance;
            _networkManager.onConnected.AddListener(() => ShowProfile(true));
            _networkManager.onDisconnected.AddListener(() => ShowProfile(false));
        }

        private void ShowProfile(bool isShown)
        {
            playerProfilePanel.SetActive(isShown);
            usernameText.text = $"Hello {_networkManager.CurrentAccount.User.Username}!";
        }

        [Button]
        public void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        public void CheckFirstTimeDeviceUser()
        {
            _deviceId = PlayerPrefs.GetString("deviceId", null);

            if (string.IsNullOrEmpty(_deviceId))
            {
                _login = CaraLogin.Device;
                usernameInputPanel.SetActive(true);
            }
            else
            {
                _networkManager.AuthenticateWithDevice(_deviceId);
            }
        }

        public void CreateCustomUser()
        {
            _login = CaraLogin.Custom;
            usernameInputPanel.SetActive(true);
        }
    
        public void CreateFirstUser()
        {
            switch (_login)
            {
                case CaraLogin.Device:
                    PlayerPrefs.SetString("deviceId", Guid.NewGuid().ToString());
                    _deviceId = PlayerPrefs.GetString("deviceId", null);
                    _networkManager.AuthenticateWithDevice(_deviceId, usernameInputField.text);
                    break;
                case CaraLogin.Custom:
                    string username = usernameInputField.text;
                    int byteCount = Encoding.ASCII.GetByteCount(username);
                    if (byteCount < 6)
                    {
                        username = username.PadRight(6, ' ');
                    }
                    _networkManager.AuthenticateWithCustomId(username);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        public void EditUsername()
        {
            usernameInputField.text = $"{_networkManager.CurrentAccount.User.Username}";
        }

        public void ChangeUsername()
        {
            _networkManager.NetworkUser.UpdateUser(usernameInputField.text);
        }

        public async void QuickMatch()
        {
            // StartCoroutine(OnLoadGame());
            await _networkManager.LobbyManager.StartQuickMatch();
        }
        
        private IEnumerator OnLoadGame()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GamePage");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}