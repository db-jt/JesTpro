ALTER TABLE `customer` 
ADD COLUMN `BirthPlace` TEXT COLLATE NOCASE DEFAULT NULL;

ALTER TABLE `customer`
ADD COLUMN `BirthProvince` TEXT COLLATE NOCASE DEFAULT NULL;
