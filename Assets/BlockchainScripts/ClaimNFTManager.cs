using System;
using System.Threading.Tasks;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClaimNFTManager : MonoBehaviour
{
    [Header("UI Buttons to Disable/Enable")]
    [Tooltip("Tất cả các button sẽ bị disable khi đang claim và enable lại khi lỗi.")]
    public Button[] actionButtons;

    [Header("Text UI to Show Status")]
    [Tooltip("Các text để hiển thị trạng thái (claiming, lỗi...)")]
    public TextMeshProUGUI[] statusTexts;

    [Header("Settings")]
    [Tooltip("Tên scene sẽ load khi claim thành công.")]
    public string successSceneName = "ShopAndPlay";

    private string walletAddress;

    public async void ClaimNFTPass()
    {
        // 🟡 Bước 1: Tắt tất cả nút và hiển thị thông báo
        SetButtonsInteractable(false);
        SetStatusTexts("Claiming NFT... Please wait.");

        try
        {
            // 🟢 Bước 2: Lấy địa chỉ ví
            walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            if (string.IsNullOrEmpty(walletAddress))
                throw new Exception("Wallet address is empty. Please connect your wallet.");

            Debug.Log($"Wallet Address: {walletAddress}");

            // 🟢 Bước 3: Lấy contract và gọi claim
            var contract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.NFTTokenGateContractAddress);
            var result = await contract.ERC721.ClaimTo(walletAddress, 1);

            Debug.Log("NFT claimed successfully!");

            // 🔹 Lấy địa chỉ ví người dùng
            string userAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            // 🔹 Lấy số dư ví (native token, ví dụ ETH, MATIC...)
            var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
            PlayerDataManager.Instance.walletBalance = bal.displayValue; // 🟢 Thêm dòng này

            // 🟢 Bước 4: Chuyển scene
            SceneManager.LoadScene(successSceneName);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to claim NFT: " + ex.Message);

            // 🔴 Bước 5: Enable lại các nút và báo lỗi
            SetButtonsInteractable(true);
            SetStatusTexts("Claim failed. Please try again.");
        }
    }

    private void SetButtonsInteractable(bool state)
    {
        foreach (var btn in actionButtons)
        {
            if (btn != null) btn.interactable = state;
        }
    }

    private void SetStatusTexts(string message)
    {
        foreach (var txt in statusTexts)
        {
            if (txt != null) txt.text = message;
        }
    }
}