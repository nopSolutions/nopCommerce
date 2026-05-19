import logging

from app.schemas import FulfillmentAcceptedResponse, FulfillmentRequest


logger = logging.getLogger("wms-sim")


def accept_fulfillment(request: FulfillmentRequest) -> FulfillmentAcceptedResponse:
    external_request_id = f"WMS-REQ-{request.orderId}"
    response = FulfillmentAcceptedResponse(
        externalRequestId=external_request_id,
        status="Accepted",
        orderGuid=request.orderGuid,
        messageId=request.messageId,
    )

    logger.info(
        "fulfillment accepted order_guid=%s message_id=%s external_request_id=%s status=%s",
        request.orderGuid,
        request.messageId,
        external_request_id,
        response.status,
    )

    return response
