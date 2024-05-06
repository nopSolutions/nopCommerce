-- insert product category mappings based on a store mapping
declare @StoreId int
declare @CategoryId int

set @StoreId = 7
set @CategoryId = 449

insert into Product_Category_Mapping
select
EntityId, @CategoryId, 0, 0
from StoreMapping
where StoreId = @StoreId
and EntityName = 'Product'