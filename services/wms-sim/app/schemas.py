from datetime import datetime
from typing import Literal
from uuid import UUID

from pydantic import BaseModel, Field, field_validator

from app.config import EXPECTED_EVENT_TYPE


class HealthResponse(BaseModel):
    status: Literal["healthy"]
    service: Literal["wms-sim"]
    mode: str


class ModeResponse(BaseModel):
    mode: str
    supportedModes: list[str]


class FulfillmentItem(BaseModel):
    orderItemId: int
    productId: int
    sku: str = Field(min_length=1)
    quantity: int = Field(gt=0)
    warehouseId: int


class FulfillmentRequest(BaseModel):
    messageId: UUID
    correlationId: UUID
    eventType: str
    occurredOnUtc: datetime
    orderGuid: UUID
    orderId: int
    storeId: int
    items: list[FulfillmentItem] = Field(min_length=1)

    @field_validator("eventType")
    @classmethod
    def event_type_must_match_contract(cls, value: str) -> str:
        if value != EXPECTED_EVENT_TYPE:
            raise ValueError(f"eventType must be '{EXPECTED_EVENT_TYPE}'")

        return value


class FulfillmentAcceptedResponse(BaseModel):
    externalRequestId: str
    status: Literal["Accepted"]
    orderGuid: UUID
    messageId: UUID
