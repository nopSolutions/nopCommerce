# Artefakt 3 — Mapa kontrybutorów (kto wie co i o co pytać)

**Źródło:** historia commitów, ostatnie 12 miesięcy (2025-07-10 → 2026-07-10), bez merge'y.
**Filtr:** boty, automatyzacje i commity agentów (Claude/Codex/Copilot/dependabot/renovate) odfiltrowane.
**Wejście:** `artifact-1-territory.md` (hot-spoty) + `artifact-2-structure.md` (huby, warstwy).

---

## Rdzeń zespołu (12 miesięcy)

| Osoba | Commity | Tożsamość | Rola wnioskowana |
|-------|--------:|-----------|------------------|
| **Sergey Koshelev** | 181 | `sergey.k@nopcommerce.com` | **Lead / architekt** — dominuje niemal każdy obszar |
| **Alexey Anokhin** | 88 | `exile.developer@gmail.com` | Frontend/storefront + domena |
| **DmitriyKulagin** | 84 | `kulagin87@gmail.com` | UI/assety + Admin + walidacje |
| RomanovM (Maxim R.) | 16 | `maxim.r@nopcommerce.com` | Lokalizacja, URL/routing, drobne poprawki rdzenia |

Reszta (Atiqur Rahman Foyshal, Karan Chadha, AndreiMaz, Jan Nielsen, bambuca…) to pojedyncze kontrybucje — **community/okazjonalni**, nie linia wsparcia.

> **Domeny `@nopcommerce.com`** (Sergey, Maxim) wskazują core-team producenta; Alexey i Dmitriy to stali kontrybutorzy z prywatnych adresów (prawdopodobnie też core-team).

---

## Top 5 obszarów wymagających kontaktu (z artefaktów 1–2)

Wybrane jako przecięcie: **gorące** (artefakt 1) × **strukturalnie krytyczne / huby** (artefakt 2).

1. **`Nop.Web/Areas/Admin`** — najgorętszy obszar repo + serce hubu `Nop.Web`.
2. **`Nop.Web.Framework`** — SDK/kontrakt dla 64 pluginów (fan-in 16); zmiana tu łamie ekosystem.
3. **`Nop.Services`** — logika biznesowa, najcięższa testowo; `Messages`/`Installation` cross-cutting.
4. **`Nop.Core/Domain` + `Nop.Data/Migrations`** — fundament + łańcuch zmiany funkcjonalnej; przeciek do MVC.
5. **Storefront (`Nop.Web` public: Controllers/Factories/Views)** + gorące pluginy (`Misc.RFQ`, `Misc.Forums`).

---

## Linia wsparcia — kto pytać o co

### 1. Admin (hub, najgorętszy) — `Nop.Web/Areas/Admin`
| Kontrybutor | Commity | Support |
|---|---:|---|
| **Sergey Koshelev** | 53 | **Pierwszy kontakt.** Prowadzi zmiany w Adminie end-to-end |
| DmitriyKulagin | 28 | UI/assety Admina, walidacje, grid inline-edit |
| Alexey Anokhin | 28 | Ekrany, factories modeli |
> Tematy Sergeya tu: `#7673 AutoMapper→Mapster`, `#8246 label localization`, `#8244 Request-a-Quote toggle`.

### 2. Nop.Web.Framework (SDK pluginów, hub)
| Kontrybutor | Commity | Support |
|---|---:|---|
| **Sergey Koshelev** | 44 | **Właściciel kontraktu.** Startupy, MVC, migracje frameworka |
| Alexey Anokhin | 24 | — |
| DmitriyKulagin | 22 | Walidacja telefonów (`#8093`, libphonenumber) |
> Krytyczny przy każdej zmianie API pluginów — konsultować z Sergeyem.

### 3. Nop.Services (logika biznesowa)
| Kontrybutor | Commity | Support |
|---|---:|---|
| **Sergey Koshelev** | 64 | **Główny.** Także `Nop.Services.Tests` (82 dotknięć — pisze testy do swoich zmian) |
| DmitriyKulagin | 27 | Ceny (`#8098 native price list`), media/obrazy |
| Alexey Anokhin | 16 | — |
> `Messages`/`MessageTokenProvider` i `Installation/InstallRequiredData.cs` (wspólny mianownik z artefaktu 1) — domena Sergeya.

### 4. Nop.Core/Domain + Nop.Data/Migrations (fundament)
| Kontrybutor | Commity | Support |
|---|---:|---|
| **Sergey Koshelev** | 29 (Core) / 25 (Data) | **Główny** dla domeny i migracji |
| DmitriyKulagin | 18 (Core) | Encje domenowe (telefon, ceny) |
| Alexey Anokhin | 14 (Core) / 15 (Data) | Domena + migracje |
> Pytania o przeciek `Core → MVC` (dług z artefaktu 2) i o wielodostawcowość DB → Sergey/Alexey.

### 5. Storefront + gorące pluginy
| Obszar | Lider | Support |
|---|---|---|
| **Storefront** (Controllers/Factories/Views) | **Alexey Anokhin** (37) | **Jedyny obszar, gdzie Alexey wyprzedza Sergeya.** Zna frontend publiczny, 3D produktów (`#4279`), EU Withdrawal (`#8161`) |
| Storefront wsparcie | Sergey (29), Dmitriy (21) | — |
| **Plugin RFQ** (najgorętszy) | **Sergey Koshelev** (25) | Właściciel Request-for-Quote |
| **Plugin Forums** | Alexey (3) / Sergey (4) | Publiczne forum — głównie Alexey |
| Lokalizacja / instalacja (`App_Data`, URL/routing) | **RomanovM** (62 w App_Data) | Pliki językowe (Crowdin), generowanie URL, typos |

---

## Specjalizacje tematyczne (skrót)

- **Sergey Koshelev** → *wszechstronny lead*: Admin, Framework/SDK, Services, migracje, RFQ, testy. Autor dużych refaktorów (Mapster). **Domyślny pierwszy kontakt dla ~każdego obszaru rdzenia.**
- **Alexey Anokhin** → *storefront & domena*: publiczny frontend (Views/Controllers/Factories), Forums, funkcje domenowe (3D, EU withdrawal). Pytać o sklep od strony klienta.
- **DmitriyKulagin** → *UI/assety & wejścia użytkownika*: `wwwroot` (474 — lider), walidacja telefonów/logowanie SMS, ceny, obrazy, WYSIWYG. Pytać o frontend techniczny i pola formularzy.
- **RomanovM** → *lokalizacja & routing*: pliki językowe, URL helpers, drobne poprawki. Pytać o i18n i generowanie linków.

---

## Ryzyka wiedzy (wejście do artefaktu 4)

- ⚠️ **Bus factor = 1 na rdzeniu.** Sergey Koshelev jest #1 w **8 z 9** badanych obszarów (jedyny wyjątek: storefront → Alexey). Utrata tej osoby = utrata wiedzy o Adminie, Framework SDK, Services i migracjach naraz.
- ✅ **Świeżość:** wszyscy trzej główni aktywni w ostatnich 90 dniach (Sergey 36, Alexey 15, Dmitriy 8) — linia wsparcia jest **żywa**, nie historyczna.
- ⚠️ **Cienka redundancja na Framework/SDK** — poza Sergeyem (44) tylko Alexey/Dmitriy z ~20; kontrakt 64 pluginów spoczywa faktycznie na jednej głowie.
- **Community** (Atiqur, Karan, Jan, bambuca…) to kontrybucje jednorazowe — nie licz na nich jako wsparcie ciągłe.

### Unknowns
- Podział ról formalnych (kto jest maintainerem vs commiterem) — historia gita tego nie pokazuje.
- Właścicielstwo pluginów zewnętrznych (Payments.*, Tax.*) poza RFQ/Forums — słabo pokryte przez rdzeń.
- Czy `exile.developer`/`kulagin87` to core-team producenta czy kontraktorzy (adresy prywatne).
