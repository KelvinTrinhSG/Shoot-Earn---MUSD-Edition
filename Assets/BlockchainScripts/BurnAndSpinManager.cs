using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BurnAndSpinManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] allButtons; // 🟢 Thêm mảng tất cả nút để disable/enable
    public Button spinButton;
    public Button spinButtonMUSD;
    public TextMeshProUGUI[] statusTexts;

    private string walletAddress;

    private string ToAmount = "0.1";

    public async void OnSpinClicked(int indexValue)
    {
        SetButtonsInteractable(false);
        SetStatusTexts("Checking token balance...");

        if (indexValue == 1) {
            try
            {
                walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
                if (string.IsNullOrEmpty(walletAddress))
                    throw new Exception("Wallet address is empty.");

                var tokenContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.TokenGemContractAddress);
                var balance = await tokenContract.ERC20.BalanceOf(walletAddress);

                int gemToken;
                if (!int.TryParse(balance.displayValue.Split('.')[0], NumberStyles.Any, CultureInfo.InvariantCulture, out gemToken))
                    gemToken = 0;

                if (gemToken < 1)
                {
                    SetStatusTexts("Not enough GEM. Please claim more.");
                    SetButtonsInteractable(true); // mở lại nút nếu không đủ
                    return;
                }

                SetStatusTexts("Burning 1 GEM...");
                await tokenContract.ERC20.Burn("1");

                SetStatusTexts("Spinning...");
                //await Task.Delay(300); // Chờ chút cho cảm giác mượt

                // ⬇️ Tách SpinEffect ra và đợi hoàn tất rồi mới mở lại nút
                StartCoroutine(SpinThenEnableButtons());
            }
            catch (Exception ex)
            {
                Debug.LogError("Spin failed: " + ex.Message);
                SetStatusTexts("Something went wrong. Try again.");
                SetButtonsInteractable(true);
            }
        }
        else {
            try
            {
                walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
                if (string.IsNullOrEmpty(walletAddress))
                    throw new Exception("Wallet address is empty.");

                var tokenContractMUSD = ThirdwebManager.Instance.SDK.GetContract(GameConfig.MUSDAddress);
                var balanceMUSD = await tokenContractMUSD.ERC20.BalanceOf(walletAddress);

                float gemTokenMUSD;
                if (!float.TryParse(balanceMUSD.displayValue, NumberStyles.Float, CultureInfo.InvariantCulture, out gemTokenMUSD))
                    gemTokenMUSD = 0f;

                if (gemTokenMUSD < 0.1f)
                {
                    SetStatusTexts("Not enough MUSD. Please claim more.");
                    SetButtonsInteractable(true); // mở lại nút nếu không đủ
                    return;
                }

                SetStatusTexts("Using 0.1 MUSD");
                await ThirdwebManager.Instance.SDK.Wallet.Transfer(GameConfig.ToAddress, ToAmount, GameConfig.MUSDAddress);
                Debug.Log("✅ Transfer successful!");

                SetStatusTexts("Spinning...");
                //await Task.Delay(300); // Chờ chút cho cảm giác mượt

                // ⬇️ Tách SpinEffect ra và đợi hoàn tất rồi mới mở lại nút
                StartCoroutine(SpinThenEnableButtons());

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Transfer failed: {ex.Message}");
            }

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

    private IEnumerator SpinThenEnableButtons()
    {
        int finalReward = UnityEngine.Random.Range(1, 11);
        float spinDuration = 1.5f;  // thời gian quay tổng
        float interval = 0.1f;      // mỗi lần nhảy số
        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            int fakeReward = UnityEngine.Random.Range(1, 11);
            SetStatusTexts($"Spinning... {fakeReward}");

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        // 🎯 Check final result
        if (finalReward == 8)
        {
            SetStatusTexts("Jackpot! You hit the special reward!");

            // Double the coin balance from PlayerPrefs
            int currentCoin = PlayerPrefs.GetInt("TotalScore", 0);
            int newCoin = currentCoin * 2;

            PlayerPrefs.SetInt("TotalScore", newCoin);
            PlayerPrefs.Save();

            Debug.Log($"Coin doubled: {currentCoin} -> {newCoin}");
        }
        else
        {
            SetStatusTexts("You didn't win the jackpot this time.");
        }

        // ✅ Quan trọng: bật lại tất cả các button
        SetButtonsInteractable(true);
        FindObjectOfType<BalanceFetcher>()?.FetchAllBalances();
    }
}