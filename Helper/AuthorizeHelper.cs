//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using LightInvoice.Models;
//using LightInvoice.Controllers;
//using System.Web.Routing;
//using System.Security.Principal;

//namespace LightInvoice.Helper
//{
//    public class AuthorizeHelper
//    {
//        //public static RequestContext CurrentRequestContext
//        //{
//        //    get
//        //    {
//        //        HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
//        //        return new RequestContext(context, RouteTable.Routes.GetRouteData(context));
//        //    }
//        //}
//        //private static void EnsureValidUser(this IPrincipal user)
//        //{
//        //    if (user.GetAccountId() == -1)
//        //        throw new Exception("Assess is denied 1");
//        //}

//        //private static long? GetAccountId(this IPrincipal user)
//        //{
//        //    var currentUserIdentifier = HttpContext.Current.User.Identity.Name;
//        //    if (currentUserIdentifier == "admin")
//        //        return null;
//        //    dynamic accountUser = AccountUserController.GetAccountUser(currentUserIdentifier);
//        //    if (accountUser != null)
//        //        return accountUser.AccountId;
//        //    dynamic clientUser = ClientUserController.GetClientUser(currentUserIdentifier);
//        //    if (clientUser != null)
//        //        return clientUser.FinalArt_Client.AccountId;
//        //    return -1;
//        //}

//        //public static void SetAccountId(this IPrincipal user, dynamic document)
//        //{
//        //    var accountId = user.GetAccountId();
//        //    if (accountId != null)
//        //        document.AccountId = accountId;
//        //}

//        //#region AddAuthorizeFilter (List) - Account
//        //public static IQueryable<Account> AddAuthorizeFilter(this IPrincipal user, IQueryable<Account> queryable)
//        //{
//        //    user.EnsureValidUser();
//        //    queryable = AddAccountLevelAuthorizeFilter(user, queryable);
//        //    queryable = AddClientLevelAuthorizeFilter(user, queryable);
//        //    return queryable;
//        //}

//        //private static IQueryable<Account> AddAccountLevelAuthorizeFilter(this IPrincipal user, IQueryable<Account> queryable)
//        //{
//        //    dynamic accountUser = AccountUserController.GetAccountUser(HttpContext.Current.User.Identity.Name);
//        //    if (accountUser != null)
//        //    {
//        //        long accountId = accountUser.AccountId;
//        //        queryable = queryable.Where(a => a.Id == accountId);
//        //    }
//        //    return queryable;
//        //}

//        //private static IQueryable<Account> AddClientLevelAuthorizeFilter(this IPrincipal user, IQueryable<Account> queryable)
//        //{
//        //    dynamic clientUser = ClientUserController.GetClientUser(HttpContext.Current.User.Identity.Name);
//        //    if (clientUser != null)
//        //    {
//        //        long accountId = clientUser.FinalArt_Client.AccountId;
//        //        queryable = queryable.Where(a => a.Id == accountId);
//        //    }
//        //    return queryable;
//        //}

//        //#endregion

//        //#region HasAssessToDocument (Edit/Create/Detail/Delete)

//        //public static void EnsureAssessToDocument(this IPrincipal user, dynamic document)
//        //{
//        //    if ((HasAdminLevelAssessToDocument(user, document)
//        //        || HasAccountLevelAssessToDocument(user, document)
//        //        || HaClientLevelAssessToDocument(user, document)) == false)
//        //        throw new Exception("Assess is denied + " + document.GetType());
//        //}

//        //private static bool HasAdminLevelAssessToDocument(this IPrincipal user, dynamic document)
//        //{
//        //    if (HttpContext.Current.User.Identity.Name == "admin")
//        //        return true;
//        //    return false;
//        //}

//        //private static bool HasAccountLevelAssessToDocument(this IPrincipal user, dynamic document)
//        //{
//        //    dynamic accountUser = AccountUserController.GetAccountUser(HttpContext.Current.User.Identity.Name);
//        //    if (accountUser != null)
//        //    {
//        //        long accountId = accountUser.AccountId;
//        //        long documentAccountId = document.AccountId;

//        //        return (accountUser.AccountId == document.AccountId) && (accountUser.IsActive);
//        //    }
//        //    return false;
//        //}

//        //private static bool HaClientLevelAssessToDocument(this IPrincipal user, dynamic document)
//        //{
//        //    dynamic clientUser = ClientUserController.GetClientUser(HttpContext.Current.User.Identity.Name);
//        //    if (clientUser != null)
//        //    {
//        //        long accountId = clientUser.FinalArt_Client.AccountId;
//        //        long clientId = clientUser.ClientId;

//        //        long documentAccountId = document.AccountId;
//        //        long docuemntClientId = document.ClientId;

//        //        return (accountId == documentAccountId) && (clientId == docuemntClientId) && (clientUser.IsActive);
//        //    }
//        //    return false;
//        //}

//        //#endregion
//    }
//}