ALTER TABLE `payment_receipt` 
ADD COLUMN `Discount` DECIMAL(15,2) NULL DEFAULT NULL AFTER `CustomerAddress`,
ADD COLUMN `DiscountType` INT NULL DEFAULT NULL AFTER `Discount`,
ADD COLUMN `DiscountDescription` TEXT NULL DEFAULT NULL AFTER `DiscountType`;
