CREATE TABLE `credit_note` (
  `Id` CHAR(36) NOT NULL,
  `IdReceipt` CHAR(36) NOT NULL,
  `Description` TEXT NOT NULL,
  `IssueDate` DATETIME NOT NULL,
  `CreditNoteNumber` VARCHAR(45) NOT NULL,
  `InvoiceNumber` VARCHAR(45) DEFAULT NULL,
  `InvoiceDate` DATETIME NOT NULL,
  `XCreateDate` DATETIME NULL DEFAULT NULL,
  `XUpdateDate` DATETIME NULL DEFAULT NULL,
  `XDeleteDate` DATETIME NULL DEFAULT NULL,
  `XLastEditUser` VARCHAR(45) NULL DEFAULT NULL,
  `XCreationUser` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`));

CREATE TABLE `creditnote_number` (
  `IdCreditNote` char(36) NOT NULL,
  `number` int(11) DEFAULT NULL,
  `year` int(11) DEFAULT NULL,
  PRIMARY KEY (`IdCreditNote`),
  UNIQUE KEY `IdCreditNote_UNIQUE` (`IdCreditNote`),
  UNIQUE KEY `UniqueNumber` (`number`,`year`)
);

CREATE PROCEDURE `SP_Save_CreditNote`(
	IN selectedIdCreditNote CHAR(36),
    IN selectedYear INT
)
BEGIN
	DECLARE cnoteNumber INT Default 0;
    DECLARE errno INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
		GET CURRENT DIAGNOSTICS CONDITION 1 errno = MYSQL_ERRNO;
		SELECT errno AS MYSQL_ERROR;
		ROLLBACK;
    END;
    START TRANSACTION;
    
	SELECT COALESCE(MAX(number),0) INTO cnoteNumber 
		FROM `creditnote_number` 
        WHERE `year` = selectedYear;
	
    INSERT INTO `creditnote_number` (
		`IdCreditNote`,
        `number`,
        `year`)
        VALUES (
			selectedIdCreditNote,
            cnoteNumber + 1,
            selectedYear
        );
        
        UPDATE `credit_note`
			SET `CreditNoteNumber` = CONCAT('CN_',selectedYear,'_', LPAD(CONVERT(cnoteNumber + 1,CHAR), 4, '0')),
				`IssueDate` = CURRENT_TIMESTAMP
			WHERE
				`Id` = selectedIdCreditNote;
	COMMIT WORK;
END
;

