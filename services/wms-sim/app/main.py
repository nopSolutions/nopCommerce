from fastapi import FastAPI

from app.config import get_configured_mode, get_slow_delay_seconds
from app.logging_config import configure_logging
from app.routes import create_router
from app.state import SimulatorState


def create_app() -> FastAPI:
    configure_logging()

    app = FastAPI(title="WMS Simulator", version="0.1.0")
    state = SimulatorState(
        initial_mode=get_configured_mode(),
        slow_delay_seconds=get_slow_delay_seconds(),
    )
    app.include_router(create_router(state))

    return app


app = create_app()
