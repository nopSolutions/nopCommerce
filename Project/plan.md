# Plano: Evolução Omnicanal do nopCommerce

## Part 1 implementada
- Entrega principal: [docs/part1/architecture-checkpoint.md](docs/part1/architecture-checkpoint.md)
- Diagramas: [docs/part1/diagrams.md](docs/part1/diagrams.md)
- Guião da apresentação de 7 minutos: [docs/part1/presentation-script.md](docs/part1/presentation-script.md)
- ADRs: [docs/adr/](docs/adr/)
- Spike de viabilidade: [docs/evidence/feasibility-spike.md](docs/evidence/feasibility-spike.md)

## Resumo e Compromisso
- Cenário escolhido: **C — Omnichannel Commerce Core**, com base nos PDFs [Final Assignment](</home/varela/Desktop/AS/Project/Group Assignment - Final Assignment.pdf>) e [Assignment 2 Slides](</home/varela/Desktop/AS/Project/Assignment 2 — Architectural Evolution of nopCommerce.pdf>).
- Compromisso: demonstrar o nopCommerce como commerce core que aceita encomendas, integra com operações externas, continua útil quando um sistema externo falha, e recupera com rastreabilidade.
- Part 1, em **05/06 de maio de 2026**: apresentação de 7 minutos com cenário, análise atual, bounded contexts, QA scenarios, ADD, arquitetura alvo, ADRs, riscos e spike.
- Final, em **02/03 de junho de 2026**: repositório executável, relatório curto, ADRs, evidência e demo live com degradação e recuperação.

## Requisitos e Escopo
- Casos obrigatórios:
  - Compra online no nopCommerce com fulfillment por outro canal, via WMS/warehouse simulator.
  - Visibilidade cross-channel de stock ou estado de encomenda, via POS/store simulator.
  - Degradação: WMS lento/indisponível/contraditório sem bloquear checkout nem derrubar a experiência principal.
- Quality attribute scenarios:
  - Resiliência: checkout continua quando WMS está indisponível; encomenda fica “fulfillment pending”.
  - Consistência eventual: updates duplicados/stale são ignorados ou marcados para reconciliação.
  - Rastreabilidade: cada encomenda liga OrderGuid, outbox message, worker log, chamada WMS e nota/status no nopCommerce.
  - Operabilidade: demo mostra fila, retries, DLQ e modo degradado.
  - Performance: integração externa não é síncrona no checkout.
- In scope:
  - Plugin nopCommerce para integração omnicanal.
  - Workflow assíncrono com RabbitMQ.
  - Outbox/inbox, idempotência, retry, circuit breaker e DLQ.
  - Um serviço independente `.NET Worker`.
  - Dois simuladores: WMS e POS.
  - Diagramas C4, ADRs, relatório, setup e evidence pack.
- Out of scope:
  - Reescrever nopCommerce ou extrair Order/Catalog como microserviços.
  - ERP/POS/WMS reais completos.
  - Keycloak, SSO, pagamentos reais, shipping carrier real.
  - Garantias de exactly-once distribuído; será at-least-once com idempotência.

## Frameworks e Arquitetura
- Framework arquitetural obrigatório: **ADD**, porque o trabalho é guiado por atributos de qualidade e decisões arquiteturais. Referência: [SEI ADD](https://www.sei.cmu.edu/library/attribute-driven-design-method-collection/).
- Documentação visual: **C4 Model** para context/container/component/dynamic/deployment views. Referência: [C4 official site](https://c4model.com/).
- Documentação de decisão: ADRs curtos com alternativa rejeitada em cada decisão.
- Stack técnica:
  - nopCommerce existente: ASP.NET Core/.NET 10, modular monolith, plugins, in-process events, scheduled tasks.
  - Plugin `Nop.Plugin.Misc.OmnichannelCore` com tabelas próprias via migrations.
  - RabbitMQ + `RabbitMQ.Client`, publisher confirms, manual acknowledgements e DLQ.
  - `.NET Worker Service` para integração externa.
  - Polly para retry/circuit breaker em chamadas HTTP ao WMS simulator.
  - Docker Compose para nopCommerce, SQL Server, RabbitMQ, worker, WMS simulator e POS simulator.
- Documentação técnica de referência:
  - [nopCommerce plugins](https://docs.nopcommerce.com/en/developer/plugins/index.html)
  - [nopCommerce plugin with data access](https://docs.nopcommerce.com/en/developer/plugins/plugin-with-data-access.html)
  - [nopCommerce scheduled tasks](https://docs.nopcommerce.com/en/developer/tutorials/scheduled-tasks.html)
  - [RabbitMQ reliability](https://www.rabbitmq.com/docs/reliability)
  - [Polly retry](https://www.pollydocs.org/strategies/retry)
  - [.NET Worker Services](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers)

## Implementação Planeada
- No nopCommerce:
  - Plugin escuta `OrderPlacedEvent` e grava `OmniOutboxMessage`.
  - Scheduled task publica eventos pendentes para RabbitMQ com publisher confirms.
  - Endpoints internos autenticados por `X-Demo-Token` recebem fulfillment/stock updates do worker.
  - Tabelas: `OmniOutboxMessage`, `OmniInboxMessage`, `OmniOrderFulfillment`, `OmniStockSyncState`.
  - Não alterar enums core de order status; usar projeção do plugin e order notes para estados omnicanal.
- Eventos públicos v1:
  - `commerce.order.placed.v1`: `messageId`, `orderGuid`, `orderId`, `items`, `storeId`, `createdOnUtc`.
  - `pos.stock.changed.v1`: `messageId`, `sku/productId`, `warehouseId`, `quantityOnHand`, `sourceVersion`.
  - `fulfillment.status.changed.v1`: `messageId`, `orderGuid`, `externalRequestId`, `status`, `trackingNumber`, `occurredOnUtc`.
- Serviço independente:
  - Consome `commerce.order.placed.v1`, chama WMS simulator e publica/encaminha estados de fulfillment.
  - Consome `pos.stock.changed.v1` e chama endpoint do plugin para atualizar stock/projeção.
  - Aplica idempotência por `messageId`, retry exponencial, circuit breaker e DLQ.
- Simuladores:
  - WMS: modos `normal`, `slow`, `unavailable`, `contradictory`.
  - POS: endpoint/script para emitir venda/reposição de stock.
- ADRs mínimos:
  - Escolher Cenário C e foco omnicanal.
  - Manter checkout/order/catalog dentro do monólito.
  - Usar outbox + RabbitMQ em vez de chamada síncrona ao WMS.
  - Usar simuladores em vez de ERP/WMS/POS reais.
  - Proibir shared database entre nopCommerce e worker.

## Validação e Entregáveis
- Testes:
  - Unit tests para serialização de eventos, idempotência inbox/outbox e transições de fulfillment.
  - Integration tests com RabbitMQ + simuladores.
  - Demo scripts: normal flow, WMS down, WMS recovery, POS stock update, duplicate/stale event.
- Evidência:
  - Screenshots/logs de encomenda criada, estado pending, retries, DLQ, recuperação e stock atualizado.
  - Medição simples: checkout não bloqueado pelo WMS degradado; comparar fluxo normal vs WMS slow/down.
  - Limitações documentadas: sem exactly-once, autenticação demo-token, simuladores não substituem sistemas reais.
- Artefactos:
  - `docs/architecture-report.md`: cenário, drivers, ADD, arquitetura alvo, evolução e limites.
  - `docs/adr/`: ADRs com alternativas rejeitadas.
  - `docs/evidence/`: resultados, screenshots, logs e instruções de reprodução.
- Assunções:
  - Apresentação e documentação podem ser em português.
  - `dotnet` não está disponível no PATH local neste momento; validação final deve usar Docker ou instalar .NET 10 SDK.
