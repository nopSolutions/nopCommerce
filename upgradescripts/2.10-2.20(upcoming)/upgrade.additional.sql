--additional upgrade scripts from nopCommerce 2.10 to nopCommerce 2.20
--Execure it if your database supports stored procedures and functions. Note that SQL Compact doesn't support them.


--SEO friendly name (products).
CREATE FUNCTION dbo.RemoveSpecialSeoChars (@s varchar(1024)) RETURNS varchar(1024)
   WITH SCHEMABINDING
BEGIN
   IF @s is null
      RETURN null
   DECLARE @s2 varchar(1024)
   SET @s2 = ''
   DECLARE @l int
   SET @l = len(@s)
   DECLARE @p int
   SET @p = 1
   while @p <= @l BEGIN
      DECLARE @c int
      SET @c = ascii(substring(@s, @p, 1))
      IF @c between 48 and 57 or @c between 65 and 90 or @c between 97 and 122 or @c = 45
         SET @s2 = @s2 + char(@c)
      SET @p = @p + 1
      END
   IF len(@s2) = 0
      RETURN null
   RETURN @s2
   END
GO
  
DECLARE cur CURSOR
FOR SELECT 
  Product_Picture_Mapping.PictureId, 
  CASE WHEN Product.SeName = '' OR Product.SeName IS NULL THEN Product.Name ELSE Product.SeName END
FROM
  Product_Picture_Mapping INNER JOIN
  Product ON Product_Picture_Mapping.ProductId = Product.Id

DECLARE @name nvarchar(300);
DECLARE @id int;
OPEN cur

FETCH NEXT FROM cur INTO @id, @name
WHILE (@@fetch_status <> -1)
BEGIN
  IF (@@fetch_status <> -2)
  BEGIN
    DECLARE @seourl nvarchar(300)
    --Removes diacritics
    SET @seourl =
        REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        REPLACE(REPLACE(@name COLLATE Latin1_General_CI_AI
        ,'a','a'),'b','b'),'c','c'),'d','d'),'e','e'),'f', 'f'),'g','g'),'h','h'),'i','i'),'j','j'),'k','k'),'l', 'l')
        ,'m','m'),'n','n'),'o','o'),'p','p'),'q','q'),'r', 'r'),'s','s'),'t','t'),'u','u'),'v','v'),'w','w'),'x', 'x')
        ,'y','y'),'z','z')
    
    SET @seourl = REPLACE(@seourl, ' ', '-');--Replaces spaces with -
    SET @seourl = dbo.RemoveSpecialSeoChars(@seourl);--Removes special chars
    
    
    SET @seourl = REPLACE(@seourl, '-', '_');
    SET @seourl = REPLACE(@seourl, '__', '_');
    SET @seourl = REPLACE(@seourl, '__', '_');--Repeat to remove series like ____
    SET @seourl = REPLACE(@seourl, '__', '_');--Repeat to remove series like ____
    PRINT @seourl
    
    UPDATE dbo.Picture
    SET SeoFilename = @seourl
    WHERE Id = @id
  END
  FETCH NEXT FROM cur INTO @id, @name
END
CLOSE cur
DEALLOCATE cur
GO

DROP FUNCTION dbo.RemoveSpecialSeoChars
GO
