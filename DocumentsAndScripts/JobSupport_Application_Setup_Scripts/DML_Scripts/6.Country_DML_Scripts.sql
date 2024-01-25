

-- use onjobsupport47

  -- SELECT * FROM [Country] WHERE Published=1

Update [Country] SET Published=0 WHERE [TwoLetterIsoCode] NOT IN ('US','GB','AU','CA','IN','NZ','DE','IT','SG','FR','AE')

Update [Country] SET Published=1 WHERE [TwoLetterIsoCode] IN ('US')
Update [Country] SET Published=0 WHERE [TwoLetterIsoCode] IN ('AE')
Update [Country] SET Published=0 WHERE [TwoLetterIsoCode]='IT'
Update [Country] SET Published=0 WHERE [TwoLetterIsoCode]='SG'
Update [Country] SET Published=0 WHERE [TwoLetterIsoCode]='NZ'

Update [Country] SET DisplayOrder=0 WHERE [TwoLetterIsoCode]='IN'
Update [Country] SET DisplayOrder=10 WHERE [TwoLetterIsoCode]='US'
Update [Country] SET DisplayOrder=20 WHERE [TwoLetterIsoCode]='CA'
Update [Country] SET DisplayOrder=30 WHERE [TwoLetterIsoCode]='AU'
Update [Country] SET DisplayOrder=40 WHERE [TwoLetterIsoCode]='FR'
Update [Country] SET DisplayOrder=50 WHERE [TwoLetterIsoCode]='DE'
Update [Country] SET DisplayOrder=60 WHERE [TwoLetterIsoCode]='IT'
Update [Country] SET DisplayOrder=70 WHERE [TwoLetterIsoCode]='SG'
Update [Country] SET DisplayOrder=80 WHERE [TwoLetterIsoCode]='GB'   