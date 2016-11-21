#pragma warning disable SA1652 // Enable XML documentation output
// <copyright file="Dialog.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Shopping.DemoApp.Helpers
#pragma warning restore SA1652 // Enable XML documentation output
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Acr.UserDialogs;

    public static class Dialog
    {
        public static void ShowRating(Action<bool> gotoRating)
        {
            ConfirmConfig confirm = new ConfirmConfig()
            {
                Message = "Time to rate the app! We'll calculate the score based on your smile.",
                OkText = "LET'S DO IT!",
                OnAction = gotoRating,
                CancelText = "CANCEL",
                Title = "Rate the app"
            };

            UserDialogs.Instance.Confirm(confirm);
        }

        public async static Task ShowAsync(string message, string title="")
        {
            var config = new AlertConfig()
            {
                Message = message,
                OkText = "Ok",
                Title = title
            };

            await UserDialogs.Instance.AlertAsync(config);
        }
        public static void Show(string message, string title = "")
        {
            var config = new AlertConfig()
            {
                Message = message,
                OkText = "Ok",
                Title = title
            };

            UserDialogs.Instance.Alert(config);
        }

        public static Task AlertAsync(string message)
        {
            return UserDialogs.Instance.AlertAsync(message);
        }

        public static void ShowLoading(string msg)
        {
            UserDialogs.Instance.ShowLoading(msg);
        }

        public static void HideLoading()
        {
            UserDialogs.Instance.HideLoading();
        }
    }
}
