CREATE PROCEDURE `SP_Save_Invoice`(
	IN selectedIdReceipt CHAR(36),
    IN selectedYear INT,
    IN customerName VARCHAR(256),
    IN customerFiscalCode VARCHAR(45),
    IN customerAddress VARCHAR(512),
    IN costAmount DECIMAL(15,2)
)
BEGIN
	DECLARE invoiceNumber INT Default 0;
    DECLARE errno INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
		GET CURRENT DIAGNOSTICS CONDITION 1 errno = MYSQL_ERRNO;
		SELECT errno AS MYSQL_ERROR;
		ROLLBACK;
    END;
    START TRANSACTION;
    
	SELECT COALESCE(MAX(number),0) INTO invoiceNumber 
		FROM `invoice_number` 
        WHERE `year` = selectedYear;
	
    INSERT INTO `invoice_number` (
		`IdReceipt`,
        `number`,
        `year`)
        VALUES (
			selectedIdReceipt,
            invoiceNumber + 1,
            selectedYear
        );
        
        UPDATE `payment_receipt`
			SET `InvoiceNumber` = CONCAT(selectedYear,'_', LPAD(CONVERT(invoiceNumber + 1,CHAR), 4, '0')),
				`PaymentDate` = CURRENT_TIMESTAMP,
                `CustomerName` = customerName,
                `CustomerFiscalCode` = customerFiscalCode,
                `CustomerAddress` = customerAddress,
                `CostAmount` = costAmount
			WHERE
				`Id` = selectedIdReceipt;
	COMMIT WORK;
END