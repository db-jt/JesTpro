ALTER TABLE `customer` 
ADD COLUMN `FullName` VARCHAR(201) NULL DEFAULT NULL AFTER `LastName`,
ADD COLUMN `Gender` CHAR(1) NULL DEFAULT NULL AFTER `FullName`;

UPDATE `customer` SET `FullName` = CONCAT(`LastName`,' ',`FirstName`)

