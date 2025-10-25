using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb;

public class BalanceFetcher : MonoBehaviour
{

    [Header("UI Components")]
    public Button refreshWalletButton;
    [Header("Text hiển thị thông tin người chơi")]
    public TextMeshProUGUI walletBalanceText;
    public TextMeshProUGUI vipNFTText;
    public TextMeshProUGUI gemTokenText;
    [Header("Text hiển thị Coin Balance (local PlayerPrefs)")]
    public TextMeshProUGUI coinBalanceText;


    private string userAddress;

    private void Start()
    {
        if (refreshWalletButton != null)
        {
            refreshWalletButton.onClick.AddListener(FetchAllBalances);
        }
        else
        {
            Debug.LogWarning("❗ Chưa gán nút refreshWalletButton.");
        }
        FetchAllBalances();
    }

    public async void FetchAllBalances()
    {
        try
        {
            // 🔹 Get Wallet Address
            userAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
            if (string.IsNullOrEmpty(userAddress))
            {
                Debug.LogWarning("❌ Wallet address is empty.");
                return;
            }

            // 🔹 Get Wallet Balance (native token)
            var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
            PlayerDataManager.Instance.walletBalance = bal.displayValue;

            // 🔹 Get NFT VIP Count
            var nftContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.NFTVIPContractAddress);
            List<NFT> nftList = await nftContract.ERC721.GetOwned(userAddress);
            PlayerDataManager.Instance.vipNFT = nftList.Count;

            // 🔹 Get GEM Token Balance
            var tokenContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.TokenGemContractAddress);
            var tokenBalance = await tokenContract.ERC20.BalanceOf(userAddress);

            // 🔹 Get MUSD Token Balance
            var tokenContractMUSD = ThirdwebManager.Instance.SDK.GetContract(GameConfig.MUSDAddress);
            var tokenBalanceMUSD = await tokenContractMUSD.ERC20.BalanceOf(userAddress);
            PlayerDataManager.Instance.walletBalanceMUSD = tokenBalanceMUSD.displayValue;

            if (int.TryParse(tokenBalance.displayValue.Split('.')[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedToken))
            {
                PlayerDataManager.Instance.gemToken = parsedToken;
            }
            else
            {
                Debug.LogWarning("⚠️ Failed to parse GEM token balance.");
                PlayerDataManager.Instance.gemToken = 0;
            }

            Debug.Log($"✅ Wallet: {PlayerDataManager.Instance.walletBalance}, NFT VIP: {PlayerDataManager.Instance.vipNFT}, GEM: {PlayerDataManager.Instance.gemToken}");

            PlayerDataManager.Instance.isChainInitialized = true;

            // Sau khi lấy xong dữ liệu và gán vào PlayerDataManager:
            UpdateWalletUI(PlayerDataManager.Instance.walletBalanceMUSD);
            UpdateVIPNFTUI(PlayerDataManager.Instance.vipNFT);
            UpdateGEMTokenUI(PlayerDataManager.Instance.gemToken);
            UpdateCoinBalanceUI(); // 🟢 THÊM DÒNG NÀY
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error fetching balances: " + ex.Message);
            walletBalanceText.text = "--";
        }
    }

    private void UpdateWalletUI(string balanceStr)
    {
        if (walletBalanceText == null)
        {
            Debug.LogWarning("❗ Chưa gán Text để hiển thị số dư ví.");
            return;
        }

        if (decimal.TryParse(balanceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balanceDecimal))
        {
            walletBalanceText.text = balanceDecimal.ToString("F2", CultureInfo.InvariantCulture);
        }
        else
        {
            walletBalanceText.text = "0.00";
        }
    }

    private void UpdateVIPNFTUI(int nftCount)
    {
        if (vipNFTText == null)
        {
            Debug.LogWarning("❗ Chưa gán Text để hiển thị số NFT VIP.");
            return;
        }

        vipNFTText.text = nftCount.ToString();
    }

    private void UpdateGEMTokenUI(int gemAmount)
    {
        if (gemTokenText == null)
        {
            Debug.LogWarning("❗ Chưa gán Text để hiển thị GEM Token.");
            return;
        }

        gemTokenText.text = gemAmount.ToString();
    }

    private void UpdateCoinBalanceUI()
    {
        if (coinBalanceText == null)
        {
            Debug.LogWarning("❗ Chưa gán Text để hiển thị Coin Balance.");
            return;
        }

        int coinBalance = PlayerPrefs.GetInt("TotalScore", 0); // Lấy giá trị từ PlayerPrefs, mặc định là 0 nếu chưa có
        coinBalanceText.text = coinBalance.ToString();
    }
}
