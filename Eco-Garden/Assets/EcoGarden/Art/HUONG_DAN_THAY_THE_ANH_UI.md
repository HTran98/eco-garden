# Hướng Dẫn Thay Thế Ảnh vào Các UI Hiện Có — Eco-Garden

## Tổng Quan

Hiện tại toàn bộ sprite trong game được tạo tự động tại runtime bởi class
`PlaceholderSpriteFactory` (`Assets/EcoGarden/Scripts/Utilities/PlaceholderSpriteFactory.cs`).
Mỗi property trong class đó tương ứng với một phần tử UI cụ thể.

Khi artwork thực sự sẵn sàng, bạn thay thế từng sprite placeholder bằng cách:
1. Import file ảnh vào đúng thư mục trong `Assets/`.
2. Gán sprite vào đúng chỗ trong code hoặc Inspector.

---

## Bước 1 — Import Ảnh vào Project

| Loại asset | Thư mục khuyến nghị |
|---|---|
| UI (panels, buttons, bars) | `Assets/EcoGarden/Art/UI/` |
| Items / Lotus | `Assets/EcoGarden/Art/Items/` |
| Characters (NPC, Butterfly) | `Assets/EcoGarden/Art/Characters/` |
| Board tiles | `Assets/EcoGarden/Art/Board/` |
| Obstacles (Weed, Pebble) | `Assets/EcoGarden/Art/Obstacles/` |
| Producers | `Assets/EcoGarden/Art/Producers/` |

**Import settings được khuyến nghị cho UI sprite:**
- Texture Type: **Sprite (2D and UI)**
- Sprite Mode: **Single** (hoặc Multiple nếu dùng sprite sheet)
- Pixels Per Unit: **100** (hoặc đồng nhất với giá trị `SpriteSize = 64` đang dùng)
- Filter Mode: **Bilinear** (hoặc Point cho pixel art)
- Compression: **None** hoặc **Lossless**

---

## Bước 2 — Danh Sách Sprite Cần Thay Thế

Dưới đây là toàn bộ sprite hiện đang dùng placeholder, kèm tên internal và UI component tương ứng.

### 2.1 UI Panels & Bars (`HudSkinController.cs`)

| Tên property trong code | GameObject được skin | Mô tả |
|---|---|---|
| `HudTopBarSprite` | `TopBar` | Thanh trên cùng của HUD |
| `HudPanelSprite` | `ObjectivePanel`, `AbilityBar`, `ResultPanel`, `LevelPanel`, `LevelViewport`, `MissionPanel`, `MissionViewport`, `MissionTrackerPanel` | Panel nền chung |
| `ShopPanelSprite` | `ShopPanel` | Panel nền cửa hàng |
| `HudButtonSprite` | `PauseButton`, `LevelButton`, `LevelCloseButton`, `MissionButton`, `MissionCloseButton`, `MissionTrackerOpenButton`, `ShopButton`, `ShopCloseButton`, `ShopCategoryBoosterButton`, `ShopCategoryDecorationButton`, `ShopCategoryUnlockButton`, `ShopCategoryCurrencyButton`, `ShopCategoryBundleButton`, `RestartButton`, `NextLevelButton`, `ShovelButton`, `MagicWandButton`, `SortingMagnetButton` | Nền tất cả các nút |

### 2.2 Shop UI

| Tên property | Mô tả |
|---|---|
| `ShopProductRowSprite` | Nền mỗi dòng sản phẩm trong Shop |
| `ShopPriceBadgeSprite` | Badge hiển thị giá |
| `ShopIconBadgeSprite` | Badge icon sản phẩm |
| `ShopPanelSprite` | Panel nền toàn bộ Shop |
| `ShopProductViewport` | Viewport danh sách sản phẩm (dùng `HudPanelSprite`) |

### 2.3 Drop Zones (`HudSkinController.cs` — `SkinDropZone`)

| Tên property | GameObject | Mô tả |
|---|---|---|
| `DeliverZoneSprite` | `DeliveryDropZone` | Vùng thả giao hàng |
| `SellBasketSprite` | `SellBasket` | Giỏ bán hàng |

### 2.4 Board Tiles

| Tên property | Mô tả |
|---|---|
| `EmptyTileSprite` | Ô trống trên bảng |
| `LockedTileSprite` | Ô bị khóa |

### 2.5 Items (Lotus)

| Tên property / method | Mô tả |
|---|---|
| `GetLotusSprite(int level)` | Sprite hoa sen theo cấp độ 1–5 |

### 2.6 Obstacles

| Tên property | Mô tả |
|---|---|
| `WeedSprite` | Cỏ dại |
| `PebbleSprite` | Đá cuội |

### 2.7 Characters

| Tên property | Mô tả |
|---|---|
| `NpcSprite` | Khách hàng (NPC) |
| `ButterflySprite` | Bướm |

### 2.8 Producers

| Tên property | Mô tả |
|---|---|
| `ProducerSprite` | Máy sản xuất hạt sen |

### 2.9 Misc

| Tên property | Mô tả |
|---|---|
| `SquareSprite` | Sprite 1×1 trắng dùng làm mask / fill chung |

---

## Bước 3 — Cách Thay Thế Sprite Trong Code

File cần chỉnh: `Assets/EcoGarden/Scripts/Utilities/PlaceholderSpriteFactory.cs`

### Ví dụ: Thay `HudPanelSprite`

**Trước (placeholder):**
```csharp
public static Sprite HudPanelSprite
{
	get
	{
		if (hudPanelSprite == null)
		{
			hudPanelSprite = CreateRoundedRectSprite(
				"ui_panel_runtime",
				new Color(0.16f, 0.23f, 0.25f, 0.90f),
				new Color(0.58f, 0.78f, 0.72f, 1f),
				12, 4);
		}
		return hudPanelSprite;
	}
}
```

**Sau (dùng ảnh thực):**
```csharp
public static Sprite HudPanelSprite
{
	get
	{
		if (hudPanelSprite == null)
		{
			hudPanelSprite = Resources.Load<Sprite>("UI/ui_panel");
		}
		return hudPanelSprite;
	}
}
```

> **Lưu ý:** File ảnh phải nằm trong thư mục `Assets/Resources/UI/` và đặt tên `ui_panel`
> (không có phần mở rộng khi gọi `Resources.Load`).

### Ví dụ: Thay Lotus Sprite theo cấp độ

**Trước:**
```csharp
public static Sprite GetLotusSprite(int level)
{
	int safeLevel = Mathf.Clamp(level, 1, 5);
	if (lotusSprites[safeLevel] == null)
	{
		lotusSprites[safeLevel] = CreateLotusSprite(safeLevel);
	}
	return lotusSprites[safeLevel];
}
```

**Sau:**
```csharp
public static Sprite GetLotusSprite(int level)
{
	int safeLevel = Mathf.Clamp(level, 1, 5);
	if (lotusSprites[safeLevel] == null)
	{
		lotusSprites[safeLevel] = Resources.Load<Sprite>("Items/item_lotus_lv0" + safeLevel);
	}
	return lotusSprites[safeLevel];
}
```

> File ảnh phải nằm tại: `Assets/Resources/Items/item_lotus_lv01.png` → `item_lotus_lv05.png`

---

## Bước 4 — Thay Sprite Qua Inspector (Không Cần Sửa Code)

Nếu bạn **không muốn chỉnh `PlaceholderSpriteFactory`**, bạn có thể gán sprite trực tiếp
trên prefab/scene thông qua Inspector sau khi game đã chạy xong `HudSkinController.Apply()`.

1. Mở Scene (`EcoGarden_FirstRelease_Progression` hoặc `EcoGarden_Level15_VerticalSlice`).
2. Tìm GameObject cần thay ảnh (xem cột "GameObject được skin" ở Bước 2).
3. Chọn component `Image` trên Inspector.
4. Kéo sprite mới vào trường **Source Image**.
5. Điều chỉnh **Color** nếu cần (các panel hiện đang tô màu bằng tham số `color` trong `SkinImage`).

> **Lưu ý:** `HudSkinController.Apply()` chạy trong `Start()` và sẽ ghi đè sprite mỗi lần
> Play. Để giữ sprite tùy chỉnh qua Inspector, hãy xóa lệnh gọi `SkinImage` tương ứng
> trong `HudSkinController.Apply()` hoặc comment nó ra sau khi đã gán sprite thực.

---

## Bước 5 — Thay Icon Text Bằng Icon Sprite (UiIconLabelCatalog)

Hiện tại các nút đang dùng text ký tự ngắn thay cho icon
(xem `Assets/EcoGarden/Scripts/UI/UiIconLabelCatalog.cs`):

| Hằng số | Giá trị text hiện tại | Mô tả |
|---|---|---|
| `Level` | `"Lv"` | Nút Level |
| `Mission` | `"Task"` | Nút Mission |
| `Shop` | `"$"` | Nút Shop |
| `Pause` | `"II"` | Nút Pause |
| `Close` | `"X"` | Nút đóng panel |
| `Restart` | `"R"` | Nút chơi lại |
| `Next` | `">"` | Nút level tiếp theo |
| `Gold` | `"G"` | Icon vàng |
| `Gem` | `"*"` | Icon đá quý |
| `Ability.Shovel` | `"SH"` | Icon xẻng |
| `Ability.MagicWand` | `"WD"` | Icon đũa phép |
| `Ability.SortingMagnet` | `"MG"` | Icon nam châm |

Khi có icon artwork, bạn có thể:
- Thêm `[SerializeField] Sprite iconSprite` vào controller tương ứng rồi gán qua Inspector, **hoặc**
- Dùng component `Image` thay cho `Text` trên các nút và gán sprite icon trực tiếp.

---

## Tóm Tắt Thứ Tự Thực Hiện

```
1. Nhận file ảnh từ artist
2. Import vào Assets/EcoGarden/Art/<loại>/ (import settings: Sprite 2D and UI)
3. Copy vào Assets/Resources/<loại>/ nếu dùng Resources.Load
4. Mở PlaceholderSpriteFactory.cs, thay từng property dùng placeholder
   bằng Resources.Load<Sprite>("...")
5. Xóa / comment dòng SkinImage/SkinButton tương ứng trong HudSkinController.Apply()
   nếu muốn kiểm soát sprite qua Inspector
6. Play scene, kiểm tra kết quả
```

---

## Ghi Chú Kỹ Thuật

- `PlaceholderSpriteFactory` dùng `HideFlags.HideAndDontSave` — các sprite tạo ra không lưu vào scene.
- `HudSkinController.Apply()` gọi `FindObjectIncludingInactive` để tìm GameObject theo tên —
  tên GameObject **phải khớp chính xác** với chuỗi truyền vào (xem cột "GameObject được skin").
- Sprite 9-slice cho panel được khuyến nghị để panel co giãn đẹp hơn:
  Bật **Mesh Type: Full Rect** và **Border** trong Sprite Editor.
- Màu tint hiện tại của từng panel được đặt trong `HudSkinController.Apply()`.
  Nếu dùng ảnh có màu đúng sẵn, đổi tham số `color` thành `Color.white`.
