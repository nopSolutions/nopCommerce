---
name: start-local-nopcommerce
description: Build and run this nopCommerce app locally in Docker, then open it in the Cursor browser. Use when the user asks to bring up, run, start, launch, or open the local nopCommerce server or app.
---

# Start Local nopCommerce

## Purpose

Bring up this nopCommerce repo locally and open the app in the Cursor browser.

This repo targets .NET 10. Prefer Docker because the host may not have the .NET SDK installed. On Apple Silicon, build and run as `linux/amd64`; the published app includes `IBM.Data.Db2.dll`, which is not compatible with an ARM64 process.

## Workflow

1. Check for an existing server:

```bash
docker ps -a --filter name=^/nopcommerce-local$ --format '{{.Names}} {{.Status}}'
```

If `nopcommerce-local` is already running, keep it and verify `http://localhost:8080/install`.

2. Build the local image:

```bash
docker build --platform linux/amd64 -t nopcommerce-local-amd64 .
```

3. Remove a stale stopped container if needed:

```bash
docker rm nopcommerce-local
```

4. Start the server in the background:

```bash
docker run --platform linux/amd64 --name nopcommerce-local -p 8080:80 nopcommerce-local-amd64
```

5. Verify it responds:

```bash
curl -I --max-time 15 http://localhost:8080
curl -I --max-time 15 http://localhost:8080/install
```

Expected results:
- `/` returns `302 Found` to `/install` before setup.
- `/install` returns `200 OK`.

6. Open the app in the Cursor browser:

Use the `cursor-ide-browser` MCP server. Before calling MCP tools, read the relevant tool descriptors. Then:
- List tabs with `browser_tabs`.
- Navigate to `http://localhost:8080/install` with `browser_navigate`.
- Take a `browser_snapshot` to confirm the page loaded.

## Notes

- Do not use the default `docker-compose.yml` on Apple Silicon unless the user specifically wants to debug the database stack. Its SQL Server service is commonly x64-only.
- The app container alone is enough to reach the nopCommerce installer.
- To stop the app, use [stop-local-nopcommerce](../stop-local-nopcommerce/SKILL.md).
