DROP procedure IF EXISTS `SP_Save_CreditNote`;

CREATE PROCEDURE `SP_Save_CreditNote`(
	IN selectedIdCreditNote CHAR(36),
    IN selectedYear INT,
    IN issuedBy CHAR(36)
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
				`IssueDate` = CURRENT_TIMESTAMP,
                `IssuedBy` = issuedBy
			WHERE
				`Id` = selectedIdCreditNote;
	COMMIT WORK;
END
;
