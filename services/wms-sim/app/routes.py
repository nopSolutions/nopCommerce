import asyncio
import logging

from fastapi import APIRouter, HTTPException, status

from app.config import SUPPORTED_MODES
from app.fulfillment import accept_fulfillment
from app.schemas import (
    FulfillmentAcceptedResponse,
    FulfillmentRequest,
    HealthResponse,
    ModeChangedResponse,
    ModeChangeRequest,
    ModeResponse,
)
from app.state import SimulatorState


logger = logging.getLogger("wms-sim")


def create_router(state_store: SimulatorState) -> APIRouter:
    router = APIRouter()

    @router.get("/health", response_model=HealthResponse)
    async def health() -> HealthResponse:
        snapshot = state_store.snapshot()
        return HealthResponse(status="healthy", service="wms-sim", mode=snapshot.mode)

    @router.get("/mode", response_model=ModeResponse)
    async def mode() -> ModeResponse:
        snapshot = state_store.snapshot()
        return ModeResponse(
            mode=snapshot.mode,
            supportedModes=sorted(SUPPORTED_MODES),
            slowDelaySeconds=snapshot.slow_delay_seconds,
        )

    @router.post("/mode", response_model=ModeChangedResponse)
    async def update_mode(request: ModeChangeRequest) -> ModeChangedResponse:
        return set_mode(request.mode)

    @router.post("/mode/{mode}", response_model=ModeChangedResponse)
    async def update_mode_from_path(mode: str) -> ModeChangedResponse:
        return set_mode(mode)

    @router.post(
        "/fulfillments",
        response_model=FulfillmentAcceptedResponse,
        status_code=status.HTTP_202_ACCEPTED,
    )
    async def create_fulfillment(
        request: FulfillmentRequest,
    ) -> FulfillmentAcceptedResponse:
        snapshot = state_store.snapshot()

        if snapshot.mode == "slow":
            await asyncio.sleep(snapshot.slow_delay_seconds)

        return accept_fulfillment(request, snapshot)

    def set_mode(mode: str) -> ModeChangedResponse:
        try:
            previous_mode, snapshot = state_store.set_mode(mode)
        except ValueError as exception:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=str(exception),
            ) from exception

        logger.info(
            "WMS mode changed previous_mode=%s mode=%s",
            previous_mode,
            snapshot.mode,
        )
        return ModeChangedResponse(
            previousMode=previous_mode,
            mode=snapshot.mode,
            supportedModes=sorted(SUPPORTED_MODES),
            slowDelaySeconds=snapshot.slow_delay_seconds,
        )

    return router
