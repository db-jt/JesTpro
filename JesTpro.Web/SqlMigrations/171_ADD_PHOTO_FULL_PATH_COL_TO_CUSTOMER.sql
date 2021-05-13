ALTER TABLE `customer` 
ADD COLUMN `PhotoFullPath` VARCHAR(1024) NULL DEFAULT NULL AFTER `Photo`;

UPDATE `customer` SET `PhotoFullPath` = `Photo`;
