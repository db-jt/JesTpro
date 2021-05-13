ALTER TABLE `jestpro_test`.`table_name` 
ADD COLUMN `XCreateDate` DATETIME NULL,
ADD COLUMN `XUpdateDate` DATETIME NULL AFTER `XCreateDate`,
ADD COLUMN `XDeleteDate` DATETIME NULL AFTER `XUpdateDate`,
ADD COLUMN `XLastEditUser` VARCHAR(45) NULL AFTER `XDeleteDate`,
ADD COLUMN `XCreationUser` VARCHAR(45) NULL AFTER `XLastEditUser`;