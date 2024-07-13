This is a web api for managing task for team members.

The admin account is created on its own on application startup. 
Please find the details of the admin account in appsettings.Development.json file.

Only admin has the authority to change/modify the roles of other users, or assign them to a team. 

All users sign up as Employees by default.

Use Role/modify endpoint using admin account token to asssign other users to different teams or modify their roles.

