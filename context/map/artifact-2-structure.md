# Artefakt 2 — Mapa strukturalna (zależności, entry pointy, cykle, lokalne centra)

**Narzędzie:** `dotnet-deptree` 0.1.4 (Python, parsuje `.csproj`) + Graphviz 15.1.0.
**Metoda:** graf budowany z `<ProjectReference>` i `<PackageReference>` na poziomie projektów/warstw. Wejście z `artifact-1-territory.md` (najgorętszy obszar = `Nop.Web/Areas/Admin`).
**Dowody (pliki w `context/map/`):**
- `deptree-core-projects.svg` / `.dot` — graf 5 rdzeniowych projektów (bez pakietów)
- `deptree-core-with-packages.svg` / `.dot` — rdzeń + 178 węzłów NuGet

> **Uwagi o narzędziu (dla powtarzalności):**
> - `.csproj` w repo mają **BOM** + znaki UTF-8 → parser padał. Obejście: nieinwazyjny mirror `.csproj` w `/tmp` ze zdjętym BOM + `PYTHONUTF8=1`. Repo nietknięte.
> - Nawet `--format dot` wymaga binarki Graphviz (`unflatten`) — zainstalowana przez winget.
> - **Pluginów `dotnet-deptree` nie zwizualizuje**: ich `ProjectReference` używają zmiennej MSBuild `$(SolutionDir)`, której narzędzie nie rozwija. Fan-in pluginów policzony osobno (grep po `.csproj`).

---

## Najważniejsze obserwacje (TL;DR)

1. **Warstwy są czyste i liniowe — zero cykli na poziomie projektów.** Łańcuch: `Nop.Core → Nop.Data → Nop.Services → Nop.Web.Framework → Nop.Web`. To rzadkość w legacy i dobra wiadomość: zmiany propagują się w jednym kierunku.
2. **Punkt rozszerzeń = warstwa prezentacji, nie usług.** 64 implementacje `BasePlugin`; z 32 projektów pluginów **17 referuje `Nop.Web`, 15 `Nop.Web.Framework`, a `Nop.Services` — 0**. Pluginy wpinają się od góry.
3. **Lokalne centra (hub) to `Nop.Web` (fan-in 18) i `Nop.Web.Framework` (16)** — nie `Nop.Core`. Zmiana kontraktu w tych dwóch projektach dotyka niemal całego ekosystemu pluginów.
4. **Przeciek warstwowy w fundamencie:** `Nop.Core` (warstwa najniższa) zależy od pakietów **ASP.NET MVC** (`Mvc.NewtonsoftJson`, `Mvc.Razor.RuntimeCompilation`) oraz od infrastruktury cache (Redis/SqlServer). Fundament wie o webie i o konkretnych backendach cache.
5. **`Nop.Services` to najcięższa warstwa do testowania w izolacji** — 10 bezpośrednich zależności, w tym natywne (SkiaSharp, HarfBuzzSharp, Svg.Skia, PdfRpt.Core, elFinder.NetCore, MaxMind.GeoIP2).

---

## Graf zależności — warstwy rdzenia

```
Nop.Core  ──►  Nop.Data  ──►  Nop.Services  ──►  Nop.Web.Framework  ──►  Nop.Web
(fundament)   (dostęp do    (logika biznesowa) (SDK web/plugin/MVC)   (host aplikacji
              danych, EF-                                              + Admin + storefront)
              free, Fluent-
              Migrator)
```
*(pełny render: `deptree-core-projects.svg`)*

**Entry pointy:**
| Entry point | Rola |
|-------------|------|
| `Nop.Web/Program.cs` | Główny start aplikacji (host ASP.NET Core) |
| `Nop.Web/Infrastructure/NopStartup.cs` | Kompozycja startu na poziomie hosta |
| `Nop.Web.Framework/Infrastructure/Nop*Startup.cs` (Mvc, Routing, Authentication, Authorization, ErrorHandler, Proxy, Common) | Modularne startupy — każdy rejestruje swój wycinek pipeline'u przez `INopStartup` |
| 64× `BasePlugin`/`IMiscPlugin`/`IWidgetPlugin`/`IPaymentMethod` w `src/Plugins` | Entry pointy pluginów (ładowane dynamicznie) |

---

## Cykle w aktywnych obszarach

| Obszar (z artefaktu 1) | Co znalazłem | Dowód z dotnet-deptree | Dlaczego ważne przy zmianie | Co sprawdzić dalej |
|---|---|---|---|---|
| Rdzeń (Core↔Data↔Services↔Framework↔Web) | **Brak cykli.** Graf jest ścisłym DAG-iem liniowym | `deptree-core-projects.dot`: dokładnie 4 krawędzie, brak krawędzi zwrotnych | Bezpieczna, przewidywalna propagacja zmian w górę warstw | Cykle na poziomie klas/namespace (poza zasięgiem deptree) — np. Factories↔Services |
| `Areas/Admin` (hot) | Nie tworzy osobnego projektu — żyje wewnątrz `Nop.Web`, więc dziedziczy jego zależności | Brak węzła w grafie; `Admin` = katalog w `Nop.Web` | Cała złożoność Admina spina się w jednym projekcie-hubie `Nop.Web` | Sprzężenia Controller→Factory→Service wewnątrz `Nop.Web` (artefakt 1 pokazał je jako współzmiany) |
| Pluginy | Brak cykli z rdzeniem — zależność jednokierunkowa Plugin→(Web/Framework) | grep: 0 pluginów referuje wstecz do niższych warstw poza prezentacją | Pluginy nie mogą "zatruć" rdzenia cyklem | Czy pluginy nie sięgają do siebie nawzajem (plugin→plugin) |

> **Ważne dla legacy:** brak cykli na poziomie projektów oznacza, że ryzyko strukturalne nie leży w splątaniu warstw, lecz w **koncentracji** (huby `Nop.Web`/`Framework`) i w **przecieku fundamentu** (Core → MVC).

---

## Granice warstw

| Sprawdzana granica | Wynik | Dowód z dotnet-deptree | Dlaczego ważne | Związek z artefaktem 1 |
|---|---|---|---|---|
| `Nop.Core` jako czysty fundament | ⚠️ **Naruszona** | `deptree-core-with-packages.dot`: `Mvc.NewtonsoftJson → Nop.Core`, `Mvc.Razor.RuntimeCompilation → Nop.Core`, `Caching.StackExchangeRedis/SqlServer → Nop.Core` | Fundament zna web i konkretne backendy cache → trudniej użyć Core w oderwaniu od ASP.NET | `Nop.Core/Domain` był gorący (110) — zmiany domeny ciągną cały stos |
| `Nop.Services` nad `Nop.Data` | ✅ Respektowana | Krawędź `Nop.Data → Nop.Services`, brak zwrotnej | Logika biznesowa nie sięga niżej niż dane | Łańcuch zmiany z artefaktu 1 (Domain→Migrations→Installation) zgodny z kierunkiem |
| Pluginy ⟶ tylko prezentacja | ✅ Respektowana | 0/32 pluginów referuje `Nop.Services`/`Core`/`Data` wprost | Przewidywalny kontrakt rozszerzeń; zmiana w `Services` nie łamie pluginów wprost, o ile stabilne API `Web.Framework` | Pluginy `Misc.RFQ`, `Misc.Forums` (najgorętsze) w pełni oparte o `Nop.Web` |
| `Nop.Data` a dostawcy DB | ⚠️ Wielodostawcowość | `FluentMigrator.Runner.{SqlServer,MySql,Postgres} → Nop.Data`, `Microsoft.Data.SqlClient → Nop.Data` | Warstwa danych zna 3 silniki DB naraz — zmiana schematu wymaga myślenia o wszystkich | `Nop.Data/Migrations` gorące (95) |

---

## Ryzyka testowalności

### Podsumowanie
Testowalność spada wraz z warstwą. `Nop.Core`/`Nop.Data` są względnie izolowalne; `Nop.Services` ciągnie natywne biblioteki graficzne/PDF; `Nop.Web` (z Adminem) to najgorętszy i najbardziej spleciony obszar — naturalnie domena testów integracyjnych/E2E.

### Lista ryzyk testowych
- **`Nop.Services` — dużo mockowania / natywne zależności.** 10 bezpośrednich zależności, w tym SkiaSharp + HarfBuzzSharp + Svg.Skia (grafika), PdfRpt.Core (PDF), elFinder.NetCore (pliki), MaxMind.GeoIP2 (geo). Test jednostkowy logiki biznesowej wymaga izolacji od tych bibliotek → dużo mocków lub fake'ów.
- **`Nop.Web` / `Areas/Admin` — test integracyjny/E2E.** Fan-in 18, host całego pipeline'u, Admin jako najgorętszy obszar (artefakt 1). Zmiany tutaj naturalnie kończą się testem E2E (potwierdza to wzrost testów w Q2 2026 z artefaktu 1: `Nop.Web.Tests` +24).
- **`Nop.Core` skażony webem** — utrudnia "czysty" test rdzenia bez podnoszenia zależności ASP.NET/MVC.
- **`Nop.Data` wielodostawcowa** — testy migracji trzeba potencjalnie mnożyć przez SqlServer/MySql/Postgres.

### Najbardziej podejrzane moduły
1. `Nop.Services/Messages` (gorący 71 + `MessageTokenProvider` cross-cutting) — wysokie sprzężenie logiczne.
2. `Nop.Services/Installation/InstallRequiredData.cs` — wspólny mianownik całego repo (artefakt 1), trudny do testowania punktowo.
3. `Nop.Web/Areas/Admin/*` — Controller+Factory+Model spięte, host-zależne.

### Co sprawdzić dalej
- Cykle i huby na poziomie **klas/namespace** wewnątrz `Nop.Web` i `Nop.Services` (deptree tego nie widzi — kandydat na Roslyn/analizę `using`).
- Czy `InstallRequiredData.cs` to jeden plik-rejestr (sprzężenie techniczne) czy realna logika (do artefaktu 3/eksploracji).
- Mapa właścicieli wiedzy dla hubów `Nop.Web`/`Framework` i `Nop.Services/Messages` → **artefakt 3 (kontrybutorzy)**.

### Opcjonalny kolejny krok: graf
Wyrenderowano dwa podgrafy odpowiadające na jedno pytanie każdy:
- `deptree-core-projects.svg` — „jak ułożone są warstwy?" (odpowiedź: liniowy DAG).
- `deptree-core-with-packages.svg` — „skąd przeciek fundamentu i ciężar testowy?" (odpowiedź: pakiety MVC na Core, natywne na Services).
Pełny graf pluginów pominięto świadomie — `dotnet-deptree` nie rozwija `$(SolutionDir)`; fan-in policzony metodą grep.

---

## Podsumowanie strukturalne (wejście do artefaktu 4)

- **Kształt:** ściśle warstwowy monolit, **liniowy DAG bez cykli** projektowych.
- **Huby (gdzie zmiana boli najbardziej):** `Nop.Web` (18) i `Nop.Web.Framework` (16) — kontrakt dla 64 pluginów.
- **Długi technologiczny:** fundament `Nop.Core` przecieka do ASP.NET MVC i konkretnych backendów cache; `Nop.Data` zna 3 silniki DB.
- **Testowalność:** rośnie ryzyko od dołu (Core, izolowalny) ku górze (Services natywne deps → Web/Admin → E2E).
- **Zgodność z artefaktem 1:** hot-spoty z historii (`Areas/Admin`, `Services/Messages`, `Installation`) leżą dokładnie w hubie `Nop.Web` i najcięższej testowo warstwie `Nop.Services` — terytorium i struktura wskazują to samo epicentrum.

### Unknowns (do artefaktu 3)
- Kto zna huby `Nop.Web`/`Framework` i `Nop.Services/Messages` (bus factor).
- Czy przeciek `Core → MVC` to świadoma decyzja architektoniczna czy dług.
- Sprzężenia na poziomie klas wewnątrz `Nop.Web` (poza rozdzielczością deptree).
