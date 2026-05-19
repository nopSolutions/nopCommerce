from fastapi import FastAPI

from app.config import get_configured_mode
from app.logging_config import configure_logging
from app.routes import create_router


def create_app() -> FastAPI:
    configure_logging()

    app = FastAPI(title="WMS Simulator", version="0.1.0")
    app.include_router(create_router(current_mode=get_configured_mode()))

    return app


app = create_app()
