Version specific steps:

1. In 4.60, we have moved customer related fields from the "GenericAttribute" table to the "Customer" table (see details in https://github.com/nopSolutions/nopCommerce/issues/4601). 
If your database has a lot of "Customer" records, auto migration (during the first application start) can take a long time, so we have prepared scripts to update the database. Execute customer_data_migrate.sql (depending on the db provider) script over your database.

2. The following step is intended store owners upgrading to version 4.60.0 (please ignore this step for further minor version, e.g. 4.60.1 or 4.60.2, etc). If length of your meta keywords or meta description has more than 255 chars, please execute the long_seo_migrate.sql (depending on the db provider) script over your database.