# Artefakt 1 — Mapa terytorium (historia gita)

**Zakres:** ostatnie 12 miesięcy (2025-07-10 → 2026-07-10), commity bez merge'y.
**Odfiltrowany szum:** `package-lock.json`, `*.min.js/css`, `*.lock`, dotenvy, `defaultResources.nopres.xml` (masowy plik lokalizacji), `styles.rtl.css` (generowany z LTR).
**Repo:** nopCommerce (ASP.NET Core, monolit warstwowy) — branch `develop`.

---

## 1. Aktywność — gdzie projekt jest realnie dotykany

Podział warstwowy top-level (liczba dotknięć plików):
`src/Presentation` 2912 · `src/Plugins` 1148 · `src/Libraries` 1055 · `src/Tests` 239.
→ **~70% pracy dzieje się w warstwie prezentacji.** Poniżej rozbicie na realne obszary hands-on.

### TOP moduły (po zejściu w głąb)

| # | Moduł | Dotknięcia | Charakter |
|---|-------|-----------:|-----------|
| 1 | `Nop.Web/Areas/Admin` (Views 348, Models 332, Controllers 117, Factories 116) | ~990 | **Panel administracyjny** — najgorętszy obszar repo |
| 2 | `Nop.Web/Views` (public storefront) | 490 | Sklep frontowy |
| 3 | `Nop.Web/wwwroot` (assets/JS/CSS) | 434 | Zasoby statyczne |
| 4 | `Nop.Web/Factories` | 119 | Budowa modeli widoku (wzorzec Factory) |
| 5 | `Nop.Core/Domain` | 110 | Encje domenowe |
| 6 | `Nop.Web.Framework/Migrations` | 105 | Migracje upgrade'owe (UpgradeTo490/500) |
| 7 | `Nop.Data/Migrations` | 95 | Migracje schematu |
| 8 | `Nop.Services/*` | — | Warstwa usług (patrz niżej) |

### TOP pojedyncze pliki (hot files)

| Dotknięcia | Plik | Dlaczego gorący |
|-----------:|------|-----------------|
| 36 | `Nop.Services/Installation/InstallRequiredData.cs` | Seed danych instalacji — dotykany przy KAŻDEJ nowej funkcji |
| 31 | `Nop.Web.Framework/Migrations/UpgradeTo500/LocalizationMigration.cs` | Zasoby lokalizacji dla wersji 5.00 |
| 29 | `Nop.Web/Areas/Admin/Controllers/SettingController.cs` | Centralny kontroler ustawień |
| 28 | `Nop.Web/Areas/Admin/Infrastructure/Mapper/AdminMapperConfiguration.cs` | Konfiguracja mapowania (entity↔model) dla całego Admina |
| 24 | `Nop.Web/Areas/Admin/Factories/SettingModelFactory.cs` | Fabryka modeli ustawień |
| 23 | `Nop.Services/Messages/MessageTokenProvider.cs` | Tokeny szablonów e-mail |
| 18 | `Nop.Web/Areas/Admin/Controllers/ProductController.cs` | Zarządzanie produktami |

### Poddomeny `Nop.Services` (warstwa usług)

`Messages` 71 · `Installation` 63 · `Orders` 54 · `Catalog` 52 · `Common` 36 · `Media` 34 · `Customers` 34 · `ExportImport` 30 · **`ArtificialIntelligence` 26** (nowy obszar) · `Html` 23 · `Security` 21 · `Payments` 18.

### Najaktywniejsze pluginy

`Misc.Forums` 190 · `Misc.RFQ` 134 · `Payments.AmazonPay` 106 · `Misc.News` 91 · `Payments.PayPalCommerce` 67 · `Misc.Polls` 64 · `Tax.Avalara` 39.
→ RFQ (Request for Quote) i Forums to najbardziej rozwijane wtyczki.

---

## 2. Nacisk pracy w czasie (kwartały)

| Kwartał | Commity | Dominujący obszar | Interpretacja |
|---------|--------:|-------------------|---------------|
| Q3 2025 | 133 | Areas/Admin + Views | Intensywna praca na UI (admin + storefront) |
| Q4 2025 | 106 | **wwwroot** (396!) + migracje | Duża przebudowa assetów/JS + migracje danych; wzrost pluginów (RFQ, News) |
| Q1 2026 | 75 | Areas + **Forums/Public** + Components | Rozwój forum publicznego, komponenty widoku |
| Q2 2026 | 81 | Areas + **Tests** (Services 71, Web 24) | Wyraźny nacisk na **testy** + Domain + migracje |
| Q3 2026 (do 07-10) | 8 | `Nop.Core/Infrastructure` | Ostatnio: refactor infrastruktury (spójne z commitem "Mapster zamiast AutoMapper") |

**Trend:** malejąca liczba commitów (133→106→75→81) przy przesunięciu od czystego UI (Q3'25) przez assety (Q4'25) ku testom i stabilizacji domeny (Q2'26). Najświeższa aktywność to zmiana infrastruktury mapowania (AutoMapper → Mapster, widoczna w historii merge'y).

---

## 3. Współzmiany — co zmienia się razem

### Najsilniejsze sprzężenia katalogów (wspólne commity)

| Wystąpień | Para katalogów | Wniosek |
|----------:|----------------|---------|
| 44 | `Web.Framework/Migrations` ↔ `Nop.Web/Areas` | Nowa funkcja w Adminie → migracja lokalizacji/ustawień |
| 36 | `Nop.Core/Domain` ↔ `Nop.Web/Areas` | Nowa encja → od razu ekran w Adminie |
| 35 | `Services/Installation` ↔ `Nop.Web/Areas` | Nowa funkcja → seed danych instalacyjnych |
| 32 | trójkąt: `Core/Domain` ↔ `Services/Installation` ↔ `Web.Framework/Migrations` | **Pełny łańcuch dodania funkcji** |
| 30 | `Areas` ↔ `Factories` | Kontroler + fabryka modelu (wzorzec) |
| 26 | `Factories` ↔ `Models` | Fabryka buduje swój model |

**Wniosek dla TOP 3:** dodanie jakiejkolwiek funkcji w nopCommerce pociąga za sobą przewidywalny łańcuch:
`Domain (encja)` → `Data/Migrations (schemat)` → `Services/Installation (seed)` → `Web.Framework/Migrations (lokalizacja/ustawienia)` → `Areas/Admin (Controller + Factory + Model + View)`. To jest "koszt wejścia" każdej zmiany funkcjonalnej.

### Wspólny mianownik całego repo

- **`Nop.Services/Installation/InstallRequiredData.cs`** — jeden plik dotykany razem z największą liczbą różnych obszarów (breadth 236, 36 commitów). To centralny seed instalacji: każda nowa funkcja rejestruje tu swoje domyślne dane/uprawnienia/ustawienia. Kandydat na wąskie gardło i źródło konfliktów merge.
- **`SettingController.cs`** i **`MessageTokenProvider.cs`** — cross-cutting w warstwie Admin i wiadomości.
- **`defaultResources.nopres.xml`** (odfiltrowany z rankingu, ale wart odnotowania) — masowy plik lokalizacji dotykany przy 50 commitach; wspólny mianownik dla całego UI.
- **Uwaga o pluginach:** `plugin.json` wielu wtyczek wykazuje wysoki "breadth", ale to artefakt **masowych commitów bumpujących wersję** przy release — nie realne sprzężenie logiczne. Odnotowane, by nie mylić z rzeczywistą kohezją.

### Weryfikacja istnienia (historia ≠ stan obecny)

Wszystkie kluczowe silnie sprzężone pliki **nadal istnieją** w repo — analiza nie opiera się na usuniętych/przeniesionych plikach:

- ✅ `Nop.Services/Installation/InstallRequiredData.cs`
- ✅ `Web.Framework/Migrations/UpgradeTo500/LocalizationMigration.cs`
- ✅ `Nop.Web/Areas/Admin/Infrastructure/Mapper/AdminMapperConfiguration.cs`
- ✅ `Nop.Services/Messages/MessageTokenProvider.cs`
- ✅ `Nop.Web/App_Data/Localization/defaultResources.nopres.xml`

---

## Podsumowanie terytorium (wejście do artefaktu 4)

- **Serce projektu:** `Nop.Web/Areas/Admin` — tu toczy się większość pracy hands-on.
- **Gorące punkty (hot files):** `InstallRequiredData.cs`, `SettingController.cs`, `AdminMapperConfiguration.cs`, `MessageTokenProvider.cs`.
- **Frozen / stabilne:** rdzeń `Nop.Core` (poza `Domain` i świeżym `Infrastructure`), większość `Nop.Data` poza migracjami.
- **Łańcuch zmiany funkcjonalnej:** Domain → Data/Migrations → Services/Installation → Framework/Migrations → Areas/Admin.
- **Wspólny mianownik:** `InstallRequiredData.cs` + plik lokalizacji `defaultResources.nopres.xml`.
- **Nowe obszary:** `Nop.Services/ArtificialIntelligence`, pluginy `Misc.RFQ` i `Misc.Forums`.

### Unknowns (do potwierdzenia w artefaktach 2–3)
- Czy `InstallRequiredData.cs` jest sprzężeniem *technicznym* (jeden plik-rejestr) czy *logicznym* — artefakt 2 (zależności).
- Realny graf zależności między `Areas/Admin`, `Factories` a `Services` — artefakt 2.
- Kto jest właścicielem wiedzy o gorących obszarach (Admin, Installation, Messages) — artefakt 3.
- Skala i kierunek nowego obszaru `ArtificialIntelligence` — poza oknem 12 mies.?
