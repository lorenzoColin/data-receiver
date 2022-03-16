select c.Id,c.firstname,c.lastname,c.phonenumber,c.company,c.adress,c.city,c.actionid,uc.userId,userId,customerId from [Identity].[Customer] as c 
left outer join  [Identity].[UserCustomer] as uc  on c.id = uc.customerId 
left outer join  [Identity].[User] as u  on u.id = uc.userId 
where not exists (select userId,customerId 
from [Identity].[UserCustomer] uc 
where uc.userId = '9902540a-4ef2-4236-989f-b92d216617af	' 
);