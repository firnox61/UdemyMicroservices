﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("resource_catalog"){Scopes={"catalog_fullpermission"}},
            new ApiResource("resource_photo_stock"){Scopes={"photo_stock_fullpermission"}},
            new ApiResource("resource_basket"){Scopes={"basket_fullpermission"}},
            new ApiResource("resource_discount"){Scopes={"discount_fullpermission"}},
            new ApiResource("resource_order"){Scopes={"order_fullpermission"}},
            new ApiResource("resource_payment"){Scopes={"payment_fullpermission"}},
            new ApiResource("resource_gateway"){Scopes={"gateway_fullpermission"}},
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)

        };
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {//erişebilieceği şeyler resorce metodunda
               new IdentityResources.Email(),//email
               new IdentityResources.OpenId(),//sub
               new IdentityResources.Profile(),
               new IdentityResource(){Name="roles", DisplayName="Roles", Description="Kullanıcı Rolleri", UserClaims=new [] {"role" } }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
               new ApiScope("catalog_fullpermission","Catalog API için full erişim"),
               new ApiScope("photo_stock_fullpermission","Photo Stock API için full erişim"),
               new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
               new ApiScope("basket_fullpermission","Basket Apı için full erişim"),
               new ApiScope("discount_fullpermission","discount Apı için full erişim"),
               new ApiScope("order_fullpermission","Order Apı için full erişim"),
               new ApiScope("payment_fullpermission","Payment Apı için full erişim"),
               new ApiScope("gateway_fullpermission","Payment Apı için full erişim"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName ="Asp.Net Core MVC",
                    ClientId="WebMvcClient",
                    ClientSecrets={new Secret("secret".Sha256()) },
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    AllowedScopes={ "catalog_fullpermission", "photo_stock_fullpermission", "gateway_fullpermission", IdentityServerConstants.LocalApi.ScopeName }
                },
                new Client
                {
                    ClientName ="Asp.Net Core MVC",
                    ClientId="WebMvcClientForUser",
                    AllowOfflineAccess=true,
                    ClientSecrets={new Secret("secret".Sha256()) },
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,//catalog_fullpermission test için ekledimsonra çıkar
                    AllowedScopes={ "basket_fullpermission","order_fullpermission","gateway_fullpermission","catalog_fullpermission",
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    IdentityServerConstants.LocalApi.ScopeName,"roles"//refresh token için tekrar giriş vb.
                    },
                    AccessTokenLifetime=1*60*60,
                    RefreshTokenExpiration=TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime=(int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
                    RefreshTokenUsage=TokenUsage.ReUse,
                },
                 new Client
                {
                    ClientName ="Token Exchange Client",
                    ClientId="TokenExchangeClient",
                    ClientSecrets={new Secret("secret".Sha256()) },
                    AllowedGrantTypes=new []{ "urn:ietf:params:oauth:grant-type:token-exchange" },
                    AllowedScopes={ "discount_fullpermission", "payment_fullpermission", IdentityServerConstants.StandardScopes.OpenId }
                }


            };
    }
}