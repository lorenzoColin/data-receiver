 select c.Id,c.company,c.phonenumber,c.city,c.adress,c.actionId, uc.customerId,uc.UserId from [Identity].[Customer] as c 
                left outer join  [Identity].[UserCustomer] as uc  on c.id = uc.customerId 
                left outer join  [Identity].[User] as u  on u.id = uc.userId 
                where not exists (SELECT  userId,customerId 
                from [Identity].[UserCustomer] uc 
                where  uc.userId = 'eefb91b6-ff69-488d-b3bd-4da83431a293'
                and uc.customerId = c.id)


           