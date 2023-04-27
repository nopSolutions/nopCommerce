/*
Author  : Mounika Subash
Changes :
	1. Added one column namely "CreatedBy"
Purpose : 
	a. This CreatedBy cloumn store the current login user emailId.
	b. We have POS customer information saved and it is necessary to monitor the individuals responsible for creating the POS user details.
Date 	: 27-04-2023
*/

ALTER TABLE "Customer"
  ADD COLUMN "CreatedBy" citext;