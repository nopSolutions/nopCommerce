-- clears out duplicate images inside of each product
declare @duplicateImages table (pictureBinary varbinary(max), pictureIdToSave int, productId int)

insert into @duplicateImages (pictureBinary, pictureIdToSave, productId)
SELECT BinaryData, MIN(pic.PictureId) as PictureIdToSave, MIN(p.Id) as ProductId
FROM [PictureBinary] pic
JOIN Product_Picture_Mapping ppm ON pic.PictureId = ppm.PictureId 
JOIN Product p ON p.Id = ppm.ProductId and p.Published = 1
GROUP BY BinaryData, ProductId HAVING COUNT(*) > 1

declare @picturesToDelete table (pictureBinary varbinary(max), pictureId int, productId int)
insert into @picturesToDelete (pictureBinary, pictureId, productId)
SELECT BinaryData, pic.PictureId, p.Id as ProductId
FROM [PictureBinary] pic
	JOIN Product_Picture_Mapping ppm ON pic.PictureId = ppm.PictureId 
	JOIN Product p ON p.Id = ppm.ProductId and p.Published = 1
                  and p.Id IN (SELECT productId FROM @duplicateImages)
    JOIN @duplicateImages di ON BinaryData = di.pictureBinary AND di.productId = p.Id
WHERE BinaryData IN (SELECT pictureBinary FROM @duplicateImages)
	AND pic.PictureId NOT IN (SELECT pictureIdToSave FROM @duplicateImages)

DELETE FROM Picture
WHERE Id In (SELECT pictureId FROM @picturesToDelete)