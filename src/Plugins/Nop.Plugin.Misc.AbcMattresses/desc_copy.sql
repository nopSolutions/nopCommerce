-- Add PLP Description
INSERT INTO GenericAttribute (EntityId, KeyGroup, [Key], [Value], StoreId, CreatedOrUpdatedDateUTC)
select hp.Id, 'Product', 'PLPDescription', aga.[Value], 0, GETDATE()
from NOPCommerce.dbo.GenericAttribute aga
join NOPCommerce.dbo.Product ap on ap.Id = aga.EntityId
join Product hp on ap.Sku = hp.Sku
where KeyGroup = 'Product' and [Key] = 'PLPDescription' and EntityId in (select ProductId from NOPCommerce.dbo.AbcMattressModel)

-- Update Short Description
UPDATE 
    Product
SET 
    ShortDescription = ap.ShortDescription,
    FullDescription = ap.FullDescription
FROM 
    Product hp
JOIN NOPCommerce.dbo.Product ap ON ap.Sku = hp.Sku
WHERE hp.Id in (select ProductId from NOPCommerce.dbo.AbcMattressModel)