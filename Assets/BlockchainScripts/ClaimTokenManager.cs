using System;
using System.Globalization;
using System.Threading.Tasks;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClaimTokenManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [Tooltip("Tất cả các button sẽ bị disable khi đang mua")]
    public Button[] allButtons;

    [Header("Text UI")]
    [Tooltip("Text hiển thị trạng thái")]
    public TextMeshProUGUI[] statusTexts;

    private string walletAddress;

    public async void ClaimToken()
    {
        // 🟡 Bắt đầu: disable tất cả button + hiển thị status
        SetButtonsInteractable(false);
        SetStatusTexts("Buying token... Please wait.");

        try
        {
            // 🟢 Lấy địa chỉ ví
            walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            if (string.IsNullOrEmpty(walletAddress))
                throw new Exception("Wallet address is empty. Please connect your wallet.");

            // 🟢 Claim 10 GEM token
            var tokenContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.TokenGemContractAddress);
            var result = await tokenContract.ERC20.Claim("10");

            Debug.Log("🎉 Token claimed successfully!");

            // 🟢 Cập nhật số dư token
            var balance = await tokenContract.ERC20.BalanceOf(walletAddress);
            if (int.TryParse(balance.displayValue.Split('.')[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedToken))
            {
                PlayerDataManager.Instance.gemToken = parsedToken;
            }
            else
            {
                Debug.LogWarning("⚠️ Failed to parse GEM token balance.");
                PlayerDataManager.Instance.gemToken = 0;
            }

            // 🔹 Lấy địa chỉ ví người dùng
            string userAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            // 🔹 Lấy số dư ví (native token, ví dụ ETH, MATIC...)
            var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
            PlayerDataManager.Instance.walletBalance = bal.displayValue; // 🟢 Thêm dòng này

            // 🟢 UI sau khi thành công
            SetStatusTexts("Completed.");
            FindObjectOfType<BalanceFetcher>()?.FetchAllBalances();
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Failed to claim token: " + ex.Message);
            SetStatusTexts("Buy failed. Please try again.");
        }
        finally
        {
            SetButtonsInteractable(true);
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