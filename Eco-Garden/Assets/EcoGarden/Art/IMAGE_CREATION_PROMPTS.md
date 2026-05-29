# Image Creation Prompts — Eco-Garden UI Art

Phong cách chung của toàn bộ game:
> **2D mobile casual, eco / nature theme, soft watercolor-meets-flat-design style,
> lush green and teal palette, rounded shapes, warm accent highlights,
> clean silhouettes, transparent background (PNG), no drop shadow baked in.**

Mỗi prompt bên dưới được viết sẵn để paste trực tiếp vào AI image generator
(Midjourney, Adobe Firefly, DALL·E, Stable Diffusion, v.v.).

---

## NHÓM 1 — UI Panels & Bars

> Tương ứng mục **2.1** trong hướng dẫn.
> Tất cả asset nhóm này lưu tại: `Assets/EcoGarden/Art/UI/`

---

### 1.1 `ui_top_bar.png` — HUD Top Bar

**Kích thước:** 1024 × 128 px (landscape banner)
**Dùng cho:** GameObject `TopBar`

```
A horizontal top bar UI element for a 2D mobile casual eco garden game.
Rounded rectangle shape, wide and flat. Dark teal base color (#1F3440, ~94% opacity).
Soft teal-green border highlight (#6BA8A4). Subtle inner gradient from center to edges.
Flat nature-inspired ornamental vine motif along the left and right edges (very subtle, low opacity).
Clean, no text, no icons. Transparent background. PNG, 1024x128 px.
Style: soft watercolor-flat hybrid, mobile UI, eco/nature theme.
```

---

### 1.2 `ui_panel.png` — HUD General Panel

**Kích thước:** 512 × 512 px (9-slice friendly — borders: 24 px)
**Dùng cho:** `ObjectivePanel`, `AbilityBar`, `ResultPanel`, `LevelPanel`,
`LevelViewport`, `MissionPanel`, `MissionViewport`, `MissionTrackerPanel`

```
A square panel background UI element for a 2D mobile casual eco garden game.
Rounded rectangle, 9-slice ready (uniform 24px border all sides).
Dark muted teal fill (#293B40, ~90% opacity). Teal-mint border (#94C7B8).
Very subtle inner vignette. Tiny leaf/water-drop corner ornaments (optional, very faint).
Clean, no text, no icons. Transparent background. PNG, 512x512 px.
Style: soft flat mobile UI, nature/eco theme, calm and legible.
```

---

### 1.3 `ui_shop_panel.png` — Shop Panel Background

**Kích thước:** 512 × 512 px (9-slice friendly — borders: 24 px)
**Dùng cho:** `ShopPanel`

```
A square panel background for an in-game shop screen in a 2D mobile eco garden game.
Rounded rectangle, 9-slice ready (24px uniform border).
Very dark teal fill (#1A2A2E, ~98% opacity). Bright mint-green border (#C2EBD1).
Slightly warmer and more premium feel than a regular panel.
Faint floral or botanical watermark pattern in the fill area (very subtle, 8% opacity).
Clean, no text. Transparent background. PNG, 512x512 px.
Style: soft flat mobile shop UI, nature/eco theme, slightly elegant.
```

---

### 1.4 `ui_button.png` — HUD Button Background

**Kích thước:** 256 × 256 px (9-slice friendly — borders: 20 px)
**Dùng cho:** tất cả các nút: `PauseButton`, `ShopButton`, `MissionButton`,
`ShovelButton`, `MagicWandButton`, `SortingMagnetButton`, v.v.

```
A square button background UI element for a 2D mobile casual eco garden game.
Rounded rectangle shape, 9-slice ready (20px uniform border).
Medium teal fill (#336673). Bright soft cyan-mint border (#94E1D7).
Glossy highlight ellipse in the bottom-left corner (soft white, low opacity).
Slight top-to-bottom gradient (slightly lighter at top).
Clean, no text, no icons. Transparent background. PNG, 256x256 px.
Style: mobile casual button, tactile and inviting, eco/nature theme.
```

---

## NHÓM 2 — Shop UI Components

> Tương ứng mục **2.2** trong hướng dẫn.
> Lưu tại: `Assets/EcoGarden/Art/UI/`

---

### 2.1 `ui_shop_product_row.png` — Product Row Background

**Kích thước:** 512 × 128 px (9-slice — borders: 16 px)
**Dùng cho:** Mỗi dòng sản phẩm trong danh sách Shop

```
A horizontal list-row background UI element for a shop screen in a 2D mobile eco garden game.
Wide rounded rectangle, 9-slice ready (16px border).
Dark muted teal-green fill (#243330, ~96% opacity). Soft sage-green border (#82BDA0).
Subtle inner gradient, slightly lighter at top.
Clean, no text, no icons. Transparent background. PNG, 512x128 px.
Style: soft flat mobile list item, shop/eco theme, clean and readable.
```

---

### 2.2 `ui_shop_price_badge.png` — Price Badge

**Kích thước:** 256 × 96 px (9-slice — borders: 16 px)
**Dùng cho:** Badge giá tiền bên cạnh sản phẩm

```
A small price badge / pill UI element for a shop in a 2D mobile eco garden game.
Rounded rectangle (pill shape), 9-slice ready (16px border).
Dark earthy-green fill (#333F33, ~96% opacity). Warm gold border (#F5C756).
Gold tint inner glow at center. Coin/gem visual language.
Clean, no text. Transparent background. PNG, 256x96 px.
Style: mobile casual shop badge, gold-accent, eco/nature theme.
```

---

### 2.3 `ui_shop_icon_badge.png` — Icon Badge

**Kích thước:** 128 × 128 px (9-slice — borders: 16 px)
**Dùng cho:** Badge nền icon sản phẩm

```
A small square icon badge / frame background for a shop product in a 2D mobile eco garden game.
Rounded square, 9-slice ready (16px border).
Medium dark teal fill (#2E4D49, ~96% opacity). Bright light-green border (#CCF2BD).
Soft inner glow at center (very faint white).
Clean, no text, no icons inside. Transparent background. PNG, 128x128 px.
Style: soft flat mobile icon frame, eco/nature theme.
```

---

## NHÓM 3 — Drop Zones

> Tương ứng mục **2.3** trong hướng dẫn.
> Lưu tại: `Assets/EcoGarden/Art/UI/`

---

### 3.1 `ui_deliver_zone.png` — Delivery Drop Zone

**Kích thước:** 256 × 256 px
**Dùng cho:** `DeliveryDropZone`

```
A delivery drop zone UI element for a 2D mobile eco garden game.
Rounded square shape. Purple-teal base fill (#712A94, ~96% opacity).
Soft lavender-pink border (#E1A8F5).
Center visual: a stylized open hand or basket silhouette receiving a flower,
rendered in a flat soft watercolor style. Subtle star/sparkle particles.
Clean, bright and inviting. Transparent background. PNG, 256x256 px.
Style: mobile casual drop-zone indicator, mystical/garden theme.
```

---

### 3.2 `ui_sell_basket.png` — Sell Basket

**Kích thước:** 256 × 256 px
**Dùng për:** `SellBasket`

```
A sell basket UI element for a 2D mobile eco garden game.
Rounded square shape. Dark forest-green base fill (#476652, ~96% opacity).
Bright yellow-green border (#B8DC7A).
Center visual: a charming woven basket with a gold coin or leaf inside,
flat watercolor illustration style, soft and friendly.
Transparent background. PNG, 256x256 px.
Style: mobile casual sell/coin indicator, eco/nature theme.
```

---

## NHÓM 4 — Board Tiles

> Tương ứng mục **2.4** trong hướng dẫn.
> Lưu tại: `Assets/EcoGarden/Art/Board/`
> **Kích thước chuẩn:** 128 × 128 px (9-slice — borders: 14 px)

---

### 4.1 `tile_empty_01.png` — Empty Tile

```
A single empty garden board tile for a 2D mobile merge/puzzle eco garden game.
Rounded square, 9-slice ready (14px border). Viewed slightly from above (mild isometric feel is ok).
Muted teal-green fill (#94C7BD). Soft mint highlight border (#C7E8DF).
Surface texture: faint subtle soil or moss micro-texture.
Clean, empty, no objects. Transparent background. PNG, 128x128 px.
Style: soft flat tile, casual mobile eco garden, calm earthy palette.
```

---

### 4.2 `tile_locked_01.png` — Locked Tile

```
A locked board tile for a 2D mobile eco garden game.
Rounded square, 9-slice ready (14px border).
Dark charcoal fill (#262D38). Muted steel-blue border (#4A596B).
Surface: faint cracked stone or dark earth texture.
Small padlock icon silhouette centered (flat, white, ~30% opacity).
Transparent background. PNG, 128x128 px.
Style: soft flat tile, casual mobile, locked/unavailable state, dark and muted.
```

---

## NHÓM 5 — Items (Lotus)

> Tương ứng mục **2.5** trong hướng dẫn.
> Lưu tại: `Assets/EcoGarden/Art/Items/`
> **Kích thước chuẩn:** 128 × 128 px, transparent background

Các sprite lotus thể hiện **chuỗi tiến hóa** từ hạt → cây trưởng thành → hoa nở.
Mỗi cấp phải nhận ra được là "cùng một cây" nhưng ở giai đoạn lớn hơn.

---

### 5.1 `item_lotus_lv01.png` — Lotus Seed (Level 1)

```
A lotus seed item sprite for a 2D mobile merge eco garden game.
Small round seed, brown earthy tones (#BA8C56 main, #956840 shadow, #E0AE74 highlight).
Slightly oval shape, soft watercolor-flat style. Faint moisture sheen.
No stem, no leaves. Transparent background. PNG, 128x128 px.
Style: cute casual mobile item, nature/eco, soft and round.
```

---

### 5.2 `item_lotus_lv02.png` — Lotus Sprout (Level 2)

```
A tiny lotus sprout item sprite for a 2D mobile merge eco garden game.
Single green stem (#40883A) with one small rolled-up leaf bud at the top (#6EBF52).
Emerging from a small water surface or implied ground.
Soft watercolor-flat style, cheerful and small. Transparent background. PNG, 128x128 px.
Style: cute casual mobile item, nature/eco, fresh and new.
```

---

### 5.3 `item_lotus_lv03.png` — Lotus Young Plant (Level 3)

```
A young lotus plant item sprite for a 2D mobile merge eco garden game.
Green stem (#40883A) with two round flat lily-pad leaves (#5AB840), angled left and right.
Small rounded bud at top (unblooomed, pale green). Water droplet on one leaf (optional).
Soft watercolor-flat style. Transparent background. PNG, 128x128 px.
Style: cute casual mobile item, nature/eco, growing and lush.
```

---

### 5.4 `item_lotus_lv04.png` — Lotus Bud (Level 4)

```
A lotus bud item sprite for a 2D mobile merge eco garden game.
Sturdy green stem (#40883A), two spread lily-pad leaves (#5AB840).
Prominent closed lotus bud at top, pink-purple tones (#D67AAD, #E8A0C8).
Soft watercolor-flat style, elegant. Transparent background. PNG, 128x128 px.
Style: cute casual mobile item, nature/eco, anticipation of bloom.
```

---

### 5.5 `item_lotus_lv05.png` — Lotus Full Bloom (Level 5)

```
A fully bloomed lotus flower item sprite for a 2D mobile merge eco garden game.
Sturdy green stem (#40883A), lush lily-pad leaves (#5AB840).
Beautiful open lotus bloom at top: layered petals in soft pink and white
(#FAB2D9 outer petals, #FDE8F3 inner petals, golden yellow center #FFD756).
Glow/shimmer aura around the bloom (very soft, optional).
Soft watercolor-flat style, vibrant and satisfying. Transparent background. PNG, 128x128 px.
Style: cute casual mobile item, nature/eco, bloomed, rewarding and beautiful.
```

---

## NHÓM 6 — Obstacles

> Tương ứng mục **2.6** trong hướng dẫn.
> Lưu tại: `Assets/EcoGarden/Art/Obstacles/`
> **Kích thước chuẩn:** 128 × 128 px, transparent background

---

### 6.1 `obs_weed_01.png` — Weed

```
A weed obstacle sprite for a 2D mobile merge eco garden game.
Scraggly wild weed plant with 2-3 unruly stems and jagged leaves.
Dark and light green tones (#385A3C dark, #6ABD57 light).
Slightly menacing but still cute/stylized. No roots visible.
Soft watercolor-flat style. Transparent background. PNG, 128x128 px.
Style: cute casual mobile obstacle, nature/eco, wild and unkempt.
```

---

### 6.2 `obs_pebble_01.png` — Pebble

```
A cluster of pebbles / stones obstacle sprite for a 2D mobile merge eco garden game.
Two or three smooth rounded stones, stacked or clustered.
Warm gray-brown tones (#7A7060 main, #948B80 highlight, #5C5648 shadow).
Mossy patch on one stone (optional, very subtle).
Soft watercolor-flat style. Transparent background. PNG, 128x128 px.
Style: cute casual mobile obstacle, nature/eco, earthy and calm.
```

---

## NHÓM 7 — Characters

> Tương ứng mục **2.7** trong hướng dẫn.
> Lưu tại: `Assets/EcoGarden/Art/Characters/`
> **Kích thước chuẩn:** 128 × 128 px (Character) / 96 × 96 px (Butterfly)
> Transparent background

---

### 7.1 `char_customer_01.png` — NPC Customer

```
A cute NPC customer character sprite for a 2D mobile eco garden game.
Chibi-style, roughly 2-head-tall proportions. Gender-neutral, friendly expression.
Wearing a soft purple tunic or apron (#A361BC) with light skin tone (#F9C89E).
Small boots, round face with dot eyes and a tiny smile.
Posed in a neutral standing position, slightly facing the viewer.
Soft watercolor-flat style. Transparent background. PNG, 128x128 px.
Style: cute casual mobile character, eco/garden theme, warm and friendly.
```

---

### 7.2 `char_butterfly.png` — Butterfly

```
A decorative butterfly character sprite for a 2D mobile eco garden game.
Small and dainty, wings spread open. Top wings: warm orange-yellow (#FFB84D).
Bottom wings: soft pink-coral (#EB7AC4). Wing pattern: simple large oval spots.
Thin dark body in the center (#332740).
Soft watercolor-flat style. Transparent background. PNG, 96x96 px.
Style: cute casual mobile ambient character, nature/eco, light and cheerful.
```

---

## NHÓM 8 — Producers

> Tương ứng mục **2.8** trong hướng dẫn.
> Lưu tại: `Assets/EcoGarden/Art/Producers/`
> **Kích thước:** 128 × 128 px, transparent background

---

### 8.1 `producer_lotus_seed_01.png` — Lotus Seed Producer

```
A lotus seed producer / dispenser building sprite for a 2D mobile merge eco garden game.
Compact, friendly machine or pond fixture that produces lotus seeds.
Rounded barrel or bowl shape, soft teal-blue tones (#4D9EC7 body, #95D3DF highlight).
Gold band or ring accent (#BF8F50). Small lotus seed or bud visible on top or inside.
Soft watercolor-flat style. Transparent background. PNG, 128x128 px.
Style: cute casual mobile building/producer, nature/eco, charming and functional.
```

---

## NHÓM 9 — Icon Sprites cho Buttons

> Tương ứng mục **Bước 5** trong hướng dẫn (thay text label bằng icon sprite).
> Lưu tại: `Assets/EcoGarden/Art/UI/Icons/`
> **Kích thước chuẩn:** 64 × 64 px, transparent background
> Phong cách: **flat line icon, white / light color on transparent, 3–4 px stroke weight**

| Tên file | Nội dung icon |
|---|---|
| `icon_pause.png` | Hai đường thẳng đứng song song (pause symbol) |
| `icon_level.png` | Ngôi sao hoặc bản đồ cuộn nhỏ với chữ "Lv" |
| `icon_mission.png` | Clipboard nhỏ hoặc checklist với dấu tích |
| `icon_shop.png` | Túi mua sắm nhỏ hoặc giỏ hàng |
| `icon_close.png` | Dấu X bo tròn |
| `icon_restart.png` | Mũi tên xoay vòng (circular arrow) |
| `icon_next.png` | Mũi tên sang phải trong vòng tròn |
| `icon_gold.png` | Đồng xu vàng với vân sáng |
| `icon_gem.png` | Viên đá quý hình kim cương, xanh tím |
| `icon_shovel.png` | Cái xẻng nhỏ, cán gỗ, lưỡi kim loại |
| `icon_magic_wand.png` | Đũa phép với ngôi sao ở đầu, tia sáng tỏa ra |
| `icon_sorting_magnet.png` | Nam châm hình chữ U với tia từ trường |

**Prompt chung cho tất cả icon:**

```
A set of flat UI icons for a 2D mobile casual eco garden game.
Each icon: white silhouette on transparent background, 3-4px stroke weight, rounded corners/caps.
Soft and friendly style, not overly geometric. Clean and readable at small sizes (64x64 px).
Icons: pause (two bars), level map scroll, mission clipboard, shop bag, close X, restart arrow,
next arrow, gold coin, gem diamond, shovel, magic wand with star, horseshoe magnet.
PNG, 64x64 px each. Transparent background. No color fill, white line art only.
Style: mobile UI icon set, flat line, eco/nature game, friendly and clear.
```

> Nếu muốn icon có màu riêng từng loại, thay `white silhouette` thành màu cụ thể,
> ví dụ: `warm gold (#F5C756) coin icon` cho `icon_gold.png`.

---

## Ghi Chú Chung Khi Generate

| Yêu cầu | Chi tiết |
|---|---|
| **Background** | Luôn yêu cầu `transparent background` hoặc `PNG with alpha channel` |
| **Kích thước** | Ghi rõ px trong prompt; nếu tool không hỗ trợ custom size thì export ở size lớn nhất rồi resize |
| **9-slice** | Các panel/button cần viền đều 4 phía để Unity 9-slice hoạt động đúng |
| **Phong cách nhất quán** | Paste thêm câu này vào đầu mọi prompt: *"Consistent with a soft watercolor-flat mobile casual 2D eco garden game art style, teal-green nature palette"* |
| **Màu tham chiếu** | Bảng màu gốc từ `PlaceholderSpriteFactory.cs` được giữ làm tham chiếu trong từng prompt |
| **Tên file** | Đặt tên file đúng như cột "Tên file" để khớp với đường dẫn `Resources.Load` trong hướng dẫn |
