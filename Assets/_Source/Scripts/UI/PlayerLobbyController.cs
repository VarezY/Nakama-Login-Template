using System;
using TMPro;
using UnityEngine;
namespace Hige.UI
{
    public class PlayerLobbyController : MonoBehaviour
    {
	    [SerializeField] private RectTransform backgroundPanel;
	    [SerializeField] private TMP_Text username;

	    private void Start()
	    {
		    
	    }

	    public void ChangeUsername(string newUsername)
	    {
		    username.text = newUsername;
	    }
	    
    	#region Private Methods
	
    	#endregion
    }
}
