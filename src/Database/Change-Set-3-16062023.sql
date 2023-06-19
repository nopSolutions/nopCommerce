/*
Author  : Mounika, Mukesh
Changes :
    1. Added Two columns namely "IsSSOUser" and "IsNopUser" in customer table.
Purpose : 
    a. This "IsSSOUser" cloumn is used to capture the type of the user that he is logged in as a SSO user. If the user is a SSO user then this field will be set as true. 
    b. This "IsNopUser" cloumn is used to capture the type of the user whether he is logged in as a Nop user. If the user is not an NOP user then this field will be set as false. 
Date     : 16-06-2023
*/

 

ALTER TABLE "Customer"
ADD COLUMN "IsSSOUser" bool DEFAULT false,
ADD COLUMN "IsNopUser" bool DEFAULT true;