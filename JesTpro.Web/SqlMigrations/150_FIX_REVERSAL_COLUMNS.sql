ALTER TABLE `customer_productinstance` 
DROP COLUMN `DiscountReversal`,
DROP COLUMN `IdRenewed`,
DROP COLUMN `RenewedDate`,
ADD COLUMN `ReversalDescription` LONGTEXT NULL DEFAULT NULL AFTER `ReversalCredit`;
