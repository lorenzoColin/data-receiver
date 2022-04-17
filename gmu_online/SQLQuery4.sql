select c.id,c.Name,c.phonenumber,c.city,c.adress,c.Actionid,uc.Userid,uc.companyId 
from [Identity].[Company] as c    
left outer join  [Identity].[UserCompany] as uc  on c.id = uc.companyId 
left outer join  [Identity].[User] as u  on u.id = uc.userId where 
not exists (select userId,companyId 
from [Identity].[UserCompany] uc 
where uc.userId = '9717c4a8-2c58-48ad-b652-3555ba81159b	'
and uc.companyId = c.id)

