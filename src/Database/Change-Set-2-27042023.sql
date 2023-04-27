/*
Author  : Pavarna Thangaraj
Changes :
	1. Added Two columns namely "IsPOSorder" and "POSUserId"
Purpose : 
	a. This "IsPOSorder" cloumn is monitor the orders are POS order else Online shopping Order.If it's POS order this column value is TRUE otherwise this column value is FALSE.
	b. We have POS customer information saved and it is necessary to monitor the individuals responsible for creating the POS user details.So "POSUserId" this column have who is placing the POS order.
Date 	: 27-04-2023
*/

ALTER TABLE "Order"
ADD COLUMN "IsPOSorder" bool,
ADD COLUMN "POSUserId" citext;