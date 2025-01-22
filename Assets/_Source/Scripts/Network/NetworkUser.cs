using System.Threading.Tasks;
using Nakama;
using UnityEngine;
namespace Hige.Network
{
    public class NetworkUser : MonoBehaviour
    {
        private NetworkManager _networkManager;

        private void Start()
        {
            _networkManager = NetworkManager.Instance;
        }
        
        public async void UpdateUser(string username = null, string displayName = null, string avatarUrl = null)
        {
            try
            {
                await _networkManager.Client.UpdateAccountAsync(_networkManager.Session,
                    username,
                    displayName,
                    avatarUrl).ContinueWith(task =>
                {
                    Debug.Log($"<color=yellow>Update</color>: User Account Updated!");
                } );
            }
            catch (ApiResponseException ex)
            {
                Debug.Log($"<color=red>Error authenticating device: </color> {ex.StatusCode}:{ex.Message}");
                throw;
            }
        }

        public async Task<IApiUsers> GetAnotherUsers(string[] userId, string[] username = null)
        {
            return await _networkManager.Client.GetUsersAsync(_networkManager.Session, userId, username);
        }
    }
}