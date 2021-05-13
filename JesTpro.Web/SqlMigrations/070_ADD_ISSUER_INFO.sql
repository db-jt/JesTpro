ALTER TABLE `payment_receipt` 
ADD COLUMN `IssuedBy` CHAR(36) NULL DEFAULT NULL AFTER `DiscountDescription`,
ADD COLUMN `PaymentType` VARCHAR(255) NULL DEFAULT NULL AFTER `IssuedBy`;

ALTER TABLE `credit_note` 
ADD COLUMN `IssuedBy` CHAR(36) NULL DEFAULT NULL AFTER `InvoiceDate`;

ALTER TABLE `customer_productinstance` 
ADD COLUMN `PaymentStatus` INT(11) NULL DEFAULT NULL AFTER `IdRenewed`;
