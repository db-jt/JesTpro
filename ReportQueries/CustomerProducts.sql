SELECT	
	B.FullName,
	B.FiscalCode,
	B.BirthDate,
	A.Name AS Product,
	C.Name AS ProductDescription,
    (
        CASE 
            WHEN C.Weeks > 0 THEN CONCAT(C.Weeks, ' settimane')
            WHEN C.Months > 0 THEN CONCAT(C.Months, ' mesi')
            WHEN C.Years > 0 THEN CONCAT(C.Years, ' anni')
            WHEN C.Days > 0 THEN CONCAT(C.Days, ' giorni')
            ELSE 'N/A'
        END
      ) AS ProductDuration,
	A.XCreateDate AS CreateDate,
	A.ExpirationDate AS ExpirationDate
FROM 
	customer_productinstance A
INNER JOIN customer B ON B.Id = A.IdCustomer AND B.XDeleteDate IS NULL
LEFT JOIN product_instance C ON C.Id = A.IdProductInstance
WHERE A.xDeleteDate IS NULL
AND A.IdReversal IS NULL
AND NOT EXISTS
(
	SELECT  null 
	FROM    credit_note D
	WHERE   D.IdReceipt = A.IdReceipt
)
ORDER BY 
	B.FullName, B.FiscalCode
-- AND A.ExpirationDate >= COALESCE(@dtReference,CURRENT_DATE)