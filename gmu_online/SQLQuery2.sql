SELECT *
FROM [Identity].[Customer] c
LEFT JOIN [Identity].[UserCustomer] uc
ON uc.[customerid] = c.id
WHERE uc.[customerid] IS NULL
OR uc.[userid] != '0a65d716-7a15-4aa1-82f9-fce4418fbf1b';