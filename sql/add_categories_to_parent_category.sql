-- Adds categories to a specific parent category
-- Once this is done, the following should be done for each new category
--	1. Add SEO friendly page name (use NOP backend)
--	2. Add photo (match picture IDs from existing categories)

DECLARE @ParentCategoryId INT;
-- Set this manually
SET @ParentCategoryId = 449;

INSERT INTO Category (Name, CategoryTemplateId, ParentCategoryId, PictureId, PageSize, AllowCustomersToSelectPageSize, PageSizeOptions, ShowOnHomepage, IncludeInTopMenu, SubjectToAcl, LimitedToStores, Published, Deleted, DisplayOrder, CreatedOnUtc, UpdatedOnUtc, PriceRangeFiltering, PriceFrom, PriceTo, ManuallyPriceRange)
VALUES
('Laundry', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Refrigeration', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Cooking', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Dishwashers', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Appliance Packages', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Air Conditioning', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Vacuums', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('HDTV', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Video', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('TV Stands', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('TV Mounting', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Furniture', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Mattresses', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Portable Audio', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Smart Home', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Home Audio', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('In-Dash', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0),
('Car Starters/Alarms', 1, @ParentCategoryId, 0, 6, 1, '20, 1000', 0, 1, 0, 0, 1, 0, 0, GetDate(), GetDate(), 1, 0, 10000, 0)