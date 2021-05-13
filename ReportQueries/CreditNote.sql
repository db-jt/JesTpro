SELECT 
	A.Id,
    A.IdReceipt,
    A.Description,
    A.IssueDate,
    A.CreditNoteNumber,
    A.CreditNotePath,
    A.InvoiceNumber,
    A.InvoiceDate,
    A.IssuedBy,
    A.XCreateDate,
    A.XUpdateDate,
    A.XDeleteDate,
    A.XLastEditUser,
    A.XCreationUser,
    B.CostAmount AS GrandTotal
FROM credit_note A
INNER JOIN payment_receipt B ON A.IdReceipt = B.Id
WHERE A.IssueDate >= @dataInizio
AND A.IssueDate < @dataFine