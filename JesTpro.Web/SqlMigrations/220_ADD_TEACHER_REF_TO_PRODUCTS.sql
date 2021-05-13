ALTER TABLE `product` 
DROP COLUMN `TeacherName`,
ADD COLUMN `IdTeacher` CHAR(36) NULL DEFAULT NULL AFTER `CategoryName`;

DELETE FROM `settings` WHERE `key` = 'product.teachers';