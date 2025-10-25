using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClaimVIPNFTManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [Tooltip("Tất cả các button sẽ bị disable khi đang mua")]
    public Button[] allButtons;

    [Tooltip("Nút Buy VIP NFT sẽ được ẩn khi thành công")]
    public Button buyVIPNFTButton;

    [Header("Text UI")]
    [Tooltip("Text hiển thị trạng thái")]
    public TextMeshProUGUI[] statusTexts;

    private string walletAddress;

    public async void ClaimVIPNFT()
    {
        // 🟡 Giai đoạn bắt đầu: disable tất cả button + hiển thị status
        SetButtonsInteractable(false);
        SetStatusTexts("Buying VIP NFT... Please wait.");

        try
        {
            // 🟢 Lấy địa chỉ ví
            walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            if (string.IsNullOrEmpty(walletAddress))
                throw new Exception("Wallet address is empty. Please connect your wallet.");

            // 🟢 Claim NFT VIP
            var contract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.NFTVIPContractAddress);
            var result = await contract.ERC721.ClaimTo(walletAddress, 1);

            Debug.Log("VIP NFT claimed!");

            // 🟢 Cập nhật lại UI sau khi claim thành công
            SetButtonsInteractable(true);
            if (buyVIPNFTButton != null)
                buyVIPNFTButton.gameObject.SetActive(false); // Ẩn nút Buy VIP NFT

            SetStatusTexts("Completed.");

            // 🔻🔻🔻 🔄 Cập nhật lại số lượng VIP NFT (KHÔNG CÓ TOKEN)
            var nftContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.NFTVIPContractAddress);
            List<NFT> nftList = await nftContract.ERC721.GetOwned(walletAddress);
            PlayerDataManager.Instance.vipNFT = nftList.Count;
            // 🔺🔺🔺

            // 🔹 Lấy địa chỉ ví người dùng
            string userAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            // 🔹 Lấy số dư ví (native token, ví dụ ETH, MATIC...)
            var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
            PlayerDataManager.Instance.walletBalance = bal.displayValue; // 🟢 Thêm dòng này

            FindObjectOfType<BalanceFetcher>()?.FetchAllBalances();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to buy VIP NFT: " + ex.Message);
            SetButtonsInteractable(true);
            SetStatusTexts("Buy failed. Please try again.");
        }
    }

    private void SetButtonsInteractable(bool state)
    {
        foreach (var btn in allButtons)
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