---
name: stop-local-nopcommerce
description: Stops and removes the local nopCommerce Docker container started by start-local-nopcommerce. Use when the user asks to stop, shut down, bring down, tear down, or clean up the local nopCommerce server.
---

# Stop Local nopCommerce

## Purpose

Stop the `nopcommerce-local` container and remove it so port `8080` is free.

Does not remove the `nopcommerce-local-amd64` image unless the user explicitly asks to delete images.

## Workflow

1. Check whether the container exists:

```bash
docker ps -a --filter name=^/nopcommerce-local$ --format '{{.Names}} {{.Status}}'
```

If nothing is returned, report that the server is not running and stop.

2. Stop the container if it is running:

```bash
docker stop nopcommerce-local
```

3. Remove the container:

```bash
docker rm nopcommerce-local
```

4. Confirm cleanup:

```bash
docker ps -a --filter name=^/nopcommerce-local$ --format '{{.Names}}'
```

Expected: no output.

Optionally verify port `8080` is no longer in use:

```bash
curl -I --max-time 3 http://localhost:8080 || true
```

Expected: connection refused or timeout.

## Notes

- Pair with [start-local-nopcommerce](../start-local-nopcommerce/SKILL.md) for bring-up.
- To also delete the built image:

```bash
docker rmi nopcommerce-local-amd64
```

Only run that when the user asks to remove images or reclaim disk space.
