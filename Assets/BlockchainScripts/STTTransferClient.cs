using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Thirdweb;

public class STTTransferClient : MonoBehaviour
{
    [Header("Server URL")]
    private string serverUrl = "https://musd-backend.onrender.com/transfer-erc20";

    [Header("PlayerPrefs Key")]
    public string playerPrefsKey = "TotalScore";

    [Header("UI")]
    public Button[] actionButtons;
    public TextMeshProUGUI[] statusTexts;
    public TextMeshProUGUI totalScoreText;  // UI hiển thị điểm tổng

    private string walletAddress;
    private float amount;

    public async void OnClaimButtonPressed()
    {
        SetButtonsInteractable(false);
        SetStatusTexts("Claiming... Please wait");

        try
        {
            // 🟢 Bước 1: Lấy địa chỉ ví từ Thirdweb
            walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            //walletAddress = "0xAe909F999CE1334eD02d40f0Afb883A967B03DEA"; //For Test

            if (string.IsNullOrEmpty(walletAddress))
                throw new Exception("Wallet address is null or empty.");

            // 🟢 Bước 2: Lấy amount từ PlayerPrefs
            amount = PlayerPrefs.GetInt(playerPrefsKey, 0);

            Debug.Log("PlayerPrefs.GetInt(playerPrefsKey, 0)" + PlayerPrefs.GetInt(playerPrefsKey, 0));

            if (amount <= 0)
                throw new Exception("No score to claim!");

            amount = amount / 10000;

            // 🟢 Bước 3: Gửi request
            StartCoroutine(SendClaimRequest(walletAddress, amount));
        }
        catch (Exception ex)
        {
            Debug.LogError("Claim error: " + ex.Message);
            SetStatusTexts(ex.Message);
            SetButtonsInteractable(true);
        }
    }

    IEnumerator SendClaimRequest(string to, float amount)
    {
        string jsonPayload = JsonUtility.ToJson(new TransferRequest(to, amount));

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Claim successful: " + request.downloadHandler.text);
                SetStatusTexts("Claimed successfully!");

                // ✅ Reset PlayerPrefs
                PlayerPrefs.SetInt(playerPrefsKey, 0);
                PlayerPrefs.Save();

                // ✅ Update UI score text về 0
                if (totalScoreText != null)
                {
                    totalScoreText.text = "0";
                }

                FindObjectOfType<BalanceFetcher>()?.FetchAllBalances();
            }
            else
            {
                Debug.LogError("Server error: " + request.error);
                SetStatusTexts("Server error: " + request.error);
            }

            SetButtonsInteractable(true);
        }
    }

    void SetButtonsInteractable(bool interactable)
    {
        foreach (var btn in actionButtons)
        {
            btn.interactable = interactable;
        }
    }

    void SetStatusTexts(string message)
    {
        foreach (var text in statusTexts)
        {
            text.SetText(message);
        }
    }

    [Serializable]
    public class TransferRequest
    {
        public string to;
        public float amount;

        public TransferRequest(string to, float amount)
        {
            this.to = to;
            this.amount = amount;
        }
    }
}
