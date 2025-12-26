# Deploy em www.budget.avila.inc — Roncav Budget

Este documento reúne os passos operacionais para colocar o front-end e as integrações em produção em `https://www.budget.avila.inc`, garantindo sincronização segura com `api.avila.inc`, coordenação com o portal do usuário (`portal.avila.inc`) e visibilidade no portal da equipe (`avila.inc`).

---

## 1. Objetivo

- Publicar a landing page/console comercial em `www.budget.avila.inc`.
- Garantir que o MAUI/WinUI App consuma `api.avila.inc` para autenticação, sincronização e analytics.
- Expor links estratégicos para `portal.avila.inc` (upgrade, assinaturas, relatórios avançados).
- Permitir que o portal da equipe (`avila.inc`) monitore o produto (links, dashboards de status).

---

## 2. Pré-requisitos

| Item | Descrição |
| --- | --- |
| Domínio | `www.budget.avila.inc` já registrado no DNS da Avila Ops
| Certificado TLS | Gerado automaticamente pelo provedor (Azure Static Web App ou App Service), ou importado via Azure Key Vault
| Repositório GitHub | `avilaops/Orcamento` com branch `main`
| Ambiente Azure | Subscription com Resource Group (`rg-Orcamento`) e permissões para:
- Static Web Apps (para landing page)
- App Service (caso precise hospedar API complementar)
- Front Door ou CDN (opcional)

---

## 3. Estrutura de deploy

1. **Front-end comercial** (Next.js + Tailwind / SSG)
   - `npm install` e `npm run build` gera `/out`.
   - Deploy em **Azure Static Web App** com contraint: `app_location: '.'`, `output_location: 'out'`.
   - Permitir configuração de `customDomain: www.budget.avila.inc`.

2. **App Shell (MAUI)**
   - Continua distribuído via lojas (Microsoft Store, Google Play, App Store) e downloads diretos.
   - O landing page direciona o usuário a baixar o aplicativo.

3. **CI/CD (GitHub Actions)**
   - Workflow `deploy-landing.yml` dispara ao publicar no `main`. Define: build -> deploy Static Web App.
   - Secretos:
     - `AZURE_STATIC_WEB_APPS_API_TOKEN`
     - `AVILA_API_BASE_URL=https://api.avila.inc/v1/
     - `PORTAL_URL=https://portal.avila.inc/`
     - `AVILA_PULSE_URL=https://avila.inc/`
   - Adicional: pipeline para gerar releases MAUI (Windows/Android/iOS) e publicar artifacts.

---

## 4. DNS e domínio personalizado

1. No DNS da Avila:
   - Crie um registro **CNAME** `www` apontando para o domínio gerado pelo Static Web App (ex: `budget-app.azurestaticapps.net`).
   - Configure um registro **ALIAS** ou **A** se usar App Service + Front Door.

2. No portal do recurso Azure:
   - Vincule `www.budget.avila.inc` ao Static Web App.
   - Valide o certificado TLS automático (Let's Encrypt). Atualize se usar certificado próprio.

3. Opcional: configure **Azure Front Door** com `www.budget.avila.inc` para cache e WAF.

---

## 5. Integração com `api.avila.inc`

1. **Configurações de ambiente** do front-end e dos artefatos MAUI:
```env
AVILA_API_BASE_URL=https://api.avila.inc/v1/
AVILA_CLIENT_ID=Orcamento
AVILA_CLIENT_SECRET=${{ secrets.AVILA_CLIENT_SECRET }}
```

2. **Headers obrigatórios**:
   - `Authorization: Bearer {access_token}`
   - `X-Client-Platform: web/desktop/mobile`
   - `X-Tracking-Source: budget-landing` (para analytics do backend).

3. **Fluxo de sincronização**:
   - O app envia dados com `SyncService` para `https://api.avila.inc/v1/sync/data`.
   - O payload deve incluir `deviceId`, `version`, `timestamp`, `items`.
   - O API responde com `SyncResult` e `serverVersion`. O app persiste em local storage.

4. **Monitoramento e alertas**:
   - Configure App Insights ou Azure Monitor no backend e nos serviços do API.
   - Habilite `Diagnostic Settings -> Log Analytics` para erros e latência.

---

## 6. Integração com `portal.avila.inc`

- Links principais no topo do landing page e dentro do App:
  - `portal.avila.inc/upgrades/Orcamento` (botão "Subir para Premium").
  - `portal.avila.inc/reports/budget` (relatórios e metas avançadas).
  - `portal.avila.inc/support` (suporte Avila Ops).
- Autenticação compartilhada:
  - Redirecionar a sessão do app para o portal via JWT (padrão SSO) usando `portal` como `aud`.
  - Ao lado do botão `upgrade`, gerar `https://portal.avila.inc/tokens/ssologin?token={ssoToken}`.

- **Webhook de billing**:
  - `portal` indica upgrade. Criar webhook `https://api.avila.inc/v1/webhooks/portal`.
  - Configurar para atualizar `subscriptionStatus` no app e exibir badge.

---

## 7. Integração com `avila.inc` (Portal da Equipe)

- Incluir relatorio de status em `https://avila.inc/pural` (dashboard geral):
  - Status de deploy (landing/MAUI)
  - Última sincronização (timestamp do SyncService)
  - Erros críticos (ex: falhas de login em `api`)
- Configure `avila.inc` com **Embedded iframe** ou link para `https://www.budget.avila.inc/status`.
- Disponibilize o **README operacional** e a `planilha de rollout` nesse portal.

---

## 8. Observabilidade

1. **Logging**
   - `AvilaApiService` já expõe `ILogger`. Configure `Azure Application Insights` para coletar `TrackEvent` e `TrackException`.

2. **Health Check**
   - Crie endpoint `/health` no backend (api) para retornar `200`.
   - Use `Azure Monitor` ou `Uptime Robot` para sondar `https://api.avila.inc/v1/health`.

3. **Alertas**
   - Notificação via Teams/Slack no canal #product quando `sync` falhar 3x consecutivas.
   - Alerta no portal da equipe `avila.inc` com Status Page (ex: `StatusCake`).

---

## 9. Pós-deploy

1. Valide `www.budget.avila.inc` com Lighthouse (PWA, SEO, Performance).
2. Verifique captura de eventos `TrackEvent` (login, upgrade, sync) na dashboard Avila Analytics.
3. Publique release notes no `portal.avila.inc/releases` e no repositório GitHub.
4. Atualize `docs/IMPLEMENTACAO_STATUS.md` com a data do deploy.
5. Monte checklist para a equipe de suporte (inclui `rota fallback` para `api.avila.inc` se o portal cair).

---

## 10. Contato responsável

- **DevOps**: @avilaops-dev
- **Product Manager**: @fernanda.roncav
- **Suporte técnico**: suporte@avila.inc
- **Portal equipe**: https://avila.inc

---

Esse plano garante que `www.budget.avila.inc` seja o frontispício comercial do Roncav Budget, totalmente integrado às APIs e portais do ecossistema Avila.
