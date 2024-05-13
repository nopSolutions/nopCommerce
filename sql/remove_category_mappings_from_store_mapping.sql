-- deletes all category mappings from a product with a specific store mapping
delete pcm
from Product_Category_Mapping pcm
join StoreMapping sm on pcm.ProductId = sm.EntityId
WHERE sm.EntityName = 'Product' AND sm.StoreId = 7 AND CategoryId != 449