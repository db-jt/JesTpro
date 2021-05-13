SELECT A.PaymentDate,
	A.InvoiceNumber,
    CASE WHEN B.LastName IS NOT NULL 
       THEN concat(B.LastName,' ',B.FirstName)
       ELSE A.CustomerName
	END AS CustomerName,
    CASE WHEN B.TutorLastName IS NOT NULL 
       THEN concat(B.TutorLastName,' ', B.TutorFirstName)
       ELSE ''
	END AS TutorName,
    CONCAT(COALESCE(C.LastName,''),' ',COALESCE(C.FirstName,'')) AS IssuedBy,
    CASE WHEN D.ReceiptDetailType = 0
       THEN 'Quota associativa'
       WHEN D.ReceiptDetailType = 1
       THEN 'Corso'
	   WHEN D.ReceiptDetailType = 2
       THEN 'Altro'
       ELSE 'Sconto'
	END AS ReceiptDetailType,
    F.Name AS ProductName,
    F.CategoryName,
	CONCAT(COALESCE(H.LastName,''),' ',COALESCE(H.FirstName,'')) AS TeacherName,
    E.Name AS ProductDetail,
     CASE WHEN A.PaymentType = 'check' THEN 'Assegno'
       WHEN A.PaymentType = 'cash' THEN 'Contanti'
	   WHEN A.PaymentType = 'creditcard' THEN 'Carta di credito'
       WHEN A.PaymentType = 'bancomat' THEN 'Bancomat'
       WHEN A.PaymentType = 'transfer' THEN 'Bonifico'
       ELSE 'Altro'
    END AS PaymentType,
    CASE 
        WHEN NOT G.CreditNoteNumber IS NULL THEN 0
        WHEN (G.CreditNoteNumber IS NULL AND D.ReceiptDetailType = 1 ) THEN CP.CostAmount
		ELSE D.CostAmount
	END AS CostAmount,
	G.CreditNoteNumber
FROM payment_receipt A LEFT JOIN customer B
  ON A.IdCustomer = B.Id AND B.XDeleteDate IS NULL
  LEFT JOIN `user` C ON A.IssuedBy = C.Id
  LEFT JOIN payment_receipt_detail D ON A.Id = D.IdReceipt AND D.XDeleteDate IS NULL
  LEFT JOIN customer_productinstance CP ON D.IdResource = CP.Id
  LEFT JOIN product_instance E ON CP.IdProductInstance = E.Id
  LEFT JOIN product F ON F.Id = E.IdProduct 
  LEFT JOIN credit_note G ON G.IdReceipt = A.Id AND G.XDeleteDate IS NULL
  LEFT JOIN user H ON H.Id = F.IdTeacher
WHERE NOT A.InvoiceNumber IS NULL
AND A.PaymentDate >= @dtFrom
AND A.PaymentDate <= DATE_ADD(COALESCE(@dtTo,CURRENT_DATE),INTERVAL 1 DAY)
order by A.PaymentDate DESC