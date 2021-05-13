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
    CONCAT(C.FirstName,' ',C.LastName) AS IssuedBy,
    A.CostAmount,
    CASE WHEN A.PaymentType = 'check' THEN 'Assegno'
       WHEN A.PaymentType = 'cash' THEN 'Contanti'
	   WHEN A.PaymentType = 'creditcard' THEN 'Carta di credito'
       WHEN A.PaymentType = 'bancomat' THEN 'Bancomat'
       WHEN A.PaymentType = 'transfer' THEN 'Bonifico'
       ELSE 'Altro'
    END AS PaymentType,
    CN.CreditNoteNumber,
    CN.IssueDate AS CreditNoteDate,
    UCN.UserName AS CreditNoteIssuedBy,
    CN.Description AS CreditNoteDescription,
	CASE WHEN NOT CN.CreditNoteNumber IS null
		THEN (A.CostAmount * (-1)) 
		ELSE null
	END AS CreditNoteAmount
FROM payment_receipt A LEFT JOIN customer B
  ON A.IdCustomer = B.Id AND B.XDeleteDate IS NULL
  LEFT JOIN `user` C ON A.IssuedBy = C.Id
  LEFT JOIN credit_note CN ON CN.IdReceipt = A.Id
  LEFT JOIN `user` UCN ON CN.IssuedBy = UCN.Id
WHERE  NOT A.InvoiceNumber IS NULL
AND A.PaymentDate >= @dtFrom
AND A.PaymentDate <= DATE_ADD(COALESCE(@dtTo,CURRENT_DATE),INTERVAL 1 DAY)
order by A.PaymentDate DESC