from dataclasses import dataclass
from threading import RLock

from app.config import SUPPORTED_MODES


@dataclass(frozen=True)
class ModeSnapshot:
    mode: str
    slow_delay_seconds: float


class SimulatorState:
    def __init__(self, initial_mode: str, slow_delay_seconds: float) -> None:
        if initial_mode not in SUPPORTED_MODES:
            supported = ", ".join(sorted(SUPPORTED_MODES))
            raise ValueError(
                f"Unsupported WMS mode '{initial_mode}'. Supported modes: {supported}."
            )

        self._mode = initial_mode
        self._slow_delay_seconds = slow_delay_seconds
        self._lock = RLock()

    def snapshot(self) -> ModeSnapshot:
        with self._lock:
            return ModeSnapshot(
                mode=self._mode,
                slow_delay_seconds=self._slow_delay_seconds,
            )

    def set_mode(self, mode: str) -> tuple[str, ModeSnapshot]:
        normalized_mode = mode.strip().lower()
        if normalized_mode not in SUPPORTED_MODES:
            supported = ", ".join(sorted(SUPPORTED_MODES))
            raise ValueError(
                f"Unsupported WMS mode '{mode}'. Supported modes: {supported}."
            )

        with self._lock:
            previous_mode = self._mode
            self._mode = normalized_mode
            return previous_mode, self.snapshot()
