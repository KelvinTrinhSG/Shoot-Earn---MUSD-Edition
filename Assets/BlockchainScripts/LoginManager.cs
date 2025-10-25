using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Thirdweb;
using TMPro; // Nếu dùng TextMeshPro

public class LoginManager : MonoBehaviour
{
    [SerializeField] private GameObject claimNFTButton;
    [SerializeField] private string sceneNameIfHasNFT = "ShopAndPlay";
    // 🔸 MỚI: Text hiển thị nếu không có NFT
    [SerializeField] private GameObject noNFTText; // <-- THÊM DÒNG NÀY

    public async void Login()
    {
        try
        {
            // Lấy địa chỉ ví người chơi
            string address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
            Debug.Log($"✅ Wallet address: {address}");

            // Lấy contract NFT
            var contract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.NFTTokenGateContractAddress);

            // Lấy danh sách NFT mà người chơi sở hữu
            List<NFT> nftList = await contract.ERC721.GetOwned(address);
            bool hasNFT = nftList != null && nftList.Count > 0;

            if (hasNFT)
            {
                Debug.Log("✅ Player has NFT – loading scene...");
                SceneManager.LoadScene(sceneNameIfHasNFT);
            }
            else
            {
                Debug.Log("❌ Player has no NFT – showing claim button...");
                if (claimNFTButton != null)
                {
                    claimNFTButton.SetActive(true);
                    // 🔸 MỚI: Hiện text nếu không có NFT
                    if (noNFTText != null)                         // <-- THÊM DÒNG NÀY
                        noNFTText.SetActive(true);                 // <-- THÊM DÒNG NÀY
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Login error: {ex.Message}");
        }
    }
}