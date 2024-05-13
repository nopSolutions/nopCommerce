-- adds store mapping from one store to another store
declare @SourceStoreId int
declare @DestinationStoreId int

set @SourceStoreId = 7
set @DestinationStoreId = 3

insert into StoreMapping
select EntityId, 'Product', @DestinationStoreId from StoreMapping
where StoreId = @SourceStoreId 
