ALTER TABLE `credit_note` 
ADD COLUMN `CreditNotePath` VARCHAR(512) NULL DEFAULT NULL AFTER `CreditNoteNumber`;
