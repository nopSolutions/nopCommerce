import os


SUPPORTED_MODES = {"normal"}
EXPECTED_EVENT_TYPE = "commerce.order.placed.v1"


def get_configured_mode() -> str:
    mode = os.getenv("WMS_MODE", "normal").strip().lower()
    if mode not in SUPPORTED_MODES:
        supported = ", ".join(sorted(SUPPORTED_MODES))
        raise RuntimeError(
            f"Unsupported WMS_MODE '{mode}'. This scaffold supports only: {supported}."
        )

    return mode
