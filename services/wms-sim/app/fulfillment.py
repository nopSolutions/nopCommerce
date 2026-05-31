import logging

from fastapi import HTTPException, status

from app.schemas import (
    FulfillmentAcceptedResponse,
    FulfillmentErrorDetail,
    FulfillmentRequest,
)
from app.state import ModeSnapshot


logger = logging.getLogger("wms-sim")


def accept_fulfillment(
    request: FulfillmentRequest,
    snapshot: ModeSnapshot,
) -> FulfillmentAcceptedResponse:
    if snapshot.mode == "unavailable":
        detail = FulfillmentErrorDetail(
            error="wms_unavailable",
            mode=snapshot.mode,
            orderGuid=request.orderGuid,
            messageId=request.messageId,
        ).model_dump(mode="json")

        logger.warning(
            "fulfillment unavailable order_guid=%s message_id=%s mode=%s",
            request.orderGuid,
            request.messageId,
            snapshot.mode,
        )

        raise HTTPException(status_code=status.HTTP_503_SERVICE_UNAVAILABLE, detail=detail)

    if snapshot.mode == "contradictory":
        detail = FulfillmentErrorDetail(
            error="inventory_contradiction",
            mode=snapshot.mode,
            orderGuid=request.orderGuid,
            messageId=request.messageId,
        ).model_dump(mode="json")

        logger.warning(
            "fulfillment contradiction order_guid=%s message_id=%s mode=%s",
            request.orderGuid,
            request.messageId,
            snapshot.mode,
        )

        raise HTTPException(status_code=status.HTTP_409_CONFLICT, detail=detail)

    external_request_id = f"WMS-REQ-{request.orderId}"
    response = FulfillmentAcceptedResponse(
        externalRequestId=external_request_id,
        status="Accepted",
        orderGuid=request.orderGuid,
        messageId=request.messageId,
    )

    logger.info(
        "fulfillment accepted order_guid=%s message_id=%s external_request_id=%s status=%s mode=%s",
        request.orderGuid,
        request.messageId,
        external_request_id,
        response.status,
        snapshot.mode,
    )

    return response
