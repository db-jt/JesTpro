
ALTER TABLE `product` 
ADD COLUMN `CategoryName` VARCHAR(255) NULL DEFAULT NULL AFTER `Description`;

INSERT INTO `settings` (`Key`, `Value`, `Type`, `Required`) VALUES ('product.categories', '[]', 'select', 0);
INSERT INTO `settings` (`Key`, `Value`, `Type`, `Required`) VALUES ('product.teachers', '[]', 'select', 0);
