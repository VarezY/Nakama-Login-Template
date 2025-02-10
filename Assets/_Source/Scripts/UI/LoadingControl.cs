using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Hige
{
    public class LoadingControl : MonoBehaviour
    {
	    public GameObject loginPanel;   // Panel login awal
	    public GameObject mainmenuPanel;
	    public GameObject loadingPanel; // Panel loading
	    public Slider loadingBar;        // Slider sebagai loading bar
	    public Button playButton;        // Tombol Play
	    

	    private void Start()
	    {
		    // Pastikan panel loading tidak terlihat saat start
		    loadingPanel.SetActive(false);
		    mainmenuPanel.SetActive(false);
		    playButton.gameObject.SetActive(false);
	    }

	    public void OnLoginButtonClicked()
	    {
		    // Sembunyikan panel login dan tampilkan panel loading
		    loginPanel.SetActive(false);
		    loadingPanel.SetActive(true);

		    // Mulai proses loading
		    StartCoroutine(LoadGame());
	    }

	    #region Loading Boongan
	    IEnumerator LoadGame()
	    {
		    float progress = 0f;

		    while (progress < 1f)
		    {
			    progress += Time.deltaTime / 3f; // Simulasi loading dalam 3 detik
			    loadingBar.value = progress;
			    yield return null;
		    }

		    // Setelah loading selesai, tampilkan tombol Play
		    playButton.gameObject.SetActive(true);
	    }
	    #endregion
	    
	    public void OnPlayButtonClicked()
	    {
		    // Sembunyikan panel loading saat tombol Play ditekan
		    loadingPanel.SetActive(false);
		    mainmenuPanel.SetActive(true);
	    }
	    
	
    	#region Private Methods
	
    	#endregion
    }
}
