# How to deploy system to Azure
1. **GitHub OAuth**
   * System is connected to GitHub as OAuth application. User signs into system using GitHub account, so that system can interact with user's repositories.
   * OAuth app needs to be created in GitHub ([Tutorial](https://docs.github.com/en/developers/apps/building-oauth-apps/creating-an-oauth-app)).
     * Callback url should be set to: (``homepage-url/signin-github``).
     * *Client ID* and *Client secret* will be used later on.
   * I suggest to create two OAuth applications, if you wish to run the application both on localhost and server.
     * For localhost, it is also required to add *Client ID* and *Client secret* into *User Secrets* in *CCMS* project ([Tutorial](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows)).
       * Name for *Client ID* secret is ```ClientId``` and name for *Client secret* is ```ClientSecret```.
2. **Database**
   * Create database which can be updated by EF migration.
     * I have used *SQL server* and *SQL database* on Azure ([Tutorial](https://docs.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?tabs=azure-portal))
       * Allow Azure services and resources to access the database.
   * Add *Connection String* into *User Secrets* in *CCMS* project ([Tutorial](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows)).
     * Name for *Connection String* is ```ConnectionString```.
   * Update database from migration using ```update-database``` command in Package Manager Console in *CCMS.DAL* project.
3. **Key vault**
   * I recommend the create the key vault at Azure to store application secrets ([Tutorial](https://docs.microsoft.com/en-us/azure/key-vault/general/quick-create-portal)).
   * Source code is adjusted to key vault usage. When key vault is used, only URI in ```Program.cs`` at line 25 needs to be changed.
   * Add ```ClientId```, ```ClientSecret``` and ```ConnectionString``` secrets with values into key vault.
4. **App service**
   * Create app service on Azure ([Documentation](https://docs.microsoft.com/en-us/azure/app-service/)).
     * I recommend to use at least B1 service plan at minimum.
   * Connect app service to key vault.
     * Generate identity for the app service in *Identity* page. Copy *Object ID*.
     * Add new access policy in the key vault in *Access policies* page, for principal select *Object ID* from last step.
     * Add *Get* and *List* permissions for secrets to the app.
5. **Publish**
   * Publish app to App service.
     * I recommend using Visual Studio - right click *CCMS* project and click on *Publish* button.