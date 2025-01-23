using TMPro;
using UnityEngine;
namespace Hige.UI
{
    public class PlayerLobbyController : MonoBehaviour
    {
	    [SerializeField] private TMP_Text username;

	    public void ChangeUsername(string newUsername)
	    {
		    username.text = newUsername;
	    }
	    
    	#region Private Methods
	
    	#endregion
    }
}
