import os


SUPPORTED_MODES = {"normal", "slow", "unavailable", "contradictory"}
EXPECTED_EVENT_TYPE = "commerce.order.placed.v1"
DEFAULT_SLOW_DELAY_SECONDS = 3.0


def get_configured_mode() -> str:
    mode = os.getenv("WMS_MODE", "normal").strip().lower()
    if mode not in SUPPORTED_MODES:
        supported = ", ".join(sorted(SUPPORTED_MODES))
        raise RuntimeError(
            f"Unsupported WMS_MODE '{mode}'. Supported modes: {supported}."
        )

    return mode


def get_slow_delay_seconds() -> float:
    raw_value = os.getenv("WMS_SLOW_DELAY_SECONDS", str(DEFAULT_SLOW_DELAY_SECONDS))

    try:
        delay = float(raw_value)
    except ValueError as exception:
        raise RuntimeError("WMS_SLOW_DELAY_SECONDS must be a number.") from exception

    if delay < 0:
        raise RuntimeError("WMS_SLOW_DELAY_SECONDS must be zero or greater.")

    return delay
