﻿ALTER TABLE `customer` 
ADD COLUMN `BirthPlace` VARCHAR(100) NULL DEFAULT NULL AFTER `BirthDate`,
ADD COLUMN `BirthProvince` VARCHAR(45) NULL DEFAULT NULL AFTER `BirthPlace`;
