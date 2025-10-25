using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public int vipNFT;
    public int gemToken;
    public int highestScore;
    public string walletBalance; // dạng string vì bal.displayValue là string
    public string walletBalanceMUSD;
    // ✅ Thêm biến để đánh dấu đã lấy dữ liệu chain xong
    public bool isChainInitialized = false;
    public bool isGameInitialized = false;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }
}