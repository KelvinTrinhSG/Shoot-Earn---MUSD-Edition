## 🤠 Shoot-And-Earn – MUSD Edition  
**Shoot fast. Earn smart. Trade steady.**

In **Shoot-And-Earn – MUSD Edition**, your goal is simple yet thrilling:  
🎯 Keep targets bouncing in the air, rack up points, and convert them into **MUSD** — the stable, Bitcoin-powered token of the **MEZO Chain**.  
Play, earn, and spin your way through the Web3 wild west!

---

### 🎮 Gameplay

- ⚡ **Fast Reflex Action** – Shoot targets before they hit the ground.  
- 🎯 **Multi-Target Juggling Bonus** – Keep multiple targets airborne for combo points.  
- 🚀 **Increasing Challenge** – Targets move faster over time.  
- 💰 **Score-to-MUSD Rewards** – Redeem your points for on-chain MUSD rewards.  
- 🎡 **Spin-to-Earn System** – Spend MUSD to spin the reward wheel for GEMs, boosters, or rare bonuses.

---

### 🌐 Web3 Integration – Built on MEZO Chain

The game runs fully on **MEZO**, a Bitcoin Layer-2 network with EVM compatibility.  
It uses smart contracts deployed via **Thirdweb**, ensuring on-chain ownership, smooth NFT gating, and transparent reward loops.

---

#### 🪙 Token Gate NFT  
**Type:** OpenEditionERC721  
**Name:** The Token Gate NFT  
**Symbol:** GATE  
**Description:** Your digital key to unlock premium content, gated modes, and exclusive areas inside the game.  
**Contract Address:** `0x0e2fa00c79C130cB16C328621c64658a30B398c6`  
**View on Thirdweb:** [View Contract](https://thirdweb.com/team/kelvincod/655b6fab497583368cc6f8ee6e1183c3/contract/31611/0x0e2fa00c79C130cB16C328621c64658a30B398c6)  
**Claim Conditions:** Free  

---

#### 💎 VIP NFT  
**Type:** OpenEditionERC721  
**Name:** The VIP NFT  
**Symbol:** VIP  
**Description:** Grants exclusive privileges to elite supporters — early access, VIP rewards, and private tournaments.  
**Contract Address:** `0xc2Ca8AE15dB2a9d8e0d138b811732D9AEf6326e0`  
**View on Thirdweb:** [View Contract](https://thirdweb.com/team/kelvincod/655b6fab497583368cc6f8ee6e1183c3/contract/31611/0xc2Ca8AE15dB2a9d8e0d138b811732D9AEf6326e0)  
**Claim Conditions:** Costs 0.0000092 BTC  

---

#### 💠 GEM Token  
**Type:** ERC20 Token Drop  
**Name:** The GEM Token  
**Symbol:** GEM  
**Description:** A utility token that rewards active players and supporters. Every shot, spin, or action helps you accumulate GEMs as proof of engagement.  
**Contract Address:** `0x0D8062C1439fb01cdd4AFa8D0751F7cAeFCF044a`  
**View on Thirdweb:** [View Contract](https://thirdweb.com/team/kelvincod/655b6fab497583368cc6f8ee6e1183c3/contract/31611/0x0D8062C1439fb01cdd4AFa8D0751F7cAeFCF044a)  
**Claim Conditions:** ~0.00000092 BTC per claim  

---

#### 💵 MUSD Token  
**Type:** ERC20 Stablecoin  
**Description:** Used for spinning the wheel and redeeming earned points.  
**Network:** Deployed on **MEZO Chain (Bitcoin L2)** — stable, secure, and low-fee.  

---

### 💎 Why MUSD on MEZO?

- **Stable & Bitcoin-backed:** Anchored to BTC value while maintaining USD stability.  
- **Easy to Use:** Fewer decimals than BTC, simpler math, and faster UI updates.  
- **EVM-Compatible:** Works seamlessly with Unity + Thirdweb SDK.  
- **Ideal for Web3 Games:** Low latency, low fees, and direct on-chain verification.  

> 💡 MUSD combines Bitcoin’s reliability with stablecoin simplicity — perfect for an on-chain gaming economy.

---

### 🚀 How to Play

1. **Claim your Token Gate NFT** – it’s free.  
2. **Connect your MEZO wallet** in-game.  
3. **Shoot targets** to keep them bouncing.  
4. **Earn points** and redeem for MUSD.  
5. **Use MUSD to spin** for random bonuses.  
6. **Compete** for leaderboard rewards and climb your way to the top.

---

### 🛠 Local Setup

**Requirements**
- Unity 2022.x or newer  
- Node.js (v18+)  
- Git  
- [Thirdweb SDK for Unity](https://portal.thirdweb.com/unity)

**Steps**

```bash
git clone https://github.com/KelvinTrinhSG/Shoot-Earn---MUSD-Edition.git
cd Shoot-Earn---MUSD-Edition
```

Open in **Unity Hub → Build Settings → WebGL → Build and Run**

**Configure Contracts**

In Unity → `Assets/Config/Web3Settings.cs`
```csharp
public string MUSDCONTRACT = "0x118917a40FAF1CD7a13dB0Ef56C86De7973Ac503";
public string GATE_NFT = "0x0e2fa00c79C130cB16C328621c64658a30B398c6";
public string VIP_NFT = "0xc2Ca8AE15dB2a9d8e0d138b811732D9AEf6326e0";
public string GEM_TOKEN = "0x0D8062C1439fb01cdd4AFa8D0751F7cAeFCF044a";
```

---

### 🌍 Play Online

🎮 [Play Shoot-And-Earn – MUSD Edition on Itch.io](https://thkien85.itch.io/shoot-and-earn-musd-edition)

---

### 🧠 Built With

- Unity Engine 2022  
- Thirdweb Unity SDK  
- **MEZO Chain (Bitcoin L2)**  
- Ethers.js + Node.js backend  

---

### 📜 License

MIT License — see `LICENSE` file for details.
