---
change_id: post-flow-analysis
title: Analiza przepływu danych w procesie składania zamówień
status: preparing
created: 2026-07-14
updated: 2026-07-14
archived_at: null
research: research.md
---

## Notes

Przenalizuj proces skladania zamowien, zwracajac szczegolna uwage na powiazane z nim obszary zdefiniowane w context/map/repo-map.md. Wykorzystaj trzech równoległych sub-agentów:

1. Trace e2e: odtwórz ścieżkę od entry pointu, przez warstwy, do zapisu/odczytu i z powrotem. Daj sekwencję kroków z file:line oraz diagram Mermaid.
2. Luki w testach: które metody i gałęzie na tej ścieżce mają pokrycie, a które nie.
3. Blast radius: co musi zmienić się razem przy zmianie tego przepływu — szew interfejsu, warstwy generowane, model, migracje, testy. Połącz graf statyczny z co-change z historii gita.

Skup się wyłącznie na analizie i opisie stanu obecnego repozytorium.

Raport musi zawierać dwie jawne i krytyczne sekcje: Feature overview, Technical debt.
