
CREATE TABLE `product_backup` (
`Id` char(36) NOT NULL,
`Name` TEXT COLLATE NOCASE NOT NULL,
`Description` TEXT COLLATE NOCASE,
`CategoryName` TEXT COLLATE NOCASE DEFAULT NULL,
`TeacherName` TEXT COLLATE NOCASE DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`StartDate` datetime NOT NULL,
`EndDate` datetime DEFAULT NULL,
PRIMARY KEY (`Id`)
);

INSERT INTO `product_backup` (
	`Name`,
	`Description`,
	`CategoryName`,
	`TeacherName`,
	`XCreationUser`,
	`XCreateDate`,
	`XUpdateDate`,
	`XDeleteDate`,
	`XLastEditUser`,
	`StartDate`,
	`EndDate`
) SELECT 
`Name`,
	`Description`,
	`CategoryName`,
	`TeacherName`,
	`XCreationUser`,
	`XCreateDate`,
	`XUpdateDate`,
	`XDeleteDate`,
	`XLastEditUser`,
	`StartDate`,
	`EndDate`
FROM `product`;

DROP TABLE product;

CREATE TABLE `product` (
	`Id` char(36) NOT NULL,
	`Name` TEXT COLLATE NOCASE NOT NULL,
	`Description` TEXT COLLATE NOCASE,
	`CategoryName` TEXT COLLATE NOCASE DEFAULT NULL,
	`IdTeacher` char(36) DEFAULT NULL,
	`XCreationUser` TEXT DEFAULT NULL,
	`XCreateDate` datetime DEFAULT NULL,
	`XUpdateDate` datetime DEFAULT NULL,
	`XDeleteDate` datetime DEFAULT NULL,
	`XLastEditUser` TEXT DEFAULT NULL,
	`StartDate` datetime NOT NULL,
	`EndDate` datetime DEFAULT NULL,
PRIMARY KEY (`Id`)
);

INSERT INTO product (
	`Id`,
	`Name`,
	`Description`,
	`CategoryName`,
	`IdTeacher`,
	`XCreationUser`,
	`XCreateDate`,
	`XUpdateDate`,
	`XDeleteDate`,
	`XLastEditUser`,
	`StartDate`,
	`EndDate`
)
SELECT 
	`Id`,
	`Name`,
	`Description`,
	`CategoryName`,
	 null AS `IdTeacher`,
	`XCreationUser`,
	`XCreateDate`,
	`XUpdateDate`,
	`XDeleteDate`,
	`XLastEditUser`,
	`StartDate`,
	`EndDate`
FROM product_backup;

DROP TABLE product_backup;

DELETE FROM `settings` WHERE `key` = 'product.teachers';
