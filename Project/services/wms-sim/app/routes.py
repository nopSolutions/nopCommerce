from fastapi import APIRouter, status

from app.config import SUPPORTED_MODES
from app.fulfillment import accept_fulfillment
from app.schemas import (
    FulfillmentAcceptedResponse,
    FulfillmentRequest,
    HealthResponse,
    ModeResponse,
)


def create_router(current_mode: str) -> APIRouter:
    router = APIRouter()

    @router.get("/health", response_model=HealthResponse)
    async def health() -> HealthResponse:
        return HealthResponse(status="healthy", service="wms-sim", mode=current_mode)

    @router.get("/mode", response_model=ModeResponse)
    async def mode() -> ModeResponse:
        return ModeResponse(
            mode=current_mode,
            supportedModes=sorted(SUPPORTED_MODES),
        )

    @router.post(
        "/fulfillments",
        response_model=FulfillmentAcceptedResponse,
        status_code=status.HTTP_202_ACCEPTED,
    )
    async def create_fulfillment(
        request: FulfillmentRequest,
    ) -> FulfillmentAcceptedResponse:
        return accept_fulfillment(request)

    return router
