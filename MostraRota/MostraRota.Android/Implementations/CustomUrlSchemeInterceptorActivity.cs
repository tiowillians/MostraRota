﻿using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;

namespace MostraRota.Droid.Implementations
{
    [Activity(Label = "CustomUrlSchemeInterceptorActivity", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataSchemes = new[] { "com.googleusercontent.apps.660052940829-k140n6d2gkuorovugfcc6hmvcdh54rka" },
        DataPath = "/oauth2redirect")]
    public class CustomUrlSchemeInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Converte Android.Net.Url para Uri
            var uri = new Uri(Intent.Data.ToString());

            // Carrega página de redirecionamento
            AuthenticationState.Authenticator.OnPageLoading(uri);

            // encerra essa Activity
            Finish();
        }
    }
}