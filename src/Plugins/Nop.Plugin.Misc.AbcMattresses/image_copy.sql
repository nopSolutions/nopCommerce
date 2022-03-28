-- collect pictures from ABC
IF OBJECT_ID('tempdb..#abcMattressPictures') IS NOT NULL DROP TABLE #abcMattressPictures;

select hp.Sku, ap.Id as AbcProductId, hp.Id as HawProductId, pb.BinaryData, ROW_NUMBER() OVER (Order By hp.Sku) row_num
into #abcMattressPictures
from Product hp
join NOPCommerce.dbo.Product ap on ap.Sku = hp.Sku
join AbcMattressModel amm on hp.Id = amm.ProductId
join NOPCommerce.dbo.Product_Picture_Mapping ppm on ap.Id = ppm.ProductId
join NOPCommerce.dbo.PictureBinary pb on ppm.PictureId = pb.PictureId

-- copies pictures over (uses row number to link)
INSERT into Picture (MimeType, AltAttribute, IsNew)
select 'image/jpeg', row_num, 0 from #abcMattressPictures amp

-- copy picture binaries
INSERT INTO PictureBinary (PictureId, BinaryData)
SELECT pic.Id, amp.BinaryData from #abcMattressPictures amp
join Picture pic on amp.row_num = pic.AltAttribute

-- copy picture mappings
INSERT INTO Product_Picture_Mapping (ProductId, PictureId, DisplayOrder)
SELECT amp.HawProductId, pic.Id, amp.row_num
from #abcMattressPictures amp
join Picture pic on amp.row_num = pic.AltAttribute




-- deletes mattress pictures (to restart)
delete from Picture
where AltAttribute in (select p.Sku COLLATE SQL_Latin1_General_CP1_CI_AS from Product p join AbcMattressModel amm on p.Id = amm.ProductId)

-- check pictures that are inside temp table
select * from #abcMattressPictures amp
join Picture p on amp.row_num = p.AltAttribute