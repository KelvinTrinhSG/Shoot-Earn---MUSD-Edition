using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionWithCleanup : MonoBehaviour
{
    [Header("Button để chuyển scene")]
    public Button transitionButton;

    [Header("Tên scene muốn chuyển tới")]
    public string targetScene;

    private void Start()
    {
        if (transitionButton != null)
            transitionButton.onClick.AddListener(OnTransitionClicked);
        else
            Debug.LogWarning("❗ transitionButton chưa được gán.");
    }

    private void OnTransitionClicked()
    {
        CleanupDontDestroyObjects();
        SceneManager.LoadScene(targetScene);
    }

    private void CleanupDontDestroyObjects()
    {
        GameObject[] rootObjects = GameObject.FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in rootObjects)
        {
            if (obj.scene.name == null || obj.scene.name == "")
            {
                // Đây là các GameObject trong DontDestroyOnLoad
                Destroy(obj);
            }
        }
        Debug.Log("✅ Đã xoá toàn bộ DontDestroyOnLoad objects.");
    }
}